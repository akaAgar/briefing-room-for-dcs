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
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a DCS World theater.
    /// </summary>
    internal class DBEntryTheater : DBEntry
    {
        /// <summary>
        /// Default daytime value to use when failed to read from the .ini file.
        /// </summary>
        private static readonly MinMaxI DEFAULT_DAYTIME = new MinMaxI(8 * 60, 19 * 60);

        /// <summary>
        /// Names to use for this theater in briefings.
        /// </summary>
        internal string[] BriefingNames { get; private set; }

        /// <summary>
        /// The default coordinates of the map center.
        /// </summary>
        internal Coordinates DefaultMapCenter { get; private set; }

        /// <summary>
        /// The internal ID of the theater in DCS World.
        /// </summary>
        internal string DCSID { get; private set; }

        /// <summary>
        /// Magnetic declination from true north.
        /// </summary>
        internal float MagneticDeclination { get; private set; }

        /// <summary>
        /// Sunrise and sunset time (in minutes) for each month (January is 0, December is 11)
        /// </summary>
        internal MinMaxI[] DayTime { get; private set; }

        /// <summary>
        /// Possible spawn points for carrier groups. Must be at least <see cref="Generator.MissionGeneratorCarrierGroup.CARRIER_COURSE_LENGTH"/> nautical miles from land in all directions.
        /// </summary>
        internal Coordinates[] CarrierGroupWaypoints { get; private set; }

        /// <summary>
        /// All spawn points in this theater.
        /// </summary>
        internal DBEntryTheaterSpawnPoint[] SpawnPoints { get; private set; }

        /// <summary>
        /// Min and max temperature (in degrees Celsius) for each month (January is 0, December is 11)
        /// </summary>
        internal MinMaxI[] Temperature { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            int i;

            using (INIFile ini = new INIFile(iniFilePath))
            {
                // [Briefing] section
                BriefingNames = ini.GetValueArray<string>("Briefing", "Names");

                // [Theater] section
                DCSID = ini.GetValue<string>("Theater", "DCSID");
                DefaultMapCenter = ini.GetValue<Coordinates>("Theater", "DefaultMapCenter");
                MagneticDeclination = ini.GetValue<float>("Theater", "MagneticDeclination");

                // [Daytime] section
                DayTime = new MinMaxI[12];
                for (i = 0; i < 12; i++)
                {
                    MinMaxI? dayTimeValue = ParseMinMaxTime(ini.GetValueArray<string>("Daytime", ((Month)i).ToString()));

                    if (!dayTimeValue.HasValue) // Cast failed
                        BriefingRoom.PrintToLog(
                            $"Wrong format for daytime value for month {(Month)i} in theater {ID}, using default value",
                            LogMessageErrorLevel.Warning);

                    DayTime[i] = dayTimeValue ?? DEFAULT_DAYTIME;
                }

                // [CarrierGroupWaypoints] section
                List<Coordinates> carrierGroupWaypointsList = new List<Coordinates>();
                foreach (string key in ini.GetKeysInSection("CarrierGroupWaypoints"))
                    carrierGroupWaypointsList.Add(ini.GetValue<Coordinates>("CarrierGroupWaypoints", key));
                CarrierGroupWaypoints = carrierGroupWaypointsList.ToArray();

                // [Temperature] section
                Temperature = new MinMaxI[12];
                for (i = 0; i < 12; i++)
                    Temperature[i] = ini.GetValue<MinMaxI>("Temperature", ((Month)i).ToString());

                // [SpawnPoints] section
                List<DBEntryTheaterSpawnPoint> spawnPointsList = new List<DBEntryTheaterSpawnPoint>();
                foreach (string key in ini.GetKeysInSection("SpawnPoints"))
                {
                    DBEntryTheaterSpawnPoint sp = new DBEntryTheaterSpawnPoint();
                    if (sp.Load(ini, key))
                        spawnPointsList.Add(sp);
                }
                SpawnPoints = spawnPointsList.ToArray();
            }

            return true;
        }

        /// <summary>
        /// Converts a pair of time strings in the "hh:mm" format into a MinMaxI with the times converted in minutes since midnight.
        /// Value #0 is the minimum value, value #1 is the maximum value.
        /// </summary>
        /// <param name="timeValues">An array containing two time strings in the "hh:mm" format</param>
        /// <returns>MinMax values with the number of minutes since midnight, or null if an error happened</returns>
        private MinMaxI? ParseMinMaxTime(string[] timeValues)
        {
            if (timeValues.Length != 2) return null;

            int[] vals = new int[2];

            for (int i = 0; i < 2; i++)
            {
                try
                {
                    double val = Convert.ToDouble(timeValues[i].Replace(':', '.').Trim(), NumberFormatInfo.InvariantInfo);
                    int hours = (int)val; // Hours are stored in the in the integer part
                    int minutes = Toolbox.Clamp((int)((val - hours) * 60.0), 0, 59); // Minutes are stores in the decimal part
                    hours = Toolbox.Clamp(hours, 0, 23);
                    vals[i] = hours * 60 + minutes;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            if (vals[0] > vals[1]) return null; // Min value > Max value. BAD!

            return new MinMaxI(vals[0], vals[1]);
        }

        /// <summary>
        /// Returns an array of all airbases in this theater.
        /// </summary>
        /// <returns>An array of <see cref="DBEntryAirbase"/></returns>
        public DBEntryAirbase[] GetAirbases()
        {
            return
                (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>()
                 where Toolbox.StringICompare(airbase.Theater, ID)
                 select airbase).ToArray();
        }
    }
}
