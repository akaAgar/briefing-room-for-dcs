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
    /// <summary>
    /// Generates units and Lua script associated with mission features.
    /// </summary>
    internal class MissionGeneratorMissionFeatures : IDisposable
    {
        /// <summary>
        /// Unit maker selector to use for mission features generation.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker to use for unit generation.</param>
        internal MissionGeneratorMissionFeatures(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal void GenerateMissionFeature(DCSMission mission, MissionTemplate template, int missionFeatureIndex, Coordinates initialCoordinates, Coordinates objectivesCenter)
        {
            DBEntryMissionFeature featureDB = Database.Instance.GetEntry<DBEntryMissionFeature>(template.MissionFeatures[missionFeatureIndex]);
            if (featureDB == null) // Feature doesn't exist
            {
                BriefingRoom.PrintToLog($"Mission feature {template.MissionFeatures[missionFeatureIndex]} not found.", LogMessageErrorLevel.Warning);
                return;
            }

            // Feature unit group
            if (FeatureHasUnitGroup(featureDB))
            {
                Coordinates coordinates = Coordinates.Lerp(initialCoordinates, objectivesCenter, 0.75) + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;
                Coordinates coordinates2 = coordinates + Coordinates.CreateRandom(10, 20) * Toolbox.NM_TO_METERS;
                UnitMakerGroupFlags groupFlags = 0;

                UnitMaker.AddUnitGroup(
                    Toolbox.RandomFrom(featureDB.UnitGroupFamilies), featureDB.UnitGroupSize.GetValue(),
                    featureDB.UnitGroupFlags.Contains(MissionFeatureUnitGroupFlags.Friendly) ? Side.Ally : Side.Enemy,
                    featureDB.UnitGroupLuaGroup, featureDB.UnitGroupLuaUnit,
                    coordinates, null, groupFlags, AircraftPayload.Default,
                    "GroupX2".ToKeyValuePair(coordinates2.X),
                    "GroupY2".ToKeyValuePair(coordinates2.Y)
                    );
            }

            // Feature lua script
            string featureLua = "";
            if (!string.IsNullOrEmpty(featureDB.IncludeLuaSettings)) featureLua = featureDB.IncludeLuaSettings + "\n";
            foreach (string luaFile in featureDB.IncludeLua)
                featureLua += Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_LUA_MISSIONFEATURES}{luaFile}") + "\n";
            mission.AppendValue("MISSION_FEATURES_LUA", featureLua);

            // Feature ogg files
            mission.AddOggFiles(featureDB.IncludeOgg);
        }

        /// <summary>
        /// Does this mission/objective feature requires an unit group to be spawned?
        /// </summary>
        /// <param name="featureDB">The mission/objective feature to check.</param>
        /// <returns>True if an unit has group must be spawned, false otherwise.</returns>
        private bool FeatureHasUnitGroup(DBEntryMissionFeature featureDB)
        {
            return (featureDB.UnitGroupFamilies.Length > 0) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaGroup) &&
                 !string.IsNullOrEmpty(featureDB.UnitGroupLuaUnit);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
