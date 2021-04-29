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
        /// 
        /// </summary>
        internal string CloudPreset { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                CloudPreset = ini.GetValue<string>("Weather", "CloudPreset");
            }

            return true;
        }

        ///// <summary>
        ///// Min,max base cloud height (in meters).
        ///// </summary>
        //internal MinMaxI CloudsBase { get; }

        ///// <summary>
        ///// Min,max cloud density (0-10).
        ///// </summary>
        //internal MinMaxI CloudsDensity { get; }

        ///// <summary>
        ///// Precipitation type. Be aware that DCS only allow some values is cloud density if high enough. Invalid values will be ignored by DCS.
        ///// </summary>
        ////internal Precipitation[] CloudsPrecipitation { get; }

        ///// <summary>
        ///// Min,max base cloud thickness in meters
        ///// </summary>
        //internal MinMaxI CloudsThickness { get; }

        ///// <summary>
        ///// Are dust storm enabled. If multiple values can be provided, a random value will be selected(e.g. "true,true,false" gives 66% chance of duststorm).
        ///// </summary>
        //internal bool[] DustEnabled { get; }

        ///// <summary>
        ///// Min,max dust storm density, if enabled (0-10).
        ///// </summary>
        //internal MinMaxI DustDensity { get; }

        ///// <summary>
        ///// Is fog enabled. If multiple values can be provided, a random value will be selected(e.g. true,true,false gives 66% chance of fog).
        ///// </summary>
        //internal bool[] FogEnabled { get; }

        ///// <summary>
        ///// Min,max fog thickness.
        ///// </summary>
        //internal MinMaxI FogThickness { get; }

        ///// <summary>
        ///// Min,max fog visibility in meters.
        ///// </summary>
        //internal MinMaxI FogVisibility { get; }

        ///// <summary>
        ///// Min,max atmospheric pressure at mean sea level.
        ///// </summary>
        //internal MinMaxI QNH { get; }

        ///// <summary>
        ///// Min,max turbulence in meters/second.
        ///// </summary>
        //internal MinMaxI Turbulence { get; }

        ///// <summary>
        ///// Min,max visibility in meters.
        ///// </summary>
        //internal MinMaxI Visibility { get; }

        //        /// <summary>
        //        /// Constructor. Loads data from weather database entry .ini file.
        //        /// </summary>
        //        /// <param name="ini">The .ini file to load from.</param>
        //        /// <param name="key">The value key.</param>
        //        internal DBEntryWeatherPreset(INIFile ini, string key)
        //{
            //CloudsBase = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Base");
            //CloudsDensity = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Density");
            ////CloudsPrecipitation = ini.GetValueArray<Precipitation>("Weather", key + ".Clouds.Precipitation");
            ////if (CloudsPrecipitation.Length == 0) CloudsPrecipitation = new Precipitation[] { Precipitation.None };
            //CloudsThickness = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Thickness");

            //DustEnabled = ini.GetValueArray<bool>("Weather", key + ".Dust.Enabled");
            //if (DustEnabled.Length == 0) DustEnabled = new bool[] { false };
            //DustDensity = ini.GetValue<MinMaxI>("Weather", key + ".Dust.Density");

            //FogEnabled = ini.GetValueArray<bool>("Weather", key + ".Fog.Enabled");
            //if (FogEnabled.Length == 0) DustEnabled = new bool[] { false };
            //FogThickness = ini.GetValue<MinMaxI>("Weather", key + ".Fog.Thickness");
            //FogVisibility = ini.GetValue<MinMaxI>("Weather", key + ".Fog.Visibility");

            //QNH = ini.GetValue<MinMaxI>("Weather", key + ".QNH");
            //Turbulence = ini.GetValue<MinMaxI>("Weather", key + ".Turbulence");
            //Visibility = ini.GetValue<MinMaxI>("Weather", key + ".Visibility");
        //}
    }
}
