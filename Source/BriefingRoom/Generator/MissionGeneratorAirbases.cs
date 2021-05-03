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
    internal class MissionGeneratorAirbases : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGeneratorAirbases()
        {

        }

        /// <summary>
        /// Picks a starting airbase for the player(s)
        /// </summary>
        /// <param name="mission">Mission for which the starting airbase must be set</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="playerAirbase">Players' takeoff/landing airbase</param>
        internal void SelectStartingAirbase(DCSMission mission, MissionTemplate template, out DBEntryAirbase playerAirbase)
        {
            playerAirbase = null;

            // Select all airbases for this theater
            DBEntryAirbase[] airbases = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater).GetAirbases();

            // Get total number of required parking spots for flight groups
            int requiredParkingSpots = (from MissionTemplateFlightGroup flightGroup in template.PlayerFlightGroups select flightGroup.Count).Sum();

            // Select all airbases with enough parking spots
            airbases = (from DBEntryAirbase airbase in airbases where airbase.ParkingSpots.Length >= requiredParkingSpots select airbase).ToArray();
            if (airbases.Length == 0)
            {
                BriefingRoom.PrintToLog($"No airbase found with {requiredParkingSpots} parking spots, cannot spawn all player aircraft.", LogMessageErrorLevel.Error);
                playerAirbase = null;
                return;
            }

            //// If a particular airbase name has been specified and an airbase with this name exists, pick it
            //if (!string.IsNullOrEmpty(template.FlightPlanTheaterStartingAirbase))
            //{
            //    airbases = (from DBEntryAirbase airbase in airbases where airbase.ParkingSpots.Length >= requiredParkingSpots select airbase).ToArray();
            //    if ()
            //    airbasesList.Add((from DBEntryAirbase airbase in airbases
            //                      where airbase.Name.ToLowerInvariant() == template.FlightPlanTheaterStartingAirbase.ToLowerInvariant()
            //                      select airbase).ToArray());

            //    if (airbasesList.Last().Length == 0)
            //        BriefingRoom.PrintToLog($"Airbase \"{template.FlightPlanTheaterStartingAirbase}\" not found or airbase doesn't have enough parking spots. Selecting a random airbase instead.", LogMessageErrorLevel.Warning);
            //}

            //// Select all airbases belonging to the player coalition (unless all airbases belong to the same coalition)
            //if ((template.ContextTheaterCountriesCoalitions == CountryCoalition.Default) ||
            //    (template.ContextTheaterCountriesCoalitions == CountryCoalition.Inverted))
            //{
            //    Coalition requiredCoalition =
            //        (template.ContextTheaterCountriesCoalitions == CountryCoalition.Inverted) ?
            //        template.ContextPlayerCoalition.GetEnemy() : template.ContextPlayerCoalition;

            //    airbasesList.Add((from DBEntryTheaterAirbase ab in airbasesList.Last() where ab.Coalition == requiredCoalition select ab).ToArray());
            //}

            //// Targets are ships, spawn players near the sea
            //bool seaTargets = false;
            //foreach (MissionTemplateObjective objective in template.Objectives)
            //    if (Database.Instance.EntryExists<DBEntryObjectiveTarget>(objective.Target) &&
            //        (Database.Instance.GetEntry<DBEntryObjectiveTarget>(objective.Target).UnitCategory == UnitCategory.Ship))
            //    {
            //        seaTargets = true;
            //        break;
            //    }

            //// If some targets are ships, or some player start on a carrier, only select airbases near water
            //if (seaTargets || !string.IsNullOrEmpty(template.PlayerFlightGroups[0].Carrier))
            //    airbasesList.Add((from DBEntryTheaterAirbase ab in airbasesList.Last() where ab.Flags.HasFlag(DBEntryTheaterAirbaseFlag.NearWater) select ab).ToArray());


            //// Check for valid airbases in all list, starting from the last one (with the most criteria filtered, and go back to the previous ones
            //// as long as no airbase is found.
            //for (int i = airbasesList.Count - 1; i >= 0; i--)
            //{
            //    if (airbasesList[i].Length > 0)
            //        playerAirbase = Toolbox.RandomFrom(airbasesList[i]);
            //}
        }

        ///// <summary>
        ///// Picks a starting airbase for the player(s)
        ///// </summary>
        ///// <param name="mission">Mission for which the starting airbase must be set</param>
        ///// <param name="template">Mission template to use</param>
        ///// <param name="theaterDB">Theater database entry</param>
        ///// <param name="lastCoordinates">Last location for referance</param>
        ///// <param name="distance">Base Distance Range</param>
        ///// <param name="first">is first objective</param>
        ///// <returns>Information about the starting airbase</returns>
        //internal DBEntryTheaterAirbase SelectObjectiveAirbase(DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB, Coordinates lastCoordinates, MinMaxD distance, bool first = false)
        //{
        //    List<DBEntryTheaterAirbase> airbasesList = new List<DBEntryTheaterAirbase>();

        //    // Select all airbases with enough parking spots, trying to match the preferred coalition for enemy unit location, if any
        //    if ((template.OptionsTheaterCountriesCoalitions == CountryCoalition.AllBlue) || (template.OptionsTheaterCountriesCoalitions == CountryCoalition.AllRed) ||
        //        (template.OptionsEnemyUnitsLocation == SpawnPointPreferredCoalition.Any))
        //        airbasesList.AddRange((from DBEntryTheaterAirbase ab in theaterDB.Airbases where ab.ParkingSpots.Length >= Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE select ab).ToArray());
        //    else
        //    {
        //        Coalition preferredCoalition;

        //        if (template.OptionsEnemyUnitsLocation == SpawnPointPreferredCoalition.Blue)
        //            preferredCoalition = (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Red : Coalition.Blue;
        //        else
        //            preferredCoalition = (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Blue : Coalition.Red;

        //        airbasesList.AddRange(
        //            (from DBEntryTheaterAirbase ab in theaterDB.Airbases where ab.ParkingSpots.Length >= Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE && ab.Coalition == preferredCoalition select ab).ToArray());

        //        if (airbasesList.Count == 0)
        //            airbasesList.AddRange((from DBEntryTheaterAirbase ab in theaterDB.Airbases where ab.ParkingSpots.Length >= Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE select ab).ToArray());
        //    }

        //    // Remove players' home airbase and airbases already used by other objectives from the list of available airbases
        //    List<int> airbasesInUse = (from DCSMissionObjective objective in mission.Objectives select objective.AirbaseID).ToList();
        //    airbasesInUse.Add(mission.InitialAirbaseID);
        //    airbasesList = (from DBEntryTheaterAirbase ab in airbasesList where !airbasesInUse.Contains(ab.DCSID) select ab).ToList();

        //    if (airbasesList.Count == 0)
        //        throw new Exception($"No airbase found with at least {Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE} parking spots to use as an objective.");

        //    int distanceMultiplier = 1;
        //    do
        //    {
        //        MinMaxD searchDistance = new MinMaxD(first ? distance.Min : 0, distance.Max * distanceMultiplier);
        //        List<DBEntryTheaterAirbase> airbasesInRange = airbasesList.FindAll(x => searchDistance.Contains(x.Coordinates.GetDistanceFrom(lastCoordinates) * Toolbox.METERS_TO_NM));
        //        if (airbasesInRange.Count > 0)
        //        {
        //            DBEntryTheaterAirbase selectedAirbase = Toolbox.RandomFrom(airbasesInRange);
        //            mission.AirbasesCoalition[selectedAirbase.DCSID] = mission.CoalitionEnemy;
        //            return selectedAirbase;
        //        }

        //        distanceMultiplier++;

        //        if (distanceMultiplier > 128)
        //            throw new Exception($"No target airbase found within range, try a larger objective range.");

        //    } while (true);
        //}

        /// <summary>
        /// Sets the coalition to which the various airbases on the theater belong.
        /// </summary>
        /// <param name="mission">Mission for which airbase coalitions must be set</param>
        /// <param name="template">Mission template</param>
        /// <param name="playerAirbase">Players' takeoff/landing airbase</param>
        internal void SetupAirbasesCoalitions(DCSMission mission, MissionTemplate template, DBEntryAirbase playerAirbase)
        {
            // Select all airbases for this theater
            DBEntryAirbase[] theaterAirbases = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater).GetAirbases();

            foreach (DBEntryAirbase airbase in theaterAirbases)
            {
                // Airbase is the player starting airbase, always set it to the player coalition
                if (airbase.DCSID == playerAirbase.DCSID)
                {
                    mission.SetAirbase(airbase.DCSID, template.ContextPlayerCoalition);
                    continue;
                }

                // Other airbases are assigned to a coalition according to the theater and the template settings
                Coalition airbaseCoalition;
                switch (template.ContextTheaterCountriesCoalitions)
                {
                    default: airbaseCoalition = airbase.Coalition; break;
                    case CountryCoalition.AllBlue: airbaseCoalition = Coalition.Blue; break;
                    case CountryCoalition.AllRed: airbaseCoalition = Coalition.Red; break;
                    case CountryCoalition.Inverted: airbaseCoalition = (Coalition)(1 - (int)airbase.Coalition); break;
                }

                mission.SetAirbase(airbase.DCSID, airbaseCoalition);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
