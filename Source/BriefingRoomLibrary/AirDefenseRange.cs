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

namespace BriefingRoom
{
    /// <summary>
    /// Enumerates the various range categories for surface-to-air defense
    /// </summary>
    public enum AirDefenseRange
    {
        /// <summary>
        /// Short-range, whether radar or IR-guided, or AAA (SA-8, SA-9, ZSU-23-4...)
        /// </summary>
        ShortRange,
        /// <summary>
        /// Medium-range (SA-6...)
        /// </summary>
        MediumRange,
        /// <summary>
        /// Long-range (Patriot, SA-10...)
        /// </summary>
        LongRange
    }
}
