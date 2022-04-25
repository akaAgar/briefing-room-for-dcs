using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    internal class DCSWrappedWaypointTask : DCSWaypointTask
    {
        internal DCSWrappedWaypointTask(string _id, Dictionary<string, object> _parameters, bool _enabled = true, bool _auto = true) : base(_id, _parameters, _enabled, _auto)
        {
        }

        internal DCSWrappedWaypointTask(DCSWaypointTask task) : base(task.Id, task.parameters, task.Enabled, task.Auto)
        {
            parameters.Remove("wrapped");
        }

        public new string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"id", "WrappedAction"},
                {"number", number},
                {"enabled", Enabled},
                {"auto", Enabled},
                {"params", new Dictionary<string, object>{
                    {"action", new Dictionary<string, object>{
                        {"id", Id},
                        {"params", parameters}
                    }}
                }},
            });
        }

    }
}