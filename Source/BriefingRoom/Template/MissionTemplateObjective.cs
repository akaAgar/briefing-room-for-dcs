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
        public string ObjectiveTarget { get; }

        /// <summary>
        /// Database ID of the <see cref="DBEntryObjectiveBehavior"/>
        /// </summary>
        public string ObjectiveTargetBehavior { get; }

        /// <summary>
        /// Number of target units at this objective.
        /// </summary>
        public Amount ObjectiveTargetCount { get; }

        /// <summary>
        /// Database ID of the <see cref="DBEntryObjectiveTask"/>
        /// </summary>
        public string ObjectiveTask { get; }

        public MissionTemplateObjective(INIFile ini, string section, string key)
        {
            ObjectiveTarget = ini.GetValue<string>(section, $"{key}.Target");
            ObjectiveTargetBehavior = ini.GetValue<string>(section, $"{key}.TargetBehavior");
            ObjectiveTargetCount = ini.GetValue<Amount>(section, $"{key}.TargetCount");
            ObjectiveTask = ini.GetValue<string>(section, $"{key}.Task");
        }

        public MissionTemplateObjective(string task, string targetID, string targetBehaviorID, Amount targetCount = Amount.Average)
        {
            ObjectiveTarget = task;
            ObjectiveTargetBehavior = targetID;
            ObjectiveTargetCount = targetCount;
            ObjectiveTask = targetBehaviorID;
        }

        public void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValue(section, $"{key}.Target", ObjectiveTarget);
            ini.SetValue(section, $"{key}.TargetBehavior", ObjectiveTargetBehavior);
            ini.SetValue(section, $"{key}.TargetCount", ObjectiveTargetCount);
            ini.SetValue(section, $"{key}.Task", ObjectiveTask);
        }
    }
}
