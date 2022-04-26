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
    internal class MissionGeneratorCarrierGroup
    {


        internal static Dictionary<string, UnitMakerGroupInfo> GenerateCarrierGroup(
            UnitMaker unitMaker, ZoneMaker zoneMaker, DCSMission mission, MissionTemplateRecord template,
            Coordinates landbaseCoordinates, Coordinates objectivesCenter, double windSpeedAtSeaLevel,
            double windDirectionAtSeaLevel)
        {
            Dictionary<string, UnitMakerGroupInfo> carrierDictionary = new Dictionary<string, UnitMakerGroupInfo>(StringComparer.InvariantCultureIgnoreCase);

            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            double carrierSpeed = Math.Max(
                    Database.Instance.Common.CarrierGroup.MinimumCarrierSpeed,
                    Database.Instance.Common.CarrierGroup.IdealWindOfDeck - windSpeedAtSeaLevel);
            if (windSpeedAtSeaLevel == 0) // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
                        windDirectionAtSeaLevel = Toolbox.RandomDouble(Toolbox.TWO_PI);
            var carrierPathDeg = ((windDirectionAtSeaLevel + Math.PI) % Toolbox.TWO_PI) * Toolbox.RADIANS_TO_DEGREES;
            var usedCoordinates = new List<Coordinates>();
            foreach (MissionTemplateFlightGroupRecord flightGroup in template.PlayerFlightGroups)
            {
                if (string.IsNullOrEmpty(flightGroup.Carrier)) continue; // No carrier for
                if (carrierDictionary.ContainsKey(flightGroup.Carrier)) continue; // Carrier type already added
                if (flightGroup.Carrier.StartsWith("FOB"))
                {
                    //It Carries therefore carrier not because I can't think of a name to rename this lot
                    GenerateFOB(unitMaker, zoneMaker, flightGroup, carrierDictionary, mission, template, landbaseCoordinates, objectivesCenter);
                    continue;
                }
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Carrier);
                if ((unitDB == null) || !unitDB.Families.Any(x => x.IsCarrier())) continue; // Unit doesn't exist or is not a carrier

                var (shipCoordinates, shipDestination) = GetSpawnAndDestination(unitMaker, template, theaterDB, usedCoordinates, landbaseCoordinates, objectivesCenter, carrierPathDeg);
                usedCoordinates.Add(shipCoordinates);
                string cvnID = carrierDictionary.Count > 0 ? (carrierDictionary.Count + 1).ToString() : "";
                int ilsChannel = 11 + carrierDictionary.Count;
                double radioFrequency = 127.5 + carrierDictionary.Count;
                string tacanCallsign = $"CVN{cvnID}";
                int tacanChannel = 74 + carrierDictionary.Count;

                UnitMakerGroupInfo? groupInfo =
                    unitMaker.AddUnitGroup(
                        new string[] { unitDB.ID }, Side.Ally, unitDB.Families[0],
                        "ShipCarrier", "Ship",
                        shipCoordinates, 0,
                        new Dictionary<string, object>{
                        {"GroupX2", shipDestination.X},
                        {"GroupY2", shipDestination.Y},
                        {"ILS", ilsChannel},
                        {"RadioBand",( int)RadioModulation.AM},
                        {"RadioFrequency", GeneratorTools.GetRadioFrenquency(radioFrequency)},
                        {"Speed", carrierSpeed},
                        {"TACANCallsign", tacanCallsign},
                        {"TACANChannel", tacanChannel},
                        {"TACANFrequency", GeneratorTools.GetTACANFrequency(tacanChannel, 'X', false)},
                        {"TACANMode"," X"},
                        {"playerCanDrive", false},
                        {"NoCM", true}});

                if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) continue; // Couldn't generate group

                mission.Briefing.AddItem(
                    DCSMissionBriefingItemType.Airbase,
                    $"{unitDB.UIDisplayName}\t-\t{GeneratorTools.FormatRadioFrequency(radioFrequency)}\t{ilsChannel}\t{tacanCallsign}, {tacanChannel}X");

                carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value);
                mission.MapData.Add($"CARRIER_{flightGroup.Carrier}", new List<Coordinates>{groupInfo.Value.Coordinates});
            }

            return carrierDictionary;
        }

        private static Tuple<Coordinates, Coordinates> GetSpawnAndDestination(
            UnitMaker unitMaker, MissionTemplateRecord template,  DBEntryTheater theaterDB, 
            List<Coordinates> usedCoordinates, Coordinates landbaseCoordinates, Coordinates objectivesCenter,
            double carrierPathDeg)
        {
            var travelMinMax = new MinMaxD(Database.Instance.Common.CarrierGroup.CourseLength, Database.Instance.Common.CarrierGroup.CourseLength * 2);
            Coordinates? carrierGroupCoordinates = null;
                Coordinates? destinationPath = null;
            var iteration = 0;
                var maxDistance = 25;
                while (iteration < 100)
                {
                    carrierGroupCoordinates = unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        new SpawnPointType[] { SpawnPointType.Sea },
                        landbaseCoordinates,
                        new MinMaxD(10, maxDistance),
                        objectivesCenter,
                        new MinMaxD(10, 99999),
                        GeneratorTools.GetSpawnPointCoalition(template, Side.Ally));
                    if(!carrierGroupCoordinates.HasValue)
                    {
                        maxDistance += 25;
                        continue;
                    }
                    var minDist = usedCoordinates.Aggregate(99999999.0, (acc,x) => x.GetDistanceFrom(carrierGroupCoordinates.Value) < acc ? x.GetDistanceFrom(carrierGroupCoordinates.Value) : acc);
                    if(minDist < Database.Instance.Common.CarrierGroup.ShipSpacing)
                        continue;

                    destinationPath = Coordinates.FromAngleAndDistance(carrierGroupCoordinates.Value, travelMinMax, carrierPathDeg);
                    if (ShapeManager.IsPosValid(destinationPath.Value, theaterDB.WaterCoordinates, theaterDB.WaterExclusionCoordinates))
                        break;
                    iteration++;
                    if(iteration > 10)
                        maxDistance += 1;
                }

                if (!carrierGroupCoordinates.HasValue)
                    throw new BriefingRoomException($"Carrier spawnpoint could not be found.");
                if (!destinationPath.HasValue)
                    throw new BriefingRoomException($"Carrier destination could not be found.");
                if (!ShapeManager.IsPosValid(destinationPath.Value, theaterDB.WaterCoordinates, theaterDB.WaterExclusionCoordinates))
                    throw new BriefingRoomException($"Carrier waypoint is on shore");

                return new(carrierGroupCoordinates.Value, destinationPath.Value);
        }

        private static void GenerateFOB(
            UnitMaker unitMaker, ZoneMaker zoneMaker, MissionTemplateFlightGroupRecord flightGroup, Dictionary<string, UnitMakerGroupInfo> carrierDictionary,
            DCSMission mission, MissionTemplateRecord template, Coordinates landbaseCoordinates, Coordinates objectivesCenter)
        {
            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            if (theaterDB == null) return; // Theater doesn't exist. Should never happen.


            Coordinates? spawnPoint =
                    unitMaker.SpawnPointSelector.GetRandomSpawnPoint(
                        new SpawnPointType[] { SpawnPointType.LandLarge },
                        landbaseCoordinates,
                        new MinMaxD(5, template.FlightPlanObjectiveDistance),
                        objectivesCenter,
                        new MinMaxD(10, template.FlightPlanObjectiveDistance/2), template.ContextPlayerCoalition);

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
                unitMaker.AddUnitGroup(
                    unitDB.Families[0], 1, Side.Ally,
                    "GroupStatic", "StaticFOB",
                    spawnPoint.Value, 0,
                    new Dictionary<string, object>{
                    {"FOBCallSignIndex", FOBNames.IndexOf(flightGroup.Carrier) + 1},
                    {"RadioBand", (int)RadioModulation.AM},
                    {"RadioFrequency", GeneratorTools.GetRadioFrenquency(radioFrequency)},
                    {"playerCanDrive", false},
                    {"NoCM", true}});
            if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) return; // Couldn't generate group
            zoneMaker.AddCTLDPickupZone(spawnPoint.Value, true);
            mission.Briefing.AddItem(
                     DCSMissionBriefingItemType.Airbase,
                     $"{unitDB.UIDisplayName}\t-\t{GeneratorTools.FormatRadioFrequency(radioFrequency)}\t\t");
            carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value); // This bit limits FOBS to one per game think about how we can fix this
            mission.MapData.Add($"FOB_{flightGroup.Carrier}", new List<Coordinates>{groupInfo.Value.Coordinates});
        }

    }
}
