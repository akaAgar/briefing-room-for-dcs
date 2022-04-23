using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class WaypointTask {

        public bool enabled {get; init;}
        public bool auto {get; init;}
        public string id {get; init;}
        public Dictionary<string, object> parameters {get; init;} = new Dictionary<string, object>();

        public WaypointTask(){}

        public WaypointTask(string _id,  Dictionary<string, object> _parameters, bool _enabled=true, bool _auto=true)
        {
            id = _id;
            parameters = _parameters;
            enabled = _enabled;
            auto = _auto;
        }
        
        public string ToLuaString(int number){
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"id", id},
                {"number", number},
                {"enabled", enabled},
                {"auto", enabled},
                {"params", parameters},
            });
        }
    }
}