using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSWaypointTask
    {

        public bool enabled { get; init; }
        public bool auto { get; init; }
        public string id { get; init; }
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        public Dictionary<string, object> parameters { 
            get { return _parameters;} 
            set { _parameters = DeterminTypes(value);} 
            } 

        public DCSWaypointTask() { }

        public DCSWaypointTask(string _id, Dictionary<string, object> _parameters, bool _enabled = true, bool _auto = true)
        {
            id = _id;
            parameters = _parameters;
            enabled = _enabled;
            auto = _auto;
        }

        public string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"id", id},
                {"number", number},
                {"enabled", enabled},
                {"auto", enabled},
                {"params", parameters},
            });
        }

        private Dictionary<string, object> DeterminTypes(Dictionary<string, object> values)
        {   
            foreach (var param in values)
            {
               values[param.Key] = DeterminType(param.Value);
            }
            return values;
        }

        private object DeterminType(object value){
            if (!(value is string))
                return value;
            var strVal = value as string;
            if(bool.TryParse(strVal, out bool boolVal))
                return boolVal;
            if(int.TryParse(strVal, out int intVal))
                return intVal;
            if(float.TryParse(strVal, out float floatVal))
                return floatVal;
            return strVal;
        }
    }
}