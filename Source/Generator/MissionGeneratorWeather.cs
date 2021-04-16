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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using System;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates a <see cref="DCSMission"/>'s weather
    /// </summary>
    public class MissionGeneratorWeather : IDisposable
    {
        /// <summary>
        /// Array of random weathers to select from.
        /// "Clearer" weather is more frequent, because it wouldn't be fun if the <see cref="Weather.Random"/> setting
        /// resulted in a storm in 20% of missions.
        /// </summary>
        private static readonly Weather[] RANDOM_WEATHER = new Weather[]
        {
            Weather.Clear, Weather.Clear, Weather.Clear,
            Weather.LightClouds, Weather.LightClouds, Weather.LightClouds,
            Weather.SomeClouds, Weather.SomeClouds,
            Weather.Overcast, Weather.Precipitation, Weather.Storm
        };

        /// <summary>
        /// Wind altitude levels (in meters) to set in the mission Lua file
        /// </summary>
        private static readonly int[] WIND_ALTITUDE = new int[] { 0, 2000, 8000 };

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGeneratorWeather() { }

        /// <summary>
        /// IDispose implementation.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Generates weather settings (precipitation, cloud coverage, temperature...) for the mission.
        /// Must be called after mission date has been set because min/max temperature changes every month.
        /// </summary>
        /// <param name="mission">The mission.</param>
        /// <param name="weather">The preferred type of weather (clear, cloudy, storm...).</param>
        /// <param name="theaterDB">Theater database entry</param>
        public void GenerateWeather(DCSMission mission, Weather weather, DBEntryTheater theaterDB, DatabaseCommon commonDB)
        {
            mission.Weather.Temperature = theaterDB.Temperature[(int)mission.DateTime.Month].GetValue();
            mission.Weather.WeatherLevel = (weather == Weather.Random) ? Toolbox.RandomFrom(RANDOM_WEATHER) : weather;

            DebugLog.Instance.WriteLine($"Weather set to {mission.Weather.WeatherLevel}", 1);

            int weatherIndex = (int)mission.Weather.WeatherLevel;

            // Clouds and precipitations
            mission.Weather.CloudBase = commonDB.Weather[weatherIndex].CloudsBase.GetValue();
            mission.Weather.CloudsDensity = commonDB.Weather[weatherIndex].CloudsDensity.GetValue();
            mission.Weather.CloudsPrecipitation = Toolbox.RandomFrom(commonDB.Weather[weatherIndex].CloudsPrecipitation);
            mission.Weather.CloudsThickness = commonDB.Weather[weatherIndex].CloudsThickness.GetValue();

            // Dust
            mission.Weather.DustEnabled = Toolbox.RandomFrom(commonDB.Weather[weatherIndex].DustEnabled);
            mission.Weather.DustDensity = mission.Weather.DustEnabled ? commonDB.Weather[weatherIndex].DustDensity.GetValue() : 0;

            // Fog
            mission.Weather.FogEnabled = Toolbox.RandomFrom(commonDB.Weather[weatherIndex].FogEnabled);
            mission.Weather.FogThickness = mission.Weather.FogEnabled ? commonDB.Weather[weatherIndex].FogThickness.GetValue() : 0;
            mission.Weather.FogVisibility = mission.Weather.FogEnabled ? commonDB.Weather[weatherIndex].FogVisibility.GetValue() : 0;

            // Pressure, turbulence and visiblity
            mission.Weather.QNH = commonDB.Weather[weatherIndex].QNH.GetValue();
            mission.Weather.Turbulence = commonDB.Weather[weatherIndex].Turbulence.GetValue();
            mission.Weather.Visibility = commonDB.Weather[(int)mission.Weather.WeatherLevel].Visibility.GetValue();

            DebugLog.Instance.WriteLine($"Cloud base altitude set to {mission.Weather.CloudBase} m", 1);
            DebugLog.Instance.WriteLine($"Cloud density set to {mission.Weather.CloudBase}", 1);
            DebugLog.Instance.WriteLine($"Precipitation set to {mission.Weather.CloudsPrecipitation}", 1);
            DebugLog.Instance.WriteLine($"Cloud thickness set to {mission.Weather.CloudsThickness} m", 1);

            DebugLog.Instance.WriteLine($"Dust set to {mission.Weather.DustEnabled}", 1);
            DebugLog.Instance.WriteLine($"Dust density set to {mission.Weather.DustDensity}", 1);

            DebugLog.Instance.WriteLine($"Fog set to {mission.Weather.FogEnabled}", 1);
            DebugLog.Instance.WriteLine($"Fog thickness set to {mission.Weather.FogThickness}", 1);
            DebugLog.Instance.WriteLine($"Fog visibility set to {mission.Weather.FogVisibility} m", 1);

            DebugLog.Instance.WriteLine($"QNH set to {mission.Weather.QNH}", 1);
            DebugLog.Instance.WriteLine($"Turbulence set to {mission.Weather.Turbulence} m/s", 1);
            DebugLog.Instance.WriteLine($"Visibility set to {mission.Weather.Visibility} m", 1);
        }

        /// <summary>
        /// Generates wind settings for the mission.
        /// Must be called once mission weather level has been set, as weather is used for auto wind.
        /// </summary>
        /// <param name="mission">The mission.</param>
        /// <param name="wind">The preferred wind speed.</param>
        /// <param name="theaterDB">Theater database entry</param>
        public void GenerateWind(DCSMission mission, Wind wind, DBEntryTheater theaterDB)
        {
            // If auto, speed depends on weather, so we never end up with no wind in a storm
            mission.Weather.WindLevel = (wind == Wind.Auto) ?
                (Wind)Toolbox.Clamp((int)mission.Weather.WeatherLevel + Toolbox.RandomMinMax(-1, 1), 0, (int)Wind.StrongGale)
                : wind;

            DebugLog.Instance.WriteLine($"Wind speed level set to {mission.Weather.WindLevel}", 1);

            int windIndex = (int)mission.Weather.WindLevel;

            for (int i = 0; i < 3; i++)
            {
                mission.Weather.WindSpeed[i] = Math.Max(0, theaterDB.Wind[windIndex].Wind.GetValue());
                mission.Weather.WindDirection[i] = (mission.Weather.WindSpeed[i] > 0) ? Toolbox.RandomInt(0, 360) : 0;
                DebugLog.Instance.WriteLine($"Wind speed at {WIND_ALTITUDE[i]} meters set to {mission.Weather.WindSpeed[i]} m/s, direction of {mission.Weather.WindDirection[i]}", 1);
            }


            // Turbulence is equals to max(weatherTurbulence, windTurbulence)
            mission.Weather.Turbulence = Math.Max(mission.Weather.Turbulence, theaterDB.Wind[windIndex].Turbulence.GetValue());
            DebugLog.Instance.WriteLine($"Turbulence updated to {mission.Weather.Turbulence} m/s", 1);
        }
    }
}
