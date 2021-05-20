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
    /// <summary>
    /// Generates friendly and enemy combat air patrols in the mission area.
    /// </summary>
    internal class MissionGeneratorCombatAirPatrols : IDisposable
    {
        /// <summary>
        /// Shorcut to <see cref="Database.Instance.Common.CAP"/>
        /// </summary>
        private DBCommonCAP CommonCAPDB { get { return Database.Instance.Common.CAP; } }

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

                CreateCAPGroups(
                    side, coalition, capAmount,
                    centerPoint, opposingPoint,
                    objectivesCenter,
                    ref capAircraftGroupIDs);
            }

            return capAircraftGroupIDs.ToArray();
        }

        private void CreateCAPGroups(Side side, Coalition coalition, AmountNR capAmount, Coordinates centerPoint, Coordinates opposingPoint, Coordinates destination, ref List<int> capAircraftGroupIDs)
        {
            DBCommonCAPLevel capLevelDB = CommonCAPDB.CAPLevels[(int)capAmount];

            int unitsLeftToSpawn = capLevelDB.UnitCount.GetValue();
            if (unitsLeftToSpawn < 1) return;  // No groups to add, no need to go any further

            do
            {
                int groupSize = Toolbox.RandomFrom(CommonCAPDB.GroupSize);
                groupSize = Math.Min(unitsLeftToSpawn, groupSize);
                unitsLeftToSpawn -= groupSize;

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        null,
                        centerPoint,
                        CommonCAPDB.DistanceFromCenter,
                        opposingPoint,
                        new MinMaxD(CommonCAPDB.MinDistanceFromOpposingPoint, 99999),
                        coalition);

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {coalition} combat air patrols.", LogMessageErrorLevel.Warning);
                    return;
                }

                Coordinates groupDestination = destination + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;

                UnitMakerGroupInfo? groupInfo = UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(CommonCAPDB.UnitFamilies), groupSize, side,
                    CommonCAPDB.LuaGroup, CommonCAPDB.LuaUnit,
                    spawnPoint.Value.Coordinates,
                    Toolbox.RandomFrom(capLevelDB.SkillLevel), 0, AircraftPayload.AirToAir,
                    "GroupX2".ToKeyValuePair(groupDestination.X),
                    "GroupY2".ToKeyValuePair(groupDestination.Y));

                if (!groupInfo.HasValue) // Failed to generate a group
                    BriefingRoom.PrintToLog($"Failed to find units for {coalition} air defense unit group.", LogMessageErrorLevel.Warning);

                capAircraftGroupIDs.Add(groupInfo.Value.GroupID);
            } while (unitsLeftToSpawn > 0);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
