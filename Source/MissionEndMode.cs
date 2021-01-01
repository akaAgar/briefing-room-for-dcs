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
    /// Enumerates possible EndTime Values
    /// </summary>
    public enum MissionEndMode
    {
        /// <summary>
        /// don't end
        /// </summary>
        NoEnd = -1,

        /// <summary>
        /// End via command
        /// </summary>
        F10Command = -2,

        /// <summary>
        /// End instantly
        /// </summary>
        Instant = 0,

        /// <summary>
        /// End in 5 Mins
        /// </summary>
        FiveMins = 5,

        /// <summary>
        /// End in 10 Mins
        /// </summary>
        TenMins = 10,

        /// <summary>
        /// End in 20 Mins
        /// </summary>
        TwentyMins = 20,
        
    }
}
