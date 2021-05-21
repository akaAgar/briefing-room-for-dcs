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
    internal abstract class MissionGeneratorFeatures : IDisposable
    {
        /// <summary>
        /// Unit maker selector to use for mission features generation.
        /// </summary>
        protected readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker to use for unit generation.</param>
        internal MissionGeneratorFeatures(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        protected UnitMakerGroupInfo? AddMissionFeature<T>(DCSMission mission, T featureDB, Coordinates coordinates, Coordinates? coordinates2 = null, params KeyValuePair<string, object>[] extraSettings) where T : DBEntryFeature
        {
            // Add secondary coordinates (destination point) to the extra settings
            if (!coordinates2.HasValue) coordinates2 = coordinates; // No destination point? Use initial point
            List<KeyValuePair<string, object>> extraSettingsList = new List<KeyValuePair<string, object>>(extraSettings)
            {
                new KeyValuePair<string, object>("GroupX2", coordinates2.Value.X),
                new KeyValuePair<string, object>("GroupY2", coordinates2.Value.Y)
            };

            // Feature unit group
            UnitMakerGroupInfo? groupInfo = null;
            if (FeatureHasUnitGroup(featureDB))
            {
                UnitMakerGroupFlags groupFlags = 0;
                if (featureDB.UnitGroupFlags.Contains(FeatureUnitGroupFlags.AlwaysOnMap)) groupFlags = UnitMakerGroupFlags.AlwaysHidden;
                else if (featureDB.UnitGroupFlags.Contains(FeatureUnitGroupFlags.NeverOnMap)) groupFlags = UnitMakerGroupFlags.NeverHidden;

                groupInfo = UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(featureDB.UnitGroupFamilies), featureDB.UnitGroupSize.GetValue(),
                    featureDB.UnitGroupFlags.Contains(FeatureUnitGroupFlags.Friendly) ? Side.Ally : Side.Enemy,
                    featureDB.UnitGroupLuaGroup, featureDB.UnitGroupLuaUnit,
                    coordinates, null, groupFlags, AircraftPayload.Default,
                    extraSettingsList.ToArray());
            }

            // Feature Lua script
            string featureLua = "";
            if (!string.IsNullOrEmpty(featureDB.IncludeLuaSettings)) featureLua = featureDB.IncludeLuaSettings + "\n";
            foreach (string luaFile in featureDB.IncludeLua)
                featureLua += Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_LUA_MISSIONFEATURES}{luaFile}") + "\n";
            foreach (KeyValuePair<string, object> extraSetting in extraSettings)
                GeneratorTools.ReplaceKey(ref featureLua, extraSetting.Key, extraSetting.Value);
            if (groupInfo.HasValue)
                GeneratorTools.ReplaceKey(ref featureLua, "FeatureGroupID", groupInfo.Value.GroupID);

            if (featureDB is DBEntryFeatureObjective) mission.AppendValue("ScriptObjectivesFeatures", featureLua);
            else mission.AppendValue("ScriptMissionFeatures", featureLua);

            // Feature ogg files
            mission.AddOggFiles(featureDB.IncludeOgg);

            return groupInfo;
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

        protected void AddBriefingRemarkFromFeature(DCSMission mission, DBEntryFeature featureDB, bool useEnemyRemarkIfAvailable, UnitMakerGroupInfo? groupInfo = null, params KeyValuePair<string, string>[] stringReplacements)
        {
            string[] remarks;
            if (useEnemyRemarkIfAvailable && featureDB.BriefingRemarks[(int)Side.Enemy].Length > 0)
                remarks = featureDB.BriefingRemarks[(int)Side.Enemy].ToArray();
            else
                remarks = featureDB.BriefingRemarks[(int)Side.Ally].ToArray();
            if (remarks.Length == 0) return; // No briefing for this feature

            string remark = Toolbox.RandomFrom(remarks);
            foreach (KeyValuePair<string, string> stringReplacement in stringReplacements)
                GeneratorTools.ReplaceKey(ref remark, stringReplacement.Key, stringReplacement.Value);

            if (groupInfo.HasValue)
            {
                GeneratorTools.ReplaceKey(ref remark, "GroupName", groupInfo.Value.Name);
                GeneratorTools.ReplaceKey(ref remark, "GroupFrequency", GeneratorTools.FormatRadioFrequency(groupInfo.Value.Frequency));
            }

            mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
