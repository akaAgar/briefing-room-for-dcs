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
using System;
using System.Collections.Generic;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates units and Lua script associated with objective features.
    /// </summary>
    internal class MissionGeneratorFeaturesObjectives : MissionGeneratorFeatures<DBEntryFeatureObjective>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker to use for unit generation.</param>
        internal MissionGeneratorFeaturesObjectives(UnitMaker unitMaker) : base(unitMaker) { }

        internal void GenerateMissionFeature(DCSMission mission, string featureID, string objectiveName, int objectiveIndex, int objectiveGroupID, Coordinates objectiveCoordinates)
        {
            DBEntryFeatureObjective featureDB = Database.Instance.GetEntry<DBEntryFeatureObjective>(featureID);
            if (featureDB == null) // Feature doesn't exist
            {
                BriefingRoom.PrintToLog($"Objective feature {featureID} not found.", LogMessageErrorLevel.Warning);
                return;
            }

            double spawnDistance = Math.Max(1.0, featureDB.UnitGroupSpawnDistance) * Toolbox.NM_TO_METERS;

            DBEntryTheaterSpawnPoint? spawnPoint =
                UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                    featureDB.UnitGroupValidSpawnPoints, objectiveCoordinates,
                    new MinMaxD(spawnDistance * .75, spawnDistance * 1.5));

            if (!spawnPoint.HasValue) // No spawn point found
            {
                BriefingRoom.PrintToLog($"No spawn point found for objective feature {featureID}.", LogMessageErrorLevel.Warning);
                return;
            }

            Coordinates coordinates2 = spawnPoint.Value.Coordinates + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;

            Dictionary<string, object> extraSettings = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            extraSettings.AddIfKeyUnused("ObjectiveName", objectiveName);
            extraSettings.AddIfKeyUnused("ObjectiveIndex", objectiveIndex + 1);
            extraSettings.AddIfKeyUnused("ObjectiveGroupID", objectiveGroupID);

            UnitMakerGroupInfo? groupInfo = AddMissionFeature(
                mission, featureDB,
                spawnPoint.Value.Coordinates, coordinates2,
                ref extraSettings);

            AddBriefingRemarkFromFeature(mission, featureDB, false, groupInfo, extraSettings);
        }
    }
}
