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

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorAirDefense
    {

        internal static void GenerateAirDefense(MissionTemplateRecord template, UnitMaker unitMaker, Coordinates averageInitialPosition, Coordinates objectivesCenter)
        {
            foreach (Coalition coalition in Toolbox.GetEnumValues<Coalition>())
            {
                bool ally = coalition == template.ContextPlayerCoalition;

                Side side = ally ? Side.Ally : Side.Enemy;
                AmountNR airDefenseAmount = ally ? template.SituationFriendlyAirDefense.Get() : template.SituationEnemyAirDefense.Get();
                Coordinates centerPoint = ally ? averageInitialPosition : objectivesCenter;
                Coordinates opposingPoint = ally ? objectivesCenter : averageInitialPosition;

                foreach (AirDefenseRange airDefenseRange in Toolbox.GetEnumValues<AirDefenseRange>())
                    CreateAirDefenseGroups(template, unitMaker, side, coalition, airDefenseAmount, airDefenseRange, centerPoint, opposingPoint);
            }
        }

        private static void CreateAirDefenseGroups(
            MissionTemplateRecord template, UnitMaker unitMaker, Side side, Coalition coalition,
            AmountNR airDefenseAmount, AirDefenseRange airDefenseRange,
            Coordinates centerPoint, Coordinates opposingPoint)
        {
            var commonAirDefenseDB = Database.Instance.Common.AirDefense;
            DBCommonAirDefenseLevel airDefenseLevelDB = commonAirDefenseDB.AirDefenseLevels[(int)airDefenseAmount];

            int groupCount = airDefenseLevelDB.GroupsInArea[(int)airDefenseRange].GetValue();
            if (groupCount < 1) return;  // No groups to add, no need to go any further

            List<UnitFamily> unitFamilies;
            SpawnPointType[] validSpawnPoints;
            switch (airDefenseRange)
            {
                case AirDefenseRange.MediumRange:
                    unitFamilies = new List<UnitFamily> { UnitFamily.VehicleSAMMedium };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandLarge };
                    break;
                case AirDefenseRange.LongRange:
                    unitFamilies = new List<UnitFamily> { UnitFamily.VehicleSAMLong };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandLarge };
                    break;
                default: // case AirDefenseRange.ShortRange:
                    unitFamilies = new List<UnitFamily> { UnitFamily.VehicleAAA, UnitFamily.VehicleAAAStatic, UnitFamily.VehicleInfantryMANPADS, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShort, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR };
                    validSpawnPoints = new SpawnPointType[] { SpawnPointType.LandSmall, SpawnPointType.LandMedium, SpawnPointType.LandLarge };
                    break;
            }

            for (int i = 0; i < groupCount; i++)
            {
                // Find spawn point at the proper distance
                Coordinates? spawnPoint =
                    unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        validSpawnPoints,
                        centerPoint,
                        commonAirDefenseDB.DistanceFromCenter[(int)side, (int)airDefenseRange],
                        opposingPoint,
                        new MinMaxD(commonAirDefenseDB.MinDistanceFromOpposingPoint[(int)side, (int)airDefenseRange], 99999),
                        GeneratorTools.GetSpawnPointCoalition(template, side));

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {airDefenseRange} air defense unit groups", LogMessageErrorLevel.Warning);
                    return;
                }

                var groupInfo = unitMaker.AddUnitGroup(
                    unitFamilies, 1, side,
                    "Vehicle", "Vehicle",
                    spawnPoint.Value,
                    0,
                    new Dictionary<string, object>());

                if (!groupInfo.HasValue) // Failed to generate a group
                    BriefingRoom.PrintToLog(
                        $"Failed to add {airDefenseRange} air defense unit group for {coalition} coalition.",
                        LogMessageErrorLevel.Warning);
            }
        }
    }
}
