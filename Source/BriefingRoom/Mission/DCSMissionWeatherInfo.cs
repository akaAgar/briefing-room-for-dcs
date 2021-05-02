///*
//==========================================================================
//This file is part of Briefing Room for DCS World, a mission
//generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

//Briefing Room for DCS World is free software: you can redistribute it
//and/or modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation, either version 3 of
//the License, or (at your option) any later version.

//Briefing Room for DCS World is distributed in the hope that it will
//be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
//==========================================================================
//*/

//using System;
//using System.Linq;

//namespace BriefingRoom4DCS.Mission
//{
//    /// <summary>
//    /// Stores weather info about a <see cref="DCSMission"/>
//    /// </summary>
//    public sealed class DCSMissionWeatherInfo : IDisposable
//    {
//        /// <summary>
//        /// The base altitude for clouds, in meters.
//        /// </summary>
//        public int CloudBase { get; internal set; } = 0;

//        /// <summary>
//        /// The density of the cloud layer (0-10).
//        /// </summary>
//        public int CloudsDensity { get; internal set; } = 0;

//        /// <summary>
//        /// The thickness of the clouds (in meters).
//        /// </summary>
//        public int CloudsThickness { get; internal set; } = 0;

//        ///// <summary>
//        ///// DCS Cloud preset to use.
//        ///// </summary>
//        //public CloudPreset CloudsPreset { get; internal set; } = CloudPreset.None;

//        /// <summary>
//        /// Is dust enabled?
//        /// </summary>
//        public bool DustEnabled { get; internal set; } = false;

//        /// <summary>
//        /// The density of the dust (0-10).
//        /// </summary>
//        public int DustDensity { get; internal set; } = 0;

//        /// <summary>
//        /// Is fog enabled?
//        /// </summary>
//        public bool FogEnabled { get; internal set; } = false;

//        /// <summary>
//        /// The thickness of the fog.
//        /// </summary>
//        public int FogThickness { get; internal set; } = 0;

//        /// <summary>
//        /// Visibiliy in the fog (in meters).
//        /// </summary>
//        public int FogVisibility { get; internal set; } = 0;

//        /// <summary>
//        /// QNH (atmospheric pressure adjusted to mean sea level)
//        /// </summary>
//        public int QNH { get; internal set; } = 0;

//        /// <summary>
//        /// Temperature (in Celsius degrees).
//        /// </summary>
//        public int Temperature { get; internal set; } = 0;

//        /// <summary>
//        /// Turbulence (in m/s)
//        /// </summary>
//        public int Turbulence { get; internal set; } = 0;

//        /// <summary>
//        /// Visibility (in meters)
//        /// </summary>
//        public int Visibility { get; internal set; } = 0;

//        /// <summary>
//        /// Overall weather setting. Not used by DCS World, only used internally and for briefings.
//        /// </summary>
//        public Weather WeatherLevel { get; internal set; } = Weather.Clear;

//        /// <summary>
//        /// Wind direction, in degrees (at 0, 2000 and 8000 meters)
//        /// </summary>
//        public int[] WindDirection { get; internal set; } = new int[] { 0, 0, 0 };

//        /// <summary>
//        /// Overall wind speed. Not used by DCS World, only used internally and for briefings.
//        /// </summary>
//        public Wind WindLevel { get; internal set; } = Wind.Calm;

//        /// <summary>
//        /// Wind speed, in m/s (at 0, 2000 and 8000 meters)
//        /// </summary>
//        public int[] WindSpeed { get; internal set; } = new int[] { 0, 0, 0 };

//        /// <summary>
//        /// Average wind speed (to display in briefings)
//        /// </summary>
//        public float WindSpeedAverage { get { return (float)WindSpeed.Sum() / WindSpeed.Length; } }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public DCSMissionWeatherInfo() { }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }
//    }
//}
