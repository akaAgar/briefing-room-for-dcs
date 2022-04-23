using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnitCargo: DCSUnitStatic
    {

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
                {"mass", 100},
                {"category", "Cargos"},
                {"canCargo", true},
                {"rate", 100},
                {"shape_name", ShapeName}
            }}.SelectMany(x => x)
                    .ToDictionary(x => x.Key, y => y.Value);
            return LuaSerialiser.Serialize(obj);
        }
    }
}