using System.Collections.Generic;


namespace BriefingRoom4DCS.Data.JSON
{
    public class Situation
    {   
        public Dictionary<string,string> DisplayName { get; set; }
        public string Theater { get; set; }
        public List<Dictionary<string,string>> BriefingDescriptions { get; set; }
        public List<string> RelatedSituations { get; set; }
        public List<List<List<double>>> redZones { get; set; }
        public List<List<List<double>>> blueZones { get; set; }
        public List<List<List<double>>> noSpawnZones { get; set; }
    }
}
