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
        /// Main unit generation method.
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

            DCSMissionUnitGroupFlags flags =
                GeneratorTools.ShouldUnitBeHidden(objectiveDB.UnitGroup, template.OptionsShowEnemyUnits) ?
                DCSMissionUnitGroupFlags.Hidden : 0;

            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                // This objective requires no unit group generation
                if (!mission.Objectives[i].TargetFamily.HasValue)
                    continue;

                string[] units =
                    coalitionsDB[(int)coalition].GetRandomUnits(
                        mission.Objectives[i].TargetFamily.Value, objectiveDB.UnitGroup.Count.GetValue());

                if (objectiveDB.UnitGroup.Flags.HasFlag(DBUnitGroupFlags.EmbeddedAirDefense) &&
                    !objectiveDB.UnitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly) &&
                    (Toolbox.GetUnitCategoryFromUnitFamily(mission.Objectives[i].TargetFamily.Value) == UnitCategory.Vehicle))
                    units = GeneratorTools.AddEmbeddedAirDefense(units, template.OppositionAirDefense, coalitionsDB[(int)coalition]);

                // Pick the skill level once for each objective so not all target groups have the same skill level when a
                // "random" skill level is chosen.
                DCSSkillLevel skillLevel =
                    Toolbox.IsUnitFamilyAircraft(mission.Objectives[i].TargetFamily.Value) ?
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.OppositionSkillLevelAir) :
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.OppositionSkillLevelGround);

                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission, units,
                    objectiveDB.UnitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly) ? Side.Ally : Side.Enemy,
                    mission.Objectives[i].Coordinates,
                    objectiveDB.UnitGroup.LuaGroup, objectiveDB.UnitGroup.LuaUnit,
                    skillLevel, flags);

                // Something went wrong, abort mission generation, objective unit groups are required for the mission to work properly.
                if (group == null)
                    throw new Exception($"Failed to create objective unit group for objective #{i + 1} made of the following units: {string.Join(", ", units)}");

                // Add aircraft group to the queue of aircraft groups to be spawned
                if ((group.Category == UnitCategory.Helicopter) || (group.Category == UnitCategory.Plane))
                    mission.AircraftSpawnQueue.Add(new DCSMissionAircraftSpawnQueueItem(group.GroupID, true));

                // Add the ID of the unit group associated with this objective to the Lua script
                mission.CoreLuaScript += $"briefingRoom.mission.objectives[{i + 1}].groupID = {group.GroupID}\r\n";
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
