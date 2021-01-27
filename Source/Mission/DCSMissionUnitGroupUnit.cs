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

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Stores information about an unit in a (<see cref="DCSMissionUnitGroup"/>).
    /// </summary>
    public class DCSMissionUnitGroupUnit
    {
        /// <summary>
        /// Map X,Y coordinates of this unit.
        /// </summary>
        public Coordinates Coordinates { get; set; } = new Coordinates();

        /// <summary>
        /// Heading of this unit, in radians.
        /// </summary>
        public double Heading { get; set; } = 0;

        /// <summary>
        /// Unique ID of this unit.
        /// </summary>
        public int ID { get; set; } = 0;

        /// <summary>
        /// Parking spot, for parked aircraft.
        /// </summary>
        public int ParkingSpot { get; set; } = 0;

        /// <summary>
        /// Internal DCS type of this unit.
        /// </summary>
        public string Type { get; set; } = "";


        /// <summary>
        /// Display name of this unit
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        public DCSMissionUnitGroupUnit() { }
    }
}
