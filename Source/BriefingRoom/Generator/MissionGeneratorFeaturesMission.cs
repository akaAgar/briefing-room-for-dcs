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
    internal class MissionGeneratorFeaturesMission : MissionGeneratorFeatures<DBEntryFeatureMission>
    {
        internal MissionGeneratorFeaturesMission(UnitMaker unitMaker, MissionTemplateRecord template) : base(unitMaker, template) { }

        internal void GenerateMissionFeature(DCSMission mission, string featureID, Coordinates initialCoordinates, Coordinates objectivesCenter)
        {
            DBEntryFeatureMission featureDB = Database.Instance.GetEntry<DBEntryFeatureMission>(featureID);
            if (featureDB == null) // Feature doesn't exist
            {
                BriefingRoom.PrintToLog($"Mission feature {featureID} not found.", LogMessageErrorLevel.Warning);
                return;
            }
            Coalition coalition = featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Friendly) ? _template.ContextPlayerCoalition : _template.ContextPlayerCoalition.GetEnemy();

            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.ForEachAirbase))
            {
                ForEachAirbase(mission, featureID, featureDB, coalition);
                return;
            }

            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.ForEachFOB))
            {
                ForEachFob(mission, featureID, featureDB);
                return;
            }
            Coordinates? spawnPoint = null;
            Coordinates? coordinates2 = null;
            if (FeatureHasUnitGroup(featureDB))
            {
                Coordinates pointSearchCenter = Coordinates.Lerp(initialCoordinates, objectivesCenter, featureDB.UnitGroupSpawnDistance);
                spawnPoint =
                    _unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        featureDB.UnitGroupValidSpawnPoints, pointSearchCenter,
                        featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.AwayFromMissionArea) ? new MinMaxD(50, 100) : new MinMaxD(0, 10),
                        coalition: (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.IgnoreBorders) || featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Neutral)) ? null : coalition
                        );
                if (!spawnPoint.HasValue) // No spawn point found
                {
                    BriefingRoom.PrintToLog($"No spawn point found for mission feature {featureID}.", LogMessageErrorLevel.Warning);
                    return;
                }


                var goPoint = spawnPoint.Value;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.MoveTowardObjectives))
                    goPoint = objectivesCenter;
                else if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.MoveAnyWhere))
                    goPoint = goPoint.CreateNearRandom(50 * Toolbox.NM_TO_METERS, 100 * Toolbox.NM_TO_METERS);
                else if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.MoveTowardPlayerBase))
                    goPoint = initialCoordinates;

                coordinates2 = goPoint + Coordinates.CreateRandom(5, 20) * Toolbox.NM_TO_METERS;
            }
            Dictionary<string, object> extraSettings = new Dictionary<string, object>();
            UnitMakerGroupInfo? groupInfo = AddMissionFeature(featureDB, mission, spawnPoint, coordinates2, ref extraSettings);

            AddBriefingRemarkFromFeature(featureDB, mission, false, groupInfo, extraSettings);
        }

        private void ForEachAirbase(DCSMission mission, string featureID, DBEntryFeatureMission featureDB, Coalition coalition)
        {
            var activeAirbases = mission.PopulatedAirbaseIds[coalition];
            var airbases = Database.Instance.GetAllEntries<DBEntryAirbase>().Where(x => x.Theater == mission.TheaterID.ToLower() && activeAirbases.Contains(x.DCSID)).ToList();
            foreach (DBEntryAirbase airbase in airbases)
            {

                Coordinates? spawnPoint = _unitMaker.SpawnPointSelector.GetNearestSpawnPoint(featureDB.UnitGroupValidSpawnPoints, airbase.Coordinates);
                if (!spawnPoint.HasValue) // No spawn point found
                {
                    BriefingRoom.PrintToLog($"No spawn point found for mission feature {featureID}.", LogMessageErrorLevel.Warning);
                    return;
                }

                Dictionary<string, object> extraSettings = new Dictionary<string, object> { { "TACAN_NAME", airbase.Name } };
                UnitMakerGroupInfo? groupInfo = AddMissionFeature(featureDB, mission, spawnPoint.Value, spawnPoint.Value, ref extraSettings);

                AddBriefingRemarkFromFeature(featureDB, mission, false, groupInfo, extraSettings);
            }
        }

        private void ForEachFob(DCSMission mission, string featureID, DBEntryFeatureMission featureDB)
        {
            var fobs = _unitMaker.carrierDictionary
                .Where(x => x.Value.UnitMakerGroupInfo.UnitDB.Families.Contains(UnitFamily.FOB))
                .Select(x => x.Value)
                .ToList();
            foreach (var fob in fobs)
            {

                var coordinates = fob.UnitMakerGroupInfo.Coordinates + Coordinates.CreateRandom(30, 100);
                Dictionary<string, object> extraSettings = new Dictionary<string, object> { { "TACAN_NAME", fob.UnitMakerGroupInfo.Name } };
                UnitMakerGroupInfo? groupInfo = AddMissionFeature(featureDB, mission, coordinates, coordinates, ref extraSettings);

                AddBriefingRemarkFromFeature(featureDB, mission, false, groupInfo, extraSettings);

            }
        }
    }

}