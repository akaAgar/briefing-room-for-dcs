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

using System.Linq;

namespace BriefingRoom4DCS.Template
{
    public class MissionTemplateObjective : MissionTemplateGroup
    {
        public List<string> Features { get { return Features_; } set { Features_ = Database.Instance.CheckIDs<DBEntryFeatureObjective>(value.ToArray()).ToList(); } }
        private List<string> Features_ = new List<string>();
        public List<ObjectiveOption> Options { get { return Options_; } set { Options_ = value.Distinct().ToList(); } }
        private List<ObjectiveOption> Options_;
        public string Preset { get { return Preset_; } set { Preset_ = Database.Instance.CheckID<DBEntryObjectivePreset>(value); } }
        private string Preset_;
        public bool HasPreset { get { return Preset_ != "Custom"; }}
        public string Target { get { return Target_; } set { Target_ = Database.Instance.CheckID<DBEntryObjectiveTarget>(value); } }
        private string Target_;
        public string TargetBehavior { get { return TargetBehavior_; } set { TargetBehavior_ = Database.Instance.CheckID<DBEntryObjectiveTargetBehavior>(value); } }
        private string TargetBehavior_;
        public Amount TargetCount { get; set; }
        public string Task { get { return Task_; } set { Task_ = Database.Instance.CheckID<DBEntryObjectiveTask>(value); } }
        private string Task_;
        public double[] CoordinateHint { get { return CoordinateHint_.ToArray(); } set { CoordinateHint_ = new Coordinates(value[0], value[1]); } }
        internal Coordinates CoordinateHint_ { get; set; }

        public List<MissionTemplateSubTask> SubTasks { get; set; } = new List<MissionTemplateSubTask>();

        public MissionTemplateObjective(bool setPreset = false)
        {
            Features = new List<string>();
            Options = new List<ObjectiveOption>();
            Preset = setPreset ? "Custom" : "Interdiction";
            Target = "VehicleAny";
            TargetBehavior = "Idle";
            TargetCount = Amount.Average;
            Task = "DestroyAll";
        }


        public MissionTemplateObjective(string presetID, Amount targetCount)
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
            Preset = ini.GetValue<string>(section, $"{key}.Preset", "Custom");
            Target = ini.GetValue<string>(section, $"{key}.Target");
            TargetBehavior = ini.GetValue<string>(section, $"{key}.TargetBehavior");
            TargetCount = ini.GetValue<Amount>(section, $"{key}.TargetCount");
            Task = ini.GetValue<string>(section, $"{key}.Task");
            CoordinateHint_ = ini.GetValue<Coordinates>(section, $"{key}.CoordinateHint");
            foreach (var subKey in ini.GetKeysInSection(section)
                .Where(x => x.Contains(key))
                .Select(x => x.Split(".")[1])
                .Where(x => x.Contains("subtask"))
                .Distinct().ToList())
            {
                SubTasks.Add(new MissionTemplateSubTask(ini, section, $"{key}.{subKey}"));
            }
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
            ini.SetValue(section, $"{key}.CoordinateHint", CoordinateHint_);
            var i = 0;
            foreach (var subTask in SubTasks)
            {
                subTask.SaveToFile(ini, section, $"{key}.SubTask{i}");
                i++;
            }
        }
    }
}
