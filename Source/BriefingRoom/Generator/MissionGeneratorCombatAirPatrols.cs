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
        /// Possible number of units in a CAP group.
        /// </summary>
        private static readonly int[] CAP_GROUP_SIZE = new int[] { 1, 2, 2, 2, 2, 3, 4, 4 };

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
            //DBCommonCAPLevel capLevelDB = Database.Instance.Common.CAP.CAPLevels[(int)capAmount];

            //int unitsLeftToSpawn = capLevelDB.AircraftCount.GetValue();
            int unitsLeftToSpawn = new MinMaxI(4, 8).GetValue();
            if (unitsLeftToSpawn < 1) return;  // No groups to add, no need to go any further

            do
            {
                int groupSize = Toolbox.RandomFrom(CAP_GROUP_SIZE);
                groupSize = Math.Min(unitsLeftToSpawn, groupSize);
                unitsLeftToSpawn -= groupSize;

                // Find spawn point at the proper distance from the objective(s), but not to close from starting airbase
                DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        null,
                        centerPoint,
                        Database.Instance.Common.AirDefense.DistanceFromCenter[(int)side, (int)AirDefenseRange.LongRange],
                        opposingPoint,
                        new MinMaxD(Database.Instance.Common.AirDefense.MinDistanceFromOpposingPoint[(int)side, (int)AirDefenseRange.LongRange], 99999),
                        coalition);

                // No spawn point found, stop here.
                if (!spawnPoint.HasValue)
                {
                    BriefingRoom.PrintToLog($"No spawn point found for {coalition} combat air patrols.", LogMessageErrorLevel.Warning);
                    return;
                }

                Coordinates coordinates2 = spawnPoint.Value.Coordinates + Coordinates.CreateRandom(20.0, 30.0) * Toolbox.NM_TO_METERS;

                UnitMakerGroupInfo? groupInfo = UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(CAP_UNIT_FAMILIES), groupSize, side,
                    "GroupAircraftCAP", "UnitAircraft",
                    spawnPoint.Value.Coordinates,
                    //Toolbox.RandomFrom(capLevelDB.SkillLevel));
                    Toolbox.RandomFrom(DCSSkillLevel.Good), 0, AircraftPayload.AirToAir,
                    "GroupX2".ToKeyValuePair(coordinates2.X),
                    "GroupY2".ToKeyValuePair(coordinates2.Y));

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
