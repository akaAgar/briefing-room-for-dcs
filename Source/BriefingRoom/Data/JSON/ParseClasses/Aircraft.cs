using System.Collections.Generic;


namespace BriefingRoom4DCS.Data.JSON
{
    public class ExtraProp
    {
        public object defValue { get; set; }
        public string id { get; set; }
    }

    public class PanelRadio
    {
        public object range { get; set; }
        public string name { get; set; }
        public List<Channel> channels { get; set; }
        public string displayUnits { get; set; }
    }

    public class Channel
    {
        public double @default { get; set; }
        public string modulation { get; set; }
        public string name { get; set; }
        public bool connect { get; set; }
    }

    public class Radio
    {
        public double frequency { get; set; }
        public int modulation { get; set; }
    }

    public class Aircraft
    {
        public string _id { get; set; }
        public Dictionary<string, List<string>> paintSchemes { get; set; }
        public List<Payload> payloadPresets { get; set; }
        public string type { get; set; }
        public string displayName { get; set; }
        public List<Task> tasks { get; set; }
        public double fuel { get; set; }
        public int flares { get; set; }
        public int chaff { get; set; }
        public Radio radio { get; set; }
        public int maxAlt { get; set; }
        public double cruiseSpeed { get; set; }
        public List<string> countries { get; set; }
        public List<int> countriesWorldID { get; set; }
        public List<PanelRadio> panelRadio { get; set; }
        public List<ExtraProp> extraProps { get; set; }
        public bool? EPLRS { get; set; }
        public string module { get; set; }
        public int? ammoType { get; set; }
        public bool? inheriteCommonCallnames { get; set; }
        public Dictionary<string, List<List<string>>> specificCallnames { get; set; }
    }

    public class Task
    {
        public string OldID { get; set; }
        public string Name { get; set; }
        public int WorldID { get; set; }
    }

    public class Payload
    {
        public List<Pylon> pylons { get; set; }
        public List<int> tasks { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
    }

    public class Pylon
    {
        public string CLSID { get; set; }
        public int num { get; set; }
    }
}
