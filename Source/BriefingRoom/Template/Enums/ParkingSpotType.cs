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

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// Enumerates the various parking spots in DCS World
    /// </summary>
    public enum ParkingSpotType
    {
        /// <summary>
        /// Runway spawn spot
        /// </summary>
        Runway,
        /// <summary>
        /// Only helicopters can be spawned on this open air parking spot.
        /// </summary>
        HelicopterOnly,
        /// <summary>
        /// Hardened hard shelter, only planes can be spawned.
        /// </summary>
        HardenedAirShelter,
        /// <summary>
        /// Only planes can be spawned on this open air or closed parking spot.
        /// </summary>
        AirplaneOnly,
        /// <summary>
        /// Open air spawned, anything can be spawned.
        /// </summary>
        OpenAirSpawn
    }
}
