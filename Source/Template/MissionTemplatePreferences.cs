/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// Enumerates all mission template preferences.
    /// </summary>
    public enum MissionTemplatePreferences
    {
        /// <summary>
        /// Should extra ingress/egress waypoints be generated in addition to the objective waypoints?
        /// </summary>
        AddExtraWaypoints,

        /// <summary>
        /// Should audio radio messages be disabled? If so, messages will only be displayed as text.
        /// </summary>
        DisableRadioSounds,

        /// <summary>
        /// Should enemy units be hidden on the F10 map, mission planning, MFD SA pages, etc?
        /// </summary>
        HideEnemyUnits
    }
}
