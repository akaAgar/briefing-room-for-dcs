using System.Collections.Generic;
using System.Linq;
using LuaTableSerializer;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSWaypoint
    {

        public int Alt { get; set; }
        public string AltType { get; set; } = "BARO";
        public string Action { get; set; }
        public double Speed { get; set; }
        private List<DCSWaypointTask> _tasks = new List<DCSWaypointTask>();
        public List<DCSWaypointTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = SortTasks(value); }
        }
        public string Type { get; set; }
        public bool EtaLocked { get; set; }

        public bool SpeedLocked { get; set; }
        public double Y { get; set; }
        public double X { get; set; }
        public string Name { get; set; }

        public int AirdromeId { get; set; }

        public int LinkUnit { get; set; }

        public int HelipadId { get; set; }

        private List<DCSWaypointTask> SortTasks(List<DCSWaypointTask> tasks) => tasks.Select(x => x.parameters.ContainsKey("wrapped") ? new DCSWrappedWaypointTask(x) : x).ToList();

        public string ToLuaString(int number)
        {
            var obj = new Dictionary<string, object> {
                {"alt", Alt},
                {"action", Action},
                {"alt_type", AltType},
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
                {"ETA_locked", EtaLocked},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"formation_template", ""},
                {"speed_locked", SpeedLocked}
            };
            if (AirdromeId != default && LinkUnit == default)
                obj.Add("airdromeId", AirdromeId);
            if (LinkUnit != default)
                obj.Add("linkUnit", LinkUnit);
            if (HelipadId != default)
                obj.Add("helipadId", HelipadId);
            return LuaSerializer.Serialize(obj);
        }
    }
}