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

using BriefingRoom.DB;
using BriefingRoom.Debug;
using BriefingRoom.Mission;
using BriefingRoom.Template;
using System;

namespace BriefingRoom.Generator
{
    /// <summary>
    /// Generates friendly support units (AWACS, tankers...)
    /// </summary>
    public class MissionGeneratorEnemySupportUnits : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorEnemySupportUnits(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="enemyCoalitionDB">Ally coalition database entry</param>
        /// <param name="unitMods">Unit mods selected units can belong to</param>
        public void CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryCoalition enemyCoalitionDB, string[] unitMods)
        {
            if(!template.SituationEnemySupportAircraft.RollChance())
                return;
            AddSupportUnit(mission, template, enemyCoalitionDB, UnitFamily.PlaneTankerBasket, unitMods, new Tacan(39, "ETR", AA: true));
            AddSupportUnit(mission, template, enemyCoalitionDB, UnitFamily.PlaneTankerBoom, unitMods, new Tacan(82, "EKR", AA: true));
            AddSupportUnit(mission, template, enemyCoalitionDB, UnitFamily.PlaneAWACS, unitMods); // AWACS must be added last, so it its inserted first into the spawning queue
        }

        /// <summary>
        /// Spawn a group of support units.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="enemyCoalitionDB">Ally coalition database entry</param>
        /// <param name="unitFamily">Family of support unit to spawn</param>
        /// <param name="unitMods">Unit mods selected units can belong to</param>
        /// <param name="TACAN">TACAN info for the unit, if any</param>
        private void AddSupportUnit(DCSMission mission, MissionTemplate template, DBEntryCoalition enemyCoalitionDB, UnitFamily unitFamily, string[] unitMods, Tacan TACAN = null)
        {
            DebugLog.Instance.WriteLine($"Adding {unitFamily} enemy support unit...", 1);

            string[] validUnitTypes = enemyCoalitionDB.GetRandomUnits(unitFamily, mission.DateTime.Decade, 1, unitMods, false);

            if (validUnitTypes.Length == 0)
            {
                DebugLog.Instance.WriteLine($"No support unit found for this role in coalition \"{enemyCoalitionDB.ID}\"", 2);
                return; // Empty FG info will automatically be discarded
            }

            string groupLua;

            switch (unitFamily)
            {
                case UnitFamily.PlaneAWACS:
                    groupLua = "GroupAircraftAWACSMortal";
                    break;
                case UnitFamily.PlaneTankerBasket:
                case UnitFamily.PlaneTankerBoom:
                    groupLua = "GroupAircraftTankerMortal";
                    break;
                default: // Should never happen
                    return; // Empty FG info will automatically be discarded
            }

            DBEntryTheaterSpawnPoint? spawnPoint =
                UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                    null,
                    mission.ObjectivesCenter, Database.Instance.Common.EnemyCAPDistanceFromObjectives,
                    mission.InitialPosition, new MinMaxD(Database.Instance.Common.EnemyCAPMinDistanceFromTakeOffLocation, 99999),
                    GeneratorTools.GetEnemySpawnPointCoalition(template));

            if (!spawnPoint.HasValue) // No spawn point found, stop here.
            {
                DebugLog.Instance.WriteLine("No spawn point found for enemy fighter patrol group.", 1, DebugLogMessageErrorLevel.Warning);
                return;
            }

            Coordinates location2 = spawnPoint.Value.Coordinates + Coordinates.CreateRandom(12, 20) * Toolbox.NM_TO_METERS;

            string unitType = Toolbox.RandomFrom(validUnitTypes);

            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                mission, new string[] { unitType },
                Side.Enemy, spawnPoint.Value.Coordinates,
                groupLua, "UnitAircraft",
                 Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationEnemySkillLevelAir), 0,
                UnitTaskPayload.Default,
                location2);
            if (group == null)
                return; // Empty FG info will automatically be discarded

            group.TACAN = TACAN;
            mission.AircraftSpawnQueue.Insert(0, new DCSMissionAircraftSpawnQueueItem(group.GroupID, true)); // Support aircraft must be activated first
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
