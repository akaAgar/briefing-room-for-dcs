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
using System.Linq;
using BriefingRoom4DCS.Data.JSON;
using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.Data
{
    internal struct DBEntryAirbaseParkingSpot
    {
        internal int DCSID { get; private set; }

        internal Coordinates Coordinates { get; private set; }

        internal ParkingSpotType ParkingType { get; private set; }

        public DBEntryAirbaseParkingSpot() { }
        internal DBEntryAirbaseParkingSpot(INIFile ini, string section, string parkingKey)
        {
            DCSID = ini.GetValue<int>(section, $"{parkingKey}.DCSID");
            Coordinates = ini.GetValue<Coordinates>(section, $"{parkingKey}.Coordinates");

            ParkingType = ini.GetValue<ParkingSpotType>(section, $"{parkingKey}.Type");
        }

        internal static DBEntryAirbaseParkingSpot[] LoadJSON(List<Parking> data, string airbaseId)
        {
            return data.Select(p =>
            {
                var parkingSpotType = ParkingSpotType.Unknown;
                try
                {
                    parkingSpotType = (ParkingSpotType)Enum.Parse(typeof(ParkingSpotType), p.Term_Type_Name, true);
                }
                catch (System.Exception)
                {
                    BriefingRoom.PrintToLog($"Failed to parse parking type: {p.Term_Type_Name} (airbase: {airbaseId}, id: {p.Term_Index})", LogMessageErrorLevel.Warning);
                }
                return new DBEntryAirbaseParkingSpot
                {
                    DCSID = p.Term_Index,
                    Coordinates = new Coordinates(p.pos.DCS.x, p.pos.DCS.z),
                    ParkingType = parkingSpotType
                };
            }).ToArray();
        }
    }
}
