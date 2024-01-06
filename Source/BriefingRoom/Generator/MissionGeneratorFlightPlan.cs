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
    internal class MissionGeneratorFlightPlan
    {
        private const double BULLSEYE_DISTANCE_MIN = 20.0;

        private const double BULLSEYE_DISTANCE_MAX = 40.0;


        internal static void GenerateBullseyes(DCSMission mission, Coordinates objectivesCenter)
        {
            mission.SetValue("BullseyeBlueX", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeBlueY", objectivesCenter.Y + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeRedX", objectivesCenter.X + GetBullseyeRandomDistance());
            mission.SetValue("BullseyeRedY", objectivesCenter.Y + GetBullseyeRandomDistance());
        }

        internal static void GenerateAircraftPackageWaypoints(MissionTemplateRecord template, DCSMission mission, List<List<List<Waypoint>>> objectiveGroupedWaypoints, Coordinates averageInitialLocation, Coordinates objectivesCenter, WaypointNameGenerator waypointNameGenerator)
        {
            foreach (var package in template.AircraftPackages)
            {
                var missionPackage = mission.MissionPackages.First(x => x.RecordIndex == template.AircraftPackages.IndexOf(package));
                missionPackage.Waypoints = objectiveGroupedWaypoints
                    .SelectMany(x => x)
                    .Where((v, i) => package.ObjectiveIndexes.Contains(i))
                    .SelectMany(x => x)
                    .ToList();
                GenerateIngressAndEgressWaypoints(template, missionPackage.Waypoints, averageInitialLocation, objectivesCenter, waypointNameGenerator);

                foreach (var waypoint in missionPackage.Waypoints)
                {
                    mission.MapData.AddIfKeyUnused($"WAYPOINT_{waypoint.Name}", new List<double[]> { waypoint.Coordinates.ToArray() });
                }
            }
        }


        internal static void GenerateIngressAndEgressWaypoints(MissionTemplateRecord template, List<Waypoint> waypoints, Coordinates averageInitialLocation, Coordinates objectivesCenter, WaypointNameGenerator waypointNameGenerator)
        {
            if (!template.MissionFeatures.Contains("IngressEgressWaypoints"))
                return;

            BriefingRoom.PrintToLog($"Generating ingress and egress waypoints...");

            double flightPathLength = (objectivesCenter - averageInitialLocation).GetLength();
            double ingressDeviation = Math.Max(4.0, flightPathLength * .15);
            Coordinates baseIngressPosition = averageInitialLocation + (objectivesCenter - averageInitialLocation) * .7f;

            waypoints.Insert(0,
                new Waypoint(
                    $"{Database.Instance.Common.Names.WPIngressName.Get().ToUpper()}_{waypointNameGenerator.GetWaypointName()}",
                    baseIngressPosition + Coordinates.CreateRandom(ingressDeviation * 0.9, ingressDeviation * 1.1)));

            waypoints.Add(
                new Waypoint(
                    $"{Database.Instance.Common.Names.WPEgressName.Get().ToUpper()}_{waypointNameGenerator.GetWaypointName()}",
                    baseIngressPosition + Coordinates.CreateRandom(ingressDeviation * 0.9, ingressDeviation * 1.1)));
        }

        internal static void GenerateObjectiveWPCoordinatesLua(MissionTemplateRecord template, DCSMission mission, List<Waypoint> waypoints, DrawingMaker DrawingMaker)
        {
            var scriptWaypoints = waypoints.Where(x => !x.ScriptIgnore).ToList();
            for (int i = 0; i < scriptWaypoints.Count; i++)
            {
                mission.AppendValue("ScriptObjectives",
                    $"briefingRoom.mission.objectives[{i + 1}].waypoint = {scriptWaypoints[i].Coordinates.ToLuaTable()}\n");
            }
            if (template.OptionsMission.Contains("MarkWaypoints"))
                foreach (var waypoint in waypoints)
                {
                    if(!waypoint.HiddenMapMarker)
                        DrawingMaker.AddDrawing(waypoint.Name, DrawingType.TextBox, waypoint.Coordinates + new Coordinates(-100, 0), "Text".ToKeyValuePair(waypoint.Name));
                }

            foreach (var waypoint in waypoints)
            {
                mission.MapData.AddIfKeyUnused($"WAYPOINT_{waypoint.Name}", new List<double[]> { waypoint.Coordinates.ToArray() });
            }
        }

        private static double GetBullseyeRandomDistance()
        {
            return Toolbox.RandomDouble(BULLSEYE_DISTANCE_MIN, BULLSEYE_DISTANCE_MAX) * Toolbox.NM_TO_METERS;
        }
    }
}