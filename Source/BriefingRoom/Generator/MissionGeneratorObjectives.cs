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
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates the <see cref="DCSMissionObjective"/> array.
    /// </summary>
    internal class MissionGeneratorObjectives : IDisposable
    {
        /// <summary>
        /// List of already used objective names, to make sure each one is different.
        /// </summary>
        private readonly List<string> UsedObjectiveNames = new List<string>();

        /// <summary>
        /// Minimum objective distance variation.
        /// </summary>
        private const double OBJECTIVE_DISTANCE_VARIATION_MIN = 0.75;

        /// <summary>
        /// Maximum objective distance variation.
        /// </summary>
        private const double OBJECTIVE_DISTANCE_VARIATION_MAX = 1.25;

        ///// <summary>
        ///// List of already used objective names, to make sure each one is different.
        ///// </summary>
        //private readonly List<string> UsedObjectiveNames = new List<string>();

        /// <summary>
        /// Unit maker selector to use for objective generation.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spawnPointSelector">Spawn point selector to use for objective generation</param>
        internal MissionGeneratorObjectives(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal Coordinates GenerateObjective(DCSMission mission, MissionTemplate template, int index, Coordinates lastCoordinates)
        {
            MissionTemplateObjective objectiveTemplate = template.Objectives[index];
            DBEntryObjectiveFeature[] featuresDB = Database.Instance.GetEntries<DBEntryObjectiveFeature>(objectiveTemplate.Features.ToArray());
            DBEntryObjectiveTarget targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            DBEntryObjectiveTargetBehavior targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior);
            DBEntryObjectiveTask taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);

            // TODO: check DB entries exist

            // Add Lua data for this objective
            string objectiveLua = $"briefingRoom.objectives[{index + 1}] = {{ ";
            objectiveLua += $"targetCategory = Unit.Category.{targetDB.UnitCategory.ToLuaName()}, ";
            objectiveLua += "},\n";
            mission.AppendValue("OBJECTIVES_LUA", objectiveLua);

            DBEntryTheaterSpawnPoint? spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                targetDB.ValidSpawnPoints, lastCoordinates,
                new MinMaxD(
                    template.FlightPlanObjectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MIN,
                    template.FlightPlanObjectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MAX));

            if (!spawnPoint.HasValue)
            {
                BriefingRoom.PrintToLog("Failed to spawn objective unit group.", LogMessageErrorLevel.Warning);
                return lastCoordinates;
            }

            bool hidden = GeneratorTools.GetHiddenStatus(template.OptionsFogOfWar, taskDB.TargetSide);
            if (objectiveTemplate.Options.Contains(ObjectiveOption.ShowTarget)) hidden = false;
            else if (objectiveTemplate.Options.Contains(ObjectiveOption.HideTarget)) hidden = true;

            UnitMaker.AddUnitGroup(
                Toolbox.RandomFrom(targetDB.UnitFamilies), targetDB.UnitCount[(int)objectiveTemplate.TargetCount].GetValue(),
                taskDB.TargetSide,
                targetBehaviorDB.GroupLua[(int)targetDB.UnitCategory], targetBehaviorDB.UnitLua[(int)targetDB.UnitCategory],
                spawnPoint.Value.Coordinates, DCSSkillLevel.Average,
                "Hidden".ToKeyValuePair(hidden));

            return spawnPoint.Value.Coordinates;
        }

        ///// <summary>
        ///// Generates the <see cref="DCSMissionObjective"/>.
        ///// </summary>
        ///// <param name="mission">The mission for which to generate objectives</param>
        ///// <param name="template">Mission template to use</param>
        //internal void CreateObjectives(DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB)
        //{
        //    // Set the array for the proper number of objective
        //    mission.Objectives = new DCSMissionObjective[template.Objectives.Count];

        //    GenerateObjectivesData(mission, template, theaterDB);
        //    GenerateObjectivesScript(mission);
        //}

        ///// <summary>
        ///// Generates the data for the objectives.
        ///// </summary>
        ///// <param name="mission">The mission for which to generate objectives</param>
        ///// <param name="template">Mission template to use</param>
        //private void GenerateObjectivesData(DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB)
        //{
        //    // Keep in mind the position of the last objective/player location.
        //    // Start with initial player location.
        //    Coordinates lastCoordinates = mission.InitialPosition;

        //    // Common family to use for all objectives if DBEntryObjectiveFlags.SingleTargetUnitFamily is set
        //    //UnitFamily singleObjectiveUnitFamily = objectiveDB.GetRandomUnitFamily();

        //    for (int i = 0; i < template.Objectives.Count; i++)
        //    {
        //        DBEntryObjectiveTarget objectiveTarget = Database.Instance.GetEntry<DBEntryObjectiveTarget>(template.Objectives[i].Target);
        //        DBEntryObjectiveTargetBehavior objectiveTargetBehavior = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(template.Objectives[i].TargetBehavior);
        //        DBEntryObjectiveTask objectiveTask = Database.Instance.GetEntry<DBEntryObjectiveTask>(template.Objectives[i].Task);

        //        // Pick a random unique name, or a waypoint number if objectives shouldn't be named
        //        string objectiveName = PickUniqueObjectiveName();

        //        BriefingRoom.PrintToLog($"Adding objective #{i + 1}, designated {objectiveName}", 1);

        //        // Compute a random distance from last position, in nautical miles
        //        double objectiveDistanceNM =
        //            (template.FlightPlanObjectiveDistance == 0) ?
        //            Toolbox.RandomInt(MissionTemplate.OBJECTIVE_DISTANCE_INCREMENT, MissionTemplate.MAX_OBJECTIVE_DISTANCE) :
        //            template.FlightPlanObjectiveDistance;

        //        if (i > 0) // Objective is not the first one, spawn it close to the previous objective
        //            objectiveDistanceNM /= 5.0;

        //        MinMaxD distanceFromLast =
        //            new MinMaxD(OBJECTIVE_DISTANCE_VARIATION_MIN, OBJECTIVE_DISTANCE_VARIATION_MAX) * objectiveDistanceNM;
        //        Coordinates objectiveCoordinates;
        //        DBEntryTheaterAirbase? airbase = null;

        //        //if(objectiveDB.UnitGroup.SpawnPoints[0] != TheaterLocationSpawnPointType.Airbase){
        //        // Look for a valid spawn point
        //        DBEntryTheaterSpawnPoint? spawnPoint =
        //            SpawnPointSelector.GetRandomSpawnPoint(
        //                // If spawn point types are specified, use them. Else look for spawn points of any type
        //                (objectiveTarget.ValidSpawnPoints.Length > 0) ? objectiveTarget.ValidSpawnPoints : null,
        //                // Select spawn points at a proper distance from last location (previous objective or home airbase)
        //                lastCoordinates, distanceFromLast,
        //                // Make sure no objective is too close to the initial location
        //                mission.InitialPosition, new MinMaxD(objectiveDistanceNM * OBJECTIVE_DISTANCE_VARIATION_MIN, 999999999),
        //                GeneratorTools.GetEnemySpawnPointCoalition(template));
        //        // No spawn point found for the objective, abort mission creation.
        //        if (!spawnPoint.HasValue)
        //            throw new Exception($"Failed to find a spawn point for objective {i + 1}");
        //        objectiveCoordinates = spawnPoint.Value.Coordinates;
        //        //} else {
        //        //    airbase = new MissionGeneratorAirbases(Database).SelectObjectiveAirbase(mission, template, theaterDB, lastCoordinates, distanceFromLast, i == 0);
        //        //    if (!airbase.HasValue)
        //        //            throw new Exception($"Failed to find a airbase point for objective {i + 1}");
        //        //    objectiveCoordinates = airbase.Value.Coordinates;
        //        //}

        //        // Set the waypoint coordinates according the the inaccuracy defined in the objective database entry
        //        Coordinates waypointCoordinates =
        //            objectiveCoordinates; // + Coordinates.CreateRandom(objectiveDB.WaypointInaccuracy * Toolbox.NM_TO_METERS);

        //        // Set the mission objective
        //        mission.Objectives[i] = new DCSMissionObjective(
        //            objectiveName, objectiveCoordinates, Toolbox.RandomFrom(objectiveTarget.UnitFamilies), waypointCoordinates, true, airbase.HasValue ? airbase.Value.DCSID : 0);

        //        // Last position is now the position of this objective
        //        lastCoordinates = objectiveCoordinates;
        //    }

        //    // If the target is a static object, make sure the correct flag is enabled as it has an influence of some scripts
        //    mission.ObjectiveIsStatic = false; // objectiveDB.UnitGroup.Category.HasValue && (objectiveDB.UnitGroup.Category.Value == UnitCategory.Static);

        //    // Make sure objectives are ordered by distance from the players' starting location
        //    mission.Objectives = mission.Objectives.OrderBy(x => mission.InitialPosition.GetDistanceFrom(x.WaypointCoordinates)).ToArray();
        //}

        ///// <summary>
        ///// Generate Lua script for the objectives.
        ///// </summary>
        ///// <param name="mission">The mission for which to generate objectives</param>
        //private void GenerateObjectivesScript(DCSMission mission)
        //{
        //    mission.CoreLuaScript += "briefingRoom.mission.objectives = { }\r\n";

        //    for (int i = 0; i < mission.Objectives.Length; i++)
        //    {
        //        mission.CoreLuaScript +=
        //            $"briefingRoom.mission.objectives[{i + 1}] = {{ " +
        //            $"[\"name\"] = \"{mission.Objectives[i].Name}\", " +
        //            $"[\"coordinates\"] = {mission.Objectives[i].Coordinates.ToLuaTable()}, " +
        //            $"[\"groupID\"] = 0, " +
        //            $"[\"menuPath\"] = nil, " +
        //            $"[\"status\"] = brMissionStatus.IN_PROGRESS, " +
        //            $"[\"task\"] = \"Complete objective {mission.Objectives[i].Name}\", " +
        //            $"[\"waypoint\"] = {mission.Objectives[i].WaypointCoordinates.ToLuaTable()}" +
        //            " }\r\n";
        //    }
        //}

        ///// <summary>
        ///// Returns an unused objective name.
        ///// </summary>
        ///// <returns>A objective name not used by any other objective</returns>
        //private string PickUniqueObjectiveName()
        //{
        //    string objectiveName;
        //    do
        //    {
        //        objectiveName = Toolbox.RandomFrom(Database.Common.WPNamesObjectives);
        //    } while (UsedObjectiveNames.Contains(objectiveName));
        //    UsedObjectiveNames.Add(objectiveName);

        //    return objectiveName;
        //}

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
