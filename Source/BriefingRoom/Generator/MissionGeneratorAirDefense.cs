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

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates enemy surface-to-air defense unit groups,
    /// except "embedded" air defense, which is generated at the same time as the group objectives.
    /// </summary>
    internal class MissionGeneratorAirDefense : IDisposable
    {
        /// <summary>
        /// Shorcut to Database.Instance.Common.AirDefense.
        /// </summary>
        private DBCommonAirDefense CommonAirDefenseDB { get { return Database.Instance.Common.AirDefense; } }

        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        internal MissionGeneratorAirDefense(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal void GenerateAirDefense(MissionTemplate template, Coordinates averageInitialPosition, Coordinates objectivesCenter)
        {
            foreach (Coalition coalition in Toolbox.GetEnumValues<Coalition>())
            {
                bool ally = coalition == template.ContextPlayerCoalition;
                
                Side side = ally ? Side.Ally : Side.Enemy;
                AmountNR airDefenseAmount = ally ? template.SituationFriendlyAirDefense.Get() : template.SituationEnemyAirDefense.Get();
                Coordinates centerPoint = ally ? averageInitialPosition : objectivesCenter;
                Coordinates opposingPoint = ally ? objectivesCenter : averageInitialPosition;

                foreach (AirDefenseRange airDefenseRange in Toolbox.GetEnumValues<AirDefenseRange>())
                    CreateAirDefenseGroups(template, side, coalition, airDefenseAmount, airDefenseRange, centerPoint, opposingPoint);
            }
        }

        private void CreateAirDefenseGroups(
            MissionTemplate template, Side side, Coalition coalition,
            AmountNR airDefenseAmount, AirDefenseRange airDefenseRange,
            Coordinates centerPoint, Coordinates opposingPoint)
        {
            DBCommonAirDefenseLevel airDefenseLevelDB = CommonAirDefenseDB.AirDefenseLevels[(int)airDefenseAmount];

            int groupCount = airDefenseLevelDB.GroupsInArea[(int)airDefenseRange].GetValue();
            if (groupCount < 1) return;  // No groups to add, no need to go any further

            UnitFamily[] unitFamilies;
            SpawnPointType[] validSpawnPoints;
            switch (airDefenseRange)
            {
                default: // case AirDefenseRange.ShortRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleAAA, UnitFamily.VehicleAAAStatic, UnitFamily.VehicleInfantryMANPADS, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandSmall, SpawnPointType.LandMedium, SpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.MediumRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMMedium };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.LongRange:
                    unitFamilies = new UnitFamily[] { UnitFamily.VehicleSAMLong };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandLarge };
                    break;
            }

            for (int i = 0; i < groupCount; i++)
            {
                // Find spawn point at the proper distance
                Coordinates? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        validSpawnPoints,
                        centerPoint,
                        CommonAirDefenseDB.DistanceFromCenter[(int)side, (int)airDefenseRange],
                        opposingPoint,
                        new MinMaxD(CommonAirDefenseDB.MinDistanceFromOpposingPoint[(int)side, (int)airDefenseRange], 99999),
                        GeneratorTools.GetSpawnPointCoalition(template, side));

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {airDefenseRange} air defense unit groups", LogMessageErrorLevel.Warning);
                    return;
                }

                UnitMakerGroupInfo? groupInfo = null;
                unitFamilies = Toolbox.ShuffleArray(unitFamilies);
                for (int j = 0; j < unitFamilies.Length; j++) // Try picking for various families until a valid one is found
                {
                    groupInfo = UnitMaker.AddUnitGroup(
                        unitFamilies[j], 1, side,
                        "GroupVehicle", "UnitVehicle",
                        spawnPoint.Value);

                    if (groupInfo.HasValue) break;
                }

                if (!groupInfo.HasValue) // Failed to generate a group
                    BriefingRoom.PrintToLog(
                        $"Failed to add {airDefenseRange} air defense unit group for {coalition} coalition.",
                        LogMessageErrorLevel.Warning);
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
