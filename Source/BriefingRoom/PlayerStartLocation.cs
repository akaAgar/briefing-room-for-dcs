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

using BriefingRoom.Attributes;

namespace BriefingRoom
{
    /// <summary>
    /// A possible player flight group start location (runway, parking, in air...)
    /// </summary>
    public enum PlayerStartLocation
    {
        [TreeViewEnum("Parking, cold", "Player(s) start on ramp, plane cold, and must perform the start up procedure.")]
        ParkingCold,
        /// <summary>
        /// 
        /// </summary>

        [TreeViewEnum("Parking, hot", "Player(s) start on ramp, plane hot, ready to taxi to the runway.")]
        ParkingHot,

        [TreeViewEnum("Runway", "Player(s) start on the runway, ready to takoff. Not available in multiplayer missions (automatically changed to \"Parking, hot\").")]
        Runway
    }
}
