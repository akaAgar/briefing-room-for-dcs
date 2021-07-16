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

using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a task to accomplish on an objective.
    /// </summary>
    internal class DBEntryObjectiveTask : DBEntry
    {
        /// <summary>
        /// ID of the <see cref="DBEntryBriefingDescription"/> to use for main briefing description text when most objectives use this task.
        /// </summary>
        internal string BriefingDescription { get; private set; }

        /// <summary>
        /// Randomly-parsed string displayed for this task in the briefing.
        /// Index #0 is singular, index #1 is plural.
        /// </summary>
        internal string[] BriefingTask { get; private set; }

        /// <summary>
        /// Lua file containing the script checking the task has been completed.
        /// </summary>
        internal string CompletionTriggerLua { get; private set; }

        /// <summary>
        /// On which side will the target units be?
        /// </summary>
        internal Side TargetSide { get; private set; }

        /// <summary>
        /// Which units categories are valid targets for this task?
        /// </summary>
        internal UnitCategory[] ValidUnitCategories { get; private set; }

        /// <summary>
        /// Ogg files to include in the .miz file when this objective task is enabled.
        /// </summary>
        internal string[] IncludeOgg { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingDescription = ini.GetValue<string>("ObjectiveTask", "BriefingDescription");
                if (!Database.Instance.EntryExists<DBEntryBriefingDescription>(BriefingDescription))
                {
                    BriefingRoom.PrintToLog($"Objective task \"{ID}\" references non-existing briefing description \"{BriefingDescription}\".", LogMessageErrorLevel.Warning);
                    return false;
                }

                BriefingTask = new string[2];
                BriefingTask[0] = ini.GetValue<string>("ObjectiveTask", "BriefingTask.Singular");
                BriefingTask[1] = ini.GetValue<string>("ObjectiveTask", "BriefingTask.Plural");

                CompletionTriggerLua = Toolbox.AddMissingFileExtension(ini.GetValue<string>("ObjectiveTask", "CompletionTriggerLua"), ".lua");
                if (!File.Exists($"{BRPaths.INCLUDE_LUA_OBJECTIVETRIGGERS}{CompletionTriggerLua}"))
                {
                    BriefingRoom.PrintToLog($"Completion trigger Lua file {CompletionTriggerLua} for objective task \"{ID}\" not found.", LogMessageErrorLevel.Warning);
                    return false;
                }
                
                TargetSide = ini.GetValue<Side>("ObjectiveTask", "TargetSide");
                
                ValidUnitCategories = ini.GetValueArray<UnitCategory>("ObjectiveTask", "ValidUnitCategories").Distinct().ToArray();
                if (ValidUnitCategories.Length == 0) ValidUnitCategories = Toolbox.GetEnumValues<UnitCategory>(); // No category means all categories

                // Included files
                IncludeOgg = Toolbox.AddMissingFileExtensions(ini.GetValueArray<string>("Include", "Ogg"), ".ogg");
            }

            return true;
        }
    }
}
