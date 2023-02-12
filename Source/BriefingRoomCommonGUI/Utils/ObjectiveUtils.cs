using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.GUI.Utils
{
    internal class ObjectiveUtils
    {
        internal ObjectiveUtils(bool usePreset)
        {
            UsePreset = usePreset;
        }
        private bool UsePreset;
        internal MissionTemplateObjective Tab;
        internal void SetObjectiveTab(MissionTemplateObjective var)
        {
            Tab = var;
        }

        internal void AddObjective(ref MissionTemplate Template)
        {
            MissionTemplateObjective obj = new(true);
            Tab = obj;
            obj.Alias = BriefingRoom.GetAlias(Template.Objectives.Count);
            if (!UsePreset)
                obj.Preset = "Custom";
            Template.Objectives.Add(obj);
        }

        internal void CloneObjective(MissionTemplateObjective obj, ref MissionTemplate Template)
        {
            MissionTemplateObjective newObj = new(true)
            {
                Target = obj.Target,
                TargetBehavior = obj.TargetBehavior,
                TargetCount = obj.TargetCount,
                Task = obj.Task,
                Options = obj.Options,
                Features = obj.Features,
                SubTasks = obj.SubTasks,
                Preset = obj.Preset
            };
            newObj.Alias = BriefingRoom.GetAlias(Template.Objectives.Count);
            Template.Objectives.Add(newObj);
            Tab = newObj;
        }

        internal void CloneObjectiveTask(MissionTemplateObjective obj)
        {
            MissionTemplateSubTask newSubT = new()
            {
                Target = obj.Target,
                TargetBehavior = obj.TargetBehavior,
                TargetCount = obj.TargetCount,
                Task = obj.Task,
                Options = obj.Options,
                Preset= obj.Preset
            };
            obj.SubTasks.Add(newSubT);
        }
        internal void CloneObjectiveSubTask(MissionTemplateSubTask subTask, MissionTemplateObjective obj)
        {
            MissionTemplateSubTask newSubT = new()
            {
                Target = subTask.Target,
                TargetBehavior = subTask.TargetBehavior,
                TargetCount = subTask.TargetCount,
                Task = subTask.Task,
                Options = subTask.Options,
                Preset= subTask.Preset
            };
            obj.SubTasks.Add(newSubT);
        }

        internal void RemoveObjective(MissionTemplateObjective obj, ref MissionTemplate Template)
        {
            Template.Objectives.Remove(obj);
            if (Tab == obj)
                Tab = Template.Objectives.First();
        }

        internal void AddSubTask(MissionTemplateObjective obj)
        {
            obj.SubTasks.Add(new MissionTemplateSubTask());
        }

        internal void RemoveSubTask(MissionTemplateObjective obj, MissionTemplateSubTask subTsk)
        {
            obj.SubTasks.Remove(subTsk);
        }

        internal void ClearSubTasks(MissionTemplateObjective obj)
        {
            obj.SubTasks = new List<MissionTemplateSubTask>();
        }

        internal void ClearObjectiveHint(MissionTemplateObjective obj)
        {
            obj.CoordinateHint = new double[]{0,0};
        }

    }
}