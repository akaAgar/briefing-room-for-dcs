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
    public enum RealismOption
    {
        /// <summary>
        /// Enables bird strikes.
        /// </summary>
        BirdStrikes,
        /// <summary>
        /// Disables units labels.
        /// </summary>
        HideLabels,
        /// <summary>
        /// Hides all units (INCLUDING the player) on the map. Supersedes <see cref="MapDisableAllButSelf"/> and <see cref="MapDisableAllButUnitsKnown"/>.
        /// </summary>
        MapDisableAll,
        /// <summary>
        /// Hides all units except the player on the map. Supersedes <see cref="MapDisableAllButUnitsKnown"/>.
        /// </summary>
        MapDisableAllButSelf,
        /// <summary>
        /// Hides all units except those in view of an ally unit ("fog of war" mode).
        /// </summary>
        MapDisableAllButUnitsKnown,
        /// <summary>
        /// Disabled battle-damage assessment when players destroy something.
        /// </summary>
        NoBDA,
        /// <summary>
        /// Disable all cheats (infinite weapons, infinite fuel, invulerability).
        /// </summary>
        NoCheats,
        /// <summary>
        /// Players cannot respawn in a new plane on crash ("single life" mode).
        /// </summary>
        NoCrashRecovery,
        /// <summary>
        /// Disable easy comms.
        /// </summary>
        NoEasyComms,
        /// <summary>
        /// Disable external views.
        /// </summary>
        NoExternalViews,
        /// <summary>
        /// Disable "game mode" simplfications (flight and avionics).
        /// </summary>
        NoGameMode,
        /// <summary>
        /// Disable overlays (minimap, 
        /// </summary>
        NoOverlays,
        /// <summary>
        /// Disable padlock views.
        /// </summary>
        NoPadlock,
        /// <summary>
        /// Enables random failures.
        /// </summary>
        RandomFailures,
        /// <summary>
        /// Enables realistic G-effects.
        /// </summary>
        RealisticGEffects,
        /// <summary>
        /// Enabled wake turbulence.
        /// </summary>
        WakeTurbulence
    }
}
