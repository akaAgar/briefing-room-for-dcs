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
        public void GenerateCarriers(DCSMission mission, MissionTemplate template, DBEntryCoalition playerCoalitionDB)
        {
             var carriers = new string[]{};
             if(template.MissionType == MissionType.SinglePlayer){
                if (string.IsNullOrEmpty(template.PlayerFlightGroups[0].Carrier))
                    return;
                carriers = carriers.Append(template.PlayerFlightGroups[0].Carrier).ToArray();
             }
             else {
                  carriers = template.PlayerFlightGroups.Aggregate(new string[]{},(acc, x) => !string.IsNullOrEmpty(x.Carrier)? acc.Append(x.Carrier).ToArray() : acc);
             }

            if (carriers.Length == 0)
                return;

            foreach (string carrier in carriers.Distinct())
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
            DebugLog.Instance.WriteLine($"Ships to be spawned {ships.Aggregate((acc, x) => $"{acc}, {x}")}", 1, DebugLogMessageErrorLevel.Warning);
            group = UnitMaker.AddUnitGroup(
                mission, ships,
                Side.Ally,
                spawnPoint.Value.Coordinates,
                "GroupCarrier", "UnitShip",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationFriendlyAISkillLevel));

            if (group == null)
                DebugLog.Instance.WriteLine($"Failed to create AI Carrier with ship of type \"{carrier}\".", 1, DebugLogMessageErrorLevel.Warning);
            else {
                //set all units against the wind
                int windDirection = mission.Weather.WindDirection[0];
                double WindSpeed = mission.Weather.WindSpeed[0];
                double windOverDeck = 12.8611; // 25kts
                group.Speed = windOverDeck - WindSpeed;
                if(group.Speed <= 2.6) {
                    group.Speed = 2.57222; //5kts
                }
                double heading = Toolbox.ClampAngle((windDirection + 180) * Toolbox.DEGREES_TO_RADIANS); 
                foreach (DCSMissionUnitGroupUnit unit in group.Units)
                {
                    unit.Heading = heading;
                }
                group.Units[0].Coordinates = group.Coordinates;
                group.Coordinates2 = Coordinates.FromAngleAndDistance(group.Coordinates, (group.Speed * Toolbox.METERS_PER_SECOND_TO_KNOTS) * Toolbox.NM_TO_METERS, heading);
            }
                string cvnId = mission.Carriers.Length > 0? (mission.Carriers.Length + 1).ToString() : "";
                group.TACAN = new Tacan(74+ mission.Carriers.Length, $"CVN{cvnId}");
                group.ILS = 11 + mission.Carriers.Length;
                group.RadioFrequency = 127.5f + mission.Carriers.Length;
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
