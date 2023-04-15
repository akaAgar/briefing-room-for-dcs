using System.Collections.Generic;


namespace BriefingRoom4DCS.Data.JSON
{
    public class Car
    {
        public string _id { get; set; }
        public string displayName { get; set; }
        public string category { get; set; }
        public Dictionary<string, List<string>> paintSchemes { get; set; }
        public string type { get; set; }
        public List<string> countries { get; set; }
        public List<int> countriesWorldID { get; set; }
        public string module { get; set; }
    }
}