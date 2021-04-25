/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom.DB;
using BriefingRoom.Debug;
using BriefingRoom.Mission;
using BriefingRoom.Template;
using System;
using System.Linq;

namespace BriefingRoom.Generator
{
    /// <summary>
    /// Generates friendly and enemy air patrols in the mission area.
    /// </summary>
    public class MissionGeneratorEnemyFighterPatrols : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        private readonly Database Database;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorEnemyFighterPatrols(Database database, UnitMaker unitMaker)
        {
            Database = database;
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="enemyCoalitionDB">Enemy coalition database entry</param>
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        public void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryCoalition enemyCoalitionDB, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        {
            //if (objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyCAP))
            //{
            //    DebugLog.Instance.WriteLine("Enemy CAP disabled for this mission objective type, not spawning any units", 1);
            //    return;
            //}
            int totalAirForcePower =
                (int)(GetMissionPackageAirPower(template, aiEscortTypeCAP, aiEscortTypeSEAD) *
                Database.Common.EnemyCAPRelativePower[(int)template.SituationEnemyAirForce.Get()]);

            DebugLog.Instance.WriteLine($"Enemy air power set to {totalAirForcePower}...", 1);

            DCSMissionUnitGroupFlags flags = template.Realism.Contains(RealismOption.HideEnemyUnits) ? DCSMissionUnitGroupFlags.Hidden : 0;

            int aircraftCount = 0;
            int groupCount = 0;

            while (totalAirForcePower > 0)
            {
                string[] unitTypes = enemyCoalitionDB.GetRandomUnits(UnitFamily.PlaneFighter, mission.DateTime.Decade, 1, template.UnitMods);
                if (unitTypes.Length == 0)
                {
                    DebugLog.Instance.WriteLine("No valid units found for enemy fighter patrols.", 1, DebugLogMessageErrorLevel.Warning);
                    break;
                }

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        null,
                        mission.ObjectivesCenter, Database.Common.EnemyCAPDistanceFromObjectives,
                        mission.InitialPosition, new MinMaxD(Database.Common.EnemyCAPMinDistanceFromTakeOffLocation, 99999),
                        GeneratorTools.GetEnemySpawnPointCoalition(template));

                if (!spawnPoint.HasValue) // No spawn point found, stop here.
                {
                    DebugLog.Instance.WriteLine("No spawn point found for enemy fighter patrol group.", 1, DebugLogMessageErrorLevel.Warning);
                    break;
                }

                int unitPower = Database.GetEntry<DBEntryUnit>(unitTypes[0]).AircraftData.AirToAirRating[1];
                int groupSize = 1;
                if (totalAirForcePower >= unitPower * 2) groupSize = 2;
                if (Toolbox.RandomDouble() < .3)
                {
                    if (totalAirForcePower >= unitPower * 4) groupSize = 4;
                    else if (totalAirForcePower >= unitPower * 3) groupSize = 3;
                }
                totalAirForcePower -= unitPower * groupSize;

                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission, Enumerable.Repeat(unitTypes[0], groupSize).ToArray(),
                    Side.Enemy, spawnPoint.Value.Coordinates,
                    "GroupAircraftCAP", "UnitAircraft",
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationEnemySkillLevelAir),
                    flags, UnitTaskPayload.AirToAir,
                    mission.ObjectivesCenter + Coordinates.CreateRandom(20, 40) * Toolbox.NM_TO_METERS);

                if (group == null)
                    DebugLog.Instance.WriteLine($"Failed to add a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}", 1, DebugLogMessageErrorLevel.Warning);
                else
                {
                    DebugLog.Instance.WriteLine($"Added a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}");
                    mission.AircraftSpawnQueue.Add(new DCSMissionAircraftSpawnQueueItem(group.GroupID, template.SituationEnemyCAPOnStationChance.RollChance()));
                }

                aircraftCount += groupSize;
                groupCount++;
            }
        }

        /// <summary>
        /// Returns the total air-to-air power rating of the player's (and AI escort) flight package
        /// </summary>
        /// <param name="template">Mission template to use</param>
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        /// <returns>Total air-to-air power rating of the flight package</returns>
        private int GetMissionPackageAirPower(MissionTemplate template, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        {
            int airPowerRating = 0;
            DBEntryUnit aircraft;

            if (template.MissionType == MissionType.SinglePlayer)
            {
                // Player flight group
                aircraft = Database.GetEntry<DBEntryUnit>(template.PlayerFlightGroups[0].Aircraft);
                airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * (template.PlayerFlightGroups[0].Count);
            }
            else // Mission is multi-player
            {
                foreach (MissionTemplateFlightGroup fg in template.PlayerFlightGroups)
                {
                    aircraft = Database.GetEntry<DBEntryUnit>(fg.Aircraft);

                    if (aircraft == null) // Aircraft doesn't exist
                    {
                        airPowerRating += fg.Count;
                        continue;
                    }

                    bool hasAirToAirLoadout = // Does the flight group have an air-to-air payload?
                        (fg.Payload == UnitTaskPayload.AirToAir) ||
                        ((fg.Payload == UnitTaskPayload.Default) && (aircraft.Families.Contains(UnitFamily.PlaneFighter) || aircraft.Families.Contains(UnitFamily.PlaneInterceptor)));

                    airPowerRating += aircraft.AircraftData.AirToAirRating[hasAirToAirLoadout ? 1 : 0] * fg.Count;
                }
            }

            // AI CAP escort
            aircraft = Database.GetEntry<DBEntryUnit>(aiEscortTypeCAP);
            airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * template.SituationFriendlyEscortCAP;

            // AI SEAD escort
            aircraft = Database.GetEntry<DBEntryUnit>(aiEscortTypeSEAD);
            airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[0] : 1) * template.SituationFriendlyEscortSEAD;

            return airPowerRating;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
