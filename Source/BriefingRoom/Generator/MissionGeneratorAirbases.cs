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
    internal class MissionGeneratorAirbases
    {
        private readonly MissionTemplateRecord _template;

        private readonly DBEntrySituation _situationDB;

        internal MissionGeneratorAirbases(MissionTemplateRecord template, DBEntrySituation situationDB)
        {
            _template = template;
            _situationDB = situationDB;
        }

        internal void SelectStartingAirbaseForPackages(DCSMission mission, DBEntryAirbase homeBase)
        {
            var missionPackages = new List<DCSMissionPackage>();
            foreach (var package in _template.AircraftPackages)
            {
                if (package.StartingAirbase == "home")
                {
                    missionPackages.Add(new DCSMissionPackage(_template.AircraftPackages.IndexOf(package), homeBase));
                    continue;
                }
                var requiredSpots = _template.PlayerFlightGroups.Where((v, i) => package.FlightGroupIndexes.Contains(i)).Sum(x => x.Count);
                var airbase = SelectStartingAirbase(mission, package.StartingAirbase, requiredSpots);

                if (missionPackages.Any(x => x.Airbase == airbase))
                    mission.Briefing.AddItem(DCSMissionBriefingItemType.Airbase, $"{airbase.Name}\t{airbase.Runways}\t{airbase.ATC}\t{airbase.ILS}\t{airbase.TACAN}");

                missionPackages.Add(new DCSMissionPackage(_template.AircraftPackages.IndexOf(package), airbase));
            }
            mission.MissionPackages.AddRange(missionPackages);
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
                    throw new BriefingRoomException($"No airbase found with ID \"{selectedAirbaseID}\", cannot spawn player aircraft.");

                return airbase;
            }

            return Toolbox.RandomFrom(
                airbases.Where(x =>
                    x.ParkingSpots.Length >= requiredParkingSpots &&
                    x.Coalition == _template.ContextPlayerCoalition &&
                    (MissionPrefersShoreAirbase() ? x.Flags.HasFlag(AirbaseFlag.NearWater) : true)
                    ).ToArray()
                );
        }

        private bool MissionPrefersShoreAirbase()
        {
            // If any objective target is a ship, return true
            foreach (MissionTemplateObjectiveRecord objective in _template.Objectives)
                if (Database.Instance.EntryExists<DBEntryObjectiveTarget>(objective.Target) &&
                    (Database.Instance.GetEntry<DBEntryObjectiveTarget>(objective.Target).UnitCategory == UnitCategory.Ship))
                    return true;

            // If any flight group takes off from a carrier, return true
            foreach (MissionTemplateFlightGroupRecord flightGroup in _template.PlayerFlightGroups)
                if (!string.IsNullOrEmpty(flightGroup.Carrier) && !flightGroup.Carrier.StartsWith("FOB"))
                    return true;

            return false;
        }

        internal void SetupAirbasesCoalitions(DCSMission mission, DBEntryAirbase playerAirbase)
        {
            // Select all airbases for this theater
            DBEntryAirbase[] situationAirbases = _situationDB.GetAirbases(_template.OptionsMission.Contains("InvertCountriesCoalitions"));

            foreach (DBEntryAirbase airbase in situationAirbases)
            {
                var coalition = airbase.DCSID == playerAirbase.DCSID || mission.MissionPackages.Any(x => x.Airbase.DCSID == airbase.DCSID) ? _template.ContextPlayerCoalition : airbase.Coalition;
                mission.SetAirbase(airbase.DCSID, coalition);
            }
        }
    }
}
