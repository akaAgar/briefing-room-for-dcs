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

namespace BriefingRoom4DCS.Generator
{
    [Flags]
    internal enum UnitMakerGroupFlags
    {
        /// <summary>
        /// Unit is always hidden.
        /// </summary>
        AlwaysHidden = 1,

        /// <summary>
        /// First unit of this group is of skill "Client". Others use the normal group skill level.
        /// </summary>
        FirstUnitIsClient = 2,

        /// <summary>
        /// Aircraft should be added to the queue of aircraft to be spawned immediately on player takeoff, and not spawned later during the mission.
        /// </summary>
        ImmediateAircraftSpawn = 4,

        /// <summary>
        /// Unit is always visible.
        /// </summary>
        NeverHidden = 8,
    }
}