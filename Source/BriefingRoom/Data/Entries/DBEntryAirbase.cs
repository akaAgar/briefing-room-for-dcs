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
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryAirbase : DBEntry
    {
        internal string ATC { get; private set; }

        internal Coalition Coalition { get; set; } = Coalition.Neutral;

        internal Coordinates Coordinates { get; private set; }

        internal int DCSID { get; private set; }

        internal double Elevation { get; private set; }

        internal AirbaseFlag Flags { get; private set; }

        internal string ILS { get; private set; }

        internal string Name { get; private set; }

        internal DBEntryAirbaseParkingSpot[] ParkingSpots { get; private set; }

        internal string Runways { get; private set; }

        internal int RunwayLengthFt { get; private set; }

        internal string TACAN { get; private set; }

        internal string Theater { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            var ini = new INIFile(iniFilePath);
            ATC = ini.GetValue<string>("Airbase", "ATC");
            Coordinates = ini.GetValue<Coordinates>("Airbase", "Coordinates");
            DCSID = ini.GetValue<int>("Airbase", "DCSID");
            Elevation = ini.GetValue<double>("Airbase", "Elevation");
            Flags = ini.GetValueArrayAsEnumFlags<AirbaseFlag>("Airbase", "Flags");
            ILS = ini.GetValue<string>("Airbase", "ILS");
            Name = ini.GetValue<string>("Airbase", "Name");
            UIDisplayName = ini.GetValue<string>("Airbase", "Name");
            Runways = ini.GetValue<string>("Airbase", "Runways");
            RunwayLengthFt = ini.GetValue<int>("Airbase", "RunwayLengthFt", -1);
            TACAN = ini.GetValue<string>("Airbase", "TACAN");
            Theater = ini.GetValue<string>("Airbase", "Theater").ToLowerInvariant();

            if (!Database.Instance.EntryExists<DBEntryTheater>(Theater))
                throw new Exception($"Airbase \"{ID}\" located on non-existing theater \"{Theater}\".");

            ParkingSpots = LoadParkingSpots(ini, "Parking");
            if (ParkingSpots.Length == 0) throw new Exception($"No parking spots for airbase \"{ID}\".");

            return true;
        }

        private DBEntryAirbaseParkingSpot[] LoadParkingSpots(INIFile ini, string section)
        {
            string[] ids = ini.GetTopLevelKeysInSection(section);
            DBEntryAirbaseParkingSpot[] parkingSpots = new DBEntryAirbaseParkingSpot[ids.Length];

            for (int i = 0; i < ids.Length; i++)
                parkingSpots[i] = new DBEntryAirbaseParkingSpot(ini, section, ids[i]);

            return parkingSpots;
        }

        public DBEntryAirbase(){}

        internal DBEntryAirbase(Coordinates coords)
        {
            Coordinates = coords;
            ParkingSpots = new DBEntryAirbaseParkingSpot[1];
            RunwayLengthFt = 99999999;
        }
    }
}
