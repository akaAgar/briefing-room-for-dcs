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

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores information about an airbase in a <see cref="DBEntryTheater"/>
    /// </summary>
    public struct DBEntryTheaterAirbase
    {
        /// <summary>
        /// ATC frequencies.
        /// </summary>
        public string ATC { get; }

        /// <summary>
        /// Which coalition this airbase belongs to?
        /// </summary>
        public Coalition Coalition { get; }

        /// <summary>
        /// Map X,Y coordinates of this airbase
        /// </summary>
        public Coordinates Coordinates { get; }

        /// <summary>
        /// Internal ID of this airbase in DCS World.
        /// </summary>
        public int DCSID { get; }

        /// <summary>
        /// Elevation of this airbase, in meters.
        /// </summary>
        public double Elevation { get; }

        /// <summary>
        /// Array of special flags for this airbase.
        /// </summary>
        public DBEntryTheaterAirbaseFlag Flags { get; }

        /// <summary>
        /// ILS frequency (null or empty if none).
        /// </summary>
        public string ILS { get; }

        /// <summary>
        /// The name of the airbase.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parking spots of this airbase.
        /// </summary>
        public DBEntryTheaterAirbaseParkingSpot[] ParkingSpots { get; }

        /// <summary>
        /// A list of runway orientations.
        /// </summary>
        public string Runways { get; }

        /// <summary>
        /// Runway spawn spots spots of this airbase.
        /// </summary>
        public DBEntryTheaterAirbaseParkingSpot[] RunwaySpawns { get; }

        /// <summary>
        /// TACAN frequency (null or empty if none).
        /// </summary>
        public string TACAN { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">The .ini file to load airbase data from.</param>
        /// <param name="airbaseKey">The top-level key (airbase unique ID)</param>
        public DBEntryTheaterAirbase(INIFile ini, string airbaseKey)
        {
            int i;

            ATC = ini.GetValue<string>("Airbases", $"{airbaseKey}.ATC");
            Coalition = ini.GetValue<Coalition>("Airbases", $"{airbaseKey}.Coalition");
            Coordinates = ini.GetValue<Coordinates>("Airbases", $"{airbaseKey}.Coordinates");
            DCSID = ini.GetValue<int>("Airbases", $"{airbaseKey}.DCSID");
            Elevation = ini.GetValue<double>("Airbases", $"{airbaseKey}.Elevation");
            Flags = ini.GetValueArrayAsEnumFlags<DBEntryTheaterAirbaseFlag>("Airbases", $"{airbaseKey}.Flags");
            ILS = ini.GetValue<string>("Airbases", $"{airbaseKey}.ILS");
            Name = ini.GetValue<string>("Airbases", $"{airbaseKey}.Name");
            ParkingSpots = new DBEntryTheaterAirbaseParkingSpot[Math.Max(0, ini.GetValue<int>("Airbases", $"{airbaseKey}.Parking.Count"))];
            for (i = 0; i < ParkingSpots.Length; i++)
                ParkingSpots[i] = new DBEntryTheaterAirbaseParkingSpot(ini, $"{airbaseKey}.Parking.Spot{i + 1}", false);
            Runways = ini.GetValue<string>("Airbases", $"{airbaseKey}.Runways");
            RunwaySpawns = new DBEntryTheaterAirbaseParkingSpot[Math.Max(0, ini.GetValue<int>("Airbases", $"{airbaseKey}.RunwaySpawns.Count"))];
            for (i = 0; i < RunwaySpawns.Length; i++)
                RunwaySpawns[i] = new DBEntryTheaterAirbaseParkingSpot(ini, $"{airbaseKey}.RunwaySpawns.Spot{i + 1}", true);
            TACAN = ini.GetValue<string>("Airbases", $"{airbaseKey}.TACAN");

            if (ParkingSpots.Length == 0)
                throw new Exception($"No parking spots for airbase \"{airbaseKey}\".");

            if (RunwaySpawns.Length == 0)
                throw new Exception($"No runway spawns for airbase \"{airbaseKey}\".");
        }
    }
}
