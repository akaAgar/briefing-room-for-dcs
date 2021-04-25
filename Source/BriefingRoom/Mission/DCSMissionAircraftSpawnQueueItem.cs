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

namespace BriefingRoom4DCS.Mission
{
    /// <summary>
    /// Stores information about an aircraft group to spawn in <see cref="DCSMission.AircraftSpawnQueue"/>.
    /// Aircraft groups are activated later on because (1) they would run out of fuel if spawned on start and (2) it's nice to face new enemy CAP units as the mission goes on
    /// </summary>
    public struct DCSMissionAircraftSpawnQueueItem
    {
        /// <summary>
        /// ID of the unit group.
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// Should the group be spawned when the first player takes off (true) or later in the mission (false)?
        /// </summary>
        public bool SpawnOnStart { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupID">ID of the unit group</param>
        /// <param name="spawnOnStart">Should the group be spawned when the first player takes off (true) or later in the mission (false)?</param>
        public DCSMissionAircraftSpawnQueueItem(int groupID, bool spawnOnStart)
        {
            GroupID = Math.Max(1, groupID);
            SpawnOnStart = spawnOnStart;
        }
    }
}
