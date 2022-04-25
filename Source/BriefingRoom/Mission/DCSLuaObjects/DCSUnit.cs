using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS.Data;
using LuaTableSerialiser;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnit
    {
        public DCSUnit(){}
        public DCSUnit(string unitType)
        {
            this.unitType = unitType;
        }

        public string unitType { get; set; }
        public string DCSID { get; set; }
        public int UnitId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
        public double Heading { get; set; }

        // Aircraft
        public int Alt { get; set; }
        public string LiveryId { get; set; }
        public string Skill { get; set; }
        public Dictionary<string, object> PropsLua { get; set; } = new Dictionary<string, object>();
        public Dictionary<int, Dictionary<string, string>> Pylons { get; set; } = new Dictionary<int, Dictionary<string, string>>();
        public Dictionary<string, object> PayloadCommon { get; set; } = new Dictionary<string, object>();
        public dynamic Callsign { get; set; }
        public string OnboardNum { get; set; }
        internal List<DBEntryUnitRadioPreset> RadioPresets { get; set; } = new List<DBEntryUnitRadioPreset>();
        public int Parking { get; set; }

        // Ship
        public float Modulation { get; set; }
        public float Frequency { get; set; }

        // Static
        public string ShapeName { get; set; }

        // Static Fob
        public int HeliportModulation { get; set; }
        public int heliportCallsignId { get; set; }
        public string heliportFrequency { get; set; }

        // Vehicle
        public bool PlayerCanDrive { get; set; }

        public string ToLuaString(int number) => unitType switch
        {
            "Aircraft" => ToLuaStringAircraft(number),
            "Ship" => ToLuaStringShip(number),
            "Cargo" => ToLuaStringCargo(number),
            "Static" => ToLuaStringStatic(number),
            "StaticFOB" => ToLuaStringStaticFOB(number),
            "Vehicle" => ToLuaStringVehicle(number),
                _ => throw new BriefingRoomException($"Unsupported unit type {unitType}"),
        };


        private string ToLuaStringAircraft(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
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
            };
            if(Parking > 0)
                obj.Add("parking", Parking);
            return LuaSerialiser.Serialize(obj);
        }

        private string ToLuaStringShip(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"transportable", new Dictionary<string, object>{{"randomTransportable", false}}},
                {"skill", Skill},
                {"modulation", Modulation},
                {"frequency", Frequency},
            };
            return LuaSerialiser.Serialize(obj);
        }

        private string ToLuaStringCargo(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"mass", 100},
                {"category", "Cargos"},
                {"canCargo", true},
                {"rate", 100},
                {"shape_name", ShapeName}
            };
            return LuaSerialiser.Serialize(obj);
        }

        private string ToLuaStringStatic(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"shape_name", ShapeName}
            };
            return LuaSerialiser.Serialize(obj);
        }

        private string ToLuaStringStaticFOB(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"mass", 100},
                {"category", "Heliports"},
                {"shape_name", "FARPS"},
                {"heliport_modulation", HeliportModulation},
                {"heliport_callsign_id", heliportCallsignId},
                {"heliport_frequency", heliportFrequency},
            };
            return LuaSerialiser.Serialize(obj);
        }

        private string ToLuaStringVehicle(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"heading", Heading},
                {"transportable", new Dictionary<string, object>{{"randomTransportable", false}}},
                {"skill", Skill},
                {"PlayerCanDrive", PlayerCanDrive},
            };
            return LuaSerialiser.Serialize(obj);
        }
    }
}