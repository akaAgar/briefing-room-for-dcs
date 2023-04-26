using System.Collections.Generic;


namespace BriefingRoom4DCS.Data.JSON
{
    public class Category
    {
        public string name { get; set; }
    }

    public class Ship
    {
        public string _id { get; set; }
        public string type { get; set; }
        public string displayName { get; set; }
        public List<Category> categories { get; set; }
        public List<string> countries { get; set; }
        public List<int> countriesWorldID { get; set; }
        public string module { get; set; }
    }
}
