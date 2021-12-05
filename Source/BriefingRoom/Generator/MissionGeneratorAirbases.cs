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
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorAirbases
    {
        private readonly MissionTemplate _template;

        private readonly DBEntrySituation _situationDB;

        internal MissionGeneratorAirbases(MissionTemplate template, DBEntrySituation situationDB)
        {
            _template = template;
            _situationDB = situationDB;
        }

        internal void SelectStartingAirbaseForPackages(DCSMission mission, DBEntryAirbase homeBase)
        {
            foreach (var package in _template.AircraftPackages)
            {
                if (package.StartingAirbase == "home")
                {
                    package.Airbase = homeBase;
                    continue;
                }
                var requiredSpots = _template.PlayerFlightGroups.Where((v, i) => package.FlightGroupIndexes.Contains(i)).Sum(x => x.Count);
                package.Airbase = SelectStartingAirbase(mission, package.StartingAirbase, requiredSpots);
            }
        }

        internal DBEntryAirbase SelectStartingAirbase(DCSMission mission, string selectedAirbaseID, int requiredParkingSpots = 0)
        {
            // Get total number of required parking spots for flight groups
            if (requiredParkingSpots == 0)
                requiredParkingSpots = _template.PlayerFlightGroups.Sum(x => x.Count);
            // Select all airbases for this theater
            DBEntryAirbase[] airbases = _situationDB.GetAirbases(_template.OptionsMission.Contains("InvertCountriesCoalitions"));
            // If a particular airbase name has been specified and an airbase with this name exists, pick it
            if (!string.IsNullOrEmpty(selectedAirbaseID))
            {
                var airbase = airbases.FirstOrDefault(x => x.ID == selectedAirbaseID);
                if (airbase is null)
                {
                    BriefingRoom.PrintToLog($"No airbase found with ID \"{selectedAirbaseID}\", cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                    return null;
                }
                if (airbase.ParkingSpots.Length < requiredParkingSpots)
                {
                    BriefingRoom.PrintToLog($"Airbase \"{selectedAirbaseID}\" has less than {requiredParkingSpots} parking spots, cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                    return null;
                }

                return airbase;
            }

            // Select all airbases with enough parking spots
            airbases = airbases.Where(x => x.ParkingSpots.Length >= requiredParkingSpots).ToArray();
            if (airbases.Length == 0)
            {
                BriefingRoom.PrintToLog($"No airbase found with {requiredParkingSpots} parking spots, cannot spawn all player aircraft.", LogMessageErrorLevel.Error);
                return null;
            }

            // Select all airbases belonging to the player coalition
            airbases = airbases.Where(x => x.Coalition == _template.ContextPlayerCoalition).ToArray();
            if (airbases.Length == 0)
            {
                BriefingRoom.PrintToLog($"No airbase belonging to player coalition found, cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                return null;
            }

            // If some targets are ships, or some player start on a carrier, try to pick an airbase near water
            if (MissionPrefersShoreAirbase())
            {
                DBEntryAirbase[] shoreAirbases = airbases.Where(x => x.Flags.HasFlag(AirbaseFlag.NearWater)).ToArray();

                if (shoreAirbases.Length > 0)
                    return Toolbox.RandomFrom(shoreAirbases);
            }

            return Toolbox.RandomFrom(airbases);
        }

        private bool MissionPrefersShoreAirbase()
        {
            // If any objective target is a ship, return true
            foreach (MissionTemplateObjective objective in _template.Objectives)
                if (Database.Instance.EntryExists<DBEntryObjectiveTarget>(objective.Target) &&
                    (Database.Instance.GetEntry<DBEntryObjectiveTarget>(objective.Target).UnitCategory == UnitCategory.Ship))
                    return true;

            // If any flight group takes off from a carrier, return true
            foreach (MissionTemplateFlightGroup flightGroup in _template.PlayerFlightGroups)
                if (!string.IsNullOrEmpty(flightGroup.Carrier))
                    return true;

            return false;
        }

        internal void SetupAirbasesCoalitions(DCSMission mission, DBEntryAirbase playerAirbase)
        {
            // Select all airbases for this theater
            DBEntryAirbase[] situationAirbases = _situationDB.GetAirbases(_template.OptionsMission.Contains("InvertCountriesCoalitions"));

            foreach (DBEntryAirbase airbase in situationAirbases)
            {
                // Airbase is the player starting airbase, always set it to the player coalition
                if (airbase.DCSID == playerAirbase.DCSID)
                {
                    mission.SetAirbase(airbase.DCSID, _template.ContextPlayerCoalition);
                    continue;
                }

                // Other airbases are assigned to a coalition according to the theater and the template settings

                mission.SetAirbase(airbase.DCSID, airbase.Coalition);
            }
        }
    }
}
