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
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorFeaturesObjectives : MissionGeneratorFeatures<DBEntryFeatureObjective>
    {
        private int prevLaserCode { get; set; } = 1687;
        internal MissionGeneratorFeaturesObjectives(UnitMaker unitMaker, MissionTemplateRecord template) : base(unitMaker, template) { }

        internal void GenerateMissionFeature(DCSMission mission, string featureID, string objectiveName, int objectiveIndex, int objectiveGroupID, Coordinates objectiveCoordinates, Side objectiveTargetSide, bool hideEnemy = false)
        {
            DBEntryFeatureObjective featureDB = Database.Instance.GetEntry<DBEntryFeatureObjective>(featureID);
            if (featureDB == null) // Feature doesn't exist
            {
                BriefingRoom.PrintToLog($"Objective feature {featureID} not found.", LogMessageErrorLevel.Warning);
                return;
            }

            double spawnDistance = Math.Max(1.0, featureDB.UnitGroupSpawnDistance) * Toolbox.NM_TO_METERS;

            Coordinates? coordinates = null;
            Coordinates? coordinates2 = null;

            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.SpawnOnObjective))
            {
                coordinates = objectiveCoordinates + Coordinates.CreateRandom(10, 50);
                if (
                    !(featureDB.UnitGroupValidSpawnPoints.Contains(SpawnPointType.Sea) || featureDB.UnitGroupValidSpawnPoints.Contains(SpawnPointType.Air)) &&
                    _unitMaker.SpawnPointSelector.CheckInSea(coordinates.Value))
                {
                    BriefingRoom.PrintToLog($"Can't spawn objective feature {featureID}, Invalid spawn.", LogMessageErrorLevel.Warning);
                    return;
                }
            }
            else if (FeatureHasUnitGroup(featureDB))
            {
                Coordinates? spawnPoint =
                    _unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        featureDB.UnitGroupValidSpawnPoints, objectiveCoordinates,
                        new MinMaxD(spawnDistance * .75, spawnDistance * 1.5));

                if (!spawnPoint.HasValue) // No spawn point found
                {
                    BriefingRoom.PrintToLog($"No spawn point found for objective feature {featureID}.", LogMessageErrorLevel.Warning);
                    return;
                }

                coordinates = spawnPoint;
            }

            if (coordinates.HasValue)
                coordinates2 = coordinates.Value + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;

            Dictionary<string, object> extraSettings = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            extraSettings.AddIfKeyUnused("ObjectiveName", objectiveName);
            extraSettings.AddIfKeyUnused("ObjectiveIndex", objectiveIndex + 1);
            extraSettings.AddIfKeyUnused("ObjectiveGroupID", objectiveGroupID);

            if (featureID == "TargetDesignationLaser")
            {
                var laserCode = _template.OptionsMission.Contains("SingleLaserCode") || _template.OptionsMission.Contains("FC3LaserCode") ? (_template.OptionsMission.Contains("FC3LaserCode") ? 1113 : 1688) : getNextLaserCode();
                extraSettings.AddIfKeyUnused("LASERCODE", laserCode);
                mission.Briefing.AddItem(DCSMissionBriefingItemType.JTAC, $"{objectiveName}\t{laserCode}");
            }

            UnitMakerGroupInfo? groupInfo = AddMissionFeature(
                featureDB, mission,
                coordinates, coordinates2,
                ref extraSettings, objectiveTargetSide, hideEnemy);

            AddBriefingRemarkFromFeature(featureDB, mission, false, groupInfo, extraSettings);
        }

        private int getNextLaserCode()
        {
            var code = prevLaserCode;
            code++;
            var digits = GetDigits(code).ToList();
            if (digits.Last() == 9)
                code += 2;
            digits = GetDigits(code).ToList();
            if (digits[2] == 9)
                code += 20;
            digits = GetDigits(code).ToList();
            if (code >= 1788)
                code = 1511;
            prevLaserCode = code;
            return code;
        }

        private static IEnumerable<int> GetDigits(int source)
        {
            Stack<int> digits = new Stack<int>();
            while (source > 0)
            {
                var digit = source % 10;
                source /= 10;
                digits.Push(digit);
            }

            return digits;
        }
    }
}
