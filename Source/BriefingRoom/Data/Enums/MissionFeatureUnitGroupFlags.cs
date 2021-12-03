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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Enumerates special flags to apply to a <see cref="DBEntryMissionFeature"/>'s unit group.
    /// </summary>
    [Flags]
    internal enum FeatureUnitGroupFlags
    {
        /// <summary>
        /// Unit is always shown on map, no matter the value of <see cref="Template.MissionTemplate.OptionsShowEnemyUnits"/>.
        /// </summary>
        AlwaysOnMap = 1,

        /// <summary>
        /// Short-range (radar, IR and AAA) should be "embedded" in this group to provide cover.
        /// Exact amount of air defense depends on the <see cref="Template.MissionTemplate.SituationEnemyAirDefense"/> setting.
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

        ///// <summary>
        ///// This unit group requires a TACAN frequency to be generated
        ///// </summary>
        TACAN = 16,

        ///// <summary>
        ///// This unit group must be spawned away from the mission area
        ///// </summary>
        AwayFromMissionArea = 32,

        ///// <summary>
        ///// Aircraft should be immediately activated when the first player takes off.
        ///// </summary>
        ImmediateAircraftActivation = 64,

        /// <summary>
        /// Spawn unit group directly on top the objective (Objective Features only).
        /// </summary>
        SpawnOnObjective = 128,

        /// <summary>
        /// Spawned unit group is on the same side as the target (Objective Features only) (default is enemy).
        /// </summary>
        SameSideAsTarget = 256,


        /// <summary>
        /// Aircraft should be activated when the player radios for it.
        /// </summary>
        RadioAircraftActivation = 512,

        /// <summary>
        /// Units ignore borders
        /// </summary>
        IgnoreBorders = 1024,

        GroundStart = 2048,

        RequiresOpenAirParking = 4096,
    }
}
