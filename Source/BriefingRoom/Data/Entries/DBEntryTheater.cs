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
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryTheater : DBEntry
    {
        private static readonly MinMaxI DEFAULT_DAYTIME = new MinMaxI(8 * 60, 19 * 60);

        internal Coordinates DefaultMapCenter { get; private set; }

        internal string DCSID { get; private set; }

        internal double MagneticDeclination { get; private set; }

        internal MinMaxI[] DayTime { get; private set; }

        internal List<Coordinates> WaterCoordinates { get; private set; }

        internal List<List<Coordinates>> WaterExclusionCoordinates { get; private set; }

        internal DBEntryTheaterSpawnPoint[] SpawnPoints { get; private set; }
        internal MinMaxI[] Temperature { get; private set; }

        internal double FalseEasting { get; private set; }
        internal double FalseNorthing { get; private set; }

        internal int CentralMeridian { get; private set; }
        internal double ScaleFactor { get; private set; }


        protected override bool OnLoad(string iniFilePath)
        {
            int i;

            var ini = new INIFile(iniFilePath);

            // [Theater] section
            DCSID = ini.GetValue<string>("Theater", "DCSID");
            DefaultMapCenter = ini.GetValue<Coordinates>("Theater", "DefaultMapCenter");
            MagneticDeclination = ini.GetValue<double>("Theater", "MagneticDeclination");

            FalseEasting = ini.GetValue<double>("Theater", "FalseEasting");
            FalseNorthing = ini.GetValue<double>("Theater", "FalseNorthing");
            CentralMeridian = ini.GetValue<int>("Theater", "CentralMeridian");
            ScaleFactor = ini.GetValue<double>("Theater", "ScaleFactor");

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


            // Water Coordinates
            WaterCoordinates = new List<Coordinates>();
            foreach (string key in ini.GetKeysInSection("WaterCoordinates"))
                WaterCoordinates.Add(ini.GetValue<Coordinates>("WaterCoordinates", key));


            List<DBEntryTheaterSpawnPoint> spawnPointsList = new List<DBEntryTheaterSpawnPoint>();
            foreach (string key in ini.GetKeysInSection("SpawnPoints"))
            {
                DBEntryTheaterSpawnPoint sp = new DBEntryTheaterSpawnPoint();
                if (sp.Load(ini, key))
                    spawnPointsList.Add(sp);
            }
            SpawnPoints = spawnPointsList.ToArray();

            WaterExclusionCoordinates = new List<List<Coordinates>>();
            if (ini.GetSections().Contains("waterexclusioncoordinates"))
            {
                // Water Exclusion Coordinates
                var tempList = new List<Coordinates>();
                var groupID = ini.GetKeysInSection("WaterExclusionCoordinates").First().Split(".")[0];
                foreach (string key in ini.GetKeysInSection("WaterExclusionCoordinates"))
                {
                    var newGroupId = key.Split(".")[0];
                    if (groupID != newGroupId)
                    {
                        groupID = newGroupId;
                        WaterExclusionCoordinates.Add(tempList);
                        tempList = new List<Coordinates>();
                    }
                    tempList.Add(ini.GetValue<Coordinates>("WaterExclusionCoordinates", key));
                }
                WaterExclusionCoordinates.Add(tempList);
            }

            // [Temperature] section
            Temperature = new MinMaxI[12];
            for (i = 0; i < 12; i++)
                Temperature[i] = ini.GetValue<MinMaxI>("Temperature", ((Month)i).ToString());

            return true;
        }

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

        internal double[] GetRealWorldCoordinates(Coordinates coords){
            var cFac = new CoordinateSystemFactory();
            List<ProjectionParameter> parameters = new List<ProjectionParameter>();
            parameters.Add(new ProjectionParameter("latitude_of_origin", 0));
            parameters.Add(new ProjectionParameter("central_meridian", CentralMeridian));
            parameters.Add(new ProjectionParameter("false_easting", FalseEasting));
            parameters.Add(new ProjectionParameter("false_northing", FalseNorthing));
            parameters.Add(new ProjectionParameter("scale_factor",ScaleFactor));
            var projection = cFac.CreateProjection("Mercator_1SP", "Mercator_1SP", parameters);
            var dcsSys = cFac.CreateProjectedCoordinateSystem("World Mercator WGS84", GeographicCoordinateSystem.WGS84, projection, LinearUnit.Metre, new AxisInfo("North", AxisOrientationEnum.North), new AxisInfo("East", AxisOrientationEnum.East));
            var trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(dcsSys, GeographicCoordinateSystem.WGS84);
            return trans.MathTransform.Transform(coords.ToList().ToArray());
        }
    }
}
