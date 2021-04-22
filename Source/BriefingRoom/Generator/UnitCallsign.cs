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

namespace BriefingRoom.Generator
{
    /// <summary>
    /// Stores information about an aircraft group callsign.
    /// Only used during mission generation, callsign data is stored as strings in <see cref="Mission.DCSMissionUnitGroup"/>.
    /// </summary>
    public struct UnitCallsign
    {
        /// <summary>
        /// Group name, e.g. "ENFIELD 1"
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Unit name, with "$INDEX$" for the unit index, e.g. "ENFIELD 1 $INDEX"
        /// </summary>
        public string UnitName { get; }

        /// <summary>
        /// Lua table with the callsign info.
        /// </summary>
        public string Lua { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="unitName">Unit name</param>
        /// <param name="lua">Lua table with the callsign info</param>
        public UnitCallsign(string groupName, string unitName, string lua)
        {
            GroupName = groupName;
            UnitName = unitName;
            Lua = lua;
        }
    }
}
