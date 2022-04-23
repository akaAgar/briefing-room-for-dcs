using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnitStaticFOB: DCSUnitStatic
    {
        public int HeliportModulation { get; set; }
        public int heliportCallsignId { get; set; }
        public string heliportFrequency { get; set; }
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
                {"category", "Heliports"},
                {"shape_name", "FARPS"},
                {"heliport_modulation", HeliportModulation},
                {"heliport_callsign_id", heliportCallsignId},
                {"heliport_frequency", heliportFrequency},
            }}.SelectMany(x => x)
                    .ToDictionary(x => x.Key, y => y.Value);
            return LuaSerialiser.Serialize(obj);
        }
    }
}