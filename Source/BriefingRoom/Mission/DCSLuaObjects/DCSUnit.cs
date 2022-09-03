using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS.Data;
using LuaTableSerializer;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSUnit
    {
        internal DCSUnit() { }
        internal DCSUnit(string unitType)
        {
            this.unitType = unitType;
        }

        internal string unitType { get; set; }
        internal string DCSID { get; set; }
        internal int UnitId { get; set; }
        internal Coordinates Coordinates { get; set; }
        internal string Name { get; set; }
        internal double Heading { get; set; }

        // Aircraft
        internal int Alt { get; set; }
        internal string LiveryId { get; set; }
        internal string Skill { get; set; }
        internal Dictionary<string, object> PropsLua { get; set; } = new Dictionary<string, object>();
        internal Dictionary<int, Dictionary<string, string>> Pylons { get; set; } = new Dictionary<int, Dictionary<string, string>>();
        internal Dictionary<string, object> PayloadCommon { get; set; } = new Dictionary<string, object>();
        internal dynamic Callsign { get; set; }
        internal string OnboardNum { get; set; }
        internal List<DBEntryUnitRadioPreset> RadioPresets { get; set; } = new List<DBEntryUnitRadioPreset>();
        internal int Parking { get; set; }

        // Ship
        internal int Modulation { get; set; }
        internal int RadioFrequency { get; set; }

        // Static
        internal string ShapeName { get; set; }

        // Static Fob
        internal int HeliportModulation { get; set; }
        internal int heliportCallsignId { get; set; }
        internal string heliportFrequency { get; set; }

        // Vehicle
        internal bool PlayerCanDrive { get; set; }

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
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
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
            };
            if (Skill == "Player" || Skill == "Client")
                obj.Add("Radio", RadioPresets);
            if (Parking > 0)
                obj.Add("parking", Parking);
            return LuaSerializer.Serialize(obj);
        }

        private string ToLuaStringShip(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
                {"name", Name},
                {"heading", Heading},
                {"livery_id", LiveryId},
                {"transportable", new Dictionary<string, object>{{"randomTransportable", false}}},
                {"skill", Skill},
                {"modulation", Modulation},
                {"frequency", RadioFrequency},
            };
            return LuaSerializer.Serialize(obj);
        }

        private string ToLuaStringCargo(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
                {"name", Name},
                {"heading", Heading},
                {"mass", 100},
                {"category", "Cargos"},
                {"canCargo", true},
                {"rate", 100},
                {"shape_name", ShapeName}
            };
            return LuaSerializer.Serialize(obj);
        }

        private string ToLuaStringStatic(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
                {"name", Name},
                {"heading", Heading},
                {"shape_name", ShapeName}
            };
            return LuaSerializer.Serialize(obj);
        }

        private string ToLuaStringStaticFOB(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
                {"name", Name},
                {"heading", Heading},
                {"mass", 100},
                {"category", "Heliports"},
                {"shape_name", ShapeName},
                {"heliport_modulation", HeliportModulation},
                {"heliport_callsign_id", heliportCallsignId},
                {"heliport_frequency", heliportFrequency},
            };
            return LuaSerializer.Serialize(obj);
        }

        private string ToLuaStringVehicle(int number)
        {
            var obj = new Dictionary<string, object>{
                {"type", DCSID},
                {"unitId", UnitId},
                {"x", Coordinates.X},
                {"y", Coordinates.Y},
                {"name", Name},
                {"heading", Heading},
                {"livery_id", LiveryId},
                {"transportable", new Dictionary<string, object>{{"randomTransportable", false}}},
                {"skill", Skill},
                {"PlayerCanDrive", PlayerCanDrive},
            };
            return LuaSerializer.Serialize(obj);
        }
    }
}