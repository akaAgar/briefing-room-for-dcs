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

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// A type of location spawn point. Used to know what can be spawned at its location.
    /// </summary>
    public enum TheaterLocationSpawnPointType
    {
        /// <summary>
        /// Small spot on land, maybe in a forest or a street
        /// </summary>
        LandSmall,
        /// <summary>
        /// Average spot on land, open land but not buildable (field, forest clearing...)
        /// </summary>
        LandMedium,
        /// <summary>
        /// Large spot on land, big enough and appropriate for a building to stand
        /// </summary>
        LandLarge,
        /// <summary>
        /// Sea
        /// </summary>
        Sea,
        /// <summary>
        /// Airbase
        /// </summary>
        Airbase
    }
}
