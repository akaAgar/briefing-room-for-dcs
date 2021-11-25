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
    internal class MissionGeneratorAirbases : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorAirbases() { }

        internal void SelectStartingAirbaseForPackages(DCSMission mission, MissionTemplate template, DBEntryAirbase homeBase)
        {
            foreach (var package in template.AircraftPackages)
            {
                if(package.StartingAirbase == "home")
                {
                    package.Airbase = homeBase;
                    continue;
                }
                var requiredSpots = template.PlayerFlightGroups.Where((v, i) => package.FlightGroupIndexes.Contains(i)).Sum(x => x.Count);
                package.Airbase = SelectStartingAirbase(mission, template, package.StartingAirbase, requiredSpots);
            }
        }

        /// <summary>
        /// Picks a starting airbase for the player(s)
        /// </summary>
        /// <param name="mission">Mission for which the starting airbase must be set</param>
        /// <param name="template">Mission template to use</param>
        /// <returns>Players' takeoff/landing airbase</returns>
        internal DBEntryAirbase SelectStartingAirbase(DCSMission mission, MissionTemplate template, string selectedAirbaseID, int requiredParkingSpots = 0)
        {
            // Get total number of required parking spots for flight groups
            if (requiredParkingSpots == 0)
                requiredParkingSpots = (from MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups select flightGroup.Count).Sum();
            // Select all airbases for this theater
            DBEntryAirbase[] airbases = Database.Instance.GetEntry<DBEntrySituation>(template.ContextSituation).GetAirbases();
            // If a particular airbase name has been specified and an airbase with this name exists, pick it
            if (!string.IsNullOrEmpty(selectedAirbaseID))
            {
                airbases = (from DBEntryAirbase airbase in airbases where airbase.ID == selectedAirbaseID select airbase).ToArray();
                if (airbases.Length == 0)
                {
                    BriefingRoom.PrintToLog($"No airbase found with ID \"{selectedAirbaseID}\", cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                    return null;
                }
                if (airbases[0].ParkingSpots.Length < requiredParkingSpots)
                {
                    BriefingRoom.PrintToLog($"Airbase \"{selectedAirbaseID}\" has less than {requiredParkingSpots} parking spots, cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                    return null;
                }

                return airbases[0];
            }

            // Select all airbases with enough parking spots
            airbases = (from DBEntryAirbase airbase in airbases where airbase.ParkingSpots.Length >= requiredParkingSpots select airbase).ToArray();
            if (airbases.Length == 0)
            {
                BriefingRoom.PrintToLog($"No airbase found with {requiredParkingSpots} parking spots, cannot spawn all player aircraft.", LogMessageErrorLevel.Error);
                return null;
            }

            // Select all airbases belonging to the player coalition
            airbases = (from DBEntryAirbase airbase in airbases where airbase.Coalition == template.ContextPlayerCoalition select airbase).ToArray();
            if (airbases.Length == 0)
            {
                BriefingRoom.PrintToLog($"No airbase belonging to player coalition found, cannot spawn player aircraft.", LogMessageErrorLevel.Error);
                return null;
            }

            // If some targets are ships, or some player start on a carrier, try to pick an airbase near water
            if (MissionPrefersShoreAirbase(template))
            {
                DBEntryAirbase[] shoreAirbases = (from DBEntryAirbase airbase in airbases where airbase.Flags.HasFlag(AirbaseFlag.NearWater) select airbase).ToArray();

                if (shoreAirbases.Length > 0)
                    return Toolbox.RandomFrom(shoreAirbases);
            }

            return Toolbox.RandomFrom(airbases);
        }

        /// <summary>
        /// Should airbases near the sea be preferred as starting/landing airbases?
        /// </summary>
        /// <param name="template">Mission template to check.</param>
        /// <returns>True or false</returns>
        private bool MissionPrefersShoreAirbase(MissionTemplate template)
        {
            // If any objective target is a ship, return true
            foreach (MissionTemplateObjective objective in template.Objectives)
                if (Database.Instance.EntryExists<DBEntryObjectiveTarget>(objective.Target) &&
                    (Database.Instance.GetEntry<DBEntryObjectiveTarget>(objective.Target).UnitCategory == UnitCategory.Ship))
                    return true;

            // If any flight group takes off from a carrier, return true
            foreach (MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups)
                if (!string.IsNullOrEmpty(flightGroup.Carrier))
                    return true;

            return false;
        }

        /// <summary>
        /// Sets the coalition to which the various airbases on the theater belong.
        /// </summary>
        /// <param name="mission">Mission for which airbase coalitions must be set</param>
        /// <param name="template">Mission template</param>
        /// <param name="playerAirbase">Players' takeoff/landing airbase</param>
        internal void SetupAirbasesCoalitions(DCSMission mission, MissionTemplate template, DBEntryAirbase playerAirbase)
        {
            // Select all airbases for this theater
            DBEntryAirbase[] situationAirbases = Database.Instance.GetEntry<DBEntrySituation>(template.ContextSituation).GetAirbases();

            foreach (DBEntryAirbase airbase in situationAirbases)
            {
                // Airbase is the player starting airbase, always set it to the player coalition
                if (airbase.DCSID == playerAirbase.DCSID)
                {
                    mission.SetAirbase(airbase.DCSID, template.ContextPlayerCoalition);
                    continue;
                }

                // Other airbases are assigned to a coalition according to the theater and the template settings

                mission.SetAirbase(airbase.DCSID, airbase.Coalition);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
