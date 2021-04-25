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

using System;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Enumerates special flags for <see cref="DBEntryObjectiveFlags"/>
    /// </summary>
    [Flags]
    internal enum DBEntryObjectiveFlags
    {
        /// <summary>
        /// Player(s) must take off from an airbase situated near water.
        /// Mostly used by antiship missions, so players don't start 200 nm inland.
        /// </summary>
        MustStartNearWater = 1,
        /// <summary>
        /// No enemy short-range air defense will be spawned in this mission, regardless of template settings.
        /// </summary>
        NoEnemyAirDefenseShort = 2,
        /// <summary>
        /// No enemy medium-range air defense will be spawned in this mission, regardless of template settings.
        /// </summary>
        NoEnemyAirDefenseMedium = 4,
        /// <summary>
        /// No enemy long-range air defense will be spawned in this mission, regardless of template settings.
        /// </summary>
        NoEnemyAirDefenseLong = 8,
        /// <summary>
        /// No enemy CAP will be spawned in this mission, regardless of template settings.
        /// </summary>
        NoEnemyCAP = 16,
        /// <summary>
        /// A single target unit family will be used for all targets.
        /// </summary>
        SingleTargetUnitFamily = 32
    }
}
