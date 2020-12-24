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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorEnemyFighterPatrols(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="enemyCoalitionDB">Enemy coalition database entry</param>
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        public void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryCoalition enemyCoalitionDB, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        {
            if (objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyCAP))
            {
                DebugLog.Instance.WriteLine("Enemy CAP disabled for this mission objective type, not spawning any units", 1);
                return;
            }

            int totalAirForcePower =
                (int)(GetMissionPackageAirPower(template, objectiveDB, aiEscortTypeCAP, aiEscortTypeSEAD) *
                Database.Instance.Common.EnemyCAPRelativePower[(int)template.OppositionAirForce]);

            DebugLog.Instance.WriteLine($"Enemy air power set to {totalAirForcePower}...", 1);

            DCSMissionUnitGroupFlags flags = template.OptionsShowEnemyUnits ? 0 : DCSMissionUnitGroupFlags.Hidden;

            int aircraftCount = 0;
            int groupCount = 0;

            while (totalAirForcePower > 0)
            {
                string[] unitTypes = enemyCoalitionDB.GetRandomUnits(UnitFamily.PlaneFighter, 1);
                if (unitTypes.Length == 0)
                {
                    DebugLog.Instance.WriteLine("No valid units found for enemy fighter patrols.", 1, DebugLogMessageErrorLevel.Warning);
                    break;
                }

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        null,
                        mission.ObjectivesCenter, Database.Instance.Common.EnemyCAPDistanceFromObjectives,
                        mission.InitialPosition, new MinMaxD(Database.Instance.Common.EnemyCAPMinDistanceFromTakeOffLocation, 99999),
                        GeneratorTools.GetEnemySpawnPointCoalition(template));

                if (!spawnPoint.HasValue) // No spawn point found, stop here.
                {
                    DebugLog.Instance.WriteLine("No spawn point found for enemy fighter patrol group.", 1, DebugLogMessageErrorLevel.Warning);
                    break;
                }

                int unitPower = Database.Instance.GetEntry<DBEntryUnit>(unitTypes[0]).AircraftData.AirToAirRating[1];
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
                    "GroupAircraftEnemyCAP", "UnitAircraft",
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.OppositionSkillLevelAir),
                    flags, UnitTaskPayload.AirToAir,
                    mission.ObjectivesCenter + Coordinates.CreateRandom(20, 40) * Toolbox.NM_TO_METERS);

                if (group == null)
                    DebugLog.Instance.WriteLine($"Failed to add a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}", 1, DebugLogMessageErrorLevel.Warning);
                else
                {
                    DebugLog.Instance.WriteLine($"Added a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}");
                    mission.AircraftSpawnQueue.Add(new DCSMissionAircraftSpawnQueueItem(group.GroupID, false));
                }

                aircraftCount += groupSize;
                groupCount++;
            }
        }

        /// <summary>
        /// Returns the total air-to-air power rating of the player's (and AI escort) flight package
        /// </summary>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        /// <returns>Total air-to-air power rating of the flight package</returns>
        private int GetMissionPackageAirPower(MissionTemplate template, DBEntryObjective objectiveDB, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        {
            int airPowerRating = 0;

            if (template.GetMissionType() == MissionType.SinglePlayer)
            {
                DBEntryUnit aircraft;

                // Player flight group
                aircraft = Database.Instance.GetEntry<DBEntryUnit>(template.PlayerSPAircraft);
                airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * (template.PlayerSPWingmen + 1);

                // AI CAP escort
                aircraft = Database.Instance.GetEntry<DBEntryUnit>(aiEscortTypeCAP);
                airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * template.PlayerSPEscortCAP;

                // AI SEAD escort
                aircraft = Database.Instance.GetEntry<DBEntryUnit>(aiEscortTypeSEAD);
                airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[0] : 1) * template.PlayerSPEscortSEAD;
            }
            else // Mission is multi-player
            {
                foreach (MissionTemplateMPFlightGroup fg in template.PlayerMPFlightGroups)
                {
                    DBEntryUnit aircraft = Database.Instance.GetEntry<DBEntryUnit>(fg.AircraftType);

                    if (aircraft == null) // Aircraft doesn't exist
                    {
                        airPowerRating += fg.Count;
                        continue;
                    }

                    bool hasAirToAirLoadout;
                    switch (fg.Task)
                    {
                        default: // case MissionTemplateMPFlightGroupTask.Objectives
                            if (objectiveDB.Payload == UnitTaskPayload.Default)
                                hasAirToAirLoadout = aircraft.DefaultFamily == UnitFamily.PlaneFighter;
                            else if (objectiveDB.Payload == UnitTaskPayload.AirToAir)
                                hasAirToAirLoadout = true;
                            else
                                hasAirToAirLoadout = false;
                            break;
                        case MissionTemplateMPFlightGroupTask.SupportCAP:
                            hasAirToAirLoadout = true;
                            break;
                        case MissionTemplateMPFlightGroupTask.SupportSEAD:
                            hasAirToAirLoadout = false;
                            break;

                    }

                    airPowerRating += aircraft.AircraftData.AirToAirRating[hasAirToAirLoadout ? 1 : 0] * fg.Count;
                }
            }

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
