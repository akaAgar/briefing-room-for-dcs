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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates the player's flight plan and waypoints.
    /// </summary>
    public class MissionGeneratorFlightPlan : IDisposable
    {
        /// <summary>
        /// An array with some random count of extra waypoints to add before and after the objective waypoints.
        /// </summary>
        private static readonly int[] EXTRA_WAYPOINT_COUNT = new int[] { 1, 1, 2, 2, 2, 2, 3 };

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGeneratorFlightPlan() { }

        /// <summary>
        /// Adds waypoints for the mission objectives.
        /// </summary>
        /// <param name="mission">Mission</param>
        /// <param name="objectiveDB">Objective database entry</param>
        public void AddObjectiveWaypoints(DCSMission mission,DCSMissionUnitGroup group, DBEntryObjective objectiveDB)
        {
            DebugLog.Instance.WriteLine("Generating objective waypoints...", 1);

            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                Coordinates waypointCoordinates = mission.Objectives[i].WaypointCoordinates;
                DebugLog.Instance.WriteLine($"Created objective waypoint at {waypointCoordinates}", 2);

                group.Waypoints.Add(
                    new DCSMissionWaypoint(
                        waypointCoordinates, mission.Objectives[i].Name,
                        objectiveDB.WaypointOnGround ? 0.0 : 1.0,
                        1.0));
            }
        }

        ///// <summary>
        ///// Adds extra waypoint before and after the objective waypoints, if required.
        ///// </summary>
        ///// <param name="mission">Mission</param>
        ///// <param name="template">Mission template</param>
        public void AddExtraWaypoints(DCSMissionUnitGroup group, MissionTemplate template, bool preObj = true)
        {
            // No objective waypoints, or no extra waypoints required, stop here
            if ((group.IsAPlayerGroup() && !template.PlayerExtraWaypoints) || (group.Waypoints.Count == 0))
                return;

            DebugLog.Instance.WriteLine("Generating extra waypoints...");

            int extraWaypointsCount = Toolbox.RandomFrom(EXTRA_WAYPOINT_COUNT);
            List<Coordinates> extraWaypoints = new List<Coordinates>();
            Coordinates startingPos = preObj? group.Waypoints.First().Coordinates : group.Waypoints.Last().Coordinates;
            for (int j = 0; j < extraWaypointsCount; j++)
            {
                if(preObj)
                    startingPos = Coordinates.Lerp(startingPos, group.Waypoints[0].Coordinates, new MinMaxD(0.2, 0.8).GetValue()).CreateNearRandom(0, 20000);
                else
                    startingPos = Coordinates.Lerp(startingPos, group.Waypoints.First().Coordinates, new MinMaxD(0.2, 0.8).GetValue()).CreateNearRandom(0, 20000);
                    extraWaypoints.Add(startingPos);
            }

            int count = 1;
            extraWaypoints =  extraWaypoints.OrderBy(x => preObj? group.Waypoints.First().Coordinates.GetDistanceFrom(x): -group.Waypoints.First().Coordinates.GetDistanceFrom(x)).ToList();
            if (preObj) {// Adding waypoints before first objective waypoint
                group.Waypoints.InsertRange(0, extraWaypoints.Select(x => new DCSMissionWaypoint(x, $"WP{count++:00}", 1.0)));
                AddExtraWaypoints(group, template, false);
            } else // Add waypoints after last objective waypoint
                group.Waypoints.AddRange(extraWaypoints.Select(x =>new DCSMissionWaypoint(x, $"WP{group.Waypoints.Count + count++:00}", 1.0)));
        }

        /// <summary>
        /// Sets the mission bullseye for both coalitions.
        /// </summary>
        /// <param name="mission">The mission</param>
        public void SetBullseye(DCSMission mission)
        {
            for (int i = 0; i < 2; i++)
                mission.Bullseye[i] =
                    mission.ObjectivesCenter +
                    Coordinates.CreateRandom(20 * Toolbox.NM_TO_METERS, 40 * Toolbox.NM_TO_METERS);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
