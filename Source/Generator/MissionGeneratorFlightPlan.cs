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
using System;

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
        /// Random variation in waypoint altitude.
        /// </summary>
        private static readonly MinMaxD WAYPOINT_ALTITUDE_VARIATION = new MinMaxD(0.85, 1.15);

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGeneratorFlightPlan() { }

        /// <summary>
        /// Adds waypoints for the mission objectives.
        /// </summary>
        /// <param name="mission">Mission</param>
        /// <param name="objectiveDB">Objective database entry</param>
        public void AddObjectiveWaypoints(DCSMission mission, DBEntryObjective objectiveDB)
        {
            DebugLog.Instance.WriteLine("Generating objective waypoints...", 1);

            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                Coordinates waypointCoordinates = mission.Objectives[i].WaypointCoordinates;
                DebugLog.Instance.WriteLine($"Created objective waypoint at {waypointCoordinates}", 2);

                mission.Waypoints.Add(
                    new DCSMissionWaypoint(
                        waypointCoordinates, mission.Objectives[i].Name,
                        objectiveDB.WaypointOnGround ? 0.0 : WAYPOINT_ALTITUDE_VARIATION.GetValue(),
                        1.0));
            }
        }

        ///// <summary>
        ///// Adds extra waypoint before and after the objective waypoints, if required.
        ///// </summary>
        ///// <param name="mission">Mission</param>
        ///// <param name="template">Mission template</param>
        //public void AddExtraWaypoints(DCSMission mission, MissionTemplate template)
        //{
        //    // No objective waypoints, or no extra waypoints required, stop here
        //    if (!template.FlightPlanExtraWaypoints || (mission.Waypoints.Count == 0))
        //        return;

        //    DebugLog.Write("Generating extra waypoints...");

        //    int i, j;
        //    double distance;

        //    for (i = 0; i < 2; i++)
        //    {
        //        int extraWaypointsCount = Toolbox.RandomFrom(EXTRA_WAYPOINT_COUNT);
        //        List<Coordinates> extraWaypoints = new List<Coordinates>();

        //        if (i == 0) // Adding waypoints before first objective waypoint
        //            distance = mission.StartingLocation.GetDistanceFrom(mission.Waypoints[0].Coordinates);
        //        else // Add waypoints after last objective waypoint
        //            distance = mission.StartingLocation.GetDistanceFrom(mission.Waypoints.Last().Coordinates);

        //        for (j = 0; j < extraWaypointsCount; j++)
        //        {
        //            if (i == 0) // Adding waypoints before first objective waypoint
        //                extraWaypoints.Add(Coordinates.Lerp(mission.StartingLocation, mission.Waypoints[0].Coordinates, (double)(i + 1) / (extraWaypointsCount + 2)) + Coordinates.CreateRandom(distance * .05, distance * .1));
        //            else // Add waypoints after last objective waypoint
        //                extraWaypoints.Add(Coordinates.Lerp(mission.Waypoints.Last().Coordinates, mission.StartingLocation, (double)(i + 1) / (extraWaypointsCount + 2)) + Coordinates.CreateRandom(distance * .05, distance * .1));
        //        }

        //        if (i == 0) // Adding waypoints before first objective waypoint
        //            extraWaypoints = extraWaypoints.OrderBy(x => mission.StartingLocation.GetDistanceFrom(x)).ToList();
        //        else // Add waypoints after last objective waypoint
        //            extraWaypoints = extraWaypoints.OrderBy(x => -mission.StartingLocation.GetDistanceFrom(x)).ToList();

        //        for (j = 0; j < extraWaypointsCount; j++)
        //        {
        //            if (i == 0) // Adding waypoints before first objective waypoint
        //                mission.Waypoints.Insert(j, new DCSMissionWaypoint(extraWaypoints[j], $"WP{j + 1:00}", WAYPOINT_ALTITUDE_VARIATION.GetValue()));
        //            else // Add waypoints after last objective waypoint
        //                mission.Waypoints.Add(new DCSMissionWaypoint(extraWaypoints[j], $"WP{mission.Waypoints.Count + 1:00}", WAYPOINT_ALTITUDE_VARIATION.GetValue()));
        //        }
        //    }
        //}

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
