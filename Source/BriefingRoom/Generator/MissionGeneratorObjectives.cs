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
using BriefingRoom4DCS.Mission.DCSLuaObjects;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorObjectives
    {
        private static readonly List<DBEntryObjectiveTargetBehaviorLocation> AIRBASE_LOCATIONS = new List<DBEntryObjectiveTargetBehaviorLocation>{
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbase,
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParking,
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter,
        };

        private static readonly List<DBEntryObjectiveTargetBehaviorLocation> AIR_ON_GROUND_LOCATIONS = new List<DBEntryObjectiveTargetBehaviorLocation>{
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParking,
            DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter
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
        }

        internal Tuple<Coordinates, List<Waypoint>> GenerateObjective(
            DCSMission mission,
            MissionTemplateRecord template,
            DBEntrySituation situationDB,
            MissionTemplateObjectiveRecord task,
            Coordinates lastCoordinates,
            DBEntryAirbase playerAirbase,
            WaypointNameGenerator waypointNameGenerator,

            ref int objectiveIndex,
            ref List<Coordinates> objectiveCoordinatesList,
            ref List<Waypoint> waypoints,
            ref List<UnitFamily> objectiveTargetUnitFamilies)
        {
            var waypointList = new List<Waypoint>();
            var (featuresID, targetDB, targetBehaviorDB, taskDB, objectiveOptions) = GetObjectiveData(task);
            var useHintCoordinates = task.CoordinatesHint.ToString() != "0,0";
            lastCoordinates = useHintCoordinates ? task.CoordinatesHint : lastCoordinates;
            var objectiveCoordinates = GetSpawnCoordinates(template, lastCoordinates, playerAirbase, targetDB, useHintCoordinates);


            CreateObjective(
                task,
                taskDB,
                targetDB,
                targetBehaviorDB,
                situationDB,
                waypointNameGenerator,
                ref objectiveIndex,
                ref objectiveCoordinates,
                objectiveOptions,
                playerAirbase,
                template,
                mission,
                ref waypoints,
                ref waypointList,
                featuresID,
                ref objectiveCoordinatesList,
                ref objectiveTargetUnitFamilies);

            var preValidSpawns = targetDB.ValidSpawnPoints.ToList();

            foreach (var subTasks in task.SubTasks)
            {
                objectiveIndex++;
                GenerateSubTask(
                    mission, template,
                    situationDB, subTasks,
                    objectiveCoordinates, playerAirbase,
                    preValidSpawns, targetBehaviorDB.Location,
                    waypointNameGenerator,
                    featuresID, ref objectiveIndex,
                    ref objectiveCoordinatesList, ref waypoints,
                    ref waypointList, ref objectiveTargetUnitFamilies);

            }
            return new(objectiveCoordinates, waypointList);
        }

        private void GenerateSubTask(
            DCSMission mission,
            MissionTemplateRecord template,
            DBEntrySituation situationDB,
            MissionTemplateSubTaskRecord task,
            Coordinates coreCoordinates,
            DBEntryAirbase playerAirbase,
            List<SpawnPointType> preValidSpawns,
            DBEntryObjectiveTargetBehaviorLocation mainObjLocation,
            WaypointNameGenerator waypointNameGenerator,
            string[] featuresID,
            ref int objectiveIndex,
            ref List<Coordinates> objectiveCoordinatesList,
            ref List<Waypoint> waypoints,
            ref List<Waypoint> waypointList,
            ref List<UnitFamily> objectiveTargetUnitFamilies
            )
        {
            var (targetDB, targetBehaviorDB, taskDB, objectiveOptions, _) = GetCustomObjectiveData(task);

            preValidSpawns.AddRange(targetDB.ValidSpawnPoints);
            if (preValidSpawns.Contains(SpawnPointType.Sea) && preValidSpawns.Any(x => LAND_SPAWNS.Contains(x)))
                throw new BriefingRoomException("Cannot Mix Land and Sea Objectives. Check Sub Objective targets");
            if (AIRBASE_LOCATIONS.Contains(targetBehaviorDB.Location) && !AIRBASE_LOCATIONS.Contains(mainObjLocation))
                throw new BriefingRoomException("Spawning on airbase is not a valid Sub Objective unless main objective is also spawning on airbase.");
            var objectiveCoords = GetNearestSpawnCoordinates(template, coreCoordinates, targetDB);
            CreateObjective(
                task,
                taskDB,
                targetDB,
                targetBehaviorDB,
                situationDB,
                waypointNameGenerator,
                ref objectiveIndex,
                ref objectiveCoords,
                objectiveOptions,
                playerAirbase,
                template,
                mission,
                ref waypoints,
                ref waypointList,
                featuresID,
                ref objectiveCoordinatesList,
                ref objectiveTargetUnitFamilies);
        }

        private void CreateObjective(
            MissionTemplateSubTaskRecord task,
            DBEntryObjectiveTask taskDB,
            DBEntryObjectiveTarget targetDB,
            DBEntryObjectiveTargetBehavior targetBehaviorDB,
            DBEntrySituation situationDB,
            WaypointNameGenerator waypointNameGenerator,
            ref int objectiveIndex,
            ref Coordinates objectiveCoordinates,
            ObjectiveOption[] objectiveOptions,
            DBEntryAirbase playerAirbase,
            MissionTemplateRecord template,
            DCSMission mission,
            ref List<Waypoint> waypoints,
            ref List<Waypoint> waypointList,
            string[] featuresID,
            ref List<Coordinates> objectiveCoordinatesList,
            ref List<UnitFamily> objectiveTargetUnitFamilies)
        {
            var extraSettings = new Dictionary<string, object>();
            var (luaUnit, unitCount, unitCountMinMax, objectiveTargetUnitFamily, groupFlags) = GetUnitData(task, targetDB, targetBehaviorDB, objectiveOptions);
            var isInverseTransportWayPoint = false;

            if (AIRBASE_LOCATIONS.Contains(targetBehaviorDB.Location) && targetDB.UnitCategory.IsAircraft())
                objectiveCoordinates = PlaceInAirbase(template, situationDB, playerAirbase, extraSettings, targetDB, targetBehaviorDB, ref luaUnit, objectiveCoordinates, unitCount, objectiveTargetUnitFamily);

            // Set destination point for moving unit groups
            Coordinates destinationPoint = objectiveCoordinates +
                (
                    targetDB.UnitCategory switch
                    {
                        UnitCategory.Plane => Coordinates.CreateRandom(30, 60),
                        UnitCategory.Helicopter => Coordinates.CreateRandom(10, 20),
                        _ => objectiveTargetUnitFamily == UnitFamily.InfantryMANPADS || objectiveTargetUnitFamily == UnitFamily.Infantry ? Coordinates.CreateRandom(1, 5) : Coordinates.CreateRandom(5, 10)
                    } * Toolbox.NM_TO_METERS
                );
            if (targetDB.DCSUnitCategory == DCSUnitCategory.Vehicle)
                destinationPoint = GetNearestSpawnCoordinates(template, destinationPoint, targetDB);

            var groupLua = targetBehaviorDB.GroupLua[(int)targetDB.DCSUnitCategory];
            if (targetBehaviorDB.Location == DBEntryObjectiveTargetBehaviorLocation.GoToPlayerAirbase)
            {
                destinationPoint = playerAirbase.ParkingSpots.Count() > 1 ? Toolbox.RandomFrom(playerAirbase.ParkingSpots).Coordinates : playerAirbase.Coordinates;
                if (objectiveTargetUnitFamily.GetUnitCategory().IsAircraft())
                {
                    groupLua = objectiveTargetUnitFamily switch
                    {
                        UnitFamily.PlaneAttack => "AircraftBomb",
                        UnitFamily.PlaneBomber => "AircraftBomb",
                        UnitFamily.PlaneStrike => "AircraftBomb",
                        UnitFamily.PlaneFighter => "AircraftCAP",
                        UnitFamily.PlaneInterceptor => "AircraftCAP",
                        UnitFamily.HelicopterAttack => "AircraftBomb",
                        _ => groupLua
                    };
                }
            }

            extraSettings.Add("GroupX2", destinationPoint.X);
            extraSettings.Add("GroupY2", destinationPoint.Y);
            extraSettings.Add("playerCanDrive", false);
            extraSettings.Add("NoCM", true);

            var unitCoordinates = objectiveCoordinates;
            var objectiveName = waypointNameGenerator.GetWaypointName();
            if (taskDB.UICategory.ContainsValue("Transport"))
            {
                Coordinates? spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                targetDB.ValidSpawnPoints,
                playerAirbase.Coordinates,
                new MinMaxD(1, 5),
                coalition: GeneratorTools.GetSpawnPointCoalition(template, Side.Ally));
                if (!spawnPoint.HasValue) // Failed to generate target group
                    throw new BriefingRoomException($"Failed to find Cargo SpawnPoint");
                unitCoordinates = spawnPoint.Value;
                if (targetBehaviorDB.ID.StartsWith("GoToPlayerBase"))
                {
                    var spawnTempCoords = objectiveCoordinates;
                    objectiveCoordinates = unitCoordinates;
                    unitCoordinates = spawnTempCoords;
                    isInverseTransportWayPoint = true;
                }
                var cargoWaypoint = GenerateObjectiveWaypoint(task, unitCoordinates, $"{objectiveName} Pickup", template, true);
                waypoints.Add(cargoWaypoint);
                waypointList.Add(cargoWaypoint);
                if (taskDB.isEscort())
                {
                    extraSettings["GroupX2"] = objectiveCoordinates.X;
                    extraSettings["GroupY2"] = objectiveCoordinates.Y;
                    groupFlags |= UnitMakerGroupFlags.RadioAircraftSpawn;
                }
                else
                {
                    // Units shouldn't really move from pickup point if not escorted.
                    extraSettings.Remove("GroupX2");
                    extraSettings.Remove("GroupY2");
                    groupLua = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>("Idle").GroupLua[(int)targetDB.DCSUnitCategory];
                }
            }

            if (
                objectiveTargetUnitFamily.GetUnitCategory().IsAircraft() &&
                !groupFlags.HasFlag(UnitMakerGroupFlags.RadioAircraftSpawn) &&
                !AIR_ON_GROUND_LOCATIONS.Contains(targetBehaviorDB.Location)
                )
                groupFlags |= UnitMakerGroupFlags.ImmediateAircraftSpawn;

            UnitMakerGroupInfo? targetGroupInfo = UnitMaker.AddUnitGroup(
                objectiveTargetUnitFamily, unitCount,
                taskDB.TargetSide,
                groupLua, luaUnit,
                unitCoordinates,
                unitCountMinMax,
                groupFlags,
                extraSettings);

            if (!targetGroupInfo.HasValue) // Failed to generate target group
                throw new BriefingRoomException($"Failed to generate group for objective.");

            if (template.MissionFeatures.Contains("ContextScrambleStart") && !taskDB.UICategory.ContainsValue("Transport"))
                targetGroupInfo.Value.DCSGroup.LateActivation = false;

            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense) && (targetDB.UnitCategory == UnitCategory.Static))
                AddEmbeddedAirDefenseUnits(template, targetDB, targetBehaviorDB, taskDB, objectiveOptions, objectiveCoordinates, groupFlags, extraSettings);

            targetGroupInfo.Value.DCSGroup.Waypoints = DCSWaypoint.CreateExtraWaypoints(targetGroupInfo.Value.DCSGroup.Waypoints, targetGroupInfo.Value.UnitDB.Families.First(), UnitMaker.SpawnPointSelector);

            // Assign target suffix
            var i = 0;
            var isStatic = objectiveTargetUnitFamily.GetUnitCategory() == UnitCategory.Static;
            targetGroupInfo.Value.DCSGroups.ForEach(x =>
            {
                x.Name += $"{(i == 0 ? "" : i)}-TGT-{objectiveName}";
                if (isStatic) x.Units.ForEach(u => u.Name += $"{(i == 0 ? "" : i)}-TGT-{objectiveName}");
                i++;
            });
            mission.Briefing.AddItem(DCSMissionBriefingItemType.TargetGroupName, $"-TGT-{objectiveName}");
            var length = isStatic ? targetGroupInfo.Value.DCSGroups.Count : targetGroupInfo.Value.UnitNames.Length;
            var pluralIndex = length == 1 ? 0 : 1;
            var taskString = GeneratorTools.ParseRandomString(taskDB.BriefingTask[pluralIndex].Get(), mission).Replace("\"", "''");
            CreateTaskString(mission, pluralIndex, ref taskString, objectiveName, objectiveTargetUnitFamily);
            CreateLua(mission, template, targetDB, taskDB, objectiveIndex, objectiveName, targetGroupInfo, taskString);

            // Add briefing remarks for this objective task
            var remarksString = taskDB.BriefingRemarks.Get();
            if (!string.IsNullOrEmpty(remarksString))
            {
                string remark = Toolbox.RandomFrom(remarksString.Split(";"));
                GeneratorTools.ReplaceKey(ref remark, "ObjectiveName", objectiveName);
                GeneratorTools.ReplaceKey(ref remark, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)objectiveTargetUnitFamily].Get().Split(",")[pluralIndex]);
                mission.Briefing.AddItem(DCSMissionBriefingItemType.Remark, remark);
            }

            // Add feature ogg files
            foreach (string oggFile in taskDB.IncludeOgg)
                mission.AddMediaFile($"l10n/DEFAULT/{oggFile}", Path.Combine(BRPaths.INCLUDE_OGG, oggFile));


            // Add objective features Lua for this objective
            mission.AppendValue("ScriptObjectivesFeatures", ""); // Just in case there's no features
            var featureList = taskDB.RequiredFeatures.Concat(featuresID).ToHashSet();
            foreach (string featureID in featureList)
                FeaturesGenerator.GenerateMissionFeature(mission, featureID, objectiveName, objectiveIndex, targetGroupInfo.Value.GroupID, targetGroupInfo.Value.Coordinates, taskDB.TargetSide, objectiveOptions.Contains(ObjectiveOption.HideTarget));

            objectiveCoordinatesList.Add(isInverseTransportWayPoint ? unitCoordinates : objectiveCoordinates);
            var waypoint = GenerateObjectiveWaypoint(task, objectiveCoordinates, objectiveName, template);
            waypoints.Add(waypoint);
            waypointList.Add(waypoint);
            mission.MapData.Add($"OBJECTIVE_AREA_{objectiveIndex}", new List<double[]> { waypoint.Coordinates.ToArray() });
            objectiveTargetUnitFamilies.Add(objectiveTargetUnitFamily);
            if (!targetGroupInfo.Value.UnitDB.IsAircraft)
                mission.MapData.Add($"UNIT-{targetGroupInfo.Value.UnitDB.Families[0]}-{taskDB.TargetSide}-{targetGroupInfo.Value.GroupID}", new List<double[]> { targetGroupInfo.Value.Coordinates.ToArray() });
        }

        private (string luaUnit, int unitCount, MinMaxI unitCountMinMax, UnitFamily objectiveTargetUnitFamily, UnitMakerGroupFlags groupFlags) GetUnitData(MissionTemplateSubTaskRecord task, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, ObjectiveOption[] objectiveOptions)
        {
            UnitMakerGroupFlags groupFlags = 0;
            if (objectiveOptions.Contains(ObjectiveOption.Invisible)) groupFlags |= UnitMakerGroupFlags.Invisible;
            if (objectiveOptions.Contains(ObjectiveOption.ShowTarget)) groupFlags = UnitMakerGroupFlags.NeverHidden;
            else if (objectiveOptions.Contains(ObjectiveOption.HideTarget)) groupFlags = UnitMakerGroupFlags.AlwaysHidden;
            if (objectiveOptions.Contains(ObjectiveOption.EmbeddedAirDefense)) groupFlags |= UnitMakerGroupFlags.EmbeddedAirDefense;
            return (targetBehaviorDB.UnitLua[(int)targetDB.DCSUnitCategory],
                targetDB.UnitCount[(int)task.TargetCount].GetValue(),
                targetDB.UnitCount[(int)task.TargetCount],
                Toolbox.RandomFrom(targetDB.UnitFamilies),
                groupFlags
            );
        }

        private Coordinates PlaceInAirbase(MissionTemplateRecord template, DBEntrySituation situationDB, DBEntryAirbase playerAirbase, Dictionary<string, object> extraSettings, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, ref string luaUnit, Coordinates objectiveCoordinates, int unitCount, UnitFamily objectiveTargetUnitFamily)
        {
            int airbaseID = 0;
            var parkingSpotIDsList = new List<int>();
            var parkingSpotCoordinatesList = new List<Coordinates>();
            var targetAirbaseOptions =
                (from DBEntryAirbase airbaseDB in situationDB.GetAirbases(template.OptionsMission.Contains("InvertCountriesCoalitions"))
                 where airbaseDB.DCSID != playerAirbase.DCSID
                 select airbaseDB).OrderBy(x => x.Coordinates.GetDistanceFrom(objectiveCoordinates));
            DBEntryAirbase targetAirbase = targetAirbaseOptions.FirstOrDefault(x => template.SpawnAnywhere ? true : x.Coalition == template.ContextPlayerCoalition.GetEnemy());

            airbaseID = targetAirbase.DCSID;

            var parkingSpots = UnitMaker.SpawnPointSelector.GetFreeParkingSpots(
                targetAirbase.DCSID,
                unitCount, objectiveTargetUnitFamily,
                targetBehaviorDB.Location == DBEntryObjectiveTargetBehaviorLocation.SpawnOnAirbaseParkingNoHardenedShelter);

            parkingSpotIDsList = parkingSpots.Select(x => x.DCSID).ToList();
            parkingSpotCoordinatesList = parkingSpots.Select(x => x.Coordinates).ToList();

            extraSettings.Add("GroupAirbaseID", airbaseID);
            extraSettings.Add("ParkingID", parkingSpotIDsList);
            extraSettings.Add("UnitCoords", parkingSpotCoordinatesList);
            return targetAirbase.Coordinates;
        }

        private Coordinates GetSpawnCoordinates(MissionTemplateRecord template, Coordinates lastCoordinates, DBEntryAirbase playerAirbase, DBEntryObjectiveTarget targetDB, bool usingHint)
        {
            Coordinates? spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                targetDB.ValidSpawnPoints,
                playerAirbase.Coordinates,
                usingHint ? Toolbox.ANY_RANGE : template.FlightPlanObjectiveDistance,
                lastCoordinates,
                usingHint ? Toolbox.HINT_RANGE : template.FlightPlanObjectiveSeparation,
                usingHint ? null : GeneratorTools.GetSpawnPointCoalition(template, Side.Enemy));

            if (!spawnPoint.HasValue)
                throw new BriefingRoomException($"Failed to spawn objective unit group. {String.Join(", ", targetDB.ValidSpawnPoints.Select(x => x.ToString()).ToList())} Please try again (Consider Adusting Flight Plan)");

            Coordinates objectiveCoordinates = spawnPoint.Value;
            return objectiveCoordinates;
        }

        private static (string[] featuresID, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, DBEntryObjectiveTask taskDB, ObjectiveOption[] objectiveOptions) GetObjectiveData(MissionTemplateObjectiveRecord objectiveTemplate)
        {
            var (targetDB, targetBehaviorDB, taskDB, objectiveOptions, presetDB) = GetCustomObjectiveData(objectiveTemplate);
            var featuresID = objectiveTemplate.HasPreset ? presetDB.Features.ToArray() : objectiveTemplate.Features.ToArray();

            ObjectiveNullCheck(targetDB, targetBehaviorDB, taskDB);
            return (featuresID, targetDB, targetBehaviorDB, taskDB, objectiveOptions);
        }

        private static (DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, DBEntryObjectiveTask taskDB, ObjectiveOption[] objectiveOptions, DBEntryObjectivePreset presetDB) GetCustomObjectiveData(MissionTemplateSubTaskRecord objectiveTemplate)
        {
            var targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            var targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior);
            var taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);
            var objectiveOptions = objectiveTemplate.Options.ToArray();
            DBEntryObjectivePreset presetDB = null;

            if (objectiveTemplate.HasPreset)
            {
                presetDB = Database.Instance.GetEntry<DBEntryObjectivePreset>(objectiveTemplate.Preset);
                if (presetDB != null)
                {
                    targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(Toolbox.RandomFrom(presetDB.Targets));
                    targetBehaviorDB = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(Toolbox.RandomFrom(presetDB.TargetsBehaviors));
                    taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(Toolbox.RandomFrom(presetDB.Tasks));
                    objectiveOptions = presetDB.Options.ToArray();
                }
            }

            ObjectiveNullCheck(targetDB, targetBehaviorDB, taskDB);
            return (targetDB, targetBehaviorDB, taskDB, objectiveOptions, presetDB);
        }

        private static void ObjectiveNullCheck(DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, DBEntryObjectiveTask taskDB)
        {
            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");
            if (targetBehaviorDB == null) throw new BriefingRoomException($"Target behavior \"{targetBehaviorDB.UIDisplayName}\" not found for objective.");
            if (taskDB == null) throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not found for objective.");
            if (!taskDB.ValidUnitCategories.Contains(targetDB.UnitCategory))
                throw new BriefingRoomException($"Task \"{taskDB.UIDisplayName}\" not valid for objective targets, which belong to category \"{targetDB.UnitCategory}\".");
        }


        private void AddEmbeddedAirDefenseUnits(MissionTemplateRecord template, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTargetBehavior targetBehaviorDB, DBEntryObjectiveTask taskDB, ObjectiveOption[] objectiveOptions, Coordinates objectiveCoordinates, UnitMakerGroupFlags groupFlags, Dictionary<string, object> extraSettings)
        {
            // Static targets (aka buildings) need to have their "embedded" air defenses spawned in another group
            string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(template, taskDB.TargetSide, UnitCategory.Static);

            if (airDefenseUnits.Length > 0)
                UnitMaker.AddUnitGroup(
                    airDefenseUnits,
                    taskDB.TargetSide, UnitFamily.VehicleAAA,
                    targetBehaviorDB.GroupLua[(int)targetDB.DCSUnitCategory], targetBehaviorDB.UnitLua[(int)targetDB.DCSUnitCategory],
                    objectiveCoordinates + Coordinates.CreateRandom(100, 500),
                    groupFlags,
                    extraSettings);
        }

        private static void CreateLua(DCSMission mission, MissionTemplateRecord template, DBEntryObjectiveTarget targetDB, DBEntryObjectiveTask taskDB, int objectiveIndex, string objectiveName, UnitMakerGroupInfo? targetGroupInfo, string taskString)
        {
            // Add Lua table for this objective
            string objectiveLua = $"briefingRoom.mission.objectives[{objectiveIndex + 1}] = {{ ";
            objectiveLua += $"complete = false, ";
            objectiveLua += $"failed = false, ";
            objectiveLua += $"groupName = \"{targetGroupInfo.Value.Name}\", ";
            objectiveLua += $"hideTargetCount = false, ";
            objectiveLua += $"name = \"{objectiveName}\", ";
            objectiveLua += $"targetCategory = Unit.Category.{targetDB.UnitCategory.ToLuaName()}, ";
            objectiveLua += $"taskType = \"{taskDB.ID}\", ";
            objectiveLua += $"task = \"{taskString}\", ";
            objectiveLua += $"unitsCount = #dcsExtensions.getUnitNamesByGroupNameSuffix(\"-TGT-{objectiveName}\"), ";
            objectiveLua += $"unitNames = dcsExtensions.getUnitNamesByGroupNameSuffix(\"-TGT-{objectiveName}\") ";
            objectiveLua += "}\n";

            // Add F10 sub-menu for this objective
            objectiveLua += $"briefingRoom.f10Menu.objectives[{objectiveIndex + 1}] = missionCommands.addSubMenuForCoalition(coalition.side.{template.ContextPlayerCoalition.ToString().ToUpper()}, \"$LANG_OBJECTIVE$ {objectiveName}\", nil)\n";
            mission.AppendValue("ScriptObjectives", objectiveLua);

            // Add objective trigger Lua for this objective
            foreach (var CompletionTriggerLua in taskDB.CompletionTriggersLua)
            {
                string triggerLua = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_LUA_OBJECTIVETRIGGERS, CompletionTriggerLua));
                GeneratorTools.ReplaceKey(ref triggerLua, "ObjectiveIndex", objectiveIndex + 1);
                mission.AppendValue("ScriptObjectivesTriggers", triggerLua);
            }
        }

        private static void CreateTaskString(DCSMission mission, int pluralIndex, ref string taskString, string objectiveName, UnitFamily objectiveTargetUnitFamily)
        {
            // Get tasking string for the briefing
            if (string.IsNullOrEmpty(taskString)) taskString = "Complete objective $OBJECTIVENAME$";
            GeneratorTools.ReplaceKey(ref taskString, "ObjectiveName", objectiveName);
            GeneratorTools.ReplaceKey(ref taskString, "UnitFamily", Database.Instance.Common.Names.UnitFamilies[(int)objectiveTargetUnitFamily].Get().Split(",")[pluralIndex]);
            mission.Briefing.AddItem(DCSMissionBriefingItemType.Task, taskString);
        }

        private Waypoint GenerateObjectiveWaypoint(MissionTemplateSubTaskRecord objectiveTemplate, Coordinates objectiveCoordinates, string objectiveName, MissionTemplateRecord template, bool scriptIgnore = false)
        {
            var targetDB = Database.Instance.GetEntry<DBEntryObjectiveTarget>(objectiveTemplate.Target);
            var targetBehaviorLocation = Database.Instance.GetEntry<DBEntryObjectiveTargetBehavior>(objectiveTemplate.TargetBehavior).Location;
            if (targetDB == null) throw new BriefingRoomException($"Target \"{targetDB.UIDisplayName}\" not found for objective.");

            Coordinates waypointCoordinates = objectiveCoordinates;
            bool onGround = !targetDB.UnitCategory.IsAircraft() || AIR_ON_GROUND_LOCATIONS.Contains(targetBehaviorLocation); // Ground targets = waypoint on the ground

            var taskDB = Database.Instance.GetEntry<DBEntryObjectiveTask>(objectiveTemplate.Task);
            if (objectiveTemplate.Options.Contains(ObjectiveOption.InaccurateWaypoint) && !taskDB.UICategory.ContainsValue("Transport"))
            {
                waypointCoordinates += Coordinates.CreateRandom(3.0, 6.0) * Toolbox.NM_TO_METERS;
                if (template.OptionsMission.Contains("MarkWaypoints"))
                    DrawingMaker.AddDrawing($"Target Zone {objectiveName}", DrawingType.Circle, waypointCoordinates, "Radius".ToKeyValuePair(6.0 * Toolbox.NM_TO_METERS));
            }
            else if (taskDB.UICategory.ContainsValue("Transport"))
                DrawingMaker.AddDrawing($"Target Zone {objectiveName}", DrawingType.Circle, waypointCoordinates, "Radius".ToKeyValuePair(500));

            return new Waypoint(objectiveName, waypointCoordinates, onGround, scriptIgnore);
        }

        //----------------SUB TASK SUPPORT FUNCTIONS-------------------------------

        private Coordinates GetNearestSpawnCoordinates(MissionTemplateRecord template, Coordinates coreCoordinates, DBEntryObjectiveTarget targetDB)
        {
            Coordinates? spawnPoint = UnitMaker.SpawnPointSelector.GetNearestSpawnPoint(
                targetDB.ValidSpawnPoints,
                coreCoordinates);

            if (!spawnPoint.HasValue)
                throw new BriefingRoomException($"Failed to spawn nearby objective point. {String.Join(",", targetDB.ValidSpawnPoints.Select(x => x.ToString()).ToList())} Please try again (Consider Adusting Flight Plan)");

            Coordinates objectiveCoordinates = spawnPoint.Value;
            return objectiveCoordinates;
        }
    }
}
