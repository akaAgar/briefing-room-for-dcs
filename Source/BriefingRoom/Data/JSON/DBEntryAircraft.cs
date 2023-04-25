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
        internal double CruiseSpeed { get; init; }
        internal bool PlayerControllable { get; init; }
        internal RadioChannel Radio { get; init; }
        internal List<List<RadioChannel>> PanelRadios { get; init; }
        internal Dictionary<string, object> ExtraProps { get; init; }
        internal bool EPLRS { get; init; }
        internal Dictionary<string, List<List<string>>> SpecificCallNames { get; init; }
        internal List<string> Callsigns { get; init; }
        internal List<Payload> Payloads { get; init; }

        protected override bool OnLoad(string o)
        {
            throw new NotImplementedException();
        }

        internal static Dictionary<string, DBEntry> LoadJSON(string filepath, Dictionary<string, DBEntryUnit> unitDict)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<Aircraft>>(File.ReadAllText(filepath));
            foreach (var aircraft in data)
            {
                var id = aircraft.type;
                if (!unitDict.ContainsKey(id))
                {
                    BriefingRoom.PrintToLog($"Ini unit missing {id}", LogMessageErrorLevel.Warning);
                    continue;
                }
                var iniUnit = unitDict[id];
                itemMap.Add(id, new DBEntryAircraft
                {
                    ID = id,
                    UIDisplayName = new LanguageString(aircraft.displayName),
                    DCSID = aircraft.type,
                    Liveries = aircraft.paintSchemes.ToDictionary(pair => (Country)Enum.Parse(typeof(Country), pair.Key.Replace(" ", ""), true), pair => pair.Value),
                    Countries = aircraft.countries.Select(x => (Country)Enum.Parse(typeof(Country), x.Replace(" ", ""), true)).ToList(),
                    Module = aircraft.module,
                    Tasks = aircraft.tasks.Where(x => x is not null).Select(x => (DCSTask)x.WorldID).ToList(),
                    Fuel = aircraft.fuel,
                    Flares = aircraft.flares,
                    Chaff = aircraft.chaff,
                    AmmoType = aircraft.ammoType,
                    MaxAlt = aircraft.maxAlt,
                    CruiseSpeed = aircraft.maxAlt,
                    PlayerControllable = iniUnit.AircraftData.PlayerControllable,
                    Radio = new RadioChannel(aircraft.radio.frequency, (RadioModulation)aircraft.radio.modulation),
                    PanelRadios = (aircraft.panelRadio ?? new List<PanelRadio>()).Select(x => x.channels.Select(x =>
                    {
                        var modulation = RadioModulation.AM;
                        if (!string.IsNullOrEmpty(x.modulation) && x.modulation != "AM/FM")
                            modulation = (RadioModulation)Enum.Parse(typeof(RadioModulation), x.modulation, true);
                        return new RadioChannel(x.@default, modulation);
                    }).ToList()).ToList(),
                    ExtraProps = (aircraft.extraProps ?? new List<ExtraProp>()).Where(x => x.defValue is not null).ToDictionary(x => x.id, x => x.defValue),
                    EPLRS = (bool)(aircraft.EPLRS ?? false),
                    Callsigns = (bool)(aircraft.inheriteCommonCallnames ?? false) ?  new List<string> { "1:Enfield", "2:Springfield", "3:Uzi", "4:Colt", "5:Dodge", "6:Ford", "7:Chevy", "8:Pontiac" } : new List<string>(),
                    SpecificCallNames = aircraft.specificCallnames,
                    Families = iniUnit.Families,
                    Payloads = aircraft.payloadPresets


                });
            }

            return itemMap;
        }

        public DBEntryAircraft() { }
    }
}
