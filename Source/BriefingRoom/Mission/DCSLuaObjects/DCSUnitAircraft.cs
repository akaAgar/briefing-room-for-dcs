using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnitAircraft : DCSUnit
    {
        public int Alt { get; set; }
        public string LiveryId { get; set; }
        public string Skill { get; set; }
        public Dictionary<string, object> PropsLua { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> Pylons { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> PayloadCommon { get; set; } = new Dictionary<string, object>();
        public int Callsign { get; set; }
        public string OnboardNum { get; set; }
        public Dictionary<string, object> RadioPresets { get; set; } = new Dictionary<string, object>();
        public int Parking { get; set; }

        public new string ToLuaString(int number)
        {
            var obj = new Dictionary<string, object>[] {ExtraLua, new Dictionary<string, object>{
                {"type", DCSID},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"type", DCSID},
                {"alt_type", "BARO"},
                {"livery_id", LiveryId},
                {"skill", Skill},
                {"speed", 138.88888888889},
                {"AddPropAircraft", PropsLua},
                {"psi", 0},
                {"payload", new Dictionary<string, object>[] {PayloadCommon, new Dictionary<string, object>{
                    {"pylons", Pylons}
                }}.SelectMany(x => x).ToDictionary(x => x.Key, y => y.Value)},
                {"callsign", Callsign},
                {"onboard_num", OnboardNum},
                {"Radio", RadioPresets}
            }}.SelectMany(x => x)
                    .ToDictionary(x => x.Key, y => y.Value);
            if(Parking > 0)
                obj.Add("parking", Parking);
            return LuaSerialiser.Serialize(obj);
        }
    }
}