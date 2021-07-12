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
    /// Enumerates various possible mission objective options.
    /// </summary>
    public enum ObjectiveOption
    {
        [Display(Name = "Embedded air defense", Description = "Spawn a short-range air defense group near the target, if air defense is enabled for the target's coalition in the mission template.")]
        EmbeddedAirDefense,

        [Display(Name = "Hide target", Description = "Target is always hidden on the map, no matter the \"fog of war\" preferences. Ignored if \"Show target\" is set.")]
        HideTarget,

        [Display(Name = "Inaccurate waypoint", Description = "Waypoint will not be spawned on the target itself (or at the target's initial position for moving targets) but a few nautical miles away, so player(s) will have to search for it.")]
        InaccurateWaypoint,

        [Display(Name = "Show target", Description = "Target is always visible on the map, no matter the \"fog of war\" preferences. Overrides \"Hide target\".")]
        ShowTarget,
    }
}
