using System.Collections.Generic;


namespace BriefingRoom4DCS.Data.JSON
{
    public class SituationZones
    {
        public List<List<List<double>>> redZones { get; set; }
        public List<List<List<double>>> blueZones { get; set; }
        public List<List<List<double>>> noSpawnCoordinates { get; set; }
    }
}
