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
    /// <summary>
    /// Generates scripts and unit groups associated with a mission feature.
    /// Abstract parent of <see cref="MissionGeneratorFeaturesMission"/> and <see cref="MissionGeneratorFeaturesObjectives"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class MissionGeneratorFeatures<T> : IDisposable where T : DBEntryFeature
    {
        /// <summary>
        /// Unit maker selector to use for mission features generation.
        /// </summary>
        protected readonly UnitMaker UnitMaker;

        /// <summary>
        /// Current TACAN index. Incremented each time a TACAN-using unit is added to make sure each has its own frequency.
        /// </summary>
        private int TACANIndex = 1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker to use for unit generation.</param>
        internal MissionGeneratorFeatures(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        protected UnitMakerGroupInfo? AddMissionFeature(DCSMission mission, T featureDB, Coordinates coordinates, Coordinates? coordinates2, ref Dictionary<string, object> extraSettings, Side? objectiveTargetSide = null)
        {
            // Add secondary coordinates (destination point) to the extra settings
            if (!coordinates2.HasValue) coordinates2 = coordinates; // No destination point? Use initial point
            extraSettings.AddIfKeyUnused("GroupX2", coordinates2.Value.X);
            extraSettings.AddIfKeyUnused("GroupY2", coordinates2.Value.Y);
            GetExtraSettingsFromFeature(ref extraSettings, featureDB); // Add specific settings for this feature (TACAN frequencies, etc)

            // Feature unit group
            UnitMakerGroupInfo? groupInfo = null;
            if (FeatureHasUnitGroup(featureDB))
            {
                UnitMakerGroupFlags groupFlags = 0;
                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.AlwaysOnMap))
                    groupFlags |= UnitMakerGroupFlags.AlwaysHidden;
                else if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.NeverOnMap))
                    groupFlags |= UnitMakerGroupFlags.NeverHidden;

                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.ImmediateAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.ImmediateAircraftSpawn;
                
                 if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.RadioAircraftActivation))
                    groupFlags |= UnitMakerGroupFlags.RadioAircraftSpawn;

                Side groupSide = Side.Enemy;
                if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.Friendly)) groupSide = Side.Ally;
                else if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.SameSideAsTarget) && objectiveTargetSide.HasValue) groupSide = objectiveTargetSide.Value;


                groupInfo = UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(featureDB.UnitGroupFamilies), featureDB.UnitGroupSize.GetValue(),
                    groupSide,
                    featureDB.UnitGroupLuaGroup, featureDB.UnitGroupLuaUnit,
                    coordinates, null, groupFlags, featureDB.UnitGroupPayload,
                    extraSettings.ToArray());
            }

            // Feature Lua script
            string featureLua = "";

            // Adds the features' group ID to the briefingRoom.mission.missionFeatures.groupsID table
            if (this is MissionGeneratorFeaturesMission)
                featureLua += $"briefingRoom.mission.missionFeatures.groupsID.{GeneratorTools.LowercaseFirstCharacter(featureDB.ID)} = {(groupInfo.HasValue ? groupInfo.Value.GroupID : 0)}\n";

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

        /// <summary>
        /// Adds specific settings for this feature (TACAN frequencies, etc).
        /// </summary>
        /// <param name="extraSettings">Dictionary of extra settings to add.</param>
        /// <param name="featureDB">The feature DBEntry to use.</param>
        /// <returns></returns>
        protected virtual void GetExtraSettingsFromFeature(ref Dictionary<string, object> extraSettings, T featureDB)
        {
            // TODO: Improve
            if (featureDB.UnitGroupFlags.HasFlag(FeatureUnitGroupFlags.TACAN) && (featureDB.UnitGroupFamilies.Length > 0))
            {
                extraSettings.AddIfKeyUnused("TACANFrequency", 1108000000);
                extraSettings.AddIfKeyUnused("TACANCallsign", $"{GeneratorTools.GetTACANCallsign(featureDB.UnitGroupFamilies[0])}{TACANIndex}");
                extraSettings.AddIfKeyUnused("TACANChannel", ((GetType() == typeof(MissionGeneratorFeaturesObjectives)) ? 30 : 20) + TACANIndex);
                if (TACANIndex < 9) TACANIndex++;
            }
        }

        /// <summary>
        /// Does this mission/objective feature requires an unit group to be spawned?
        /// </summary>
        /// <param name="featureDB">The mission/objective feature to check.</param>
        /// <returns>True if an unit has group must be spawned, false otherwise.</returns>
        protected bool FeatureHasUnitGroup(DBEntryFeature featureDB)
        {
            return (featureDB.UnitGroupFamilies.Length > 0) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaGroup) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaUnit);
        }

        protected void AddBriefingRemarkFromFeature(DCSMission mission, DBEntryFeature featureDB, bool useEnemyRemarkIfAvailable, UnitMakerGroupInfo? groupInfo, Dictionary<string, object> stringReplacements)
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
            }

            mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark, featureDB is DBEntryFeatureMission);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
