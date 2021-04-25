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
    /// Stores information about a flight group to be displayed in the mission briefing.
    /// </summary>
    public struct UnitFlightGroupBriefingDescription
    {
        /// <summary>
        /// Flight group callsign.
        /// </summary>
        public string Callsign { get; }

        /// <summary>
        /// Flight group unit count.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Flight group radio frequency.
        /// </summary>
        public string Radio { get; }

        /// <summary>
        /// Extra info about the flight group.
        /// </summary>
        public string Remarks { get; }

        /// <summary>
        /// Flight group payload.
        /// </summary>
        public string Payload { get; }
        /// <summary>
        /// Flight group unit type name.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="callsign">Flight group callsign</param>
        /// <param name="count">Flight group unit count</param>
        /// <param name="type">Flight group unit type name</param>
        /// <param name="payload">Flight group payload</param>
        /// <param name="radio">Flight group radio frequency</param>
        /// <param name="remarks">Extra info about the flight group</param>
        public UnitFlightGroupBriefingDescription(string callsign, int count, string type, string payload, string radio, string remarks = "")
        {
            Callsign = callsign;
            Count = count;
            Type = type;
            Payload = payload;
            Radio = radio;
            Remarks = remarks;
        }
    }
}
