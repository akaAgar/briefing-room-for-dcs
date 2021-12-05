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
    internal class DBEntryObjectiveTask : DBEntry
    {
        internal string BriefingDescription { get; private set; }

        internal string[] BriefingTask { get; private set; }

        internal string[] BriefingRemarks { get; private set; }

        internal string CompletionTriggerLua { get; private set; }

        internal Side TargetSide { get; private set; }

        internal UnitCategory[] ValidUnitCategories { get; private set; }

        internal string[] IncludeOgg { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            var ini = new INIFile(iniFilePath);
            BriefingDescription = ini.GetValue<string>("Briefing", "Description");
            if (!Database.Instance.EntryExists<DBEntryBriefingDescription>(BriefingDescription))
            {
                BriefingRoom.PrintToLog($"Objective task \"{ID}\" references non-existing briefing description \"{BriefingDescription}\".", LogMessageErrorLevel.Warning);
                return false;
            }

            BriefingTask = new string[2];
            BriefingTask[0] = ini.GetValue<string>("Briefing", "Task.Singular");
            BriefingTask[1] = ini.GetValue<string>("Briefing", "Task.Plural");

            BriefingRemarks = ini.GetValueArray<string>("Briefing", "Remarks", ';');

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

            return true;
        }
    }
}
