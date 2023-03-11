
using System;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data.JSON
{
    public class MongoBase
    {
        public string _id { get; set; }

        [JsonProperty("@dcsversion")]
        public string dcsversion { get; set; }

        [JsonProperty("@created")]
        public DateTime created { get; set; }
    }

    public class WorldPos
    {
        public double lon { get; set; }
        public double lat { get; set; }
        public double alt { get; set; }
    }

    public class Pos
    {
        public DCSPos DCS { get; set; }
        public WorldPos World { get; set; }
    }

    public class DCSPos
    {
        public double y { get; set; }
        public double x { get; set; }
        public double z { get; set; }
    }
}
