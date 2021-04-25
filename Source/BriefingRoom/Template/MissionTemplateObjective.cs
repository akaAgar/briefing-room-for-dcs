/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom.DB;

namespace BriefingRoom.Template
{
    /// <summary>
    /// Stores information about a template objective.
    /// </summary>
    public struct MissionTemplateObjective
    {
        /// <summary>
        /// Database ID of the <see cref="DBEntryObjectiveTarget"/>
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Database ID of the <see cref="DBEntryObjectiveBehavior"/>
        /// </summary>
        public string TargetBehavior { get; }

        /// <summary>
        /// Number of target units at this objective.
        /// </summary>
        public Amount TargetCount { get; }

        /// <summary>
        /// Database ID of the <see cref="DBEntryObjectiveTask"/>
        /// </summary>
        public string Task { get; }

        public MissionTemplateObjective(INIFile ini, string section, string key)
        {
            Target = ini.GetValue<string>(section, $"{key}.Target");
            TargetBehavior = ini.GetValue<string>(section, $"{key}.TargetBehavior");
            TargetCount = ini.GetValue<Amount>(section, $"{key}.TargetCount");
            Task = ini.GetValue<string>(section, $"{key}.Task");
        }

        public MissionTemplateObjective(string task, string targetID, string targetBehaviorID, Amount targetCount = Amount.Average)
        {
            Target = task;
            TargetBehavior = targetID;
            TargetCount = targetCount;
            Task = targetBehaviorID;
        }

        public void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.Target", Target);
            ini.SetValue(section, $"{key}.TargetBehavior", TargetBehavior);
            ini.SetValue(section, $"{key}.TargetCount", TargetCount);
            ini.SetValue(section, $"{key}.Task", Task);
        }
    }
}
