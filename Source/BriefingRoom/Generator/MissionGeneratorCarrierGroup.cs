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

            Coordinates? carrierGroupCoordinates = null;
            Coordinates? destinationPath = null;

            if(theaterDB.ShapeSpawnSystem) {
                
                var iteration = 0;
                while (iteration < 5)
                {
                    carrierGroupCoordinates  = UnitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        new SpawnPointType[] {SpawnPointType.Sea},
                        landbaseCoordinates,
                        new MinMaxD(15, 300),
                        objectivesCenter,
                        new MinMaxD(15, 99999),
                        GeneratorTools.GetSpawnPointCoalition(template, Side.Ally));

                    if (windSpeedAtSeaLevel == 0) // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
                        windDirectionAtSeaLevel = Toolbox.RandomDouble(Toolbox.TWO_PI);
                    destinationPath = Coordinates.FromAngleInRadians(windDirectionAtSeaLevel + Math.PI) * Database.Instance.Common.CarrierGroup.CourseLength;

                    if(ShapeManager.IsPosValid(destinationPath.Value, theaterDB.WaterCoordinates, theaterDB.WaterExclusionCoordinates))
                        break;
                    iteration++;
                }
                if(!carrierGroupCoordinates.HasValue)
                    return carrierDictionary;
            } else {
                // Pick the carrier spawn point closer from the initial airbase
                carrierGroupCoordinates = theaterDB.CarrierGroupWaypoints.OrderBy(x => x.GetDistanceFrom(landbaseCoordinates)).First();

                if (windSpeedAtSeaLevel == 0) // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
                    windDirectionAtSeaLevel = Toolbox.RandomDouble(Toolbox.TWO_PI);
                destinationPath = Coordinates.FromAngleInRadians(windDirectionAtSeaLevel + Math.PI) * Database.Instance.Common.CarrierGroup.CourseLength;
            }

            double carrierSpeed = Math.Max(
                Database.Instance.Common.CarrierGroup.MinimumCarrierSpeed,
                Database.Instance.Common.CarrierGroup.IdealWindOfDeck - windSpeedAtSeaLevel);

            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
            {
                if (string.IsNullOrEmpty(flightGroup.Carrier)) continue; // No carrier for
                if (carrierDictionary.ContainsKey(flightGroup.Carrier)) continue; // Carrier type already added
                if (flightGroup.Carrier.StartsWith("FOB"))
                {
                    //It Carries therefore carrier not because I can't think of a name to rename this lot
                    GenerateFOB(flightGroup, carrierDictionary, mission, template, landbaseCoordinates, objectivesCenter);
                    continue;
                }
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Carrier);
                if ((unitDB == null) || !unitDB.Families.Any(x => x.IsCarrier())) continue; // Unit doesn't exist or is not a carrier
                Coordinates shipCoordinates = carrierGroupCoordinates.Value + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierDictionary.Count * Database.Instance.Common.CarrierGroup.ShipSpacing;
                Coordinates shipDestination = shipCoordinates + destinationPath.Value;

                string cvnID = carrierDictionary.Count > 0 ? (carrierDictionary.Count + 1).ToString() : "";
                int ilsChannel = 11 + carrierDictionary.Count;
                double radioFrequency = 127.5 + carrierDictionary.Count;
                string tacanCallsign = $"CVN{cvnID}";
                int tacanChannel = 74 + carrierDictionary.Count;

                UnitMakerGroupInfo? groupInfo =
                    UnitMaker.AddUnitGroup(
                        new string[] { unitDB.ID }, Side.Ally, unitDB.Families[0],
                        "GroupShipCarrier", "UnitShip",
                        shipCoordinates, DCSSkillLevel.Excellent, 0, "default",
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


            Coordinates? spawnPoint =
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
            var FOBNames = new List<string>{
                "FOB_London",
                "FOB_Dallas",
                "FOB_Paris",
                "FOB_Moscow",
                "FOB_Berlin"
            };

            UnitMakerGroupInfo? groupInfo =
                UnitMaker.AddUnitGroup(
                    unitDB.Families[0], 1, Side.Ally,
                    "GroupStatic", "UnitStaticFOB",
                    spawnPoint.Value, DCSSkillLevel.Excellent, 0, "Default",
                    "FOBCallSignIndex".ToKeyValuePair(FOBNames.IndexOf(flightGroup.Carrier) + 1),
                    "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                    "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(radioFrequency)));
            if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) return; // Couldn't generate group
       
           mission.Briefing.AddItem(
                    DCSMissionBriefingItemType.Airbase,
                    $"{unitDB.UIDisplayName}\t-\t{GeneratorTools.FormatRadioFrequency(radioFrequency)}\t\t");
            carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value); // This bit limits FOBS to one per game think about how we can fix this
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
