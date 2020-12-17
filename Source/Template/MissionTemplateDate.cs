/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCSWorld.TypeConverters;
using System;
using System.ComponentModel;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// Stores date information for a <see cref="MissionTemplate"/>
    /// </summary>
    public class MissionTemplateDate : IDisposable
    {
        /// <summary>
        /// Minimum possible year.
        /// </summary>
        private const int MIN_YEAR = 1940;

        /// <summary>
        /// Maximum possible year.
        /// </summary>
        private const int MAX_YEAR = 2020;

        [DisplayName("(Enable custom date)"), Description("If enabled, the custom date will be used. If not, a random mission date will be generated.")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool Enabled { get; set; }

        /// <summary>
        /// Day of the month during which the mission should take place.
        /// </summary>
        [DisplayName("Day"), Description("Day of the month during which the mission should take place.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Day { get { return Day_; } set { Day_ = value; CheckDate(); } }
        private int Day_;

        /// <summary>
        /// Month during which the mission should take place.
        /// </summary>
        [DisplayName("Month"), Description("Month during which the mission should take place.")]
        [RefreshProperties(RefreshProperties.All)]
        public Month Month { get { return Month_; } set { Month_ = value; CheckDate(); } }
        private Month Month_;

        /// <summary>
        /// Year during which the mission should take place.
        /// </summary>
        [DisplayName("Year"), Description("Year during which the mission should take place.")]
        [RefreshProperties(RefreshProperties.All)]
        public int Year { get { return Year_; } set { Year_ = value; CheckDate(); } }
        private int Year_;

        private void CheckDate()
        {
            Year_ = Toolbox.Clamp(Year_, MIN_YEAR, MAX_YEAR);
            Day_ = Toolbox.Clamp(Day_, 1, Toolbox.GetDaysPerMonth(Month_, Year_));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionTemplateDate()
        {
            Clear();
        }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            Enabled = false;
            Day_ = 1;
            Month_ = Month.January;
            Year_ = 2000;
        }

        /// <summary>
        /// Loads the mission date from a mission template .ini file.
        /// </summary>
        /// <param name="ini">.ini file to load the date from</param>
        public void LoadFromFile(INIFile ini)
        {
            Clear();

            Enabled = ini.GetValue("Briefing", "Date.Enabled", Enabled);
            Day_ = ini.GetValue("Briefing", "Date.Day", Day_);
            Month_ = ini.GetValue("Briefing", "Date.Month", Month_);
            Year_ = ini.GetValue("Briefing", "Date.Year", Year_);

            CheckDate();
        }

        /// <summary>
        /// Saves the mission date to a mission template .ini file.
        /// </summary>
        /// <param name="ini">.ini file to save the date to</param>
        public void SaveToFile(INIFile ini)
        {
            ini.SetValue("Briefing", "Date.Enabled", Enabled);
            ini.SetValue("Briefing", "Date.Day", Day_);
            ini.SetValue("Briefing", "Date.Month", Month_);
            ini.SetValue("Briefing", "Date.Year", Year_);
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>A string representation of the <see cref="MissionTemplateDate"/> to display in the PropertyGrid</returns>
        public override string ToString()
        {
            if (!Enabled) return "(Disabled)";
            return $"{Month_} the {Toolbox.GetOrdinalAdjective(Day_)}, {Year}";
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
