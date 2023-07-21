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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal abstract class MissionGeneratorFeatures<T> where T : DBEntryFeature
    {
        protected readonly UnitMaker _unitMaker;

        protected readonly MissionTemplateRecord _template;


        private int TACANIndex = 1;

        internal MissionGeneratorFeatures(UnitMaker unitMaker, MissionTemplateRecord template)
        {
            _unitMaker = unitMaker;
            _template = template;
        }

        protected UnitMakerGroupInfo? AddMissionFeature(T featureDB, DCSMission mission, Coordinates? coordinates, Coordinates? coordinates2, ref Dictionary<string, object> extraSettings, Side? objectiveTargetSide = null, bool hideEnemy = false, UnitFamily? preSelectedUnitFamily = null)
        {
            // Add secondary coordinates (destination point) to the extra settings

            // Feature unit group
            UnitMakerGroupInfo? groupInfo = null;
            if (FeatureHasUnitGroup(featureDB))
            {
                if (!coordinates2.HasValue) coordinates2 = coordinates; // No destination point? Use initial point
                extraSettings.AddIfKeyUnused("GroupX2", coordinates2.Value.X);
                extraSettings.AddIfKeyUnused("GroupY2", coordinates2.Value.Y);
                var TACANStr = GetExtraSettingsFromFeature(featureDB, ref extraSettings); // Add specific settings for this feature (TACAN frequencies, etc)
                var coordinatesValue = coordinates.Value;
                UnitMakerGroupFlags groupFlags = 0;
                var flags = featureDB.UnitGroupFlags;
                if (flags.HasFlag(FeatureUnitGroupFlags.Immortal))
                    groupFlags |= UnitMakerGroupFlags.Immortal;

                if (flags.HasFlag(FeatureUnitGroupFlags.StaticAircraft))
                    groupFlags |= UnitMakerGroupFlags.StaticAircraft;

                if (flags.HasFlag(FeatureUnitGroupFlags.Inert))
                    groupFlags |= UnitMakerGroupFlags.Inert;

                if (flags.HasFlag(FeatureUnitGroupFlags.Invisible))
                    groupFlags |= UnitMakerGroupFlags.Invisible;


                if (flags.HasFlag(FeatureUnitGroupFlags.ImmediateAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.ImmediateAircraftSpawn;

                if (_template.MissionFeatures.Contains("ContextScrambleStart"))
                    groupFlags |= UnitMakerGroupFlags.ScrambleStart;

                if (flags.HasFlag(FeatureUnitGroupFlags.RadioAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.RadioAircraftSpawn;

                if (flags.HasFlag(FeatureUnitGroupFlags.LowUnitVariation))
                    groupFlags |= UnitMakerGroupFlags.LowUnitVariation;

                Side groupSide = Side.Enemy;
                if (flags.HasFlag(FeatureUnitGroupFlags.Friendly)) groupSide = Side.Ally;
                else if (flags.HasFlag(FeatureUnitGroupFlags.Neutral)) groupSide = Side.Neutral;
                else if (flags.HasFlag(FeatureUnitGroupFlags.SameSideAsTarget) && objectiveTargetSide.HasValue) groupSide = objectiveTargetSide.Value;

                if (hideEnemy && groupSide == Side.Enemy)
                    groupFlags |= UnitMakerGroupFlags.AlwaysHidden;

                extraSettings.AddIfKeyUnused("DCSTask", featureDB.UnitGroupTask);

                var groupLua = featureDB.UnitGroupLuaGroup;
                var unitCount = featureDB.UnitGroupSize.GetValue();
                var unitFamily = preSelectedUnitFamily.HasValue ? preSelectedUnitFamily.Value : Toolbox.RandomFrom(featureDB.UnitGroupFamilies);
                var luaUnit = featureDB.UnitGroupLuaUnit;
                var (units, unitDBs) = _unitMaker.GetUnits(unitFamily, unitCount, groupSide, groupFlags, ref extraSettings);
                if(units.Count == 0)
                {
                    _unitMaker.SpawnPointSelector.RecoverSpawnPoint(coordinatesValue);
                    return groupInfo;
                }
                var unitDB = unitDBs.First();
                SetAirbase(featureDB, ref mission, unitDB, ref groupLua, ref luaUnit, groupSide, ref coordinatesValue, coordinates2.Value, unitCount, ref extraSettings);

                groupInfo = _unitMaker.AddUnitGroup(
                    units,
                    groupSide,
                    unitFamily,
                    groupLua, luaUnit,
                    coordinatesValue, groupFlags,
                    extraSettings);

                SetCarrier(featureDB, groupSide, ref groupInfo);
                SetSupportingTargetGroupName(ref groupInfo, flags, extraSettings);
                if (
                    groupSide == Side.Ally &&
                    groupInfo.HasValue &&
                    groupInfo.Value.UnitDB != null &&
                    groupInfo.Value.UnitDB.IsAircraft &&
                    !flags.HasFlag(FeatureUnitGroupFlags.StaticAircraft))
                    mission.Briefing.AddItem(DCSMissionBriefingItemType.FlightGroup,
                            $"{groupInfo.Value.Name.Split("-")[0]}\t" +
                            $"{unitCount}× {groupInfo.Value.UnitDB.UIDisplayName.Get()}\t" +
                            $"{GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency)}{TACANStr}\t" +
                            $"{featureDB.UnitGroupTask}"); // TODO: human-readable payload name

                if (!groupInfo.Value.UnitDB.IsAircraft)
                    mission.MapData.Add($"UNIT-{groupInfo.Value.UnitDB.Families[0]}-{groupSide}-{groupInfo.Value.GroupID}", new List<double[]> { groupInfo.Value.Coordinates.ToArray() });

                if (featureDB.ExtraGroups.Max > 1)
                    SpawnExtraGroups(featureDB, mission, groupSide, groupFlags, coordinatesValue, coordinates2.Value, extraSettings);
            }

            // Feature Lua script
            string featureLua = "";

            // Adds the features' group ID to the briefingRoom.mission.missionFeatures.groupsID table
            if (this is MissionGeneratorFeaturesMission)
            {
                featureLua += $"briefingRoom.mission.missionFeatures.groupNames.{GeneratorTools.LowercaseFirstCharacter(featureDB.ID)} = \"{(groupInfo.HasValue ? groupInfo.Value.Name : 0)}\"\n";
                featureLua += $"briefingRoom.mission.missionFeatures.unitNames.{GeneratorTools.LowercaseFirstCharacter(featureDB.ID)} = {{{(groupInfo.HasValue ? string.Join(",", groupInfo.Value.UnitNames.Select(x => $"\"{x}\"")) : "")}}}\n";
            }

            if (!string.IsNullOrEmpty(featureDB.IncludeLuaSettings)) featureLua = featureDB.IncludeLuaSettings + "\n";
            foreach (string luaFile in featureDB.IncludeLua)
            {
                var fileLua = Toolbox.ReadAllTextIfFileExists(Path.Combine(featureDB.SourceLuaDirectory, luaFile));
                if (fileLua.StartsWith("-- BR SINGLETON FLAG"))
                { // Script should be used only once in the app and should be ordered infront of all feature scripts
                    mission.AppendSingletonValue(luaFile, "ScriptSingletons", fileLua);
                    continue;
                }
                featureLua += fileLua + "\n";
            }
            foreach (KeyValuePair<string, object> extraSetting in extraSettings)
                GeneratorTools.ReplaceKey(ref featureLua, extraSetting.Key, extraSetting.Value);
            if (groupInfo.HasValue)
                GeneratorTools.ReplaceKey(ref featureLua, "FeatureGroupID", groupInfo.Value.GroupID);

            if (featureDB is DBEntryFeatureObjective) mission.AppendValue("ScriptObjectivesFeatures", featureLua);
            else mission.AppendValue("ScriptMissionFeatures", featureLua);

            // Add feature ogg files
            foreach (string oggFile in featureDB.IncludeOgg)
                mission.AddMediaFile($"l10n/DEFAULT/{oggFile}", Path.Combine(BRPaths.INCLUDE_OGG, oggFile));

            if (!String.IsNullOrEmpty(featureDB.IncludeOggFolder))
                mission.AddMediaFolder(featureDB.IncludeOggFolder, Path.Combine(BRPaths.INCLUDE_OGG, featureDB.IncludeOggFolder));

            return groupInfo;
        }

        protected void AddBriefingRemarkFromFeature(T featureDB, DCSMission mission, bool useEnemyRemarkIfAvailable, UnitMakerGroupInfo? groupInfo, Dictionary<string, object> stringReplacements)
        {
            string remarkString;
            if (useEnemyRemarkIfAvailable && !string.IsNullOrEmpty(featureDB.BriefingRemarks[(int)Side.Enemy].Get()))
                remarkString = featureDB.BriefingRemarks[(int)Side.Enemy].Get();
            else
                remarkString = featureDB.BriefingRemarks[(int)Side.Ally].Get();
            if (string.IsNullOrEmpty(remarkString)) return; // No briefing remarks for this feature

            string remark = Toolbox.RandomFrom(remarkString.Split(";"));
            foreach (KeyValuePair<string, object> stringReplacement in stringReplacements)
                GeneratorTools.ReplaceKey(ref remark, stringReplacement.Key, stringReplacement.Value.ToString());

            if (groupInfo.HasValue)
            {
                GeneratorTools.ReplaceKey(ref remark, "GroupName", groupInfo.Value.Name);
                GeneratorTools.ReplaceKey(ref remark, "GroupFrequency", GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency));
                GeneratorTools.ReplaceKey(ref remark, "GroupUnitName", groupInfo.Value.UnitDB.UIDisplayName);
            }

            mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark, featureDB is DBEntryFeatureMission);
        }


        private string GetExtraSettingsFromFeature(T featureDB, ref Dictionary<string, object> extraSettings)
        {
            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.TACAN) && (featureDB.UnitGroupFamilies.Length > 0))
            {
                var callsign = (GetType() == typeof(MissionGeneratorFeaturesObjectives) && extraSettings.ContainsKey("ObjectiveName")) ? extraSettings["ObjectiveName"].ToString().Substring(0, 3) : $"{GeneratorTools.GetTACANCallsign(featureDB.UnitGroupFamilies[0])}{TACANIndex}";
                if (extraSettings.ContainsKey("TACAN_NAME"))
                    callsign = extraSettings["TACAN_NAME"].ToString().Substring(0, 3);
                var channel = ((GetType() == typeof(MissionGeneratorFeaturesObjectives)) ? 31 : 25) + TACANIndex;
                extraSettings.AddIfKeyUnused("TACANFrequency", 1108000000);
                extraSettings.AddIfKeyUnused("TACANCallsign", callsign);
                extraSettings.AddIfKeyUnused("TACANChannel", channel);
                if (TACANIndex < 9) TACANIndex++;
                return $",\n{channel}X (callsign {callsign})";
            }
            return "";
        }

        internal bool FeatureHasUnitGroup(T featureDB)
        {
            return (featureDB.UnitGroupFamilies.Length > 0) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaGroup) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaUnit);
        }

        private void SpawnExtraGroups(T featureDB, DCSMission mission, Side groupSide, UnitMakerGroupFlags groupFlags, Coordinates coordinates, Coordinates coordinates2, Dictionary<string, object> extraSettings)
        {
            var flags = featureDB.UnitGroupFlags;
            foreach (var i in Enumerable.Range(1, featureDB.ExtraGroups.GetValue()))
            {
                if (flags.HasFlag(FeatureUnitGroupFlags.MoveAnyWhere))
                {
                    coordinates = coordinates.CreateNearRandom(50 * Toolbox.NM_TO_METERS, 100 * Toolbox.NM_TO_METERS);
                    coordinates2 = coordinates.CreateNearRandom(50 * Toolbox.NM_TO_METERS, 100 * Toolbox.NM_TO_METERS);
                    extraSettings["GroupX2"] = coordinates2.X;
                    extraSettings["GroupY2"] = coordinates2.Y;
                }
                var groupLua = featureDB.UnitGroupLuaGroup;
                var unitCount = featureDB.UnitGroupSize.GetValue();
                var unitFamily = Toolbox.RandomFrom(featureDB.UnitGroupFamilies);
                var luaUnit = featureDB.UnitGroupLuaUnit;
                Coordinates? spawnCoords;
                if (flags.HasFlag(FeatureUnitGroupFlags.ExtraGroupsNearby))
                    spawnCoords = _unitMaker.SpawnPointSelector.GetNearestSpawnPoint(featureDB.UnitGroupValidSpawnPoints, coordinates);
                else
                    spawnCoords = _unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        featureDB.UnitGroupValidSpawnPoints, coordinates,
                        new MinMaxD(0, 5),
                        coalition: GeneratorTools.GetSpawnPointCoalition(_template, groupSide),
                        nearFrontLineFamily: (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.UseFrontLine) ? unitFamily : null)
                        );

                if (!spawnCoords.HasValue)
                    continue;


                var (units, unitDBs) = _unitMaker.GetUnits(unitFamily, unitCount, groupSide, groupFlags, ref extraSettings);
                if(units.Count() == 0)
                {
                    _unitMaker.SpawnPointSelector.RecoverSpawnPoint(spawnCoords.Value);
                    continue;
                }
                var unitDB = unitDBs.First();
                SetAirbase(featureDB, ref mission, unitDB, ref groupLua, ref luaUnit, groupSide, ref coordinates, coordinates2, unitCount, ref extraSettings);

                var groupInfo = _unitMaker.AddUnitGroup(
                    units,
                    groupSide,
                    unitFamily,
                    groupLua, luaUnit,
                    spawnCoords.Value, groupFlags,
                    extraSettings);

                SetCarrier(featureDB, groupSide, ref groupInfo);
                SetSupportingTargetGroupName(ref groupInfo, flags, extraSettings);


                if (
                   groupSide == Side.Ally &&
                   groupInfo.HasValue &&
                   groupInfo.Value.UnitDB != null &&
                   groupInfo.Value.UnitDB.IsAircraft &&
                   !flags.HasFlag(FeatureUnitGroupFlags.StaticAircraft))
                    mission.Briefing.AddItem(DCSMissionBriefingItemType.FlightGroup,
                            $"{groupInfo.Value.Name.Split("-")[0]}\t" +
                            $"{unitCount}× {groupInfo.Value.UnitDB.UIDisplayName.Get()}\t" +
                            $"{GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency)}\t" +
                            $"{featureDB.UnitGroupTask}");
                if (!groupInfo.Value.UnitDB.IsAircraft)
                    mission.MapData.Add($"UNIT-{groupInfo.Value.UnitDB.Families[0]}-{groupSide}-{groupInfo.Value.GroupID}", new List<double[]> { groupInfo.Value.Coordinates.ToArray() });
            }
        }

        private void SetAirbase(T featureDB, ref DCSMission mission, DBEntryJSONUnit unitDB, ref string groupLua, ref string luaUnit, Side groupSide, ref Coordinates coordinates, Coordinates coordinates2, int unitCount, ref Dictionary<string, object> extraSettings)
        {
            if ((_template.MissionFeatures.Contains("ContextGroundStartAircraft") || featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.GroundStart)) && unitDB.IsAircraft && featureDB.UnitGroupLuaGroup != "AircraftEscort")
            {
                var coalition = GeneratorTools.GetSpawnPointCoalition(_template, groupSide, true).Value;
                var (airbase, parkingSpotIDsList, parkingSpotCoordinatesList) = _unitMaker.SpawnPointSelector.GetAirbaseAndParking(
                    _template, coordinates, unitCount,
                    coalition,
                    (DBEntryAircraft)unitDB);
                coordinates = airbase.Coordinates;
                extraSettings["ParkingID"] = parkingSpotIDsList;
                extraSettings["GroupAirbaseID"] = airbase.DCSID;
                mission.PopulatedAirbaseIds[coalition].Add(airbase.DCSID);
                extraSettings["UnitCoords"] = parkingSpotCoordinatesList;

                var midPoint = Coordinates.Lerp(coordinates, coordinates2, 0.4);
                extraSettings.AddIfKeyUnused("GroupMidX", midPoint.X);
                extraSettings.AddIfKeyUnused("GroupMidY", midPoint.Y);
                mission.MapData.AddIfKeyUnused($"AIRBASE_AI_{groupSide}_${airbase.Name}", new List<double[]> { airbase.Coordinates.ToArray() });
            }
        }

        private void SetCarrier(T featureDB, Side side, ref UnitMakerGroupInfo? groupInfo)
        {

            if (
                side == Side.Enemy ||
                (!_template.MissionFeatures.Contains("ContextGroundStartAircraft") && featureDB.ID != "FriendlyStaticAircraftCarrier") ||
                true //groupInfo.Value.UnitDB.AircraftData.CarrierTypes.Count() == 0
            )
                return;

            UnitFamily targetFamily = UnitFamily.ShipCarrierSTOVL;
            if (groupInfo.Value.UnitDB.Families.Contains(UnitFamily.PlaneCATOBAR))
                targetFamily = UnitFamily.ShipCarrierCATOBAR;
            if (groupInfo.Value.UnitDB.Families.Contains(UnitFamily.PlaneSTOBAR))
                targetFamily = UnitFamily.ShipCarrierSTOBAR;
            var unitCount = groupInfo.Value.DCSGroup.Units.Count;
            var carrierPool = _unitMaker.carrierDictionary.Where(x =>
                    x.Value.UnitMakerGroupInfo.UnitDB.Families.Contains(targetFamily) &&
                    x.Value.RemainingSpotCount >= unitCount
                ).ToDictionary(x => x.Key, x => x.Value);

            if (carrierPool.Count == 0)
                return;

            var carrier = Toolbox.RandomFrom(carrierPool.Values.ToArray());
            groupInfo.Value.DCSGroup.Waypoints[0].LinkUnit = carrier.UnitMakerGroupInfo.DCSGroup.Units[0].UnitId;
            groupInfo.Value.DCSGroup.Waypoints[0].HelipadId = carrier.UnitMakerGroupInfo.DCSGroup.Units[0].UnitId;
            groupInfo.Value.DCSGroup.Waypoints[0].X = (float)carrier.UnitMakerGroupInfo.Coordinates.X;
            groupInfo.Value.DCSGroup.Waypoints[0].Y = (float)carrier.UnitMakerGroupInfo.Coordinates.Y;
            groupInfo.Value.DCSGroup.X = (float)carrier.UnitMakerGroupInfo.Coordinates.X;
            groupInfo.Value.DCSGroup.Y = (float)carrier.UnitMakerGroupInfo.Coordinates.Y;
            groupInfo.Value.DCSGroup.Name = groupInfo.Value.DCSGroup.Name.Replace("-STATIC-", ""); // Remove Static code if on carrier as we can't replace it automatically
            carrier.RemainingSpotCount = carrier.RemainingSpotCount - unitCount;
        }

        private void SetSupportingTargetGroupName(ref UnitMakerGroupInfo? groupInfo, FeatureUnitGroupFlags flags, Dictionary<string, object> extraSettings)
        {
            if (flags.HasFlag(FeatureUnitGroupFlags.SupportingTarget))
                groupInfo.Value.DCSGroups.ForEach(x => x.Name += $"-STGT-{extraSettings["ObjectiveName"].ToString()}");
        }

    }
}
