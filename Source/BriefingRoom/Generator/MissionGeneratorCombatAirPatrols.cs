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
    internal class MissionGeneratorCombatAirPatrols
    {

        internal static int[] GenerateCAP(UnitMaker unitMaker, MissionTemplate template, Coordinates averageInitialPosition, Coordinates objectivesCenter)
        {
            List<int> capAircraftGroupIDs = new List<int>();
            var commonCAPDB = Database.Instance.Common.CAP;
            foreach (Coalition coalition in Toolbox.GetEnumValues<Coalition>())
            {
                if (coalition == Coalition.Neutural) // Skip Neutural
                    continue;

                bool ally = coalition == template.ContextPlayerCoalition;

                Side side = ally ? Side.Ally : Side.Enemy;
                AmountNR capAmount = ally ? template.SituationFriendlyAirForce.Get() : template.SituationEnemyAirForce.Get();
                Coordinates flyPathtoObjectives = (objectivesCenter - averageInitialPosition).Normalize() * Toolbox.NM_TO_METERS * commonCAPDB.MinDistanceFromOpposingPoint; // TODO: distance according to decade
                Coordinates centerPoint = objectivesCenter;
                if (ally) centerPoint -= flyPathtoObjectives;
                else centerPoint += flyPathtoObjectives;

                Coordinates opposingPoint = objectivesCenter;

                CreateCAPGroups(
                    unitMaker,
                    template, side, coalition, capAmount,
                    centerPoint, opposingPoint,
                    objectivesCenter,
                    ref capAircraftGroupIDs);
            }

            return capAircraftGroupIDs.ToArray();
        }

        private static void CreateCAPGroups(
            UnitMaker unitMaker, MissionTemplate template, Side side,
            Coalition coalition, AmountNR capAmount, Coordinates centerPoint,
            Coordinates opposingPoint, Coordinates destination, ref List<int> capAircraftGroupIDs)
        {
            var commonCAPDB = Database.Instance.Common.CAP;
            DBCommonCAPLevel capLevelDB = commonCAPDB.CAPLevels[(int)capAmount];

            int unitsLeftToSpawn = capLevelDB.UnitCount.GetValue();
            if (unitsLeftToSpawn < 1) return;  // No groups to add, no need to go any further

            do
            {
                int groupSize = Toolbox.RandomFrom(commonCAPDB.GroupSize);
                groupSize = Math.Min(unitsLeftToSpawn, groupSize);
                unitsLeftToSpawn -= groupSize;

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                Coordinates? spawnPoint =
                    unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        new SpawnPointType[] { SpawnPointType.Air },
                        centerPoint,
                        commonCAPDB.DistanceFromCenter,
                        opposingPoint,
                        new MinMaxD(commonCAPDB.MinDistanceFromOpposingPoint, 99999),
                        GeneratorTools.GetSpawnPointCoalition(template, side));

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {coalition} combat air patrols.", LogMessageErrorLevel.Warning);
                    return;
                }

                Coordinates groupDestination = destination + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;

                var extraSettings = new Dictionary<string, object>{
                    {"Payload", "air-to-air"},
                    {"GroupX2", groupDestination.X},
                    {"GroupY2", groupDestination.Y}
                };

                var luaGroup = commonCAPDB.LuaGroup;
                var spawnpointCoordinates = spawnPoint.Value;
                if (template.MissionFeatures.Contains("ContextGroundStartAircraft"))
                {
                    luaGroup += "Parked";
                    var (airbase, parkingSpotIDsList, parkingSpotCoordinatesList) = unitMaker.SpawnPointSelector.GetAirbaseAndParking(template, spawnPoint.Value, groupSize, coalition, false);
                    spawnpointCoordinates = airbase.Coordinates;
                    extraSettings.AddIfKeyUnused("ParkingID", parkingSpotIDsList.ToArray());
                    extraSettings.AddIfKeyUnused("GroupAirbaseID", airbase.DCSID);
                    extraSettings.AddIfKeyUnused("UnitX", (from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.X).ToArray());
                    extraSettings.AddIfKeyUnused("UnitY", (from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.Y).ToArray());
                }


                UnitMakerGroupInfo? groupInfo = unitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(commonCAPDB.UnitFamilies), groupSize, side,
                    luaGroup, commonCAPDB.LuaUnit,
                    spawnpointCoordinates,
                    0,
                    extraSettings.ToArray());

                if (!groupInfo.HasValue) // Failed to generate a group
                    BriefingRoom.PrintToLog($"Failed to find units for {coalition} air defense unit group.", LogMessageErrorLevel.Warning);

                capAircraftGroupIDs.Add(groupInfo.Value.GroupID);
            } while (unitsLeftToSpawn > 0);
        }
    }
}
