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



namespace BriefingRoom.DB
{
    /// <summary>
    /// Stores information about a set of weather conditions in a.
    /// </summary>
    public struct DBEntryWeather
    {
        /// <summary>
        /// Min,max base cloud height (in meters).
        /// </summary>
        public MinMaxI CloudsBase { get; }

        /// <summary>
        /// Min,max cloud density (0-10).
        /// </summary>
        public MinMaxI CloudsDensity { get; }

        /// <summary>
        /// Precipitation type. Be aware that DCS only allow some values is cloud density if high enough. Invalid values will be ignored by DCS.
        /// </summary>
        public Precipitation[] CloudsPrecipitation { get; }

        /// <summary>
        /// Min,max base cloud thickness in meters
        /// </summary>
        public MinMaxI CloudsThickness { get; }

        /// <summary>
        /// Are dust storm enabled. If multiple values can be provided, a random value will be selected(e.g. "true,true,false" gives 66% chance of duststorm).
        /// </summary>
        public bool[] DustEnabled { get; }

        /// <summary>
        /// Min,max dust storm density, if enabled (0-10).
        /// </summary>
        public MinMaxI DustDensity { get; }

        /// <summary>
        /// Is fog enabled. If multiple values can be provided, a random value will be selected(e.g. true,true,false gives 66% chance of fog).
        /// </summary>
        public bool[] FogEnabled { get; }

        /// <summary>
        /// Min,max fog thickness.
        /// </summary>
        public MinMaxI FogThickness { get; }

        /// <summary>
        /// Min,max fog visibility in meters.
        /// </summary>
        public MinMaxI FogVisibility { get; }

        /// <summary>
        /// Min,max atmospheric pressure at mean sea level.
        /// </summary>
        public MinMaxI QNH { get; }

        /// <summary>
        /// Min,max turbulence in meters/second.
        /// </summary>
        public MinMaxI Turbulence { get; }

        /// <summary>
        /// Min,max visibility in meters.
        /// </summary>
        public MinMaxI Visibility { get; }

        /// <summary>
        /// Constructor. Loads data from weather database entry .ini file.
        /// </summary>
        /// <param name="ini">The .ini file to load from.</param>
        /// <param name="key">The value key.</param>
        public DBEntryWeather(INIFile ini, string key)
        {
            CloudsBase = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Base");
            CloudsDensity = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Density");
            CloudsPrecipitation = ini.GetValueArray<Precipitation>("Weather", key + ".Clouds.Precipitation");
            if (CloudsPrecipitation.Length == 0) CloudsPrecipitation = new Precipitation[] { Precipitation.None };
            CloudsThickness = ini.GetValue<MinMaxI>("Weather", key + ".Clouds.Thickness");

            DustEnabled = ini.GetValueArray<bool>("Weather", key + ".Dust.Enabled");
            if (DustEnabled.Length == 0) DustEnabled = new bool[] { false };
            DustDensity = ini.GetValue<MinMaxI>("Weather", key + ".Dust.Density");

            FogEnabled = ini.GetValueArray<bool>("Weather", key + ".Fog.Enabled");
            if (FogEnabled.Length == 0) DustEnabled = new bool[] { false };
            FogThickness = ini.GetValue<MinMaxI>("Weather", key + ".Fog.Thickness");
            FogVisibility = ini.GetValue<MinMaxI>("Weather", key + ".Fog.Visibility");

            QNH = ini.GetValue<MinMaxI>("Weather", key + ".QNH");
            Turbulence = ini.GetValue<MinMaxI>("Weather", key + ".Turbulence");
            Visibility = ini.GetValue<MinMaxI>("Weather", key + ".Visibility");
        }
    }
}
