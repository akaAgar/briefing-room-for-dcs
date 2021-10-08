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
    /// Enumerates various possible mission options.
    /// </summary>
    public enum MissionOption
    {
        [Display(Name = "Enable civilian traffic", Description = "If true, civilian traffic will be enabled. Can have an impact on performances.")]
        EnableCivilianTraffic,

        [Display(Name = "Allow spawning anywhere", Description = "Allows spawing anywhere on the map not just in areas of control or neutural zones.")]
        SpawnAnywhere,

        [Display(Name = "ForceOldSpawning", Description = "Forces the system to use a older spawning logic.")]
        ForceOldSpawning,

        [Display(Name = "Use imperial units", Description = "Use imperial units for briefing instead of the metric system.")]
        ImperialUnitsForBriefing,

        [Display(Name = "Invert countries coalitions", Description = "Invert blue and red countries on the map (e.g. In Caucasus, Russia becomes blue and Georgia becomes red).")]
        InvertCountriesCoalitions,

        [Display(Name = "Mark waypoints", Description = "Create Markers on waypoints.")]
        MarkWaypoints,

        [Display(Name = "Text-only radio messages", Description = "Display radio messages in text-format only (no voiceover). Selecting this option will severly decrease the size of the .miz files.")]
        RadioMessagesTextOnly,
    }
}
