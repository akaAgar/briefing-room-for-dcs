/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BriefingRoom4DCS.Data.JSON;
using BriefingRoom4DCS.Template;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryAircraft : DBEntryJSONUnit
    {
        internal List<DCSTask> Tasks { get; init; }
        internal double Fuel { get; init; }
        internal int Flares { get; init; }
        internal int Chaff { get; init; }
        internal int? AmmoType { get; init; }
        internal int MaxAlt { get; init; }
        internal int CruiseAlt { get { return (int)Math.Floor(MaxAlt * 0.6);}}
        internal double CruiseSpeed { get; init; }
        internal bool PlayerControllable { get; init; }
        internal RadioChannel Radio { get; init; }
        internal List<DBEntryUnitRadioPreset> PanelRadios { get; init; }
        internal Dictionary<string, object> ExtraProps { get; init; }
        internal bool EPLRS { get; init; }
        internal Dictionary<string, List<List<string>>> SpecificCallNames { get; init; }
        internal List<string> CallSigns { get; init; }
        internal List<Payload> Payloads { get; init; }
        internal int MinimumRunwayLengthFt { get; init; }

        internal Dictionary<string, object> PayloadCommon
        {
            get
            {
                var dict = new Dictionary<string, object>{
                {"flare", Flares},
                {"chaff", Chaff},
                {"gun", 100},
                {"fuel", Fuel}
                };
                if (AmmoType.HasValue)
                    dict.Add("ammo_type", AmmoType.Value);
                return dict;
            }
        }

        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }

        internal static Dictionary<string, DBEntry> LoadJSON(string filepath)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<Aircraft>>(File.ReadAllText(filepath));
            var supportData = JsonConvert.DeserializeObject<List<AircraftBRInfo>>(File.ReadAllText($"{filepath.Replace(".json", "")}BRInfo.json")).ToDictionary(x => x.type, x => x);

            foreach (var aircraft in data)
            {
                var id = aircraft.type;
                if (!supportData.ContainsKey(id))
                {
                    BriefingRoom.PrintToLog($"Aircraft {id} missing support data.", LogMessageErrorLevel.Warning);
                    continue;
                }
                var supportInfo = supportData[id];

                var countryList = aircraft.countries.Select(x => (Country)Enum.Parse(typeof(Country), x.Replace(" ", ""), true)).ToList();
                countryList.AddRange(supportInfo.extraOperators.Select(x => (Country)Enum.Parse(typeof(Country), x, true)));

                itemMap.Add(id, new DBEntryAircraft
                {
                    ID = id,
                    UIDisplayName = new LanguageString(aircraft.displayName),
                    DCSID = aircraft.type,
                    Liveries = aircraft.paintSchemes.ToDictionary(pair => (Country)Enum.Parse(typeof(Country), pair.Key.Replace(" ", ""), true), pair => pair.Value),
                    Countries = countryList,
                    Module = aircraft.module,
                    Tasks = aircraft.tasks.Where(x => x is not null).Select(x => (DCSTask)x.WorldID).ToList(),
                    Fuel = aircraft.fuel,
                    Flares = aircraft.flares,
                    Chaff = aircraft.chaff,
                    AmmoType = aircraft.ammoType,
                    MaxAlt = aircraft.maxAlt,
                    CruiseSpeed = aircraft.cruiseSpeed,
                    Radio = new RadioChannel(aircraft.radio.frequency, (RadioModulation)aircraft.radio.modulation),
                    PanelRadios = (aircraft.panelRadio ?? new List<PanelRadio>()).Select(radio =>
                    {
                        return new DBEntryUnitRadioPreset(radio.channels.Select(x => x.@default).ToArray(), radio.channels.Select(x =>
                        {
                            var modulation = RadioModulation.AM;
                            if (!string.IsNullOrEmpty(x.modulation) && x.modulation != "AM/FM")
                                modulation = (RadioModulation)Enum.Parse(typeof(RadioModulation), x.modulation, true);
                            return (int)modulation;
                        }).ToArray(), RadioType.Unknown);
                    }).ToList(),
                    ExtraProps = (aircraft.extraProps ?? new List<ExtraProp>()).Where(x => x.defValue is not null).ToDictionary(x => x.id, x => x.defValue),
                    EPLRS = (bool)(aircraft.EPLRS ?? false),
                    CallSigns = new List<string> { "1:Enfield", "2:Springfield", "3:Uzi", "4:Colt", "5:Dodge", "6:Ford", "7:Chevy", "8:Pontiac" },
                    SpecificCallNames = aircraft.specificCallnames,
                    Payloads = aircraft.payloadPresets,
                    Shape = aircraft.shape,
                    PlayerControllable = supportInfo.playerControllable,
                    Families = supportInfo.families.Select(x => (UnitFamily)Enum.Parse(typeof(UnitFamily), x, true)).ToArray(),
                    Operational = supportInfo.operational.Select(x => (Decade)x).ToList(),
                    LowPoly = supportInfo.lowPoly
                });

            }

            return itemMap;
        }

        public DBEntryAircraft() { }

        internal Dictionary<int, Dictionary<string, string>> GetPylonsObject(string aircraftPayload)
        {
            var payload = Payloads.Find(x => x.name == aircraftPayload);
            if (payload == null)
                return new Dictionary<int, Dictionary<string, string>>();
            return payload.pylons.ToDictionary(x => x.num, x => new Dictionary<string, string> { { "CLSID", x.CLSID } });
        }

        internal Dictionary<int, Dictionary<string, string>> GetPylonsObject(DCSTask task)
        {
            if (Payloads.Count() == 0)
                return new Dictionary<int, Dictionary<string, string>>();
            var payload = Toolbox.RandomFrom(Payloads.Where(x => x.tasks.Contains((int)task)).ToList());
            if (payload == null)
                payload = Toolbox.RandomFrom(Payloads);

            return payload.pylons.Where(x => x != null).ToDictionary(x => x.num, x => new Dictionary<string, string> { { "CLSID", x.CLSID } });

        }

    }
}
