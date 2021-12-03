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
    public class MissionTemplateObjective : MissionTemplateGroup
    {
        [Required, DatabaseSourceType(DatabaseEntryType.ObjectiveFeature)]
        [Display(Name = "Objective features", Description = "Special features to include in this objective.")]
        public List<string> Features { get { return Features_; } set { Features_ = Database.Instance.CheckIDs<DBEntryFeatureObjective>(value.ToArray()).ToList(); } }
        private List<string> Features_ = new List<string>();

        [Required]
        [Display(Name = "Options", Description = "Miscellaneous options for this mission objective.")]
        public List<ObjectiveOption> Options { get { return Options_; } set { Options_ = value.Distinct().ToList(); } }
        private List<ObjectiveOption> Options_;

        [Required, DatabaseSourceType(DatabaseEntryType.ObjectivePreset)]
        [Display(Name = "Preset", Description = "Objective preset to use for this objective if mission is generated with the useObjectivePresets option.")]
        public string Preset { get { return Preset_; } set { Preset_ = Database.Instance.CheckID<DBEntryObjectivePreset>(value); } }
        private string Preset_;

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

        public MissionTemplateObjective()
        {
            Features = new List<string>();
            Options = new List<ObjectiveOption>();
            Preset = "Interdiction";
            Target = "VehicleAny";
            TargetBehavior = "Idle";
            TargetCount = Amount.Average;
            Task = "DestroyAll";
        }

        public MissionTemplateObjective(string target, string targetBehavior, string task, string[] features, Amount targetCount = Amount.Average, params ObjectiveOption[] options)
        {
            Features = new List<string>(features);
            Options = new List<ObjectiveOption>(options);
            Preset = "";
            Target = target;
            TargetBehavior = targetBehavior;
            TargetCount = targetCount;
            Task = task;
        }

        public MissionTemplateObjective(string presetID, Amount targetCount = Amount.Average)
        {
            DBEntryObjectivePreset preset = Database.Instance.GetEntry<DBEntryObjectivePreset>(presetID);

            if (preset == null) // Preset doesn't exist.
            {
                Features = new List<string>();
                Options = new List<ObjectiveOption>();
                Preset = "";
                Target = "VehicleAny";
                TargetBehavior = "Idle";
                TargetCount = Amount.Average;
                Task = "DestroyAll";
            }
            else
            {
                Features = preset.Features.ToList();
                Options = preset.Options.ToList();
                Preset = presetID;
                Target = Toolbox.RandomFrom(preset.Targets);
                TargetBehavior = Toolbox.RandomFrom(preset.TargetsBehaviors);
                TargetCount = targetCount;
                Task = Toolbox.RandomFrom(preset.Tasks);
            }
        }

        internal MissionTemplateObjective(INIFile ini, string section, string key)
        {
            LoadFromFile(ini, section, key);
        }

        internal void LoadFromFile(INIFile ini, string section, string key)
        {
            Features = Database.Instance.CheckIDs<DBEntryFeatureObjective>(ini.GetValueArray<string>(section, $"{key}.Features")).ToList();
            Options = ini.GetValueArray<ObjectiveOption>(section, $"{key}.Options").ToList();
            Preset = ini.GetValue<string>(section, $"{key}.Preset");
            Target = ini.GetValue<string>(section, $"{key}.Target");
            TargetBehavior = ini.GetValue<string>(section, $"{key}.TargetBehavior");
            TargetCount = ini.GetValue<Amount>(section, $"{key}.TargetCount");
            Task = ini.GetValue<string>(section, $"{key}.Task");
        }

        internal void SaveToFile(INIFile ini, string section, string key)
        {
            ini.SetValueArray(section, $"{key}.Features", Features.ToArray());
            ini.SetValueArray(section, $"{key}.Options", Options.ToArray());
            ini.SetValue(section, $"{key}.Preset", Preset);
            ini.SetValue(section, $"{key}.Task", Task);
            ini.SetValue(section, $"{key}.Target", Target);
            ini.SetValue(section, $"{key}.TargetBehavior", TargetBehavior);
            ini.SetValue(section, $"{key}.TargetCount", TargetCount);
        }
    }
}
