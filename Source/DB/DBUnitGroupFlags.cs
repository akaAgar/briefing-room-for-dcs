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

using System;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Enumerates special flags to apply to an <see cref="DBUnitGroup"/>
    /// </summary>
    [Flags]
    public enum DBUnitGroupFlags
    {
        /// <summary>
        /// Unit is always shown on map, no matter the value of <see cref="Template.MissionTemplate.OptionsShowEnemyUnits"/>.
        /// </summary>
        AlwaysOnMap = 1,
        /// <summary>
        /// Short-range (radar, IR and AAA) should be "embedded" in this group to provide cover.
        /// Exact amount of air defense depends on the <see cref="Template.MissionTemplate.OppositionAirDefense"/> setting.
        /// Only valid for groups of category <see cref="UnitCategory.Vehicle"/>.
        /// </summary>
        EmbeddedAirDefense = 2,
        /// <summary>
        /// Unit belongs to the friendly coalition (default is enemy).
        /// </summary>
        Friendly = 4,
        /// <summary>
        /// Unit is never shown on map, no matter the value of <see cref="Template.MissionTemplate.OptionsShowEnemyUnits"/>.
        /// </summary>
        NeverOnMap = 8,
    }
}
