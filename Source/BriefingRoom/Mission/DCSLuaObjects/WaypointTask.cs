using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    internal class WaypointTask {

        internal bool enabled {get; init;}
        internal bool auto {get; init;}
        internal string id {get; init;}
        internal Dictionary<string, object> parameters {get; init;}

        internal WaypointTask(string _id,  Dictionary<string, object> _parameters, bool _enabled=true, bool _auto=true)
        {
            id = _id;
            parameters = _parameters;
            enabled = _enabled;
            auto = _auto;
        }
        
        internal string ToLuaString(int number){
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