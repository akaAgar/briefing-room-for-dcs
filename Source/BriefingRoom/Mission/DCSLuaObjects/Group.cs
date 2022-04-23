using System.Collections.Generic;
using System.Linq;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class Group
    {

        public bool LateActivation { get; set; }
        public float Modulation { get; set; }
        public bool Uncontrolled { get; set; }
        public bool TaskSelected { get; set; }
        public string Task { get; set; }
        public List<Waypoint> Waypoints { get; set; }
        public int GroupId { get; set; }
        public bool Hidden { get; set; }
        public List<Unit> Units { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string name { get; set; }
        public float frequency { get; set; }

        public string ToLuaString(int number)
        {
            return LuaSerialiser.Serialize(new Dictionary<string, object> {
                {"lateActivation", LateActivation},
                {"modulation", Modulation},
                {"tasks", new string []{}},
                {"uncontrolled", Uncontrolled},
                {"taskSelected", TaskSelected},
                {"task", Task},
                {"route", new Dictionary<string, object> {
                    {"points", Waypoints}
                }},
                {"groupId", GroupId},
                {"hidden", Hidden},
                {"units", Units},
                {"x", X},
                {"y", Y},
                {"name", name},
                {"communication", true},
                {"start_time", 0},
                {"frequency", frequency},
            });
        }
    }
}