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
    internal class MissionGeneratorObjectives
    {
        private readonly List<string> ObjectiveNames = new List<string>();

        private const double OBJECTIVE_DISTANCE_VARIATION_MIN = 0.75;

        private const double OBJECTIVE_DISTANCE_VARIATION_MAX = 1.25;

        private static readonly List<DBEntryObjectiveTargetBehaviorLocation> AIRBASE_LOCATIONS = new List<DBEntryObjectiveTargetBehaviorLocation>{
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbase,
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParking,
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter,
        };

        private static readonly List<SpawnPointType> LAND_SPAWNS = new List<SpawnPointType>{
            SpawnPointType.LandSmall,
            SpawnPointType.LandMedium,
            SpawnPointType.LandLarge,
        };

        private readonly UnitMaker UnitMaker;

        private readonly DrawingMaker DrawingMaker;

        private readonly MissionGeneratorFeaturesObjectives FeaturesGenerator;

        internal MissionGeneratorObjectives(UnitMaker unitMaker, DrawingMaker drawingMaker, MissionTemplateRecord template)
        {
            UnitMaker = unitMaker;
            DrawingMaker = drawingMaker;
            FeaturesGenerator = new MissionGeneratorFeaturesObjectives(unitMaker, template);
            ObjectiveNames = new List<string>(Database.Instance.Common.Names.WPObjectivesNames);
        }

        internal Tuple<Coordinates, List<Waypoint>> GenerateObjective(
            DCSMission mission,
            MissionTemplateRecord template,
            DBEntrySituation situationDB,
            MissionTemplateObjectiveRecord objectiveTemplate,
            Coordinates lastCoordinates,
            DBEntryAirbase playerAirbase,
            bool useObjectivePreset,
            ref int objectiveIndex,
            ref List<Coordinates> objectiveCoordinatesList,
            ref List<Waypoint> waypoints,
            ref List<UnitFamily> objectiveTargetUnitFamilies)
        {
            var extraSettings = new List<KeyValuePair<string, object>>();
            var waypointList = new List<Waypoint>();
            string[] featuresID;
            DBEntryObjectiveTarget targetDB;
            DBEntryObjectiveTargetBehavior targetBehaviorDB;
            DBEntryObjectiveTask taskDB;
            ObjectiveOption[] objectiveOptions;

            GetObjectiveData(objectiveTemplate, useObjectivePreset, out featuresID, out targetDB, out targetBehaviorDB, out taskDB, out objectiveOptions);
            var luaUnit = targetBehaviorDB.UnitLua[(int)targetDB.UnitCategory];
            Coordinates objectiveCoordinates = GetSpawnCoordinates(template, lastCoordinates, playerAirbase, targetDB);

            // Spawn target on airbase
            var unitCount = targetDB.UnitCount[(int)objectiveTemplate.TargetCount].GetValue();
            var objectiveTargetUnitFamily = Toolbox.RandomFrom(targetDB.UnitFamilies);
            if (AIRBASE_LOCATIONS.Contains(targetBehaviorDB.Location))
                objectiveCoordinates = PlaceInAirbase(template, situationDB, playerAirbase, extraSettings, targetDB, targetBehaviorDB, ref luaUnit, objectiveCoordinates, unitCount, objectiveTargetUnitFamily);

            UnitMakerGroupFlags groupFlags = 0;
            if (objectiveOptions.Contains(ObjectiveOption.ShowTarget)) groupFlags = UnitMakerGroupFlags.NeverHidden;
            else if (objectiveOptions.Contains(ObjectiveOption.HideTarget)) groupFlags = UnitMakerGroupFlags.AlwaysHidden;
            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense)) groupFlags |= UnitMakerGroupFlags.EmbeddedAirDefense;

            // Set destination point for moving unit groups
            Coordinates destinationPoint = objectiveCoordinates +
                (
                    (targetDB.UnitCategory == UnitCategory.Plane ? Coordinates.CreateRandom(30, 60) : Coordinates.CreateRandom(10, 20)) *
                    Toolbox.NM_TO_METERS
                );

            if (targetBehaviorDB.Location == DBEntryObjectiveTargetBehaviorLocation.GoToPlayerAirbase)
                destinationPoint = playerAirbase.Coordinates;


            extraSettings.Add("GroupX2".ToKeyValuePair(destinationPoint.X));
            extraSettings.Add("GroupY2".ToKeyValuePair(destinationPoint.Y));

            UnitMakerGroupInfo? targetGroupInfo = UnitMaker.AddUnitGroup(
                objectiveTargetUnitFamily, unitCount,
                taskDB.TargetSide,
                targetBehaviorDB.GroupLua[(int)targetDB.UnitCategory], luaUnit,
                objectiveCoordinates,
                groupFlags,
                extraSettings.ToArray());

            if (!targetGroupInfo.HasValue) // Failed to generate target group
                throw new BriefingRoomException($"Failed to generate group for objective.");

            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense) && (targetDB.UnitCategory == UnitCategory.Static))
                AddEmbeddedAirDefenseUnits(template, targetDB, targetBehaviorDB, taskDB, objectiveOptions, objectiveCoordinates, groupFlags, extraSettings);

            var pluralIndex = targetGroupInfo.Value.UnitsID.Length == 1 ? 0 : 1;
            var taskString = GeneratorTools.ParseRandomString(taskDB.BriefingTask[pluralIndex]).Replace("\"", "''");
            // Pick a name, then remove it from the list
            var objectiveName = Toolbox.RandomFrom(ObjectiveNames);
            ObjectiveNames.Remove(objectiveName);
            CreateTaskString(mission, pluralIndex, ref taskString, objectiveName, objectiveTargetUnitFamily);

            CreateLua(mission, template, targetDB, taskDB, objectiveIndex, objectiveName, targetGroupInfo, taskString);

            // Add briefing remarks for this objective task
            if (taskDB.BriefingRemarks.Length > 0)
            {
                string remark = Toolbox.RandomFrom(taskDB.BriefingRemarks);
                GeneratorTools.ReplaceKey(ref remark, "ObjectiveName", objectiveName);
                GeneratorTools.ReplaceKey(ref remark, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)objectiveTargetUnitFamily][pluralIndex]);
                mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark);
            }

            // Add feature ogg files
            foreach (string oggFile in taskDB.IncludeOgg)
                mission.AddMediaFile($"l10n/DEFAULT/{oggFile}", $"{BRPaths.INCLUDE_OGG}{oggFile}");

            // Add objective features Lua for this objective
            mission.AppendValue("ScriptObjectivesFeatures", ""); // Just in case there's no features
            foreach (string featureID in featuresID)
                FeaturesGenerator.GenerateMissionFeature(mission, featureID, objectiveName, objectiveIndex, targetGroupInfo.Value.GroupID, objectiveCoordinates, taskDB.TargetSide, objectiveOptions.Contains(ObjectiveOption.HideTarget));

            objectiveCoordinatesList.Add(objectiveCoordinates);
            var waypoint = GenerateObjectiveWaypoint(objectiveTemplate, objectiveCoordinates, objectiveName, template);
            waypoints.Add(waypoint);
            waypointList.Add(waypoint);
            objectiveTargetUnitFamilies.Add(objectiveTargetUnitFamily);

            var preValidSpawns = targetDB.ValidSpawnPoints.ToList();
            
            foreach (var subTasks in objectiveTemplate.SubTasks)
            {
                objectiveIndex++;
                GenerateSubTask(mission, template, situationDB,
                subTasks, objectiveCoordinates,
                playerAirbase,
                preValidSpawns,
                targetBehaviorDB.Location,
                featuresID,
                ref objectiveIndex,
                ref objectiveCoordinatesList, ref waypoints, ref waypointList, ref objectiveTargetUnitFamilies);

            }
            return new (objectiveCoordinates, waypointList);
        }

        private void GenerateSubTask(
            DCSMission mission,
            MissionTemplateRecord template,
            DBEntrySituation situationDB,
            MissionTemplateSubTask subTask,
            Coordinates coreCoordinates,
            DBEntryAirbase playerAirbase,
            List<SpawnPointType> preValidSpawns,
            DBEntryObjectiveTargetBehaviorLocation mainObjLocation,
            string[] featuresID,
            ref int objectiveIndex,
            ref List<Coordinates> objectiveCoordinatesList,
            ref List<Waypoint> waypoints,
            ref List<Waypoint> waypointList,
            ref List<UnitFamily> objectiveTargetUnitFamilies
            )
        {
             var extraSettings = new List<KeyValuePair<string, object>>();
            DBEntryObjectiveTarget targetDB;
            DBEntryObjectiveTargetBehavior targetBehaviorDB;
            DBEntryObjectiveTask taskDB;
            ObjectiveOption[] objectiveOptions;

            GetSubTaskData(subTask, out targetDB, out targetBehaviorDB, out taskDB, out objectiveOptions);
            
            preValidSpawns.AddRange(targetDB.ValidSpawnPoints);
            if(preValidSpawns.Contains(SpawnPointType.Sea) && preValidSpawns.Any(x => LAND_SPAWNS.Contains(x)))
                throw new BriefingRoomException("Cannot Mix Land and Sea Objectives. Check Sub Objective targets");
            var luaUnit = targetBehaviorDB.UnitLua[(int)targetDB.UnitCategory];
            Coordinates objectiveCoordinates = GetNearestSpawnCoordinates(template, coreCoordinates, targetDB);

            // Spawn target on airbase
            var unitCount = targetDB.UnitCount[(int)subTask.TargetCount].GetValue();
            var objectiveTargetUnitFamily = Toolbox.RandomFrom(targetDB.UnitFamilies);
            if (AIRBASE_LOCATIONS.Contains(targetBehaviorDB.Location) && !AIRBASE_LOCATIONS.Contains(mainObjLocation))
                throw new BriefingRoomException("Spawning on airbase is not a valid Sub Objective unless main objective is also spawning on airbase.");

            UnitMakerGroupFlags groupFlags = 0;
            if (objectiveOptions.Contains(ObjectiveOption.ShowTarget)) groupFlags = UnitMakerGroupFlags.NeverHidden;
            else if (objectiveOptions.Contains(ObjectiveOption.HideTarget)) groupFlags = UnitMakerGroupFlags.AlwaysHidden;
            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense)) groupFlags |= UnitMakerGroupFlags.EmbeddedAirDefense;

            // Set destination point for moving unit groups
            Coordinates destinationPoint = objectiveCoordinates +
                (
                    (targetDB.UnitCategory == UnitCategory.Plane ? Coordinates.CreateRandom(30, 60) : Coordinates.CreateRandom(10, 20)) *
                    Toolbox.NM_TO_METERS
                );

            if (targetBehaviorDB.Location == DBEntryObjectiveTargetBehaviorLocation.GoToPlayerAirbase)
                destinationPoint = playerAirbase.Coordinates;


            extraSettings.Add("GroupX2".ToKeyValuePair(destinationPoint.X));
            extraSettings.Add("GroupY2".ToKeyValuePair(destinationPoint.Y));

            UnitMakerGroupInfo? targetGroupInfo = UnitMaker.AddUnitGroup(
                objectiveTargetUnitFamily, unitCount,
                taskDB.TargetSide,
                targetBehaviorDB.GroupLua[(int)targetDB.UnitCategory], luaUnit,
                objectiveCoordinates,
                groupFlags,
                extraSettings.ToArray());

            if (!targetGroupInfo.HasValue) // Failed to generate target group
                throw new BriefingRoomException($"Failed to generate group for objective.");

            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense) && (targetDB.UnitCategory == UnitCategory.Static))
                AddEmbeddedAirDefenseUnits(template, targetDB, targetBehaviorDB, taskDB, objectiveOptions, objectiveCoordinates, groupFlags, extraSettings);

            var pluralIndex = targetGroupInfo.Value.UnitsID.Length == 1 ? 0 : 1;
            var taskString = GeneratorTools.ParseRandomString(taskDB.BriefingTask[pluralIndex]).Replace("\"", "''");
            // Pick a name, then remove it from the list
            var objectiveName = Toolbox.RandomFrom(ObjectiveNames);
            ObjectiveNames.Remove(objectiveName);
            CreateTaskString(mission, pluralIndex, ref taskString, objectiveName, objectiveTargetUnitFamily);
            CreateLua(mission, template, targetDB, taskDB, objectiveIndex, objectiveName, targetGroupInfo, taskString);

            // Add briefing remarks for this objective task
            if (taskDB.BriefingRemarks.Length > 0)
            {
                string remark = Toolbox.RandomFrom(taskDB.BriefingRemarks);
                GeneratorTools.ReplaceKey(ref remark, "ObjectiveName", objectiveName);
                GeneratorTools.ReplaceKey(ref remark, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)objectiveTargetUnitFamily][pluralIndex]);
                mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark);
            }

            // Add feature ogg files
            foreach (string oggFile in taskDB.IncludeOgg)
                mission.AddMediaFile($"l10n/DEFAULT/{oggFile}", $"{BRPaths.INCLUDE_OGG}{oggFile}");
            

            // Add objective features Lua for this objective
            mission.AppendValue("ScriptObjectivesFeatures", ""); // Just in case there's no features
            foreach (string featureID in featuresID)
                FeaturesGenerator.GenerateMissionFeature(mission, featureID, objectiveName, objectiveIndex, targetGroupInfo.Value.GroupID, objectiveCoordinates, taskDB.TargetSide, objectiveOptions.Contains(ObjectiveOption.HideTarget));
            objectiveCoordinatesList.Add(objectiveCoordinates);
            var waypoint = GenerateSubTaskWaypoint(subTask, objectiveCoordinates, objectiveName, template);
            waypoints.Add(waypoint);
            waypointList.Add(waypoint);
            objectiveTargetUnitFamilies.Add(objectiveTargetUnitFamily);

        }

        private Coordinates PlaceInAirbase(MissionTemplateRecord template, DBEntrySituation situationDB, DBEntryAirbase playerAirbase, List<KeyValuePair<string, object>> extraSettings, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, ref string luaUnit, Coordinates objectiveCoordinates, int unitCount, UnitFamily objectiveTargetUnitFamily)
        {
            int airbaseID = 0;
            var parkingSpotIDsList = new List<int>();
            var parkingSpotCoordinatesList = new List<Coordinates>();
            var targetAirbaseOptions =
                (from DBEntryAirbase airbaseDB in situationDB.GetAirbases(template.OptionsMission.Contains("InvertCountriesCoalitions"))
                 where airbaseDB.DCSID != playerAirbase.DCSID
                 select airbaseDB).OrderBy(x => x.Coordinates.GetDistanceFrom(objectiveCoordinates));
            DBEntryAirbase targetAirbase = targetAirbaseOptions.FirstOrDefault(x => template.OptionsMission.Contains("SpawnAnywhere") ? true : x.Coalition == template.ContextPlayerCoalition.GetEnemy());
            
            airbaseID = targetAirbase.DCSID;

            if ((targetBehaviorDB.Location != DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbase) && targetDB.UnitCategory.IsAircraft())
            {
                var parkingSpots = UnitMaker.SpawnPointSelector.GetFreeParkingSpots(
                    targetAirbase.DCSID,
                    unitCount, objectiveTargetUnitFamily,
                    targetBehaviorDB.Location == DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter);

                parkingSpotIDsList = parkingSpots.Select(x => x.DCSID).ToList();
                parkingSpotCoordinatesList = parkingSpots.Select(x => x.Coordinates).ToList();
            }
            luaUnit += "Parked";
            extraSettings.Add("GroupAirbaseID".ToKeyValuePair(airbaseID));
            extraSettings.Add("ParkingID".ToKeyValuePair(parkingSpotIDsList.ToArray()));
            extraSettings.Add("UnitX".ToKeyValuePair((from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.X).ToArray()));
            extraSettings.Add("UnitY".ToKeyValuePair((from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.Y).ToArray()));
            return targetAirbase.Coordinates;
        }

        private Coordinates GetSpawnCoordinates(MissionTemplateRecord template, Coordinates lastCoordinates, DBEntryAirbase playerAirbase, DBEntryObjectiveTarget targetDB)
        {
            int objectiveDistance = template.FlightPlanObjectiveDistance;
            if (objectiveDistance < 1) objectiveDistance = Toolbox.RandomInt(40, 160);

            int objectiveSeperation = template.FlightPlanObjectiveSeperation;
            if (objectiveSeperation < 1) objectiveSeperation = Toolbox.RandomInt(10, 100);

            Coordinates? spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                targetDB.ValidSpawnPoints,
                 playerAirbase.Coordinates,
                new MinMaxD(
                    objectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MIN,
                    objectiveDistance * OBJECTIVE_DISTANCE_VARIATION_MAX),
                lastCoordinates,
                new MinMaxD(
                    objectiveSeperation * OBJECTIVE_DISTANCE_VARIATION_MIN,
                    objectiveSeperation * OBJECTIVE_DISTANCE_VARIATION_MAX),
                GeneratorTools.GetSpawnPointCoalition(template, Side.Enemy));

            if (!spawnPoint.HasValue)
                throw new BriefingRoomException($"Failed to spawn objective unit group. {String.Join(",", targetDB.ValidSpawnPoints.Select(x => x.ToString()).ToList())} Please try again (Consider Adusting Flight Plan)");

            Coordinates objectiveCoordinates = spawnPoint.Value;
            return objectiveCoordinates;
        }

        private static void GetObjectiveData(MissionTemplateObjectiveRecord objectiveTemplate, bool useObjectivePreset, out string[] featuresID, out DBEntryObjectiveTarget targetDB, out DBEntryObjectiveTargetBehavior targetBehaviorDB, out DBEntryObjectiveTask taskDB, out ObjectiveOption[] objectiveOptions)
        {
            featuresID = objectiveTemplate.Features.ToArray();
            targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior);
            taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);
            objectiveOptions = objectiveTemplate.Options.ToArray();
            if (useObjectivePreset)
            {
                DBEntryObjectivePreset presetDB = Database.Instance.GetEntry<DBEntryObjectivePreset>(objectiveTemplate.Preset);
                if (presetDB != null)
                {
                    featuresID = presetDB.Features.ToArray();
                    targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(Toolbox.RandomFrom(presetDB.Targets));
                    targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(Toolbox.RandomFrom(presetDB.TargetsBehaviors));
                    taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(Toolbox.RandomFrom(presetDB.Tasks));
                    objectiveOptions = presetDB.Options.ToArray();
                }
            }

            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");
            if (targetBehaviorDB == null) throw new BriefingRoomException($"Target behavior \"{targetBehaviorDB.UIDisplayName}\" not found for objective.");
            if (taskDB == null) throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not found for objective.");
            if (!taskDB.ValidUnitCategories.Contains(targetDB.UnitCategory))
                throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not valid for objective targets, which belong to category \"{targetDB.UnitCategory}\".");
        }

        

        private void AddEmbeddedAirDefenseUnits(MissionTemplateRecord template, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, DBEntryObjectiveTask taskDB, ObjectiveOption[] objectiveOptions, Coordinates objectiveCoordinates, UnitMakerGroupFlags groupFlags, List<KeyValuePair<string, object>> extraSettings)
        {
            // Static targets (aka buildings) need to have their "embedded" air defenses spawned in another group
            string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(template, taskDB.TargetSide);

            if (airDefenseUnits.Length > 0)
                UnitMaker.AddUnitGroup(
                    airDefenseUnits,
                    taskDB.TargetSide, UnitFamily.VehicleAAA,
                    targetBehaviorDB.GroupLua[(int)targetDB.UnitCategory], targetBehaviorDB.UnitLua[(int)targetDB.UnitCategory],
                    objectiveCoordinates + Coordinates.CreateRandom(100, 500),
                    groupFlags,
                    extraSettings.ToArray());
        }

        private static void CreateLua(DCSMission mission, MissionTemplateRecord template, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTask taskDB, int objectiveIndex, string objectiveName, UnitMakerGroupInfo? targetGroupInfo, string taskString)
        {
            // Add Lua table for this objective
            string objectiveLua = $"briefingRoom.mission.objectives[{objectiveIndex + 1}] = {{ ";
            objectiveLua += $"complete = false, ";
            objectiveLua += $"groupID = {targetGroupInfo.Value.GroupID}, ";
            objectiveLua += $"hideTargetCount = false, ";
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
        }

        private static void CreateTaskString(DCSMission mission, int pluralIndex, ref string taskString, string objectiveName, UnitFamily objectiveTargetUnitFamily)
        {
            // Get tasking string for the briefing
            if (string.IsNullOrEmpty(taskString)) taskString = "Complete objective $OBJECTIVENAME$";
            GeneratorTools.ReplaceKey(ref taskString, "ObjectiveName", objectiveName);
            GeneratorTools.ReplaceKey(ref taskString, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)objectiveTargetUnitFamily][pluralIndex]);
            mission.Briefing.AddItem(DCSMissionBriefingItemType.Task, taskString);
        }

        private Waypoint GenerateObjectiveWaypoint(MissionTemplateObjectiveRecord objectiveTemplate, Coordinates objectiveCoordinates, string objectiveName, MissionTemplateRecord template)
        {
            var AirOnGroundBehaviorLocations = new List<DBEntryObjectiveTargetBehaviorLocation>{
                DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParking,
                DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter};

            DBEntryObjectiveTarget targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            DBEntryObjectiveTargetBehaviorLocation targetBehaviorLocation = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior).Location;
            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");

            Coordinates waypointCoordinates = objectiveCoordinates;
            bool onGround = !targetDB.UnitCategory.IsAircraft() || AirOnGroundBehaviorLocations.Contains(targetBehaviorLocation); // Ground targets = waypoint on the ground

            if (objectiveTemplate.Options.Contains(ObjectiveOption.InaccurateWaypoint))
            {
                waypointCoordinates += Coordinates.CreateRandom(3.0, 6.0) * Toolbox.NM_TO_METERS;
                if (template.OptionsMission.Contains("MarkWaypoints"))
                    DrawingMaker.AddDrawing($"Target Zone {objectiveName}", DrawingType.Circle, waypointCoordinates, "Radius".ToKeyValuePair(6.0 * Toolbox.NM_TO_METERS));
            }

            return new Waypoint(objectiveName, waypointCoordinates, onGround);
        }

        //----------------SUB TASK SUPPORT FUNCTIONS-------------------------------

        private static void GetSubTaskData(MissionTemplateSubTask objectiveTemplate, out DBEntryObjectiveTarget targetDB, out DBEntryObjectiveTargetBehavior targetBehaviorDB, out DBEntryObjectiveTask taskDB, out ObjectiveOption[] objectiveOptions)
        {
            targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior);
            taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);
            objectiveOptions = objectiveTemplate.Options.ToArray();

            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");
            if (targetBehaviorDB == null) throw new BriefingRoomException($"Target behavior \"{targetBehaviorDB.UIDisplayName}\" not found for objective.");
            if (taskDB == null) throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not found for objective.");
            if (!taskDB.ValidUnitCategories.Contains(targetDB.UnitCategory))
                throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not valid for objective targets, which belong to category \"{targetDB.UnitCategory}\".");
        }

        private Coordinates GetNearestSpawnCoordinates(MissionTemplateRecord template, Coordinates coreCoordinates, DBEntryObjectiveTarget targetDB)
        {
            Coordinates? spawnPoint = UnitMaker.SpawnPointSelector.GetNearestSpawnPoint(
                targetDB.ValidSpawnPoints,
                coreCoordinates);

            if (!spawnPoint.HasValue)
                throw new BriefingRoomException($"Failed to spawn objective unit group. {String.Join(",", targetDB.ValidSpawnPoints.Select(x => x.ToString()).ToList())} Please try again (Consider Adusting Flight Plan)");

            Coordinates objectiveCoordinates = spawnPoint.Value;
            return objectiveCoordinates;
        }

        private Waypoint GenerateSubTaskWaypoint(MissionTemplateSubTask objectiveTemplate, Coordinates objectiveCoordinates, string objectiveName, MissionTemplateRecord template)
        {
            var AirOnGroundBehaviorLocations = new List<DBEntryObjectiveTargetBehaviorLocation>{
                DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParking,
                DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter};

            DBEntryObjectiveTarget targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            DBEntryObjectiveTargetBehaviorLocation targetBehaviorLocation = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior).Location;
            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");

            Coordinates waypointCoordinates = objectiveCoordinates;
            bool onGround = !targetDB.UnitCategory.IsAircraft() || AirOnGroundBehaviorLocations.Contains(targetBehaviorLocation); // Ground targets = waypoint on the ground

            if (objectiveTemplate.Options.Contains(ObjectiveOption.InaccurateWaypoint))
            {
                waypointCoordinates += Coordinates.CreateRandom(3.0, 6.0) * Toolbox.NM_TO_METERS;
                if (template.OptionsMission.Contains("MarkWaypoints"))
                    DrawingMaker.AddDrawing($"Target Zone {objectiveName}", DrawingType.Circle, waypointCoordinates, "Radius".ToKeyValuePair(6.0 * Toolbox.NM_TO_METERS));
            }

            return new Waypoint(objectiveName, waypointCoordinates, onGround);
        }
    }
}
