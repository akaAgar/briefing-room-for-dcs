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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates friendly and enemy combat air patrols in the mission area.
    /// </summary>
    internal class MissionGeneratorCombatAirPatrols : IDisposable
    {
        /// <summary>
        /// Possible number of units in a CAP group.
        /// </summary>
        private static readonly int[] CAP_GROUP_SIZE = new int[] { 1, 1, 2, 2, 2, 2, 3, 4, 4 };

        /// <summary>
        /// Possible unit families in a CAP group.
        /// </summary>
        private static readonly UnitFamily[] CAP_UNIT_FAMILIES = new UnitFamily[] { UnitFamily.PlaneFighter, UnitFamily.PlaneFighter, UnitFamily.PlaneInterceptor };

        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        internal MissionGeneratorCombatAirPatrols(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal int[] GenerateCAP(MissionTemplate template, Coordinates initialPosition, Coordinates objectivesCenter)
        {
            List<int> capAircraftGroupIDs = new List<int>();

            foreach (Coalition coalition in Toolbox.GetEnumValues<Coalition>())
            {
                bool ally = coalition == template.ContextPlayerCoalition;

                Side side = ally ? Side.Ally : Side.Enemy;
                AmountNR capAmount = ally ? template.SituationFriendlyAirForce.Get() : template.SituationEnemyAirForce.Get();
                Coordinates centerPoint = ally ? initialPosition : objectivesCenter;
                Coordinates opposingPoint = ally ? objectivesCenter : initialPosition;

                CreateCAPGroups(side, coalition, capAmount, centerPoint, opposingPoint, ref capAircraftGroupIDs);
            }

            return capAircraftGroupIDs.ToArray();
        }

        private void CreateCAPGroups(Side side, Coalition coalition, AmountNR capAmount, Coordinates centerPoint, Coordinates opposingPoint, ref List<int> capAircraftGroupIDs)
        {
            return;

            DBCommonCAPLevel capLevelDB = Database.Instance.Common.CAP.CAPLevels[(int)capAmount];

            int unitsLeftToSpawn = capLevelDB.AircraftCount.GetValue();
            if (unitsLeftToSpawn < 1) return;  // No groups to add, no need to go any further

            do
            {
                int groupSize = Toolbox.RandomFrom(CAP_GROUP_SIZE);
                groupSize = Math.Min(unitsLeftToSpawn, groupSize);
                unitsLeftToSpawn -= groupSize;

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint = null;
                //DBEntryTheaterSpawnPoint? spawnPoint =
                //    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                //        null,
                //        centerPoint,
                //        Database.Instance.Common.AirDefense.DistanceFromCenter[(int)side, (int)airDefenseRange],
                //        opposingPoint,
                //        new MinMaxD(Database.Instance.Common.AirDefense.MinDistanceFromOpposingPoint[(int)side, (int)airDefenseRange], 99999),
                //        coalition);

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {coalition} combat air patrols.", LogMessageErrorLevel.Warning);
                    return;
                }

                UnitMakerGroupInfo? groupInfo = UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(CAP_UNIT_FAMILIES), groupSize, side,
                    "GroupAircraftCAP", "UnitAircraft",
                    spawnPoint.Value.Coordinates,
                    Toolbox.RandomFrom(capLevelDB.SkillLevel));

                if (!groupInfo.HasValue) // Failed to generate a group
                    BriefingRoom.PrintToLog($"Failed to find units for {coalition} air defense unit group.", LogMessageErrorLevel.Warning);

                capAircraftGroupIDs.Add(groupInfo.Value.GroupID);
            } while (unitsLeftToSpawn > 0);
        }


        ///// <summary>
        ///// Main unit generation method.
        ///// </summary>
        ///// <param name="mission">Mission to which generated units should be added</param>
        ///// <param name="template">Mission template to use</param>
        ///// <param name="enemyCoalitionDB">Enemy coalition database entry</param>
        ///// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        ///// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        //internal void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryCoalition enemyCoalitionDB, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        //{
        //    //if (objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyCAP))
        //    //{
        //    //    BriefingRoom.PrintToLog("Enemy CAP disabled for this mission objective type, not spawning any units", 1);
        //    //    return;
        //    //}
        //    int totalAirForcePower =
        //        (int)(GetMissionPackageAirPower(template, aiEscortTypeCAP, aiEscortTypeSEAD) *
        //        Database.Common.EnemyCAPRelativePower[(int)template.SituationEnemyAirForce.Get()]);

        //    BriefingRoom.PrintToLog($"Enemy air power set to {totalAirForcePower}...", 1);

        //    DCSMissionUnitGroupFlags flags = template.Realism.Contains(RealismOption.HideEnemyUnits) ? DCSMissionUnitGroupFlags.Hidden : 0;

        //    int aircraftCount = 0;
        //    int groupCount = 0;

        //    while (totalAirForcePower > 0)
        //    {
        //        string[] unitTypes = enemyCoalitionDB.GetRandomUnits(UnitFamily.PlaneFighter, mission.DateTime.Decade, 1, template.UnitMods);
        //        if (unitTypes.Length == 0)
        //        {
        //            BriefingRoom.PrintToLog("No valid units found for enemy fighter patrols.", 1, DebugLogMessageErrorLevel.Warning);
        //            break;
        //        }

        //        // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
        //        DBEntryTheaterSpawnPoint? spawnPoint =
        //            UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
        //                null,
        //                mission.ObjectivesCenter, Database.Common.EnemyCAPDistanceFromObjectives,
        //                mission.InitialPosition, new MinMaxD(Database.Common.EnemyCAPMinDistanceFromTakeOffLocation, 99999),
        //                GeneratorTools.GetEnemySpawnPointCoalition(template));

        //        if (!spawnPoint.HasValue) // No spawn point found, stop here.
        //        {
        //            BriefingRoom.PrintToLog("No spawn point found for enemy fighter patrol group.", 1, DebugLogMessageErrorLevel.Warning);
        //            break;
        //        }

        //        int unitPower = Database.GetEntry<DBEntryUnit>(unitTypes[0]).AircraftData.AirToAirRating[1];
        //        int groupSize = 1;
        //        if (totalAirForcePower >= unitPower * 2) groupSize = 2;
        //        if (Toolbox.RandomDouble() < .3)
        //        {
        //            if (totalAirForcePower >= unitPower * 4) groupSize = 4;
        //            else if (totalAirForcePower >= unitPower * 3) groupSize = 3;
        //        }
        //        totalAirForcePower -= unitPower * groupSize;

        //        DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
        //            mission, Enumerable.Repeat(unitTypes[0], groupSize).ToArray(),
        //            Side.Enemy, spawnPoint.Value.Coordinates,
        //            "GroupAircraftCAP", "UnitAircraft",
        //            Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationEnemySkillLevelAir),
        //            flags, UnitTaskPayload.AirToAir,
        //            mission.ObjectivesCenter + Coordinates.CreateRandom(20, 40) * Toolbox.NM_TO_METERS);

        //        if (group == null)
        //            BriefingRoom.PrintToLog($"Failed to add a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}", 1, DebugLogMessageErrorLevel.Warning);
        //        else
        //        {
        //            BriefingRoom.PrintToLog($"Added a group of {groupSize}× {unitTypes[0]} at {spawnPoint.Value.Coordinates}");
        //            mission.AircraftSpawnQueue.Add(new DCSMissionAircraftSpawnQueueItem(group.GroupID, template.SituationEnemyCAPOnStationChance.RollChance()));
        //        }

        //        aircraftCount += groupSize;
        //        groupCount++;
        //    }
        //}

        ///// <summary>
        ///// Returns the total air-to-air power rating of the player's (and AI escort) flight package
        ///// </summary>
        ///// <param name="template">Mission template to use</param>
        ///// <param name="aiEscortTypeCAP">Type of aircraft selected for player AI CAP escort (single-player only)</param>
        ///// <param name="aiEscortTypeSEAD">Type of aircraft selected for player AI SEAD escort (single-player only)</param>
        ///// <returns>Total air-to-air power rating of the flight package</returns>
        //private int GetMissionPackageAirPower(MissionTemplate template, string aiEscortTypeCAP, string aiEscortTypeSEAD)
        //{
        //    int airPowerRating = 0;
        //    DBEntryUnit aircraft;

        //    if (template.MissionType == MissionType.SinglePlayer)
        //    {
        //        // Player flight group
        //        aircraft = Database.GetEntry<DBEntryUnit>(template.PlayerFlightGroups[0].Aircraft);
        //        airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * (template.PlayerFlightGroups[0].Count);
        //    }
        //    else // Mission is multi-player
        //    {
        //        foreach (MissionTemplateFlightGroup fg in template.PlayerFlightGroups)
        //        {
        //            aircraft = Database.GetEntry<DBEntryUnit>(fg.Aircraft);

        //            if (aircraft == null) // Aircraft doesn't exist
        //            {
        //                airPowerRating += fg.Count;
        //                continue;
        //            }

        //            bool hasAirToAirLoadout = // Does the flight group have an air-to-air payload?
        //                (fg.Payload == UnitTaskPayload.AirToAir) ||
        //                ((fg.Payload == UnitTaskPayload.Default) && (aircraft.Families.Contains(UnitFamily.PlaneFighter) || aircraft.Families.Contains(UnitFamily.PlaneInterceptor)));

        //            airPowerRating += aircraft.AircraftData.AirToAirRating[hasAirToAirLoadout ? 1 : 0] * fg.Count;
        //        }
        //    }

        //    // AI CAP escort
        //    aircraft = Database.GetEntry<DBEntryUnit>(aiEscortTypeCAP);
        //    airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[1] : 1) * template.SituationFriendlyEscortCAP;

        //    // AI SEAD escort
        //    aircraft = Database.GetEntry<DBEntryUnit>(aiEscortTypeSEAD);
        //    airPowerRating += ((aircraft != null) ? aircraft.AircraftData.AirToAirRating[0] : 1) * template.SituationFriendlyEscortSEAD;

        //    return airPowerRating;
        //}

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
