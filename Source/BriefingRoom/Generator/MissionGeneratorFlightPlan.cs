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

using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates the player's flight plan and waypoints.
    /// </summary>
    internal class MissionGeneratorFlightPlan : IDisposable
    {
        /// <summary>
        /// An array with some random count of extra waypoints to add before and after the objective waypoints.
        /// </summary>
        private static readonly int[] EXTRA_WAYPOINT_COUNT = new int[] { 1, 1, 2, 2, 2, 2, 3 };

        /// <summary>
        /// Minimum distance between the bullseyes and the objective's center, in nautical miles.
        /// </summary>
        private const double BULLSEYE_DISTANCE_MIN = 20.0;

        /// <summary>
        /// Maximum distance between the bullseyes and the objective's center, in nautical miles.
        /// </summary>
        private const double BULLSEYE_DISTANCE_MAX = 40.0;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorFlightPlan() { }

        ///// <summary>
        ///// Adds waypoints for the mission objectives.
        ///// </summary>
        ///// <param name="mission">Mission</param>
        //internal void AddObjectiveWaypoints(DCSMission mission)
        //{
        //    BriefingRoom.PrintToLog("Generating objective waypoints...");

        //    for (int i = 0; i < mission.Objectives.Length; i++)
        //    {
        //        Coordinates waypointCoordinates = mission.Objectives[i].WaypointCoordinates;
        //        BriefingRoom.PrintToLog($"Created objective waypoint at {waypointCoordinates}");

        //        mission.Waypoints.Add(
        //            new DCSMissionWaypoint(
        //                waypointCoordinates, mission.Objectives[i].Name,
        //                mission.Objectives[i].WaypointOnGround ? 0.0 : 1.0,
        //                1.0));
        //    }
        //}

        /////// <summary>
        /////// Adds extra waypoint before and after the objective waypoints, if required.
        /////// </summary>
        /////// <param name="mission">Mission</param>
        /////// <param name="template">Mission template</param>
        //internal void AddExtraWaypoints(DCSMission mission, MissionTemplate template, bool preObj = true)
        //{
        //    // No objective waypoints, or no extra waypoints required, stop here
        //    if ((template.FlightPlanAddExtraWaypoints == YesNo.No) || (mission.Waypoints.Count == 0))
        //        return;

        //}

        ///// <summary>
        ///// Sets the mission bullseye for both coalitions.
        ///// </summary>
        ///// <param name="mission">The mission</param>
        //internal void SetBullseye(DCSMission mission)
        //{
        //    for (int i = 0; i < 2; i++)
        //        mission.Bullseye[i] =
        //            mission.ObjectivesCenter +
        //            Coordinates.CreateRandom(20 * Toolbox.NM_TO_METERS, 40 * Toolbox.NM_TO_METERS);
        //}

        private double GetBullseyeRandomDistance()
        {
            return Toolbox.RandomDouble(BULLSEYE_DISTANCE_MIN, BULLSEYE_DISTANCE_MAX) * Toolbox.NM_TO_METERS;
        }

        internal void GenerateBullseyes(DCSMission mission, Coordinates objectivesCenter)
        {
            mission.SetValue("BULLSEYE_BLUE_X", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BULLSEYE_BLUE_Y", objectivesCenter.Y + GetBullseyeRandomDistance());
            mission.SetValue("BULLSEYE_RED_X", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BULLSEYE_RED_Y", objectivesCenter.Y + GetBullseyeRandomDistance());
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }

        internal void GenerateExtraWaypoints(MissionTemplate template, Coordinates initialCoordinates, List<Waypoint> waypoints, bool afterObjective)
        {
            if (!template.OptionsMission.Contains(MissionOption.AddExtraWaypoints))
                return; // No extra waypoints required

            BriefingRoom.PrintToLog($"Generating extra {(afterObjective ? "egress" : "ingress")} waypoints...");

            int extraWaypointsCount = Toolbox.RandomFrom(EXTRA_WAYPOINT_COUNT);
            List<Coordinates> extraWaypoints = new List<Coordinates>();

            Coordinates startingPos = afterObjective ? waypoints.Last().Coordinates : waypoints.First().Coordinates;
            for (int i = 0; i < extraWaypointsCount; i++)
            {
                if (afterObjective)
                    startingPos = Coordinates.Lerp(startingPos, initialCoordinates, new MinMaxD(0.2, 0.8).GetValue()).CreateNearRandom(0, 20000);
                else
                    startingPos = Coordinates.Lerp(startingPos, waypoints.First().Coordinates, new MinMaxD(0.2, 0.8).GetValue()).CreateNearRandom(0, 20000);
                extraWaypoints.Add(startingPos);
            }

            int count = 1;
            extraWaypoints = extraWaypoints.OrderBy(x => afterObjective ? -initialCoordinates.GetDistanceFrom(x) : initialCoordinates.GetDistanceFrom(x)).ToList();
            if (afterObjective) // Adding waypoints before first objective waypoint
                waypoints.AddRange(extraWaypoints.Select(wpCoordinates => new Waypoint($"WP{waypoints.Count + count++:00}", wpCoordinates)));
            else // Add waypoints after last objective waypoint
                waypoints.InsertRange(0, extraWaypoints.Select(wpCoordinates => new Waypoint( $"WP{count++:00}", wpCoordinates)));
        }
    }
}
