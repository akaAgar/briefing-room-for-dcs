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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// Generates player-controlled unit groups (and their AI CAP/SEAD escort for single-player missions)
    /// </summary>
    internal class MissionGeneratorPlayerFlightGroups : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        internal MissionGeneratorPlayerFlightGroups(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        internal void GeneratePlayerFlightGroup(DCSMission mission, MissionTemplateFlightGroup flightGroup, DBEntryAirbase playerAirbase, List<Waypoint> waypoints, Dictionary<string, UnitMakerGroupInfo> carrierDictionary)
        {
            DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(flightGroup.Aircraft);

            // Not an unit, or not a player-controllable unit, abort.
            if ((unitDB == null) || !unitDB.AircraftData.PlayerControllable)
                throw new BriefingRoomException($"Player flight group unit \"{flightGroup.Aircraft}\" does not exist or is not player-controllable.");

            List<int> parkingSpotIDsList = new List<int>();
            List<Coordinates> parkingSpotCoordinatesList = new List<Coordinates>();
            List<Waypoint> flightWaypoints = new List<Waypoint>(waypoints);
            string groupLuaFile = "GroupAircraftPlayer";
            int carrierUnitID = 0;
            Coordinates groupStartingCoords = playerAirbase.Coordinates;
            if (!string.IsNullOrEmpty(flightGroup.Carrier) && carrierDictionary.ContainsKey(flightGroup.Carrier)) // Carrier take off
            {
                var carrier = carrierDictionary[flightGroup.Carrier];
                groupLuaFile = "GroupAircraftPlayerCarrier";
                carrierUnitID = carrier.UnitsID[0];

                for (int i = 0; i < flightGroup.Count; i++)
                {
                    parkingSpotIDsList.Add(i + 1);
                    parkingSpotCoordinatesList.Add(carrier.Coordinates);
                }
                groupStartingCoords = carrier.Coordinates;
                
                //overwrite waypoints
                flightWaypoints[0] = new Waypoint(flightWaypoints.First().Name, carrier.Coordinates);
                flightWaypoints[flightWaypoints.Count -1] = new Waypoint(flightWaypoints.Last().Name, carrier.Coordinates);
            }
            else // Land airbase take off
            {
                Coordinates? lastParkingCoordinates = null;

                for (int i = 0; i < flightGroup.Count; i++)
                {
                    int parkingSpot = UnitMaker.SpawnPointSelector.GetFreeParkingSpot(playerAirbase.DCSID, out Coordinates parkingSpotCoordinates, lastParkingCoordinates);
                    if (parkingSpot < 0) throw new BriefingRoomException("No parking spot found for player aircraft.");
                    lastParkingCoordinates = parkingSpotCoordinates;

                    parkingSpotIDsList.Add(parkingSpot);
                    parkingSpotCoordinatesList.Add(parkingSpotCoordinates);
                }
            }

            UnitMakerGroupFlags unitMakerGroupFlags = flightGroup.AIWingmen ? UnitMakerGroupFlags.FirstUnitIsClient : 0;
            DCSSkillLevel skillLevel = flightGroup.AIWingmen ? Toolbox.RandomFrom(DCSSkillLevel.High, DCSSkillLevel.Excellent) : DCSSkillLevel.Client;

            UnitMakerGroupInfo? groupInfo = UnitMaker.AddUnitGroup(
                Enumerable.Repeat(flightGroup.Aircraft, flightGroup.Count).ToArray(), Side.Ally, unitDB.Families[0],
                groupLuaFile, "UnitAircraft", groupStartingCoords,
                skillLevel, unitMakerGroupFlags, flightGroup.Payload,
                "PlayerStartingAction".ToKeyValuePair(GeneratorTools.GetPlayerStartingAction(flightGroup.StartLocation)),
                "PlayerStartingType".ToKeyValuePair(GeneratorTools.GetPlayerStartingType(flightGroup.StartLocation)),
                "Country".ToKeyValuePair(flightGroup.Country),
                "InitialWPName".ToKeyValuePair(Database.Instance.Common.Names.WPInitialName),
                "FinalWPName".ToKeyValuePair(Database.Instance.Common.Names.WPFinalName),
                "ParkingID".ToKeyValuePair(parkingSpotIDsList.ToArray()),
                "PlayerWaypoints".ToKeyValuePair(GenerateFlightPlanLua(flightWaypoints)),
                "LastPlayerWaypointIndex".ToKeyValuePair(flightWaypoints.Count + 2),
                "LinkUnit".ToKeyValuePair(carrierUnitID),
                "UnitX".ToKeyValuePair((from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.X).ToArray()),
                "UnitY".ToKeyValuePair((from Coordinates coordinates in parkingSpotCoordinatesList select coordinates.Y).ToArray()),
                "MissionAirbaseX".ToKeyValuePair(groupStartingCoords.X),
                "MissionAirbaseY".ToKeyValuePair(groupStartingCoords.Y),
                "MissionAirbaseID".ToKeyValuePair(playerAirbase.DCSID));

            if (!groupInfo.HasValue)
            {
                BriefingRoom.PrintToLog("Failed to generate player flight group.", LogMessageErrorLevel.Warning);
                return;
            }

            mission.Briefing.AddItem(DCSMissionBriefingItemType.FlightGroup,
                $"{groupInfo.Value.Name}\t" +
                $"{flightGroup.Count}× {unitDB.UIDisplayName}\t" +
                $"{GeneratorTools.FormatRadioFrequency(unitDB.AircraftData.RadioFrequency)}\t" +
                $"{flightGroup.Payload}"); // TODO: human-readable payload name
        }

        private string GenerateFlightPlanLua(List<Waypoint> waypoints)
        {
            string flightPlanLua = "";
            string waypointLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}WaypointPlayer.lua");

            for (int i = 0; i < waypoints.Count; i++)
            {
                string waypointLua = waypointLuaTemplate;

                GeneratorTools.ReplaceKey(ref waypointLua, "Index", i + 2);
                GeneratorTools.ReplaceKey(ref waypointLua, "Name", waypoints[i].Name);
                GeneratorTools.ReplaceKey(ref waypointLua, "X", waypoints[i].Coordinates.X);
                GeneratorTools.ReplaceKey(ref waypointLua, "Y", waypoints[i].Coordinates.Y);
                if (waypoints[i].OnGround) GeneratorTools.ReplaceKey(ref waypointLua, "Altitude", "0");

                flightPlanLua += waypointLua + "\n";
            }

            return flightPlanLua;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
