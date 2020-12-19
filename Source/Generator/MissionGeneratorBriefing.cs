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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates the mission name and briefing (both raw text and HTML version).
    /// </summary>
    public class MissionGeneratorBriefing : IDisposable
    {
        private static readonly string HTML_TEMPLATE_FILE = $"{BRPaths.INCLUDE}Briefing.html";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGeneratorBriefing() { }

        /// <summary>
        /// Generate a random mission name if none is provided in the template, or returns the provided name if there is one.
        /// </summary>
        /// <param name="mission">A mission</param>
        /// <param name="template">Mission template to use</param>
        public void GenerateMissionName(DCSMission mission, MissionTemplate template)
        {
            DebugLog.Instance.WriteLine("Generating mission name...", 1);

            string fixedName = GeneratorTools.SanitizeString(template.BriefingName);
            if (!string.IsNullOrEmpty(fixedName)) // A custom mission name has been provided, use it
                mission.MissionName = fixedName;
            else // No custom name? Generate one
            {
                mission.MissionName = Database.Instance.Common.MissionNameTemplate;
                for (int i = 0; i < DatabaseCommon.MISSION_NAMES_PART_COUNT; i++)
                    mission.MissionName = mission.MissionName.Replace($"$P{i + 1}$", Toolbox.RandomFrom(Database.Instance.Common.MissionNameParts[i]));
            }

            DebugLog.Instance.WriteLine($"Mission name set to \"{mission.MissionName}\"", 2);
        }

        /// <summary>
        /// Generates the mission briefing.
        /// </summary>
        /// <param name="mission">Mission</param>
        /// <param name="template">Template from which the mission should be built</param>
        /// <param name="airbaseDB">Airbase player will take off from and land back on</param>
        /// <param name="coalitionsDB">Database entries for the mission coalitions</param>
        public void GenerateMissionBriefing(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryTheaterAirbase airbaseDB, List<UnitFlightGroupBriefingDescription> flightGroups, DBEntryCoalition[] coalitionsDB)
        {
            DebugLog.Instance.WriteLine("Generating mission briefing...", 1);

            // Get mission features
            DBEntryMissionFeature[] features = Database.Instance.GetEntries<DBEntryMissionFeature>(objectiveDB.MissionFeatures);

            string description = GeneratorTools.SanitizeString(template.BriefingDescription);
            if (string.IsNullOrEmpty(description)) // No custom mission description has been provided, generate one
            {
                description = objectiveDB.BriefingDescriptionByUnitFamily[(int)mission.Objectives[0].TargetFamily];
                if (string.IsNullOrEmpty(description)) // No custom briefing for this target family, use the default
                    description = objectiveDB.BriefingDescription;

                description =
                    GeneratorTools.MakeBriefingStringReplacements(GeneratorTools.ParseRandomString(description), mission, coalitionsDB);
            }
            description = GeneratorTools.SanitizeString(description);

            // Generate tasks
            List<string> tasks = new List<string> { $"Take off from {airbaseDB.Name}" };
            string objectiveTask = GeneratorTools.ParseRandomString(objectiveDB.BriefingTask);
            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                string taskString = GeneratorTools.MakeBriefingStringReplacements(objectiveTask, mission, coalitionsDB, i);
                tasks.Add(taskString);
                mission.CoreLuaScript += $"briefingRoom.mission.objectives[{i + 1}].task = \"{taskString}\"\r\n";
            }
            tasks.Add($"Return to {airbaseDB.Name}");
            DebugLog.Instance.WriteLine($"{tasks.Count} task(s)", 2);

            // Generate mission remarks...
            List<string> remarks = new List<string>();
            remarks.AddRange( // ...from objective
                from string remark in objectiveDB.BriefingRemarks
                select GeneratorTools.MakeBriefingStringReplacements(GeneratorTools.ParseRandomString(remark), mission, coalitionsDB)); 
            foreach (DBEntryMissionFeature feature in features)
                remarks.AddRange( // ...from features
                    from string remark in feature.BriefingRemarks
                    select GeneratorTools.MakeBriefingStringReplacements(GeneratorTools.ParseRandomString(remark), mission, coalitionsDB));
            remarks.Add("Use the \"F10/Other\" item in the comms for additional options");
            DebugLog.Instance.WriteLine($"{remarks.Count} remark(s)", 2);

            mission.BriefingHTML = CreateHTMLBriefing(mission, template, description, tasks, remarks, flightGroups, airbaseDB, coalitionsDB);
            mission.BriefingTXT = CreateTXTBriefing(description, tasks, remarks, flightGroups, airbaseDB);
        }

        private string CreateHTMLBriefing(
            DCSMission mission, MissionTemplate template, string description,
            List<string> tasks, List<string> remarks,
            List<UnitFlightGroupBriefingDescription> flightGroups, DBEntryTheaterAirbase airbaseDB,
            DBEntryCoalition[] coalitionsDB)
        {
            DebugLog.Instance.WriteLine("Generating HTML mission briefing...", 2);

            if (!File.Exists(HTML_TEMPLATE_FILE)) // Briefing template not found
            {
                DebugLog.Instance.WriteLine("HTML template file not found.", 1, DebugLogMessageErrorLevel.Warning);
                return "HTML template file not found.";
            }

            string briefing = File.ReadAllText(HTML_TEMPLATE_FILE);

            // Title
            briefing = briefing.Replace("$MISSIONNAME$", mission.MissionName);
            briefing = briefing.Replace("$MISSIONTYPE$",
                GeneratorTools.RemoveAfterComma(template.ObjectiveType) + " mission " +
                ((template.GetMissionType() == MissionType.SinglePlayer) ?
                "(single-player)" : $"({template.GetPlayerCount()}-players multiplayer)"));

            // Situation summary
            briefing = briefing.Replace("$LONGDATE$", mission.DateTime.ToDateString(true));
            briefing = briefing.Replace("$LONGTIME$", mission.DateTime.ToTimeString());
            briefing = briefing.Replace("$SHORTDATE$", mission.DateTime.ToDateString(false));
            briefing = briefing.Replace("$SHORTTIME$", mission.DateTime.ToTimeString());
            briefing = briefing.Replace("$WEATHER$", GeneratorTools.GetEnumString(mission.Weather.WeatherLevel));
            briefing = briefing.Replace("$WIND$", GeneratorTools.GetEnumString(mission.Weather.WindLevel));
            briefing = briefing.Replace("$WINDSPEED$", mission.Weather.WindSpeedAverage.ToString("F0"));

            // Friends and enemies
            briefing = briefing.Replace("$PLAYERCOALITION$", GeneratorTools.RemoveAfterComma(template.GetCoalition(mission.CoalitionPlayer)));
            briefing = briefing.Replace("$ENEMYCOALITION$", GeneratorTools.RemoveAfterComma(template.GetCoalition(mission.CoalitionEnemy)));

            // Description
            briefing = briefing.Replace("$DESCRIPTION$", description.Replace("\n", "<br />"));

            // Tasks
            string tasksHTML = "";
            foreach (string task in tasks) tasksHTML += $"<li>{task}</li>";
            briefing = briefing.Replace("$TASKS$", tasksHTML);

            // Remarks
            string remarksHTML = "";
            foreach (string remark in remarks) remarksHTML += $"<li>{remark}</li>";
            briefing = briefing.Replace("$REMARKS$", remarksHTML);

            // Flight groups
            string flightGroupsHTML = "";
            foreach (UnitFlightGroupBriefingDescription fg in flightGroups)
                flightGroupsHTML +=
                    "<tr>" +
                    $"<td>{fg.Callsign}</td>" +
                    $"<td>{fg.Count}×{fg.Type}</td>" +
                    $"<td>{fg.Task}</td><td>{fg.Radio}</td>" +
                    $"<td>{fg.Remarks}</td>" +
                    "</tr>";
            briefing = briefing.Replace("$FLIGHTGROUPS$", flightGroupsHTML);

            // Airbases
            string airbasesHTML =
                "<tr>" +
                $"<td>{airbaseDB.Name}</td>" +
                $"<td>{airbaseDB.Runways}</td>" +
                $"<td>{airbaseDB.ATC}</td>" +
                $"<td>{airbaseDB.ILS}</td>" +
                $"<td>{airbaseDB.TACAN}</td>" +
                "</tr>";
            briefing = briefing.Replace("$AIRBASES$", airbasesHTML);

            // Waypoints
            string waypointsHTML = "";
            double distance;
            double totalDistance = 0.0;
            Coordinates currentPosition = mission.InitialPosition;
            waypointsHTML += $"<tr><td><strong>TAKEOFF</strong></td><td>-</td><td>-</td></tr>";
            foreach (DCSMissionWaypoint wp in mission.Waypoints)
            {
                distance = currentPosition.GetDistanceFrom(wp.Coordinates);
                totalDistance += distance;
                currentPosition = wp.Coordinates;
                
                waypointsHTML +=
                    $"<tr><td>{wp.Name}</td>" +
                    $"<td>{GeneratorTools.ConvertDistance(distance, coalitionsDB[(int)mission.CoalitionPlayer].BriefingUnitSystem)}</td>" +
                    $"<td>{GeneratorTools.ConvertDistance(totalDistance, coalitionsDB[(int)mission.CoalitionPlayer].BriefingUnitSystem)}</td></tr>";
            }
            distance = currentPosition.GetDistanceFrom(mission.InitialPosition);
            totalDistance += distance;
            waypointsHTML += $"<tr><td><strong>LANDING</strong></td>" +
                $"<td>{GeneratorTools.ConvertDistance(distance, coalitionsDB[(int)mission.CoalitionPlayer].BriefingUnitSystem)}</td>" +
                $"<td>{GeneratorTools.ConvertDistance(totalDistance, coalitionsDB[(int)mission.CoalitionPlayer].BriefingUnitSystem)}</td></tr>";
            briefing = briefing.Replace("$WAYPOINTS$", waypointsHTML);

            return briefing;
        }

        /// <summary>
        /// Creates the raw text briefing, to be used for the mission description inside DCS World.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="tasks"></param>
        /// <param name="remarks"></param>
        /// <param name="flightGroups"></param>
        /// <param name="airbaseDB">Airbase player will take off from and land back on</param>
        /// <returns></returns>
        private string CreateTXTBriefing(
            string description, List<string> tasks, List<string> remarks, 
            List<UnitFlightGroupBriefingDescription> flightGroups, DBEntryTheaterAirbase airbaseDB)
        {
            DebugLog.Instance.WriteLine("Generating raw text mission briefing...", 2);

            string briefing = description + "\n\n";

            briefing += "TASKS:\n";
            foreach (string t in tasks) briefing += $"- {t}\n";
            briefing += "\n";

            briefing += "REMARKS:\n";
            foreach (string r in remarks) briefing += $"- {r}\n";
            briefing += "\n";

            briefing += "MISSION PACKAGE:\n";
            foreach (UnitFlightGroupBriefingDescription fg in flightGroups)
                briefing += $"- {fg.Callsign} ({fg.Count}×{fg.Type}, {fg.Task}) - {fg.Radio}{(string.IsNullOrEmpty(fg.Remarks) ? "" : $", {fg.Remarks}")}\n";
            briefing += "\n";

            briefing += "AIRBASES:\n";
            briefing += $"{airbaseDB.Name} ({airbaseDB.ATC}), RWY: {airbaseDB.Runways}, ILS: {airbaseDB.ILS}, TACAN: {airbaseDB.TACAN}\n";
            briefing += "\n";

            briefing += $"This mission was generated by BriefingRoom for DCS {BriefingRoom.BRIEFINGROOM_VERSION}, a random mission generator for DCS World ({BriefingRoom.WEBSITE_URL})";

            return GeneratorTools.SanitizeString(briefing);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }

    }
}
