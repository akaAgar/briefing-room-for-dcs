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
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates friendly carrier group.
    /// </summary>
    internal class MissionGeneratorCarrierGroup : IDisposable
    {
        private static readonly UnitFamily[] ESCORT_UNIT_FAMILIES = new UnitFamily[] { UnitFamily.ShipFrigate, UnitFamily.ShipFrigate, UnitFamily.ShipCruiser, UnitFamily.ShipCruiser, UnitFamily.ShipTransport };

        /// <summary>
        /// Carrier course length (in nautical miles)
        /// </summary>
        private const double CARRIER_COURSE_LENGTH = 50.0;

        /// <summary>
        /// Carrier speed (in knots)
        /// </summary>
        private const double CARRIER_SPEED = 12.0;

        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        internal MissionGeneratorCarrierGroup(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal void GenerateCarrierGroup(DCSMission mission, MissionTemplate template, Coordinates initialCoordinates, double? windDirectionAtSeaLevel)
        {
            Dictionary<string, int> carrierUnitGroups = new Dictionary<string, int>();

            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            if (theaterDB == null) return; // Theater doesn't exist. Should never happen.

            // Pick the carrier spawn point closer from the initial airbase
            Coordinates carrierGroupCoordinates = theaterDB.CarrierGroupWaypoints.OrderBy(x => x.GetDistanceFrom(initialCoordinates)).First();

            windDirectionAtSeaLevel = windDirectionAtSeaLevel ?? Toolbox.RandomDouble(Toolbox.TWO_PI); // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
            Coordinates destinationPath = Coordinates.FromAngleInRadians(windDirectionAtSeaLevel.Value + Math.PI) * CARRIER_COURSE_LENGTH * Toolbox.NM_TO_METERS;

            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
            {
                if (string.IsNullOrEmpty(flightGroup.Carrier)) continue; // No carrier for
                string carrierID = flightGroup.Carrier.ToLowerInvariant();
                if (carrierUnitGroups.ContainsKey(carrierID)) continue; // Carrier type already added
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(carrierID);
                if ((unitDB == null) || !unitDB.Families[0].IsCarrier()) continue; // Unit doesn't exist or is not a carrier

                Coordinates shipCoordinates = carrierGroupCoordinates + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierUnitGroups.Count * Toolbox.NM_TO_METERS;
                Coordinates shipDestination = shipCoordinates + destinationPath;

                string cvnID = carrierUnitGroups.Count > 0 ? (carrierUnitGroups.Count + 1).ToString() : "";
                UnitMakerGroupFlags unitMakerGroupFlags = 0;
                if (template.MissionType == MissionType.SinglePlayer)
                    unitMakerGroupFlags = UnitMakerGroupFlags.FirstUnitIsPlayer;

                UnitMakerGroupInfo? groupInfo =
                    UnitMaker.AddUnitGroup(
                        new string[] { unitDB.DCSIDs[0] }, Side.Ally, unitDB.Families[0],
                        "GroupShipCarrier", "UnitShip",
                        shipCoordinates, DCSSkillLevel.Excellent, unitMakerGroupFlags, AircraftPayload.Default,
                        "GroupX2".ToKeyValuePair(shipDestination.X),
                        "GroupY2".ToKeyValuePair(shipDestination.Y),
                        "ILS".ToKeyValuePair(11 + carrierUnitGroups.Count),
                        "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                        "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(127.5 + carrierUnitGroups.Count)),
                        "Speed".ToKeyValuePair(CARRIER_SPEED * Toolbox.KNOTS_TO_METERS_PER_SECOND),
                        "TACANCallsign".ToKeyValuePair($"CVN{cvnID}"),
                        "TACANChannel".ToKeyValuePair(74 + carrierUnitGroups.Count),
                        "TACANFrequency".ToKeyValuePair(GeneratorTools.GetTACANFrequency(74 + carrierUnitGroups.Count, 'X', false)),
                        "TACANMode".ToKeyValuePair("X"));

                if (!groupInfo.HasValue) continue; // Couldn't generate group

                carrierUnitGroups.Add(carrierID, groupInfo.Value.GroupID);
            }

            if (carrierUnitGroups.Count > 0) // Add escorts, if there's a carrier group
            {
                // Randomize escort unit families order so they don't always appear in the same order
                UnitFamily[] escortUnitFamilies = ESCORT_UNIT_FAMILIES.OrderBy(x => Toolbox.RandomInt()).ToArray();

                foreach (UnitFamily escortUnitFamily in escortUnitFamilies)
                {
                    Coordinates shipCoordinates = carrierGroupCoordinates + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierUnitGroups.Count * Toolbox.NM_TO_METERS;
                    Coordinates shipDestination = shipCoordinates + destinationPath;

                    UnitMakerGroupInfo? groupInfo =
                        UnitMaker.AddUnitGroup(
                            escortUnitFamily, 1, Side.Ally,
                            "GroupShipMoving", "UnitShip",
                            shipCoordinates, DCSSkillLevel.Excellent,
                            0, AircraftPayload.Default,
                            "GroupX2".ToKeyValuePair(shipDestination.X),
                            "GroupY2".ToKeyValuePair(shipDestination.Y),
                            "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                            "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(127.5 + carrierUnitGroups.Count))
                            );

                    if (!groupInfo.HasValue) continue; // Couldn't generate group

                    carrierUnitGroups.Add($"*ESCORT{carrierUnitGroups.Count}", groupInfo.Value.GroupID);
                }
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="mission"></param>
        ///// <param name="template"></param>
        ///// <param name="playerCoalitionDB"></param>
        ///// <param name="windDirection0">Wind direction at altitude 0, in degrees. Used by carrier groups to make sure carriers sail into the wind.</param>
        ///// <returns></returns>
        //internal void GenerateCarrierGroups(DCSMission mission, MissionTemplate template, DBEntryCoalition playerCoalitionDB)
        //{
            

        //    //var carriers = new string[] { };
        //    //if (template.MissionType == MissionType.SinglePlayer)
        //    //{
        //    //    if (string.IsNullOrEmpty(template.PlayerFlightGroups[0].Carrier))
        //    //        return;
        //    //    carriers = carriers.Append(template.PlayerFlightGroups[0].Carrier).ToArray();
        //    //}
        //    //else
        //    //{
        //    //    carriers = template.PlayerFlightGroups.Aggregate(new string[] { }, (acc, x) => !string.IsNullOrEmpty(x.Carrier) ? acc.Append(x.Carrier).ToArray() : acc);
        //    //}

        //    //if (carriers.Length == 0)
        //    //    return;

        //    //foreach (string carrier in carriers)
        //    //{

        //    //    DBEntryTheaterSpawnPoint? spawnPoint =
        //    //            UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
        //    //                // If spawn point types are specified, use them. Else look for spawn points of any type
        //    //                new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.Sea },
        //    //                // Select spawn points at a proper distance from last location (previous objective or home airbase)
        //    //                mission.InitialPosition, new MinMaxD(10, 50),
        //    //                // Make sure no objective is too close to the initial location
        //    //                null, null,
        //    //                GeneratorTools.GetAllySpawnPointCoalition(template));

        //    //    if (!spawnPoint.HasValue)
        //    //        throw new Exception($"Failed to find a spawn point for Carrier");

        //    //    Coordinates position = mission.InitialPosition;
        //    //    DCSMissionUnitGroup group;
        //    //    string[] ships = new string[] { carrier };
        //    //    foreach (var ship in new UnitFamily[]{
        //    //    UnitFamily.ShipFrigate,
        //    //    UnitFamily.ShipFrigate,
        //    //    UnitFamily.ShipCruiser,
        //    //    UnitFamily.ShipCruiser,
        //    //    UnitFamily.ShipTransport
        //    //})
        //    //    {
        //    //        ships = ships.Append(playerCoalitionDB.GetRandomUnits(ship, mission.DateTime.Decade, 1, template.UnitMods)[0]).ToArray();
        //    //    }
        //    //    BriefingRoom.PrintToLog($"Ships to be spawned {ships.Aggregate((acc, x) => $"{acc}, {x}")}", 1, DebugLogMessageErrorLevel.Warning);
        //    //    group = UnitMaker.AddUnitGroup(
        //    //        mission, ships,
        //    //        Side.Ally,
        //    //        spawnPoint.Value.Coordinates,
        //    //        "GroupCarrier", "UnitShip",
        //    //        Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationFriendlyAISkillLevel));

        //    //    if (group == null)
        //    //        BriefingRoom.PrintToLog($"Failed to create AI Carrier with ship of type \"{carrier}\".", 1, DebugLogMessageErrorLevel.Warning);
        //    //    else
        //    //    {
        //    //        //set all units against the wind
        //    //        int windDirection = mission.Weather.WindDirection[0];
        //    //        double WindSpeed = mission.Weather.WindSpeed[0];
        //    //        double windOverDeck = 12.8611; // 25kts
        //    //        group.Speed = windOverDeck - WindSpeed;
        //    //        if (group.Speed <= 2.6)
        //    //        {
        //    //            group.Speed = 2.57222; //5kts
        //    //        }
        //    //        double heading = Toolbox.ClampAngle((windDirection + 180) * Toolbox.DEGREES_TO_RADIANS);
        //    //        foreach (DCSMissionUnitGroupUnit unit in group.Units)
        //    //        {
        //    //            unit.Heading = heading;
        //    //        }
        //    //        group.Units[0].Coordinates = group.Coordinates;
        //    //        group.Coordinates2 = Coordinates.FromAngleAndDistance(group.Coordinates, (group.Speed * Toolbox.METERS_PER_SECOND_TO_KNOTS) * Toolbox.NM_TO_METERS, heading);
        //    //    }
        //    //    string cvnId = mission.Carriers.Length > 0 ? (mission.Carriers.Length + 1).ToString() : "";
        //    //    group.TACAN = new Tacan(74 + mission.Carriers.Length, $"CVN{cvnId}");
        //    //    group.ILS = 11 + mission.Carriers.Length;
        //    //    group.RadioFrequency = 127.5f + mission.Carriers.Length;
        //    //    mission.Carriers = mission.Carriers.Append(group).ToArray();
        //    //}
        //    //return;
        //}

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
