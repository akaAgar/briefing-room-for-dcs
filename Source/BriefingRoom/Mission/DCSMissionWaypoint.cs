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

//namespace BriefingRoom4DCS.Mission
//{
//    /// <summary>
//    /// Stores information about a mission waypoint.
//    /// </summary>
//    public struct DCSMissionWaypoint
//    {
//        /// <summary>
//        /// Minimum altitude multiplier.
//        /// </summary>
//        private const double MIN_ALTITUDE_MULTIPLIER = 0.0;
        
//        /// <summary>
//        /// Maxmimum altitude multiplier.
//        /// </summary>
//        private const double MAX_ALTITUDE_MULTIPLIER = 2.0;

//        /// <summary>
//        /// Minimum speed multiplier.
//        /// </summary>
//        private const double MIN_SPEED_MULTIPLIER = 0.0;

//        /// <summary>
//        /// Maximum speed multiplier.
//        /// </summary>
//        private const double MAX_SPEED_MULTIPLIER = 2.0;

//        /// <summary>
//        /// Default WP action.
//        /// </summary>
//        private const string DEFAULT_WP_ACTION = "Turning Point";

//        /// <summary>
//        /// Default WP action type.
//        /// </summary>
//        private const string DEFAULT_WP_ACTION_TYPE = "Turning Point";

//        /// <summary>
//        /// ID of the airdrome to use for this waypoint, if any.
//        /// </summary>
//        public int AirdromeID { get; }

//        /// <summary>
//        /// Altitude multiplier (1.0 = default altitude for aircraft, 0.0 = ground)
//        /// </summary>
//        public double AltitudeMultiplier { get; }

//        /// <summary>
//        /// X,Y coordinates of the waypoint.
//        /// </summary>
//        public Coordinates Coordinates { get; }

//        /// <summary>
//        /// Name of the waypoint.
//        /// </summary>
//        public string Name { get; }

//        /// <summary>
//        /// Speed multiplier (1.0 = default speed for aircraft, 0.0 = no speed)
//        /// </summary>
//        public double SpeedMultiplier { get; }

//        /// <summary>
//        /// Waypoint action (as written in Mission.lua file)
//        /// </summary>
//        public string WPAction { get; }

//        /// <summary>
//        /// Waypoint action type (as written in Mission.lua file)
//        /// </summary>
//        public string WPType { get; }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="coordinates">X,Y coordinates of the waypoint.</param>
//        /// <param name="name">Name of the waypoint.</param>
//        /// <param name="altitudeMultiplier">Altitude multiplier (1.0 = default altitude for aircraft, 0.0 = ground)</param>
//        /// <param name="speedMultiplier">Speed multiplier (1.0 = default speed for aircraft, 0.0 = no speed)</param>
//        /// <param name="wpAction">Waypoint action (as written in Mission.lua file)</param>
//        /// <param name="wpType">Waypoint action type (as written in Mission.lua file)</param>
//        /// <param name="airdromeID">ID of the airdrome to use for this waypoint, if any.</param>
//        public DCSMissionWaypoint(
//            Coordinates coordinates,
//            string name = "WP$INDEX$",
//            double altitudeMultiplier = 1.0,
//            double speedMultiplier = 1.0,
//            string wpAction = DEFAULT_WP_ACTION,
//            string wpType = DEFAULT_WP_ACTION_TYPE,
//            int airdromeID = 0)
//        {
//            Coordinates = coordinates;
//            Name = name;
//            AltitudeMultiplier = Toolbox.Clamp(altitudeMultiplier, MIN_ALTITUDE_MULTIPLIER, MAX_ALTITUDE_MULTIPLIER);
//            SpeedMultiplier = Toolbox.Clamp(speedMultiplier, MIN_SPEED_MULTIPLIER, MAX_SPEED_MULTIPLIER);
//            WPAction = wpAction;
//            WPType = wpType;
//            AirdromeID = airdromeID;
//        }
//    }
//}
