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
    /// The time of the day (used by mission starting time), with "Random" and "Random, but not at night" options.
    /// </summary>
    public enum TimeOfDay
    {
        [TreeViewEnum("Random", "Any time of the day or night.")]
        Random,
        
        [TreeViewEnum("Random, daytime", "Any time during daylight hours.")]
        RandomDaytime,

        [TreeViewEnum("Dawn", "Just after sunrise.")]
        Dawn,

        [TreeViewEnum("Noon", "In the middle of the day.")]
        Noon,
        
        [TreeViewEnum("Twilight", "Just before sunset.")]
        Twilight,
        
        [TreeViewEnum("Night", "At night.")]
        Night
    }
}
