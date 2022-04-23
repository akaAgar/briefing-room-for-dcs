using System.Collections.Generic;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSWaypointTask
    {

        public bool Enabled { get; init; }
        public bool Auto { get; init; }
        public string Id { get; init; }
        public string Key {get; set;}
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        public Dictionary<string, object> parameters { 
            get { return _parameters;} 
            set { _parameters = DeterminTypes(value);} 
            } 

        public DCSWaypointTask() { }

        public DCSWaypointTask(string _id, Dictionary<string, object> _parameters, bool _enabled = true, bool _auto = true)
        {
            Id = _id;
            parameters = _parameters;
            Enabled = _enabled;
            Auto = _auto;
        }

        public string ToLuaString(int number)
        {   var obj = new Dictionary<string, object> {
                {"id", Id},
                {"number", number},
                {"enabled", Enabled},
                {"auto", Enabled},
                {"params", parameters},
            };
            if(!string.IsNullOrEmpty(Key))
                obj.Add("key", Key);
            return LuaSerialiser.Serialize(obj);
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