using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    internal class WrappedWaypointTask : WaypointTask
    {
        internal WrappedWaypointTask(string _id, Dictionary<string, object> _parameters, bool _enabled = true, bool _auto = true) : base(_id, _parameters, _enabled, _auto)
        {
        }

        internal WrappedWaypointTask(WaypointTask task) : base(task.id, task.parameters, task.enabled, task.auto)
        {
            parameters.Remove("wrapped");
        }

        public new string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"id", "WrappedAction"},
                {"number", number},
                {"enabled", enabled},
                {"auto", enabled},
                {"params", new Dictionary<string, object>{
                    {"action", new Dictionary<string, object>{
                        {"id", id},
                        {"params", parameters}
                    }}
                }},
            });
        }

    }
}