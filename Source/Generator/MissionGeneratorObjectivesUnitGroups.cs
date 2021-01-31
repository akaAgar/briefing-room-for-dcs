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
    /// Generates unit groups associated with the mission objectives.
    /// </summary>
    public class MissionGeneratorObjectivesUnitGroups : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorObjectivesUnitGroups(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Decide what should be spawned.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="coalitionsDB">Coalitions database entries</param>
        public void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryCoalition[] coalitionsDB)
        {
            Coalition coalition =
                            objectiveDB.UnitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly) ?
                            mission.CoalitionPlayer : mission.CoalitionEnemy;
            Side side = objectiveDB.UnitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly) ? Side.Ally : Side.Enemy;
            SpawnUnitGroups(mission, template, objectiveDB.UnitGroup, coalitionsDB, side, coalition);
            if (objectiveDB.AllyUnitGroup.Category != null)
            {
                DebugLog.Instance.WriteLine($"Generating Friendly units for objective");
                SpawnUnitGroups(mission, template, objectiveDB.AllyUnitGroup, coalitionsDB, Side.Ally, mission.CoalitionPlayer);
            }
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="coalitionsDB">Coalitions database entries</param>
        /// <param name="moving">Will the group be moving</param>
        /// <param name="objectiveGroup">If the group should be tracked as an objective</param>
        public void SpawnUnitGroups(DCSMission mission, MissionTemplate template, DBUnitGroup unitGroup, DBEntryCoalition[] coalitionsDB, Side side, Coalition coalition)
        {
            DCSMissionUnitGroupFlags flags =
                GeneratorTools.ShouldUnitBeHidden(unitGroup, !template.OptionsPreferences.Contains(MissionTemplatePreferences.HideEnemyUnits)) ?
                DCSMissionUnitGroupFlags.Hidden : 0;

            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                // This objective requires no unit group generation
                if (!mission.Objectives[i].TargetFamily.HasValue)
                    continue;

                string[] units =
                    coalitionsDB[(int)coalition].GetRandomUnits(
                        mission.Objectives[i].TargetFamily.Value, mission.DateTime.Decade,
                        unitGroup.Count.GetValue(), template.OptionsUnitMods);

                // Pick the skill level once for each objective so not all target groups have the same skill level when a "random" skill level is chosen.
                DCSSkillLevel skillLevel =
                    Toolbox.IsUnitFamilyAircraft(mission.Objectives[i].TargetFamily.Value) ?
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.OppositionSkillLevelAir) :
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.OppositionSkillLevelGround);
                DCSMissionUnitGroup group;
                DBEntryTheaterSpawnPoint? spawnPoint = null;
                if (unitGroup.SpawnPoints[0] != TheaterLocationSpawnPointType.Airbase)
                {
                    if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.DestinationObjective))
                    {
                        spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                            unitGroup.SpawnPoints,
                            mission.Objectives[i].Coordinates,
                            unitGroup.DistanceFromPoint);
                        if (!spawnPoint.HasValue)
                            throw new Exception($"Failed to find spawn point for moving objective unit");
                    }
                }

                if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.EmbeddedAirDefense)) // Add "embedded" close range surface-to-air defense
                {
                    if (Toolbox.GetUnitCategoryFromUnitFamily(mission.Objectives[i].TargetFamily.Value) == UnitCategory.Vehicle) // Objectives are ground vehicles, insert air defense units in the group itself
                        units = GeneratorTools.AddEmbeddedAirDefense(units, template.OppositionAirDefense, coalitionsDB[(int)coalition], mission.DateTime.Decade, template.OptionsUnitMods);
                    else // Objectives are not ground vehicles, create another group nearby
                    {
                        // TODO: make sure the group is not spawn in water
                        string[] airDefenseGroupUnits = new string[0];
                        for (int j = 0; j < 2; j++)
                            airDefenseGroupUnits = GeneratorTools.AddEmbeddedAirDefense(airDefenseGroupUnits, template.OppositionAirDefense, coalitionsDB[(int)coalition], mission.DateTime.Decade, template.OptionsUnitMods);

                        UnitMaker.AddUnitGroup(
                            mission, airDefenseGroupUnits,
                            side,
                            (spawnPoint != null ? spawnPoint.Value.Coordinates : mission.Objectives[i].Coordinates) + Coordinates.CreateRandom(0.5, 1.5) * Toolbox.NM_TO_METERS,
                            "GroupVehicle", "UnitVehicle",
                            skillLevel, flags);
                    }
                }

                group = UnitMaker.AddUnitGroup(
                    mission, units,
                    side,
                    spawnPoint != null? spawnPoint.Value.Coordinates : mission.Objectives[i].Coordinates,
                    Toolbox.RandomFrom(unitGroup.LuaGroup), unitGroup.LuaUnit,
                    skillLevel, flags, coordinates2: getDestination(unitGroup, mission, i),
                    airbaseID: mission.Objectives[i].AirbaseID,
                    requiresParkingSpots: mission.Objectives[i].AirbaseID > 0,
                    requiresOpenAirParking: unitGroup.Flags.HasFlag(DBUnitGroupFlags.AvoidHardenedBunkers)
                    );

                // Something went wrong, abort mission generation, objective unit groups are required for the mission to work properly.
                if (group == null)
                    throw new Exception($"Failed to create objective unit group for objective #{i + 1} made of the following units: {string.Join(", ", units)}");

                // Add aircraft group to the queue of aircraft groups to be spawned
                if ((group.Category == UnitCategory.Helicopter) || (group.Category == UnitCategory.Plane) || unitGroup.Flags.HasFlag(DBUnitGroupFlags.DelaySpawn))
                    mission.AircraftSpawnQueue.Add(new DCSMissionAircraftSpawnQueueItem(group.GroupID, true));

                if (!unitGroup.Flags.HasFlag(DBUnitGroupFlags.NotObjectiveTarget))
                {
                    if(mission.ObjectiveIsStatic) {
                        mission.CoreLuaScript += $"briefingRoom.mission.objectives[{i + 1}].groupID = {group.Units[0].ID}\r\n";
                    } else
                        // Add the ID of the unit group associated with this objective to the Lua script
                        mission.CoreLuaScript += $"briefingRoom.mission.objectives[{i + 1}].groupID = {group.GroupID}\r\n";
                }
            }
        }

        private Coordinates? getDestination(DBUnitGroup unitGroup, DCSMission mission, int i){
            if(unitGroup.Flags.HasFlag(DBUnitGroupFlags.DestinationObjective))
                return mission.Objectives[i].Coordinates;
            else if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.DestinationPlayerBase))
                return mission.InitialPosition;
            return null;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
