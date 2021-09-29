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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
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

        private double GetBullseyeRandomDistance()
        {
            return Toolbox.RandomDouble(BULLSEYE_DISTANCE_MIN, BULLSEYE_DISTANCE_MAX) * Toolbox.NM_TO_METERS;
        }

        internal void GenerateBullseyes(DCSMission mission, Coordinates objectivesCenter)
        {
            mission.SetValue("BullseyeBlueX", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeBlueY", objectivesCenter.Y + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeRedX", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeRedY", objectivesCenter.Y + GetBullseyeRandomDistance());
        }

        internal void GenerateAircraftPackageWaypoints(MissionTemplate template, List<Waypoint> waypoints, Coordinates averageInitialLocation, Coordinates objectivesCenter)
        {
            foreach (var package in template.AircraftPackages)
            {
                package.Waypoints = waypoints.Where((v, i) => package.ObjectiveIndexes.Contains(i)).ToList();
                GenerateIngressAndEgressWaypoints(template, package.Waypoints, averageInitialLocation, objectivesCenter);
            }
        }


        internal void GenerateIngressAndEgressWaypoints(MissionTemplate template, List<Waypoint> waypoints, Coordinates averageInitialLocation, Coordinates objectivesCenter)
        {
            if(!template.MissionFeatures.Contains("IngressEgressWaypoints"))
                return;

            BriefingRoom.PrintToLog($"Generating ingress and egress waypoints...");

            double flightPathLength = (objectivesCenter - averageInitialLocation).GetLength();
            double ingressDeviation = Math.Max(4.0, flightPathLength * .15);
            Coordinates baseIngressPosition = averageInitialLocation + (objectivesCenter - averageInitialLocation) * .7f;

            waypoints.Insert(0,
                new Waypoint(
                    Database.Instance.Common.Names.WPIngressName,
                    baseIngressPosition + Coordinates.CreateRandom(ingressDeviation * 0.9, ingressDeviation * 1.1)));

            waypoints.Add(
                new Waypoint(
                    Database.Instance.Common.Names.WPEgressName,
                    baseIngressPosition + Coordinates.CreateRandom(ingressDeviation * 0.9, ingressDeviation * 1.1)));
        }   

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }

        internal void GenerateObjectiveWPCoordinatesLua(MissionTemplate template, DCSMission mission, List<Waypoint> waypoints)
        {
            for (int i = 0; i < waypoints.Count; i++)
            {
                mission.AppendValue("ScriptObjectives",
                    $"briefingRoom.mission.objectives[{i + 1}].waypoint = {waypoints[i].Coordinates.ToLuaTable()}\n");
                if(template.OptionsMission.Contains(MissionOption.MarkWaypoints))
                    mission.AppendValue("ScriptObjectives",
                        $"trigger.action.markToCoalition({i + 1}, \"{waypoints[i].Name}\", {{ x={waypoints[i].Coordinates.X.ToInvariantString()}, y=0, z={waypoints[i].Coordinates.Y.ToInvariantString()} }}, coalition.side.{template.ContextPlayerCoalition.ToString().ToUpperInvariant()}, true, nil)\n");
            }
        }
    }
}