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
    public enum RealismOption
    {
        [TreeViewEnum("Bird strikes", "Enable bird strikes.")]
        BirdStrikes,

        [TreeViewEnum("Disable radio assists", "Disable DCS radio assists (\"fire\" when weapon release parameters are met, warning when a threat approaches, etc.)")]
        DisableDCSRadioAssists,

        [TreeViewEnum("Hide enemy units", "Hide enemy units on the F10 map, mission planning, MFD SA pages, etc.")]
        HideEnemyUnits,

        [TreeViewEnum("Hide labels", "Disable unit labels.")]
        HideLabels,

        [TreeViewEnum("Map, disable all", "Hide all units (INCLUDING the player) on the F10 map. Supersedes \"Map, disable all but self\" and \"Map, disable all but known units\".")]
        MapDisableAll,

        [TreeViewEnum("Map, disable all but self", "Hide all units except the player on the F10 map. Supersedes \"Map, disable all but known units\".")]
        MapDisableAllButSelf,

        [TreeViewEnum("Map, disable all but known units", "Hide all units except those in view of an ally unit (\"fog of war\" mode).")]
        MapDisableAllButUnitsKnown,

        [TreeViewEnum("No BDA", "Disable DCS World's battle-damage assessment log displayed when players damage or destroy something.")]
        NoBDA,

        [TreeViewEnum("No cheats", "Disable all cheats (infinite weapons, infinite fuel, invulerability).")]
        NoCheats,

        [TreeViewEnum("No crash recovery", "Players cannot respawn in a new plane when crashing (\"single life\" mode).")]
        NoCrashRecovery,

        [TreeViewEnum("No easy comms", "Enforce realistic communications.")]
        NoEasyComms,

        [TreeViewEnum("No external views", "Disable external views.")]
        NoExternalViews,

        [TreeViewEnum("No game mode", "Disable \"game mode\" simplfications (flight model and avionics).")]
        NoGameMode,

        [TreeViewEnum("No overlays", "Disable overlays (arcade minimap, etc.)")]
        NoOverlays,

        [TreeViewEnum("No padlock", "Disable padlock views.")]
        NoPadlock,

        [TreeViewEnum("Random failures", "Enable random aircraft system failures.")]
        RandomFailures,

        [TreeViewEnum("Realistic G-effects", "Enable ealistic G-effects.")]
        RealisticGEffects,

        [TreeViewEnum("Wake turbulence", "Enable realistic wake turbulence.")]
        WakeTurbulence
    }
}
