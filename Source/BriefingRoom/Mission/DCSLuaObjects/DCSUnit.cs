using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnit
    {
        public string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object>());
        }
    }
}