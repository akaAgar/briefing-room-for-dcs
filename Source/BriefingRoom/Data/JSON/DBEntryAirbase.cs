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
using BriefingRoom4DCS.Generator;
using Newtonsoft.Json;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryAirbase : DBEntry
    {
        internal string ATC { get; private set; }

        internal Coalition Coalition { get; set; } = Coalition.Neutral;

        internal Coordinates Coordinates { get; private set; }

        internal int DCSID { get; private set; }

        internal double Elevation { get; private set; }

        internal string ILS { get; private set; }

        internal string Name { get; private set; }

        internal DBEntryAirbaseParkingSpot[] ParkingSpots { get; private set; }

        internal string Runways { get; private set; }

        internal int RunwayLengthFt { get; private set; }

        internal string TACAN { get; private set; }

        internal string Theater { get; private set; }

        protected override bool OnLoad(string o)
        {
           throw new NotImplementedException();
        }


        internal static Dictionary<string, DBEntry> LoadJSON(string filepath)
        {
            var itemMap = new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase);
            var data = JsonConvert.DeserializeObject<List<Airbase>>(File.ReadAllText(filepath));
            foreach (var airbase in data)
            {
                var id = $"{airbase.theatre}{airbase.typeName}";
                itemMap.Add(id, new DBEntryAirbase{
                    ID = id,
                    UIDisplayName =  new LanguageString(airbase.displayName),
                    ATC = String.Join("/", airbase.airdromeData.ATC.Select(x =>GeneratorTools.FormatRadioFrequency(x))),
                    Coordinates = new Coordinates(airbase.pos.DCS.x, airbase.pos.DCS.z),
                    DCSID = airbase.ID,
                    Elevation = airbase.pos.World.alt,
                    ILS = String.Join("/", airbase.airdromeData.ILS.Select(x => GeneratorTools.FormatRadioFrequency(x))),
                    Name = airbase.displayName,
                    Runways = String.Join("/", airbase.airdromeData.runways),
                    RunwayLengthFt = (int)(airbase.runways.Select(x => x.length).DefaultIfEmpty().Max() * Toolbox.METERS_TO_FEET),
                    TACAN = String.Join("/", airbase.airdromeData.TACAN.Select(x => $"{x}X")),
                    Theater = airbase.theatre.ToLower(),
                    ParkingSpots = DBEntryAirbaseParkingSpot.LoadJSON(airbase.parking, id)
                });
            }

            return itemMap;
        }

        private DBEntryAirbaseParkingSpot[] LoadParkingSpots(INIFile ini, string section)
        {
            string[] ids = ini.GetTopLevelKeysInSection(section);
            DBEntryAirbaseParkingSpot[] parkingSpots = new DBEntryAirbaseParkingSpot[ids.Length];

            for (int i = 0; i < ids.Length; i++)
                parkingSpots[i] = new DBEntryAirbaseParkingSpot(ini, section, ids[i]);

            return parkingSpots;
        }

        public DBEntryAirbase() { }

        internal DBEntryAirbase(Coordinates coords)
        {
            Coordinates = coords;
            ParkingSpots = new DBEntryAirbaseParkingSpot[1];
            RunwayLengthFt = 99999999;
        }
    }
}
