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
    /// Generates friendly support units (AWACS, tankers...)
    /// </summary>
    public class MissionGeneratorCarrier : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorCarrier(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        public void GenerateCarrier(DCSMission mission, MissionTemplate template, DBEntryCoalition playerCoalitionDB)
        {

            string[] ship;

            ship = playerCoalitionDB.GetRandomUnits(UnitFamily.ShipCarrier, 1);
            DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        // If spawn point types are specified, use them. Else look for spawn points of any type
                        new TheaterLocationSpawnPointType[]{TheaterLocationSpawnPointType.Sea},
                        // Select spawn points at a proper distance from last location (previous objective or home airbase)
                        mission.InitialPosition, new MinMaxD(20, 99999999),
                        // Make sure no objective is too close to the initial location
                        mission.InitialPosition, new MinMaxD(20, 99999999),
                        GeneratorTools.GetEnemySpawnPointCoalition(template));
            
            if (!spawnPoint.HasValue)
                    throw new Exception($"Failed to find a spawn point for Carrier");

            Coordinates position = mission.InitialPosition;
            DCSMissionUnitGroup group;
            group = UnitMaker.AddUnitGroup(
                mission, ship,
                Side.Ally,
                spawnPoint.Value.Coordinates,
                "GroupShip", "UnitShip",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel));
            

            if (group == null)
            {
                DebugLog.Instance.WriteLine($"Failed to create AI Carrier with ship of type \"{ship[0]}\".", 1, DebugLogMessageErrorLevel.Warning);
            }


            DebugLog.Instance.WriteLine($"I think I spawned a ship {group.UnitID} @ {group.Coordinates}");
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
