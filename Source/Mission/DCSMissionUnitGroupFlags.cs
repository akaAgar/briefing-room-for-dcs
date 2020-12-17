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

using System;

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Enumerates all special flags for <see cref="DCSMissionUnitGroup"/>.
    /// </summary>
    [Flags]
    public enum DCSMissionUnitGroupFlags
    {
        /// <summary>
        /// Overrides the skill level of the first unit of the group to make it PLAYER-controlled (for SP mission, other units in the group are AI wingmen)
        /// </summary>
        FirstUnitIsPlayer = 1,
        /// <summary>
        /// Unit group will be hidden in the planning, on MFD SA pages and on the F10 map.
        /// </summary>
        Hidden = 2,
    }
}
