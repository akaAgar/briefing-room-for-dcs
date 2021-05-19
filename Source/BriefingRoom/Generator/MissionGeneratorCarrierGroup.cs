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
        /// Minimum carrier speed (in knots)
        /// </summary>
        private const double MIN_CARRIER_SPEED = 5.0;

        /// <summary>
        /// Target wind on deck (in knots)
        /// </summary>
        private const double IDEAL_WIND_ON_DECK = 20.0;

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
        /// <param name="template">The mission template to use.</param>
        /// <param name="landbaseCoordinates">Coordinates of the players' land base.</param>
        /// <param name="windSpeedAtSeaLevel">Wind speed at sea level, in m/s.</param>
        /// <param name="windDirectionAtSeaLevel">Wind direction at sea level, in radians.</param>
        /// <returns>A dictionary of carrier group units info, with the database ID of the ship as key.</returns>
        internal Dictionary<string, UnitMakerGroupInfo> GenerateCarrierGroup(MissionTemplate template, Coordinates landbaseCoordinates, double windSpeedAtSeaLevel, double windDirectionAtSeaLevel)
        {
            Dictionary<string, UnitMakerGroupInfo> carrierDictionary = new Dictionary<string, UnitMakerGroupInfo>(StringComparer.InvariantCultureIgnoreCase);

            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            if (theaterDB == null) return carrierDictionary; // Theater doesn't exist. Should never happen.

            // Pick the carrier spawn point closer from the initial airbase
            Coordinates carrierGroupCoordinates = theaterDB.CarrierGroupWaypoints.OrderBy(x => x.GetDistanceFrom(landbaseCoordinates)).First();

            if (windSpeedAtSeaLevel == 0) // No wind? Pick a random direction so carriers don't always go to a 0 course when wind is calm.
                windDirectionAtSeaLevel = Toolbox.RandomDouble(Toolbox.TWO_PI);
            Coordinates destinationPath = Coordinates.FromAngleInRadians(windDirectionAtSeaLevel + Math.PI) * CARRIER_COURSE_LENGTH * Toolbox.NM_TO_METERS;

            double carrierSpeed = Math.Max(MIN_CARRIER_SPEED * Toolbox.KNOTS_TO_METERS_PER_SECOND, IDEAL_WIND_ON_DECK * Toolbox.KNOTS_TO_METERS_PER_SECOND - windSpeedAtSeaLevel);

            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
            {
                if (string.IsNullOrEmpty(flightGroup.Carrier)) continue; // No carrier for
                if (carrierDictionary.ContainsKey(flightGroup.Carrier)) continue; // Carrier type already added
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Carrier);
                if ((unitDB == null) || !unitDB.Families[0].IsCarrier()) continue; // Unit doesn't exist or is not a carrier

                Coordinates shipCoordinates = carrierGroupCoordinates + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierDictionary.Count * Toolbox.NM_TO_METERS;
                Coordinates shipDestination = shipCoordinates + destinationPath;

                string cvnID = carrierDictionary.Count > 0 ? (carrierDictionary.Count + 1).ToString() : "";
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
                        "ILS".ToKeyValuePair(11 + carrierDictionary.Count),
                        "RadioBand".ToKeyValuePair((int)RadioModulation.AM),
                        "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(127.5 + carrierDictionary.Count)),
                        "Speed".ToKeyValuePair(carrierSpeed),
                        "TACANCallsign".ToKeyValuePair($"CVN{cvnID}"),
                        "TACANChannel".ToKeyValuePair(74 + carrierDictionary.Count),
                        "TACANFrequency".ToKeyValuePair(GeneratorTools.GetTACANFrequency(74 + carrierDictionary.Count, 'X', false)),
                        "TACANMode".ToKeyValuePair("X"));

                if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) continue; // Couldn't generate group

                carrierDictionary.Add(flightGroup.Carrier, groupInfo.Value);
            }

            if (carrierDictionary.Count > 0) // Add escorts, if there's a carrier group
            {
                // Randomize escort unit families order so they don't always appear in the same order
                UnitFamily[] escortUnitFamilies = ESCORT_UNIT_FAMILIES.OrderBy(x => Toolbox.RandomInt()).ToArray();

                foreach (UnitFamily escortUnitFamily in escortUnitFamilies)
                {
                    Coordinates shipCoordinates = carrierGroupCoordinates + Coordinates.FromAngleInRadians(Toolbox.RandomAngle()) * carrierDictionary.Count * Toolbox.NM_TO_METERS;
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
                            "RadioFrequency".ToKeyValuePair(GeneratorTools.GetRadioFrenquency(127.5 + carrierDictionary.Count))
                            );

                    if (!groupInfo.HasValue || (groupInfo.Value.UnitsID.Length == 0)) continue; // Couldn't generate group

                    carrierDictionary.Add($"*ESCORT{carrierDictionary.Count}", groupInfo.Value);
                }
            }

            return carrierDictionary;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
