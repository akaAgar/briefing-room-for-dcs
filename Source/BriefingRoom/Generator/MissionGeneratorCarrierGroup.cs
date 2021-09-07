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
    /// <summary>
    /// Generates friendly carrier group.
    /// </summary>
    internal class MissionGeneratorCarrierGroup : IDisposable
    {
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

        /// <summary>
        /// Generates the carrier group.
        /// </summary>
        /// <param name="mission">The mission to generate.</param>
        /// <param name="template">The mission template to use.</param>
        /// <param name="landbaseCoordinates">Coordinates of the players' land base.</param>
        /// <param name="windSpeedAtSeaLevel">Wind speed at sea level, in m/s.</param>
        /// <param name="windDirectionAtSeaLevel">Wind direction at sea level, in radians.</param>
        /// <returns>A dictionary of carrier group units info, with the database ID of the ship as key.</returns>
        internal Dictionary<string, UnitMakerGroupInfo> GenerateCarrierGroup(DCSMission mission, MissionTemplate template, Coordinates landbaseCoordinates, Coordinates objectivesCenter, double windSpeedAtSeaLevel, double windDirectionAtSeaLevel)
        {
            Dictionary<string, UnitMakerGroupInfo> carrierDictionary = new Dictionary<string, UnitMakerGroupInfo>(StringComparer.InvariantCultureIgnoreCase);

            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            if (theaterDB == null) return carrierDictionary; // Theater doesn't exist. Should never happen.

            // Pick the carrier spawn point closer from the initial airbase
            Coordinates carrierGroupCoordinates = theaterDB.CarrierGroupWaypoints.OrderBy(x => x.GetDistanceFrom(landbaseCoordinates)).First();

            if (windSpeedAtSeaLevel == 0) // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
                windDirectionAtSeaLevel = Toolbox.RandomDouble(Toolbox.TWO_PI);
            Coordinates destinationPath = Coordinates.FromAngleInRadians(windDirectionAtSeaLevel + Math.PI) * Database.Instance.Common.CarrierGroup.CourseLength;

            double carrierSpeed = Math.Max(
                Database.Instance.Common.CarrierGroup.MinimumCarrierSpeed,
                Database.Instance.Common.CarrierGroup.IdealWindOfDeck - windSpeedAtSeaLevel);

            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
            {
                if (string.IsNullOrEmpty(flightGroup.Carrier)) continue; // No carrier for
                if (flightGroup.Carrier == "FOB")
                {
                    //It Carries therefore carrier not because I can't think of a name to rename this lot
                    GenerateFOB(flightGroup, carrierDictionary, mission, template, landbaseCoordinates, objectivesCenter);
                    continue;
                }
                if (carrierDictionary.ContainsKey(flightGroup.Carrier)) continue; // Carrier type already added
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Carrier);
                if ((unitDB == null) || !unitDB.Families.Any(x => x.IsCarrier())) continue; // Unit doesn't exist or is not a carrier
                Coordinates shipCoordinates = carrierGroupCoordinates + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierDictionary.Count * Database.Instance.Common.CarrierGroup.ShipSpacing;
                Coordinates shipDestination = shipCoordinates + destinationPath;

                string cvnID = carrierDictionary.Count > 0 ? (carrierDictionary.Count + 1).ToString() : "";
                int ilsChannel = 11 + carrierDictionary.Count;
                double radioFrequency = 127.5 + carrierDictionary.Count;
                string tacanCallsign = $"CVN{cvnID}";
                int tacanChannel = 74 + carrierDictionary.Count;

                UnitMakerGroupInfo? groupInfo =
                    UnitMaker.AddUnitGroup(
                        new string[] { unitDB.ID }, Side.Ally, unitDB.Families[0],
                        "GroupShipCarrier", "UnitShip",
                        shipCoordinates, DCSSkillLevel.Excellent, 0, AircraftPayload.Default,
                        "GroupX2".ToKeyValuePair(shipDestination.X),
                        "GroupY2".ToKeyValuePair(shipDestination.Y),
                        "ILS".ToKeyValuePair(ilsChannel),
                        "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                        "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(radioFrequency)),
                        "Speed".ToKeyValuePair(carrierSpeed),
                        "TACANCallsign".ToKeyValuePair(tacanCallsign),
                        "TACANChannel".ToKeyValuePair(tacanChannel),
                        "TACANFrequency".ToKeyValuePair(GeneratorTools.GetTACANFrequency(tacanChannel, 'X', false)),
                        "TACANMode".ToKeyValuePair("X"));

                if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) continue; // Couldn't generate group

                mission.Briefing.AddItem(
                    DCSMissionBriefingItemType.Airbase,
                    $"{unitDB.UIDisplayName}\t-\t{GeneratorTools.FormatRadioFrequency(radioFrequency)}\t{ilsChannel}\t{tacanCallsign}, {tacanChannel}X");

                carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value);
            }

            return carrierDictionary;
        }

        internal void GenerateFOB(MissionTemplateFlightGroup flightGroup, Dictionary<string, UnitMakerGroupInfo> carrierDictionary, DCSMission mission, MissionTemplate template, Coordinates landbaseCoordinates, Coordinates objectivesCenter)
        {
            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            if (theaterDB == null) return; // Theater doesn't exist. Should never happen.


            DBEntryTheaterSpawnPoint? spawnPoint =
                    UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        new SpawnPointType[] { SpawnPointType.LandLarge },
                        landbaseCoordinates,
                        new MinMaxD(10, 50),
                        objectivesCenter,
                        new MinMaxD(10, template.FlightPlanObjectiveDistance));

            if (!spawnPoint.HasValue)
            {
                BriefingRoom.PrintToLog($"No spawn point found for FOB air defense unit groups", LogMessageErrorLevel.Warning);
                return;
            }

            DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Carrier);
            if (unitDB == null) return; // Unit doesn't exist or is not a carrier

            double radioFrequency = 127.5 + carrierDictionary.Count;

            UnitMakerGroupInfo? groupInfo =
                UnitMaker.AddUnitGroup(
                    unitDB.Families[0], 1, Side.Ally,
                    "GroupStatic", "UnitStaticFOB",
                    spawnPoint.Value.Coordinates, DCSSkillLevel.Excellent, 0, AircraftPayload.Default,
                    "FOBCallSignIndex".ToKeyValuePair(carrierDictionary.Count + 1),
                    "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                    "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(radioFrequency)));
            if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) return; // Couldn't generate group

            var FOBNames = new string[]{
                "London",
                "Dallas",
                "Paris",
                "Moscow",
                "Berlin"
            };
           mission.Briefing.AddItem(
                    DCSMissionBriefingItemType.Airbase,
                    $"FOB {FOBNames[carrierDictionary.Count(x => x.Key == "FOB")]}\t-\t{GeneratorTools.FormatRadioFrequency(radioFrequency)}\t\t");

            carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
