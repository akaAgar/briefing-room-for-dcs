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

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Stores date/time information for a <see cref="DCSMission"/>.
    /// Not using <see cref="System.DateTime"/> because it can throw unexpected exceptions when date takes an impossible value
    /// (something that can happen during mission generation).
    /// </summary>
    public class DCSMissionDateTime
    {
        public int Hour { get; set; } = 0;
        
        public int Minute { get; set; } = 0;

        public int Day { get; set; } = 1;

        public Month Month { get; set; } = Month.January;

        public int Year { get; set; } = 1970;

        public string ToDateString(bool longFormat)
        {
            DayOfWeek dayOfWeek;
            try
            {
                DateTime dateValue = new DateTime(Year, (int)Month + 1, Day);
                dayOfWeek = dateValue.DayOfWeek;
            }
            catch (Exception) // Date was not valid
            {
                dayOfWeek = DayOfWeek.Monday;
            }

            if (longFormat)
                return $"{dayOfWeek}, {Month} {Day}, {Year:0000}";
            
            return $"{((int)Month + 1):00}/{Day:00}/{Year:0000}";
        }

        public Decade Decade
        {
            get
            {
                if (Year >= 2020) return Decade.Decade2020;
                if (Year >= 2010) return Decade.Decade2010;
                if (Year >= 2000) return Decade.Decade2000;
                if (Year >= 1990) return Decade.Decade1990;
                if (Year >= 1980) return Decade.Decade1980;
                if (Year >= 1970) return Decade.Decade1970;
                if (Year >= 1960) return Decade.Decade1960;
                if (Year >= 1950) return Decade.Decade1950;
                return Decade.Decade1940;
            }
        }

        public string ToTimeString()
        {
            return $"{Hour:00}:{Minute:00}";
        }
    }
}
