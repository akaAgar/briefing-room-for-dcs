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
    /// Enumerates all categories of support units a coalition can have
    /// </summary>
    public enum SupportUnitRoles
    {
        /// <summary>
        /// AWACS/early warning plane
        /// </summary>
        AWACS,
        /// <summary>
        /// Tanker plane (with basket)
        /// </summary>
        TankerBasket,
        /// <summary>
        /// Tanker plane (with boom)
        /// </summary>
        TankerBoom
    }
}
