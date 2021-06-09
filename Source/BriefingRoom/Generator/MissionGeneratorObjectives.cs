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
    /// Generates the <see cref="DCSMissionObjective"/> array.
    /// </summary>
    internal class MissionGeneratorObjectives : IDisposable
    {
        /// <summary>
        /// List of available objective names, to make sure each one is different.
        /// </summary>
        private readonly List<string> ObjectiveNames = new List<string>();

        /// <summary>
        /// Minimum objective distance variation.
        /// </summary>
        private const double OBJECTIVE_DISTANCE_VARIATION_MIN = 0.75;

        /// <summary>
        /// Maximum objective distance variation.
        /// </summary>
        private const double OBJECTIVE_DISTANCE_VARIATION_MAX = 1.25;

        /// <summary>
        /// Unit maker selector to use for objective generation.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// MissionGeneratorFeaturesObjectives to use to generate objective features;
        /// </summary>
        private readonly MissionGeneratorFeaturesObjectives FeaturesGenerator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spawnPointSelector">Spawn point selector to use for objective generation</param>
        internal MissionGeneratorObjectives(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
            FeaturesGenerator = new MissionGeneratorFeaturesObjectives(unitMaker);
            ObjectiveNames = new List<string>(Database.Instance.Common.Names.WPObjectivesNames);
        }

        internal Coordinates GenerateObjective(DCSMission mission, MissionTemplate template, int objectiveIndex, Coordinates lastCoordinates, out string objectiveName)
        {
            MissionTemplateObjective objectiveTemplate = template.Objectives[objectiveIndex];
            DBEntryFeatureObjective[] featuresDB = Database.Instance.GetEntries<DBEntryFeatureObjective>(objectiveTemplate.Features.ToArray());
            DBEntryObjectiveTarget targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            DBEntryObjectiveTargetBehavior targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior);
            DBEntryObjectiveTask taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);

            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective #{objectiveIndex + 1}.");
            if (targetBehaviorDB == null) throw new BriefingRoomException($"Target behavior \"{targetBehaviorDB.UIDisplayName}\" not found for objective #{objectiveIndex + 1}.");
            if (taskDB == null) throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not found for objective #{objectiveIndex + 1}.");

            if (!taskDB.ValidUnitCategories.Contains(targetDB.UnitCategory))
                throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not valid for objective #{objectiveIndex + 1} targets, which belong to category \"{targetDB.UnitCategory}\".");

            DBEntryTheaterSpawnPoint? spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                targetDB.ValidSpawnPoints, lastCoordinates,
                new MinMaxD(
                    template.FlightPlanObjectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MIN,
                    template.FlightPlanObjectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MAX),
                null, null,
                GeneratorTools.GetSpawnPointCoalition(template, Side.Enemy));

            if (!spawnPoint.HasValue)
                throw new BriefingRoomException("Failed to spawn objective unit group.");

            // Pick a name, then remove it from the list
            objectiveName = Toolbox.RandomFrom(ObjectiveNames);
            ObjectiveNames.Remove(objectiveName);

            UnitMakerGroupFlags groupFlags = 0;
            if (objectiveTemplate.Options.Contains(ObjectiveOption.ShowTarget)) groupFlags = UnitMakerGroupFlags.NeverHidden;
            else if (objectiveTemplate.Options.Contains(ObjectiveOption.HideTarget)) groupFlags = UnitMakerGroupFlags.AlwaysHidden;

            UnitFamily targetFamily = Toolbox.RandomFrom(targetDB.UnitFamilies);

            UnitMakerGroupInfo? targetGroupInfo = UnitMaker.AddUnitGroup(
                targetFamily, targetDB.UnitCount[(int)objectiveTemplate.TargetCount].GetValue(),
                taskDB.TargetSide,
                targetBehaviorDB.GroupLua[(int)targetDB.UnitCategory], targetBehaviorDB.UnitLua[(int)targetDB.UnitCategory],
                spawnPoint.Value.Coordinates,
                null, groupFlags,
                AircraftPayload.Default);

            if (!targetGroupInfo.HasValue) // Failed to generate target group
                throw new BriefingRoomException($"Failed to generate group for objective {objectiveIndex + 1}");

            // Get tasking string for the briefing
            int pluralIndex = targetGroupInfo.Value.UnitsID.Length == 1 ? 0 : 1; // 0 for singular, 1 for plural. Used for task/names arrays.
            string taskString = GeneratorTools.ParseRandomString(taskDB.BriefingTask[pluralIndex]).Replace("\"", "''");
            if (string.IsNullOrEmpty(taskString)) taskString = "Perform task at objective $OBJECTIVENAME$";
            GeneratorTools.ReplaceKey(ref taskString, "ObjectiveName", objectiveName);
            GeneratorTools.ReplaceKey(ref taskString, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)targetFamily][pluralIndex]);
            mission.Briefing.AddItem(DCSMissionBriefingItemType.Task, taskString);

            // Add Lua table for this objective
            string objectiveLua = $"briefingRoom.mission.objectives[{objectiveIndex + 1}] = {{ ";
            objectiveLua += $"complete = false, ";
            objectiveLua += $"groupID = {targetGroupInfo.Value.GroupID}, ";
            objectiveLua += $"name = \"{objectiveName}\", ";
            objectiveLua += $"targetCategory = Unit.Category.{targetDB.UnitCategory.ToLuaName()}, ";
            objectiveLua += $"task = \"{taskString}\", ";
            objectiveLua += $"unitsCount = {targetGroupInfo.Value.UnitsID.Length}, ";
            objectiveLua += $"unitsID = {{ {string.Join(", ", targetGroupInfo.Value.UnitsID)} }} ";
            objectiveLua += "}\n";

            // Add F10 sub-menu for this objective
            objectiveLua += $"briefingRoom.f10Menu.objectives[{objectiveIndex + 1}] = missionCommands.addSubMenuForCoalition(coalition.side.{template.ContextPlayerCoalition.ToString().ToUpperInvariant()}, \"Objective {objectiveName}\", nil)\n";
            mission.AppendValue("ScriptObjectives", objectiveLua);

            // Add objective trigger Lua for this objective
            string triggerLua = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_LUA_OBJECTIVETRIGGERS}{taskDB.CompletionTriggerLua}");
            GeneratorTools.ReplaceKey(ref triggerLua, "ObjectiveIndex", objectiveIndex + 1);
            mission.AppendValue("ScriptObjectivesTriggers", triggerLua);

            // Add objective features Lua for this objective
            mission.AppendValue("ScriptObjectivesFeatures", ""); // Just in case there's no features
            foreach (string featureID in objectiveTemplate.Features.ToArray())
                FeaturesGenerator.GenerateMissionFeature(mission, featureID, objectiveIndex, targetGroupInfo.Value.GroupID, spawnPoint.Value.Coordinates);

            return spawnPoint.Value.Coordinates;
        }

        internal Waypoint GenerateObjectiveWaypoint(MissionTemplateObjective objectiveTemplate, Coordinates objectiveCoordinates, string objectiveName)
        {
            DBEntryObjectiveTarget targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");

            Coordinates waypointCoordinates = objectiveCoordinates;
            bool onGround = !targetDB.UnitCategory.IsAircraft(); // Ground targets = waypoint on the ground

            if (objectiveTemplate.Options.Contains(ObjectiveOption.InaccurateWaypoint))
                waypointCoordinates += Coordinates.CreateRandom(3.0, 6.0) * Toolbox.NM_TO_METERS;

            return new Waypoint(objectiveName, waypointCoordinates, onGround);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            FeaturesGenerator.Dispose();
        }
    }
}
