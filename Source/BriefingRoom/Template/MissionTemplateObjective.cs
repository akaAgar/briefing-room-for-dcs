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
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCS.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// Stores information about a mission template objective.
    /// </summary>
    public class MissionTemplateObjective
    {
        [Required, DatabaseSourceType(DatabaseEntryType.ObjectiveFeature)]
        [Display(Name = "Objective features", Description = "Special features to include in this objective.")]
        public List<string> Features { get { return Features_; } set { Features_ = Database.Instance.CheckMissionFeaturesIDs<DBEntryFeatureObjective>(value.ToArray()).ToList(); } }
        private List<string> Features_ = new List<string>();

        [Required]
        [Display(Name = "Options", Description = "Miscellaneous options for this mission objective.")]
        public List<ObjectiveOption> Options { get { return Options_; } set { Options_ = value.Distinct().ToList(); } }
        private List<ObjectiveOption> Options_;

        [Required, DatabaseSourceType(DatabaseEntryType.ObjectiveTarget)]
        [Display(Name = "Target", Description = "Family of units to use as a target for this objective.")]
        public string Target { get { return Target_; } set { Target_ = Database.Instance.CheckID<DBEntryObjectiveTarget>(value); } }
        private string Target_;

        [Required, DatabaseSourceType(DatabaseEntryType.ObjectiveTargetBehavior)]
        [Display(Name = "Target behavior", Description = "How will the target units behave?")]
        public string TargetBehavior { get { return TargetBehavior_; } set { TargetBehavior_ = Database.Instance.CheckID<DBEntryObjectiveTargetBehavior>(value); } }
        private string TargetBehavior_;

        [Required]
        [Display(Name = "Target count", Description = "Number of target units at this objective.")]
        public Amount TargetCount { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.ObjectiveTask)]
        [Display(Name = "Task", Description = "Task to accomplish at this objective.")]
        public string Task { get { return Task_; } set { Task_ = Database.Instance.CheckID<DBEntryObjectiveTask>(value); } }
        private string Task_;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionTemplateObjective()
        {
            Features = new List<string>();
            Options = new List<ObjectiveOption>();
            Target = "VehicleAny";
            TargetBehavior = "Idle";
            TargetCount = Amount.Average;
            Task = "DestroyAll";
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Family of units to use as a target for this objective.</param>
        /// <param name="targetBehavior"></param>
        /// <param name="task">Task to accomplish at this objective.</param>
        /// <param name="features">Special features to include in this objective.</param>
        /// <param name="targetCount">Number of target units at this objective.</param>
        /// <param name="options">Special options to apply to this objective.</param>
        public MissionTemplateObjective(string target, string targetBehavior, string task, string[] features, Amount targetCount = Amount.Average, params ObjectiveOption[] options)
        {
            Features = new List<string>(features);
            Options = new List<ObjectiveOption>(options);
            Target = target;
            TargetBehavior = targetBehavior;
            TargetCount = targetCount;
            Task = task;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">INI file to load from.</param>
        /// <param name="section">INI section.</param>
        /// <param name="key">INI key.</param>
        internal MissionTemplateObjective(INIFile ini, string section, string key)
        {
            LoadFromFile(ini, section, key);
        }

        /// <summary>
        /// Loads the objective template from an .ini file.
        /// </summary>
        /// <param name="ini">INI file to save to.</param>
        /// <param name="section">INI section.</param>
        /// <param name="key">INI key.</param>
        internal void LoadFromFile(INIFile ini, string section, string key)
        {
            Features = Database.Instance.CheckIDs<DBEntryFeatureObjective>(ini.GetValueArray<string>(section, $"{key}.Features")).ToList();
            Options = ini.GetValueArray<ObjectiveOption>(section, $"{key}.Options").ToList();
            Target = ini.GetValue<string>(section, $"{key}.Target");
            TargetBehavior = ini.GetValue<string>(section, $"{key}.TargetBehavior");
            TargetCount = ini.GetValue<Amount>(section, $"{key}.TargetCount");
            Task = ini.GetValue<string>(section, $"{key}.Task");
        }

        /// <summary>
        /// Saves the objective template to an .ini file.
        /// </summary>
        /// <param name="ini">INI file to save to.</param>
        /// <param name="section">INI section.</param>
        /// <param name="key">INI key.</param>
        internal void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValueArray(section, $"{key}.Features", Features.ToArray());
            ini.SetValueArray(section, $"{key}.Options", Options.ToArray());
            ini.SetValue(section, $"{key}.Task", Task);
            ini.SetValue(section, $"{key}.Target", Target);
            ini.SetValue(section, $"{key}.TargetBehavior", TargetBehavior);
            ini.SetValue(section, $"{key}.TargetCount", TargetCount);
        }
    }
}
