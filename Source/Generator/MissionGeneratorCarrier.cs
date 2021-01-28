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
        public void GenerateCarriers(DCSMission mission, MissionTemplate template, DBEntryCoalition playerCoalitionDB, int windDirection0)
        {
            if (template.TheaterCarriers.Length == 0)
            {
                return;
            }

            foreach (string carrier in template.TheaterCarriers)
            {

            DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        // If spawn point types are specified, use them. Else look for spawn points of any type
                        new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.Sea },
                        // Select spawn points at a proper distance from last location (previous objective or home airbase)
                        mission.InitialPosition, new MinMaxD(10, 50),
                        // Make sure no objective is too close to the initial location
                        null, null,
                        GeneratorTools.GetAllySpawnPointCoalition(template));

            if (!spawnPoint.HasValue)
                throw new Exception($"Failed to find a spawn point for Carrier");

            Coordinates position = mission.InitialPosition;
            DCSMissionUnitGroup group;
            string[] ships = new string[] { carrier };
            foreach (var ship in new UnitFamily[]{
                UnitFamily.ShipFrigate,
                UnitFamily.ShipFrigate,
                UnitFamily.ShipCruiser,
                UnitFamily.ShipCruiser,
                UnitFamily.ShipTransport
            })
            {
                ships = ships.Append(playerCoalitionDB.GetRandomUnits(ship, mission.DateTime.Decade, 1, template.OptionsUnitMods)[0]).ToArray();
            }
            DebugLog.Instance.WriteLine($"Ships to be spawned {ships.Aggregate((acc, x) => $"{acc}, {x}")}", 1, DebugLogMessageErrorLevel.Warning);
            group = UnitMaker.AddUnitGroup(
                mission, ships,
                Side.Ally,
                spawnPoint.Value.Coordinates,
                "GroupCarrier", "UnitShip",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel));

            if (group == null)
                DebugLog.Instance.WriteLine($"Failed to create AI Carrier with ship of type \"{template.TheaterCarriers}\".", 1, DebugLogMessageErrorLevel.Warning);
            else {
                //set all units against the wind
                double heading = Toolbox.ClampAngle((windDirection0 + 180) * Toolbox.DEGREES_TO_RADIANS); 
                foreach (DCSMissionUnitGroupUnit unit in group.Units)
                {
                    unit.Heading = heading;
                }
            }
                string cvnId = mission.Carriers.Length > 0? (mission.Carriers.Length + 1).ToString() : "";
                group.TACAN = new Tacan(74+ mission.Carriers.Length, $"CVN{cvnId}");
                group.ILS = 11 + mission.Carriers.Length;

                mission.Carriers = mission.Carriers.Append(group).ToArray();
            }
            return;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
