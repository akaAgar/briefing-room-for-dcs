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

        protected UnitMakerGroupInfo? AddMissionFeature(T featureDB, DCSMission mission, Coordinates coordinates, Coordinates? coordinates2, ref Dictionary<string, object> extraSettings, Side? objectiveTargetSide = null, bool hideEnemy = false)
        {
            // Add secondary coordinates (destination point) to the extra settings
            if (!coordinates2.HasValue) coordinates2 = coordinates; // No destination point? Use initial point
            extraSettings.AddIfKeyUnused("GroupX2", coordinates2.Value.X);
            extraSettings.AddIfKeyUnused("GroupY2", coordinates2.Value.Y);
            var TACANStr = GetExtraSettingsFromFeature(featureDB, ref extraSettings); // Add specific settings for this feature (TACAN frequencies, etc)

            // Feature unit group
            UnitMakerGroupInfo? groupInfo = null;
            if (FeatureHasUnitGroup(featureDB))
            {
                UnitMakerGroupFlags groupFlags = 0;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Immortal))
                    groupFlags |= UnitMakerGroupFlags.Immortal;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Inert))
                    groupFlags |= UnitMakerGroupFlags.Inert;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Invisible))
                    groupFlags |= UnitMakerGroupFlags.Invisible;


                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.ImmediateAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.ImmediateAircraftSpawn;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.RadioAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.RadioAircraftSpawn;

                Side groupSide = Side.Enemy;
                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Friendly)) groupSide = Side.Ally;
                else if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.SameSideAsTarget) && objectiveTargetSide.HasValue) groupSide = objectiveTargetSide.Value;

                if (hideEnemy && groupSide == Side.Enemy)
                    groupFlags |= UnitMakerGroupFlags.AlwaysHidden;

                extraSettings.AddIfKeyUnused("Payload", featureDB.UnitGroupPayload);
                
                var groupLua = featureDB.UnitGroupLuaGroup;
                var unitCount = featureDB.UnitGroupSize.GetValue();
                var unitFamily = Toolbox.RandomFrom(featureDB.UnitGroupFamilies);
                var luaUnit = featureDB.UnitGroupLuaUnit;
                SetAirbase(featureDB, unitFamily, ref groupLua, ref luaUnit, groupSide, ref coordinates, coordinates2.Value, unitCount, ref extraSettings);

                groupInfo = _unitMaker.AddUnitGroup(
                    unitFamily, unitCount,
                    groupSide,
                    groupLua, luaUnit,
                    coordinates, groupFlags,
                    extraSettings);

                SetCarrier(featureDB, ref groupInfo);

                if (
                    groupSide == Side.Ally &&
                    groupInfo.HasValue &&
                    groupInfo.Value.UnitDB != null &&
                    groupInfo.Value.UnitDB.IsAircraft)
                    mission.Briefing.AddItem(DCSMissionBriefingItemType.FlightGroup,
                            $"{groupInfo.Value.Name}\t" +
                            $"{unitCount}× {groupInfo.Value.UnitDB.UIDisplayName}\t" +
                            $"{GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency)}{TACANStr}\t" +
                            $"{Toolbox.FormatPayload(featureDB.UnitGroupPayload)}"); // TODO: human-readable payload name

                if (featureDB.ExtraGroups.Max > 1)
                    SpawnExtraGroups(featureDB, mission, groupSide, groupFlags, coordinates, coordinates2.Value, extraSettings);
            }

            // Feature Lua script
            string featureLua = "";

            // Adds the features' group ID to the briefingRoom.mission.missionFeatures.groupsID table
            if (this is MissionGeneratorFeaturesMission)
            {
                featureLua += $"briefingRoom.mission.missionFeatures.groupsID.{GeneratorTools.LowercaseFirstCharacter(featureDB.ID)} = {(groupInfo.HasValue ? groupInfo.Value.GroupID : 0)}\n";
                featureLua += $"briefingRoom.mission.missionFeatures.unitsID.{GeneratorTools.LowercaseFirstCharacter(featureDB.ID)} = {{{(groupInfo.HasValue ? string.Join(",", groupInfo.Value.UnitsID) : "")}}}\n";
            }

            if (!string.IsNullOrEmpty(featureDB.IncludeLuaSettings)) featureLua = featureDB.IncludeLuaSettings + "\n";
            foreach (string luaFile in featureDB.IncludeLua)
                featureLua += Toolbox.ReadAllTextIfFileExists($"{featureDB.SourceLuaDirectory}{luaFile}") + "\n";
            foreach (KeyValuePair<string, object> extraSetting in extraSettings)
                GeneratorTools.ReplaceKey(ref featureLua, extraSetting.Key, extraSetting.Value);
            if (groupInfo.HasValue)
                GeneratorTools.ReplaceKey(ref featureLua, "FeatureGroupID", groupInfo.Value.GroupID);

            if (featureDB is DBEntryFeatureObjective) mission.AppendValue("ScriptObjectivesFeatures", featureLua);
            else mission.AppendValue("ScriptMissionFeatures", featureLua);

            // Add feature ogg files
            foreach (string oggFile in featureDB.IncludeOgg)
                mission.AddMediaFile($"l10n/DEFAULT/{oggFile}", $"{BRPaths.INCLUDE_OGG}{oggFile}");

            return groupInfo;
        }

        protected void AddBriefingRemarkFromFeature(T featureDB, DCSMission mission, bool useEnemyRemarkIfAvailable, UnitMakerGroupInfo? groupInfo, Dictionary<string, object> stringReplacements)
        {
            string[] remarks;
            if (useEnemyRemarkIfAvailable && featureDB.BriefingRemarks[(int)Side.Enemy].Length > 0)
                remarks = featureDB.BriefingRemarks[(int)Side.Enemy].ToArray();
            else
                remarks = featureDB.BriefingRemarks[(int)Side.Ally].ToArray();
            if (remarks.Length == 0) return; // No briefing remarks for this feature

            string remark = Toolbox.RandomFrom(remarks);
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
            // TODO: Improve
            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.TACAN) && (featureDB.UnitGroupFamilies.Length > 0))
            {
                var callsign = $"{GeneratorTools.GetTACANCallsign(featureDB.UnitGroupFamilies[0])}{TACANIndex}";
                var channel = ((GetType() == typeof(MissionGeneratorFeaturesObjectives)) ? 30 : 20) + TACANIndex;
                extraSettings.AddIfKeyUnused("TACANFrequency", 1108000000);
                extraSettings.AddIfKeyUnused("TACANCallsign", callsign);
                extraSettings.AddIfKeyUnused("TACANChannel", channel);
                if (TACANIndex < 9) TACANIndex++;
                return $",\n{channel}X (callsign {callsign})";
            }
            return "";
        }

        private bool FeatureHasUnitGroup(T featureDB)
        {
            return (featureDB.UnitGroupFamilies.Length > 0) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaGroup) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaUnit);
        }

        private void SpawnExtraGroups(T featureDB, DCSMission mission, Side groupSide, UnitMakerGroupFlags groupFlags, Coordinates coordinates, Coordinates coordinates2, Dictionary<string, object> extraSettings)
        {
            foreach (var i in Enumerable.Range(1, featureDB.ExtraGroups.GetValue()))
            {
                var groupLua = featureDB.UnitGroupLuaGroup;
                var unitCount = featureDB.UnitGroupSize.GetValue();
                var unitFamily = Toolbox.RandomFrom(featureDB.UnitGroupFamilies);
                var luaUnit = featureDB.UnitGroupLuaUnit;
                var spawnCoords = _unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                    featureDB.UnitGroupValidSpawnPoints, coordinates,
                    new MinMaxD(0, 5),
                    coalition: GeneratorTools.GetSpawnPointCoalition(_template, groupSide)
                    );
                if (!spawnCoords.HasValue)
                    continue;

                SetAirbase(featureDB, unitFamily, ref groupLua, ref luaUnit, groupSide, ref coordinates, coordinates2, unitCount, ref extraSettings);

                var groupInfo = _unitMaker.AddUnitGroup(
                   unitFamily, unitCount,
                   groupSide,
                   groupLua, luaUnit,
                   spawnCoords.Value, groupFlags,
                   extraSettings);

                SetCarrier(featureDB, ref groupInfo);

                
                 if (
                    groupSide == Side.Ally &&
                    groupInfo.HasValue &&
                    groupInfo.Value.UnitDB != null &&
                    groupInfo.Value.UnitDB.IsAircraft)
                    mission.Briefing.AddItem(DCSMissionBriefingItemType.FlightGroup,
                            $"{groupInfo.Value.Name}\t" +
                            $"{unitCount}× {groupInfo.Value.UnitDB.UIDisplayName}\t" +
                            $"{GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency)}\t" +
                            $"{Toolbox.FormatPayload(featureDB.UnitGroupPayload)}");
            }
        }

        private void SetAirbase(T featureDB, UnitFamily unitFamily, ref string groupLua, ref string luaUnit, Side groupSide, ref Coordinates coordinates, Coordinates coordinates2, int unitCount, ref Dictionary<string, object> extraSettings)
        {
            if ((_template.MissionFeatures.Contains("ContextGroundStartAircraft") || featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.GroundStart)) && Toolbox.IsAircraft(unitFamily.GetUnitCategory()))
            {
                var (airbase, parkingSpotIDsList, parkingSpotCoordinatesList) = _unitMaker.SpawnPointSelector.GetAirbaseAndParking(
                    _template, coordinates, unitCount,
                    GeneratorTools.GetSpawnPointCoalition(_template, groupSide, true).Value,
                    unitFamily);
                coordinates = airbase.Coordinates;
                extraSettings["ParkingID"] = parkingSpotIDsList;
                extraSettings["GroupAirbaseID"] = airbase.DCSID;
                extraSettings["UnitCoords"] = parkingSpotCoordinatesList;

                var midPoint = Coordinates.Lerp(coordinates, coordinates2, 0.4);
                extraSettings.AddIfKeyUnused("GroupMidX", midPoint.X);
                extraSettings.AddIfKeyUnused("GroupMidY", midPoint.Y);
            }
        }

        private void SetCarrier(T featureDB, ref UnitMakerGroupInfo? groupInfo)
        {
            if(featureDB.ID != "FriendlyStaticAircraftCarrier")
                return;
            UnitFamily targetFamily = UnitFamily.ShipCarrierSTOVL;
            if(groupInfo.Value.UnitDB.Families.Contains(UnitFamily.PlaneCATOBAR))
                targetFamily = UnitFamily.ShipCarrierCATOBAR;
            if(groupInfo.Value.UnitDB.Families.Contains(UnitFamily.PlaneSTOBAR))
                targetFamily = UnitFamily.ShipCarrierSTOBAR;

            var carrierPool = _unitMaker.carrierDictionary.Where(x => x.Value.UnitDB.Families.Contains(targetFamily)).ToDictionary(x => x.Key, x => x.Value);
            if(carrierPool.Count > 0)
                {
                    var carrier = Toolbox.RandomFrom(carrierPool.Values.ToArray());
                    groupInfo.Value.DCSGroup.Waypoints[0].LinkUnit = carrier.UnitsID[0];
                    groupInfo.Value.DCSGroup.Waypoints[0].HelipadId = carrier.UnitsID[0];
                    groupInfo.Value.DCSGroup.Waypoints[0].X = (float) carrier.Coordinates.X;
                    groupInfo.Value.DCSGroup.Waypoints[0].Y = (float) carrier.Coordinates.Y;
                    groupInfo.Value.DCSGroup.X = (float) carrier.Coordinates.X;
                    groupInfo.Value.DCSGroup.Y = (float) carrier.Coordinates.Y;
                }
        }

    }
}
