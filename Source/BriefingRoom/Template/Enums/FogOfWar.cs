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
    public enum FogOfWar
    {
        [Display(Name = "Show all units", Description = "Show all units on the F10 map and the players' mission planner, perfect intelligence.")]
        All,

        [Display(Name = "Show allies only", Description = "Show only allied units on the F10 map and the players' mission planner.")]
        AlliesOnly,

        [Display(Name = "Show known units only", Description = "Hide all units except those in view of an ally unit.")]
        KnownUnitsOnly,

        [Display(Name = "Show self only", Description = "Hide all units except the player on the F10 map.")]
        SelfOnly,

        [Display(Name = "Show no units", Description = "Hide all units (INCLUDING the player) on the F10 map.")]
        None
    }
}


