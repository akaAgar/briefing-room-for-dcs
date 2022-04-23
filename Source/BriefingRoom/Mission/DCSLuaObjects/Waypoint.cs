using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class Waypoint {

        private List<WaypointTask> _tasks;

        public List<WaypointTask> Tasks {
            get { return _tasks; }
            set { _tasks = SortTasks(value);}
        }

        private List<WaypointTask> SortTasks(List<WaypointTask> tasks) => tasks.Select(x =>  x.parameters.ContainsKey("wrapped")? new WrappedWaypointTask(x) : x).ToList();

        public string ToLuaString(int number){
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"tasks", Tasks},
            });
        }
    }
}