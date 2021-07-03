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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates a <see cref="DCSMission"/> date and time parameters
    /// </summary>
    internal class MissionGeneratorDateTime : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorDateTime() { }

        /// <summary>
        /// Picks a date (day, month and year) for the mission.
        /// </summary>
        /// <param name="mission">The mission</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="month">Selected month (used for weather generation)</param>
        internal void GenerateMissionDate(DCSMission mission, MissionTemplate template, out Month month)
        {
            int day, year;

            // Select a random year from the most recent coalition's decade.
            year = Toolbox.GetRandomYearFromDecade(template.ContextDecade);

            BriefingRoom.PrintToLog($"No fixed date provided in the mission template, generating date in decade {template.ContextDecade}");

            if (template.EnvironmentSeason == Season.Random) // Random season, pick any day of the year.
            {
                month = (Month)Toolbox.RandomInt(12);
                day = Toolbox.RandomMinMax(1, GeneratorTools.GetDaysPerMonth(month, year));
            }
            else // Pick a date according to the desired season
            {
                Month[] seasonMonths = GetMonthsForSeason(template.EnvironmentSeason);

                int monthIndex = Toolbox.RandomInt(4);
                month = seasonMonths[monthIndex];
                switch (monthIndex)
                {
                    case 0: // First month of the season, season begins on the 21st
                        day = Toolbox.RandomMinMax(21, GeneratorTools.GetDaysPerMonth(month, year)); break;
                    case 3: // Last month of the season, season ends on the 20th
                        day = Toolbox.RandomMinMax(1, 20); break;
                    default:
                        day = Toolbox.RandomMinMax(1, GeneratorTools.GetDaysPerMonth(month, year)); break;
                }
            }

            mission.SetValue("DateDay", day);
            mission.SetValue("DateMonth", (int)month + 1);
            mission.SetValue("DateYear", year);
            mission.SetValue("BriefingDate", $"{(int)month + 1:00}/{day:00}/{year:0000}");

            BriefingRoom.PrintToLog($"Misson date set to {day} {month} {year}.");
        }

        /// <summary>
        /// Picks a starting time for the mission.
        /// </summary>
        /// <param name="mission">Mission to generate.</param>
        /// <param name="template">Mission template to use.</param>
        /// <param name="theaterDB">Theater database entry.</param>
        /// <param name="month">Month during which the mission takes place.</param>
        internal void GenerateMissionTime(DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB, Month month)
        {
            double totalMinutes;
            int hour, minute;

            switch (template.EnvironmentTimeOfDay)
            {
                default: // case TimeOfDay.Random
                    totalMinutes = Toolbox.RandomInt(Toolbox.MINUTES_PER_DAY);
                    break;

                case TimeOfDay.RandomDaytime:
                    totalMinutes = Toolbox.RandomInt(theaterDB.DayTime[(int)month].Min, theaterDB.DayTime[(int)month].Max - 60);
                    break;

                case TimeOfDay.Dawn:
                    totalMinutes = Toolbox.RandomInt(theaterDB.DayTime[(int)month].Min, theaterDB.DayTime[(int)month].Min + 120);
                    break;

                case TimeOfDay.Noon:
                    totalMinutes = Toolbox.RandomInt(
                        (theaterDB.DayTime[(int)month].Min + theaterDB.DayTime[(int)month].Max) / 2 - 90,
                        (theaterDB.DayTime[(int)month].Min + theaterDB.DayTime[(int)month].Max) / 2 + 90);
                    break;

                case TimeOfDay.Twilight:
                    totalMinutes = Toolbox.RandomInt(theaterDB.DayTime[(int)month].Max - 120, theaterDB.DayTime[(int)month].Max + 30);
                    break;

                case TimeOfDay.Night:
                    totalMinutes = Toolbox.RandomInt(0, theaterDB.DayTime[(int)month].Min - 120);
                    break;
            }

            hour = Toolbox.Clamp((int)Math.Floor(totalMinutes / 60), 0, 23);
            minute = Toolbox.Clamp((int)Math.Floor((totalMinutes - hour * 60) / 15) * 15, 0, 45);

            mission.SetValue("BriefingTime", $"{hour:00}:{minute:00}");
            mission.SetValue("StartTime", hour * 3600 + minute * 60); // DCS World time is stored in seconds since midnight
        }

        /// <summary>
        /// Returns the months of a given season.
        /// Season begins on the 21st of the first month and ends on the 20th of the last month.
        /// </summary>
        /// <param name="season">A season</param>
        /// <returns>An array of four months</returns>
        private static Month[] GetMonthsForSeason(Season season)
        {
            switch (season)
            {
                default: return new Month[] { Month.March, Month.April, Month.May, Month.June }; // case Season.Spring or Season.Random
                case Season.Summer: return new Month[] { Month.June, Month.July, Month.August, Month.September };
                case Season.Fall: return new Month[] { Month.September, Month.October, Month.November, Month.December };
                case Season.Winter: return new Month[] { Month.December, Month.January, Month.February, Month.March };
            }
        }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }
    }
}
