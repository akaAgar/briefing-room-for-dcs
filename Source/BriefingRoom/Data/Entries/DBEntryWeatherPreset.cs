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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a weather condition preset.
    /// </summary>
    internal class DBEntryWeatherPreset : DBEntry
    {
        /// <summary>
        /// Is this weather considered bad weather (for "bad weather chance" in campaign generation)?
        /// </summary>
        internal bool BadWeather { get; private set; }

        /// <summary>
        /// String to display in the briefing when this weather preset is selected.
        /// </summary>
        internal string BriefingDescription { get; private set; }

        /// <summary>
        /// Min,max base cloud height (in meters).
        /// </summary>
        internal MinMaxI CloudsBase { get; private set; }

        /// <summary>
        /// DCS World 2.7 cloud presets to use with this weather pattern.
        /// </summary>
        internal string[] CloudsPresets { get; private set; }

        /// <summary>
        /// Min,max base cloud thickness in meters
        /// </summary>
        internal MinMaxI CloudsThickness { get; private set; }

        /// <summary>
        /// Are dust storms enabled?
        /// </summary>
        internal bool Dust { get; private set; }

        /// <summary>
        /// Min,max dust storm density, if enabled (0-10).
        /// </summary>
        internal MinMaxI DustDensity { get; private set; }

        /// <summary>
        /// Is fog enabled?
        /// </summary>
        internal bool Fog { get; private set; }

        /// <summary>
        /// Min,max fog thickness.
        /// </summary>
        internal MinMaxI FogThickness { get; private set; }

        /// <summary>
        /// Min,max fog visibility in meters.
        /// </summary>
        internal MinMaxI FogVisibility { get; private set; }

        /// <summary>
        /// Min,max atmospheric pressure at mean sea level.
        /// </summary>
        internal MinMaxI QNH { get; private set; }

        /// <summary>
        /// Amount of turbulence (in m/s) to add to the wind turbulence.
        /// </summary>
        internal MinMaxI Turbulence { get; private set; }

        /// <summary>
        /// Min,max visibility in meters.
        /// </summary>
        internal MinMaxI Visibility { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingDescription = ini.GetValue<string>("Briefing", "Description");

                CloudsBase = ini.GetValue<MinMaxI>("Weather", "Clouds.Base");
                CloudsPresets = ini.GetValueArray<string>("Weather", "Clouds.Presets");
                CloudsThickness = ini.GetValue<MinMaxI>("Weather", "Clouds.Thickness");

                Dust = ini.GetValue<bool>("Weather", "Dust");
                DustDensity = ini.GetValue<MinMaxI>("Weather", "Dust.Density");

                Fog = ini.GetValue<bool>("Weather", "Fog");
                FogThickness = ini.GetValue<MinMaxI>("Weather", "Fog.Thickness");
                FogVisibility = ini.GetValue<MinMaxI>("Weather", "Fog.Visibility");

                BadWeather = ini.GetValue<bool>("Weather", "IsBadWeather");
                QNH = ini.GetValue<MinMaxI>("Weather", "QNH");
                Turbulence = ini.GetValue<MinMaxI>("Weather", "Turbulence");
                Visibility = ini.GetValue<MinMaxI>("Weather", "Visibility");
            }

            return true;
        }
    }
}
