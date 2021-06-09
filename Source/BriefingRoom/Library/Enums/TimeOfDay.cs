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

using System.ComponentModel.DataAnnotations;

namespace BriefingRoom4DCS
{
    /// <summary>
    /// The time of the day (used by mission starting time), with "Random" and "Random, but not at night" options.
    /// </summary>
    public enum TimeOfDay
    {
        [Display(Name = "Random", Description = "Any time of the day or night.")]
        Random,
        
        [Display(Name = "Random, daytime", Description = "Any time during daylight hours.")]
        RandomDaytime,

        [Display(Name = "Dawn", Description = "Just after sunrise.")]
        Dawn,

        [Display(Name = "Noon", Description = "In the middle of the day.")]
        Noon,
        
        [Display(Name = "Twilight", Description = "Just before sunset.")]
        Twilight,
        
        [Display(Name = "Night", Description = "At night.")]
        Night
    }
}
