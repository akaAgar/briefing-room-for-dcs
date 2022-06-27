using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LuaTableSerialiser;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSGroup
    {
        public bool Static { get; set; }
        public bool LateActivation { get; set; }
        public float Modulation { get; set; }
        public bool Uncontrolled { get; set; }
        public bool TaskSelected { get; set; }
        public string Task { get; set; }
        public List<DCSWaypoint> Waypoints { get; set; }
        public int GroupId { get; set; }
        public bool Hidden { get; set; }
        public List<DCSUnit> Units { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
        public float Frequency { get; set; }
        public bool RouteRelativeTOT { get; set; } = false;
        public bool? Visible { get; set; }

        public bool? HiddenOnMFD { get; set; }

        public bool Dead {get; set;}

        public bool RadioSet {get; set;} = false;

        public string ToLuaString(int number)
        {   
            if(Static) return ToLuaStringStatic(number);
            var obj = new Dictionary<string, object> {
                {"lateActivation", LateActivation},
                {"modulation", Modulation},
                {"tasks", new string []{}},
                {"uncontrolled", Uncontrolled},
                {"taskSelected", TaskSelected},
                {"task", Task},
                {"route", new Dictionary<string, object> {
                    {"routeRelativeTOT", RouteRelativeTOT},
                    {"points", Waypoints}
                }},
                {"groupId", GroupId},
                {"hidden", Hidden},
                {"units", Units},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"communication", true},
                {"start_time", 0},
                {"frequency", Frequency},
                {"visible", Visible},
                {"hiddenOnMFD", Visible},
            };
            if(RadioSet)
                obj.Add("radioSet", true);
            return LuaSerialiser.Serialize(obj.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value));
        }

        private string ToLuaStringStatic(int number)
        {
            var obj = new Dictionary<string, object> {
                {"heading", 0},
                {"route", new Dictionary<string, object> {
                    {"points", Waypoints}
                }},
                {"groupId", GroupId},
                {"units", Units},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"dead", Dead},
            };
            return LuaSerialiser.Serialize(obj);
        }

        public static DCSGroup YamlToGroup(string yaml){
            foreach (Match match in Regex.Matches(yaml, "\\$.*?\\$"))
                BriefingRoom.PrintToLog($"Found a non-assigned value ({match.Value}) in Group Yaml \"{yaml}\".", LogMessageErrorLevel.Info);
            yaml = Regex.Replace(yaml, "\\$.*?\\$", "0");
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            try
            {
                return deserializer.Deserialize<Mission.DCSLuaObjects.DCSGroup>(yaml); 
            }
            catch (System.Exception e)
            {
                throw new BriefingRoomException($"Failed Deserializing yaml: {yaml}", e);
            }
        }
    }
}