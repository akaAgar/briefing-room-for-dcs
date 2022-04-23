using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnitStatic: DCSUnit
    {
        public string ShapeName { get; set; }

        public new string ToLuaString(int number)
        {
            var obj = new Dictionary<string, object>[] {ExtraLua, new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"type", DCSID},
                {"shape_name", ShapeName}
            }}.SelectMany(x => x)
                    .ToDictionary(x => x.Key, y => y.Value);
            return LuaSerialiser.Serialize(obj);
        }
    }
}