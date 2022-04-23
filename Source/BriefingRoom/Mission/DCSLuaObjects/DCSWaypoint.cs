using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSWaypoint
    {

        public int Alt { get; set; }
        public string Action { get; set; }
        public int Speed { get; set; }
        private List<DCSWaypointTask> _tasks = new List<DCSWaypointTask>();
        public List<DCSWaypointTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = SortTasks(value); }
        }
        public string Type { get; set; }
        public bool etaLocked { get; set; }
        public float Y { get; set; }
        public float X { get; set; }
        public string Name { get; set; }

        public int AirdromeId { get; set; }

        private List<DCSWaypointTask> SortTasks(List<DCSWaypointTask> tasks) => tasks.Select(x => x.parameters.ContainsKey("wrapped") ? new WrappedWaypointTask(x) : x).ToList();

        public string ToLuaString(int number)
        {
            var obj = new Dictionary<string, object> {
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
            };
            if(AirdromeId > 0)
                obj.Add("airdromeId", AirdromeId);
            return LuaSerialiser.Serialize(obj);
        }
    }
}