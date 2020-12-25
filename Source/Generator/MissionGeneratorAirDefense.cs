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

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates enemy surface-to-air defense unit groups,
    /// except "embedded" air defense, which is generated at the same time as the group objectives.
    /// </summary>
    public class MissionGeneratorAirDefense : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;
        private readonly AmountN airDefense;
        private readonly Coordinates centerPoint;
        private readonly Coordinates opposingPoint;
        private readonly MinMaxD[] distsFromCenter;
        private readonly int[] minDistFromOpposingPoint;
        private readonly BRSkillLevel skillLevel;
        private readonly DCSMissionUnitGroupFlags optionsShowEnemyUnits;
        private readonly bool ally;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorAirDefense(UnitMaker unitMaker, bool _ally, MissionTemplate template, DCSMission mission)
        {
            UnitMaker = unitMaker;
            ally = _ally;
            if (ally)
            {
                airDefense = template.AllyAirDefense;
                centerPoint = mission.InitialPosition;
                opposingPoint = mission.ObjectivesCenter;
                distsFromCenter = Database.Instance.Common.AllyAirDefenseDistanceFromTakeOffLocation;
                minDistFromOpposingPoint = Database.Instance.Common.AllyAirDefenseDistanceFromObjectives;
                skillLevel = template.PlayerAISkillLevel;
                optionsShowEnemyUnits = 0;
                return;
            }

            airDefense = template.OppositionAirDefense;
            centerPoint = mission.ObjectivesCenter;
            opposingPoint = mission.InitialPosition;
            distsFromCenter = Database.Instance.Common.EnemyAirDefenseDistanceFromObjectives;
            minDistFromOpposingPoint = Database.Instance.Common.EnemyAirDefenseDistanceFromTakeOffLocation;
            skillLevel = template.OppositionSkillLevelGround;
            optionsShowEnemyUnits = !template.OptionsShowEnemyUnits ? DCSMissionUnitGroupFlags.Hidden : 0;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="enemyCoalitionDB">Enemy coalition database entry</param>
        public void CreateUnitGroups(DCSMission mission, DBEntryObjective objectiveDB, DBEntryCoalition enemyCoalitionDB, Coalition coalition)
        {
            foreach (AirDefenseRange airDefenseRange in (AirDefenseRange[])Enum.GetValues(typeof(AirDefenseRange)))
            {
                DebugLog.Instance.WriteLine($"Adding {Toolbox.SplitCamelCase(airDefenseRange)} air defense", 1);
                if (
                    ((airDefenseRange == AirDefenseRange.ShortRange) && objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyAirDefenseShort)) ||
                    ((airDefenseRange == AirDefenseRange.MediumRange) && objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyAirDefenseMedium)) ||
                    ((airDefenseRange == AirDefenseRange.LongRange) && objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.NoEnemyAirDefenseLong)))
                {
                    DebugLog.Instance.WriteLine($"{Toolbox.SplitCamelCase(airDefenseRange)} air defense disabled for this mission objective type, not spawning any units", 1);
                    continue;
                }

                AddAirDefenseUnits(mission, airDefenseRange, enemyCoalitionDB, coalition);
            }
        }

        /// <summary>
        /// Add surface-to-air defense groups.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="airDefenseRange">Air-defense range category</param>
        /// <param name="enemyCoalitionDB">Enemy coalition database entry</param>
        private void AddAirDefenseUnits(DCSMission mission, AirDefenseRange airDefenseRange, DBEntryCoalition enemyCoalitionDB, Coalition coalition)
        {
            // Get the proper number of groups
            int groupCount = Database.Instance.Common.
                EnemyAirDefense[(int)airDefense].GroupsInArea[(int)airDefenseRange].GetValue();
            if (groupCount < 1) return;  // No groups to add, no need to go any further

            DCSMissionUnitGroupFlags flags = optionsShowEnemyUnits;

            UnitFamily[] unitFamilies;
            TheaterLocationSpawnPointType[] validSpawnPoints;
            switch (airDefenseRange)
            {
                default: // case AirDefenseRange.ShortRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleAAA, UnitFamily.VehicleAAAStatic, UnitFamily.VehicleInfantryMANPADS, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandSmall, TheaterLocationSpawnPointType.LandMedium, TheaterLocationSpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.MediumRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMMedium };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandMedium, TheaterLocationSpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.LongRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMLong };
                    validSpawnPoints = new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandLarge };
                    break;
            }

            for (int i = 0; i < groupCount; i++)
            {
                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        validSpawnPoints,
                        centerPoint,
                        distsFromCenter[(int)airDefenseRange],
                        opposingPoint,
                        new MinMaxD(minDistFromOpposingPoint[(int)airDefenseRange], 99999),
                        coalition);

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    DebugLog.Instance.WriteLine($"No spawn point found for {airDefenseRange} air defense unit groups", 1, DebugLogMessageErrorLevel.Warning);
                    return;
                }

                string[] units = enemyCoalitionDB.GetRandomUnits(Toolbox.RandomFrom(unitFamilies), 1);

                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission, units, Side.Enemy,
                    spawnPoint.Value.Coordinates,
                    "GroupVehicle", "UnitVehicle",
                    Toolbox.BRSkillLevelToDCSSkillLevel(skillLevel),
                    flags);

                if (group == null)
                    DebugLog.Instance.WriteLine($"Failed to add {airDefenseRange} air defense unit group of type {units[0]}", 1, DebugLogMessageErrorLevel.Warning);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
