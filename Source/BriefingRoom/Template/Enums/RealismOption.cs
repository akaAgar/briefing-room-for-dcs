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
    public enum RealismOption
    {
        [Display(Name = "Bird strikes", Description = "Enable bird strikes.")]
        BirdStrikes,

        [Display(Name = "Disable radio assists", Description = "Disable DCS radio assists (\"fire\" when weapon release parameters are met, warning when a threat approaches, etc.)")]
        DisableDCSRadioAssists,

        [Display(Name = "Hide labels", Description = "Disable unit labels.")]
        HideLabels,

        [Display(Name = "No BDA", Description = "Disable DCS World's battle-damage assessment log displayed when players damage or destroy something.")]
        NoBDA,

        [Display(Name = "No cheats", Description = "Disable all cheats (infinite weapons, infinite fuel, invulerability).")]
        NoCheats,

        [Display(Name = "No crash recovery", Description = "Players cannot respawn in a new plane when crashing (\"single life\" mode).")]
        NoCrashRecovery,

        [Display(Name = "No easy comms", Description = "Enforce realistic communications.")]
        NoEasyComms,

        [Display(Name = "No external views", Description = "Disable external views.")]
        NoExternalViews,

        [Display(Name = "No game mode", Description = "Disable \"game mode\" simplfications (flight model and avionics).")]
        NoGameMode,

        [Display(Name = "No overlays", Description = "Disable overlays (arcade minimap, etc.)")]
        NoOverlays,

        [Display(Name = "No padlock", Description = "Disable padlock views.")]
        NoPadlock,

        [Display(Name = "Random failures", Description = "Enable random aircraft system failures.")]
        RandomFailures,

        [Display(Name = "Realistic G-effects", Description = "Enable ealistic G-effects.")]
        RealisticGEffects,

        [Display(Name = "Wake turbulence", Description = "Enable realistic wake turbulence.")]
        WakeTurbulence
    }
}
