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

namespace BriefingRoom4DCSWorld.Mission
{
    /// <summary>
    /// Stores information about a mission objective.
    /// </summary>
    public struct DCSMissionObjective
    {
        /// <summary>
        /// X,Y coordinates of the objective target.
        /// </summary>
        public Coordinates Coordinates { get; }

        /// <summary>
        /// Name of the objective.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Unit family of the target at this objective, if any.
        /// </summary>
        public UnitFamily? TargetFamily { get; }

        /// <summary>
        /// X,Y coordinates of the objective waypoint.
        /// </summary>
        public Coordinates WaypointCoordinates { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the objective</param>
        /// <param name="coordinates">X,Y coordinates of the objective target</param>
        /// <param name="targetFamily">Unit family of the target at this objective</param>
        /// <param name="waypointCoordinates">X,Y coordinates of the objective waypoint</param>
        public DCSMissionObjective(string name, Coordinates coordinates, UnitFamily? targetFamily, Coordinates waypointCoordinates)
        {
            Coordinates = coordinates;
            Name = name;
            TargetFamily = targetFamily;
            WaypointCoordinates = waypointCoordinates;
        }
    }
}
