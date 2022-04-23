using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class Waypoint
    {

        public int Alt { get; set; }
        public string Action { get; set; }
        public int Speed { get; set; }
        private List<WaypointTask> _tasks = new List<WaypointTask>();
        public List<WaypointTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = SortTasks(value); }
        }
        public string Type { get; set; }
        public bool etaLocked { get; set; }
        public float Y { get; set; }
        public float X { get; set; }
        public string Name { get; set; }

        private List<WaypointTask> SortTasks(List<WaypointTask> tasks) => tasks.Select(x => x.parameters.ContainsKey("wrapped") ? new WrappedWaypointTask(x) : x).ToList();

        public string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"alt", Alt},
                {"action", Action},
                {"alt_type", "BARO"},
                {"speed", Speed},
                {"task", new Dictionary<string, object>{
                    {"id", "ComboTask"},
                    {"params", new Dictionary<string, object>{
                        {"tasks", Tasks}
                    }}
                    }
                },
                {"type", Type},
                {"ETA", 0},
                {"ETA_locked", etaLocked},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"formation_template", ""},
                {"speed_locked", true}
            });
        }
    }
}