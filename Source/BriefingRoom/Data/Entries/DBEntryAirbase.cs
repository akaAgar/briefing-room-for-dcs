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
    /// <summary>
    /// Stores information about a DCS World theater airbase
    /// </summary>
    internal class DBEntryAirbase : DBEntry
    {
        /// <summary>
        /// ATC frequencies.
        /// </summary>
        internal string ATC { get; private set; }

        /// <summary>
        /// Which coalition this airbase belongs to?
        /// </summary>
        internal Coalition Coalition { get; private set; }

        /// <summary>
        /// Map X,Y coordinates of this airbase
        /// </summary>
        internal Coordinates Coordinates { get; private set; }

        /// <summary>
        /// Internal ID of this airbase in DCS World.
        /// </summary>
        internal int DCSID { get; private set; }

        /// <summary>
        /// Elevation of this airbase, in meters.
        /// </summary>
        internal double Elevation { get; private set; }

        /// <summary>
        /// Array of special flags for this airbase.
        /// </summary>
        internal AirbaseFlag Flags { get; private set; }

        /// <summary>
        /// ILS frequency (null or empty if none).
        /// </summary>
        internal string ILS { get; private set; }

        /// <summary>
        /// The name of the airbase.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// Parking spots of this airbase.
        /// </summary>
        internal DBEntryAirbaseParkingSpot[] ParkingSpots { get; private set; }

        /// <summary>
        /// A list of runway orientations.
        /// </summary>
        internal string Runways { get; private set; }

        /// <summary>
        /// Runway spawn spots spots of this airbase.
        /// </summary>
        internal DBEntryAirbaseParkingSpot[] RunwaySpawns { get; private set; }

        /// <summary>
        /// TACAN frequency (null or empty if none).
        /// </summary>
        internal string TACAN { get; private set; }

        /// <summary>
        /// ID of the theater this airbase is located on.
        /// </summary>
        internal string Theater { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                ATC = ini.GetValue<string>("Airbase", "ATC");
                Coalition = ini.GetValue<Coalition>("Airbase", "Coalition");
                Coordinates = ini.GetValue<Coordinates>("Airbase", "Coordinates");
                DCSID = ini.GetValue<int>("Airbase", "DCSID");
                Elevation = ini.GetValue<double>("Airbase", "Elevation");
                Flags = ini.GetValueArrayAsEnumFlags<AirbaseFlag>("Airbase", "Flags");
                ILS = ini.GetValue<string>("Airbase", "ILS");
                Name = ini.GetValue<string>("Airbase", "Name");
                UIDisplayName = ini.GetValue<string>("Airbase", "Name");
                Runways = ini.GetValue<string>("Airbase", "Runways");
                TACAN = ini.GetValue<string>("Airbase", "TACAN");
                Theater = ini.GetValue<string>("Airbase", "Theater").ToLowerInvariant();

                if (!Database.Instance.EntryExists<DBEntryTheater>(Theater))
                    throw new Exception($"Airbase \"{ID}\" located on non-existing theater \"{Theater}\".");

                RunwaySpawns = LoadParkingSpots(ini, "RunwaySpawns", true);
                if (RunwaySpawns.Length == 0) throw new Exception($"No runway spawns for airbase \"{ID}\".");
             
                ParkingSpots = LoadParkingSpots(ini, "Parking");
                if (ParkingSpots.Length == 0) throw new Exception($"No parking spots for airbase \"{ID}\".");
            }

            return true;
        }

        private DBEntryAirbaseParkingSpot[] LoadParkingSpots(INIFile ini, string section, bool isRunway = false)
        {
            string[] ids = ini.GetTopLevelKeysInSection(section);
            DBEntryAirbaseParkingSpot[] parkingSpots = new DBEntryAirbaseParkingSpot[ids.Length];

            for (int i = 0; i < ids.Length; i++)
                parkingSpots[i] = new DBEntryAirbaseParkingSpot(ini, section, ids[i], isRunway);

            return parkingSpots;
        }
    }
}
