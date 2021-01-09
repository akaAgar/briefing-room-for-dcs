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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="template"></param>
        /// <param name="playerCoalitionDB"></param>
        /// <param name="windDirection0">Wind direction at altitude 0, in degrees. Used by carrier groups to make sure carriers sail into the wind.</param>
        /// <returns></returns>
        public DBEntryUnit GenerateCarrier(DCSMission mission, MissionTemplate template, DBEntryCoalition playerCoalitionDB, int windDirection0)
        {
            if (string.IsNullOrEmpty(template.PlayerCarrier))
            {
                return null;
            }

            DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        // If spawn point types are specified, use them. Else look for spawn points of any type
                        new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.Sea },
                        // Select spawn points at a proper distance from last location (previous objective or home airbase)
                        mission.InitialPosition, new MinMaxD(10, 200),
                        // Make sure no objective is too close to the initial location
                        null, null,
                        GeneratorTools.GetAllySpawnPointCoalition(template));

            if (!spawnPoint.HasValue)
                throw new Exception($"Failed to find a spawn point for Carrier");

            Coordinates position = mission.InitialPosition;
            DCSMissionUnitGroup group;
            group = UnitMaker.AddUnitGroup(
                mission, new string[] { template.PlayerCarrier },
                Side.Ally,
                spawnPoint.Value.Coordinates,
                "GroupCarrier", "UnitShip",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel));

            UnitFamily[] ships = new UnitFamily[]{
                UnitFamily.ShipFrigate,
                UnitFamily.ShipFrigate,
                UnitFamily.ShipCruiser,
                UnitFamily.ShipCruiser,
                UnitFamily.ShipTransport
            };
            foreach (var ship in ships)
            {
                spawnPoint = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                    new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.Sea },
                    spawnPoint.Value.Coordinates, new MinMaxD(1, 5),
                    // Make sure no objective is too close to the initial location
                    null, null,
                    GeneratorTools.GetAllySpawnPointCoalition(template));

                UnitMaker.AddUnitGroup(
                    mission, playerCoalitionDB.GetRandomUnits(ship, mission.DateTime.Decade, 1),
                    Side.Ally,
                    spawnPoint.Value.Coordinates,
                    "GroupShip", "UnitShip",
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel));
            }

            if (group == null)
                DebugLog.Instance.WriteLine($"Failed to create AI Carrier with ship of type \"{template.PlayerCarrier}\".", 1, DebugLogMessageErrorLevel.Warning);
            else
                group.Units[0].Heading = Toolbox.ClampAngle((windDirection0 + 180) * Toolbox.DEGREES_TO_RADIANS);

            mission.Carrier = group.Units[0];
            return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>()
                    where unit.ID == template.PlayerCarrier
                    select unit).ToArray()[0];
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
