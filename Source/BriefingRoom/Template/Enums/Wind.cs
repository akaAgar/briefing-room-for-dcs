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

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// How windy should the weather be during a mission.
    /// </summary>
    public enum Wind
    {
        [Display(Name = "Random", Description = "Wind level will be picked randomly")]
        Random = -1, // Auto must be < 0 for casts from Int32

        [Display(Name = "Calm", Description = "Force 0, No wind at all. ")]
        Calm,

        [Display(Name = "Light Breeze", Description = "Force 1-2, Light wind.")]
        LightBreeze,

        [Display(Name = "Moderate breeze", Description = "Force 3-4, Average wind.")]
        ModerateBreeze,

        [Display(Name = "Strong breeze", Description = "Force 5-6, Noticeable wind, flying and aiming may require skill.")]
        StrongBreeze,

        [Display(Name = "Gale", Description = "Force 7-8, Strong wind. Flying and aiming will be heavily impaired.")]
        Gale,

        [Display(Name = "Storm", Description = "Force 9-10, Very strong wind. Flying in these condition may prove dangerous.")]
        Storm
    }
}
