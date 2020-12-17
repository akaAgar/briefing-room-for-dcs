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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Adds media files, scripts and units associated with mission features.
    /// </summary>
    public class MissionGeneratorExtensionsAndFeatures : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorExtensionsAndFeatures(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Adds media files, scripts and units associated with mission features.
        /// </summary>
        /// <param name="mission">Mission to which features and extensions should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="coalitionsDB">Coalitions database entries</param>
        public void GenerateExtensionsAndFeatures(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryCoalition[] coalitionsDB)
        {
            DBEntryExtension[] extensions = Database.Instance.GetEntries<DBEntryExtension>(template.OptionsScriptExtensions);
            foreach (DBEntryExtension extension in extensions)
                AddIncludedFiles(mission, extension);

            DBEntryMissionFeature[] features = Database.Instance.GetEntries<DBEntryMissionFeature>(objectiveDB.MissionFeatures);
            foreach (DBEntryMissionFeature feature in features)
            {
                AddIncludedFiles(mission, feature);

                // No unit family in the unit group, so not unit group to add
                if (feature.UnitGroup.Families.Length == 0) continue;

                // TODO: add feature unit group
            }
        }

        /// <summary>
        /// Adds required Lua and ogg files from a extension/feature to the mission.
        /// </summary>
        /// <param name="mission">Mission in which files should be included</param>
        /// <param name="extension">Extension/mission feature to read from</param>
        private void AddIncludedFiles(DCSMission mission, DBEntryExtension extension)
        {
            // Add lua settings
            if (!string.IsNullOrEmpty(extension.LuaSettings))
                mission.LuaSettings += extension.LuaSettings + "\r\n";

            // Add feature Lua scripts
            mission.IncludedLuaScripts.AddRange(extension.IncludeLua);

            // Add feature ogg files
            mission.OggFiles.AddRange(extension.IncludeOgg);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
