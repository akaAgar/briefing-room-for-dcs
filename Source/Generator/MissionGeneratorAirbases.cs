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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    public class MissionGeneratorAirbases : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGeneratorAirbases() { }

        /// <summary>
        /// Picks a starting airbase for the player(s)
        /// </summary>
        /// <param name="mission">Mission for which the starting airbase must be set</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="theaterDB">Theater database entry</param>
        /// <param name="objectiveDB">Objective database entry</param>
        /// <returns>Information about the starting airbase</returns>
        public DBEntryTheaterAirbase SelectStartingAirbase(DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB, DBEntryObjective objectiveDB)
        {
            List<DBEntryTheaterAirbase[]> airbasesList = new List<DBEntryTheaterAirbase[]>();

            // Select all airbases with enough parking spots
            int requiredParkingSpots = template.GetMissionPackageRequiredParkingSpots();
            airbasesList.Add((from DBEntryTheaterAirbase ab in theaterDB.Airbases where ab.ParkingSpots.Length >= requiredParkingSpots select ab).ToArray());

            // Select all airbases belonging to the proper coalition (unless all airbase belong to the same coalition)
            if ((template.TheaterRegionsCoalitions == CountryCoalition.Default) || (template.TheaterRegionsCoalitions == CountryCoalition.Inverted))
            {
                Coalition requiredCoalition = template.TheaterRegionsCoalitions == CountryCoalition.Inverted ? mission.CoalitionEnemy : mission.CoalitionPlayer;
                airbasesList.Add((from DBEntryTheaterAirbase ab in airbasesList.Last() where ab.Coalition == requiredCoalition select ab).ToArray());
            }

            // If mission must start near water, or some player start on a carrier, select all airbases near water
            if (objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.MustStartNearWater) || !string.IsNullOrEmpty(template.PlayerCarrier))
                airbasesList.Add((from DBEntryTheaterAirbase ab in airbasesList.Last() where ab.Flags.HasFlag(DBEntryTheaterAirbaseFlag.NearWater) select ab).ToArray());

            // If a particular airbase name has been specified and an airbase with this name exists, pick it
            if (!string.IsNullOrEmpty(template.TheaterStartingAirbase))
            {
                string airbaseName = template.TheaterStartingAirbase.Trim();
                if (airbaseName.Contains(",")) airbaseName = airbaseName.Substring(airbaseName.IndexOf(',')).Trim(' ', ',');
                airbasesList.Add((from DBEntryTheaterAirbase airbase in theaterDB.Airbases where airbase.Name == airbaseName select airbase).ToArray());

                if (airbasesList.Last().Length == 0)
                    DebugLog.Instance.WriteLine($"Airbase \"{airbaseName}\" not found or airbase doesn't have enough parking spots. Selecting a random airbase instead.", 1, DebugLogMessageErrorLevel.Warning);
            }

            // Check for valid airbases in all list, starting from the last one (with the most criteria filtered, and go back to the previous ones
            // as long as no airbase is found.
            for (int i = airbasesList.Count - 1; i >= 0; i--)
            {
                if (airbasesList[i].Length > 0)
                    return Toolbox.RandomFrom(airbasesList[i]);
            }

            throw new Exception($"No airbase found with {requiredParkingSpots} parking spots, cannot spawn all player aircraft.");
        }

        /// <summary>
        /// Sets the coalition to which the various airbases on the theater belong.
        /// </summary>
        /// <param name="mission">Mission for which airbase coalitions must be set</param>
        /// <param name="theaterAirbasesCoalitions">Airbase coalition setting</param>
        /// <param name="theaterDB">Theater database entry</param>
        public void SetupAirbasesCoalitions(DCSMission mission, CountryCoalition theaterAirbasesCoalitions, DBEntryTheater theaterDB)
        {
            mission.AirbasesCoalition.Clear();
            foreach (DBEntryTheaterAirbase ab in theaterDB.Airbases)
            {
                // Airbase ID already exists in the mission
                if (mission.AirbasesCoalition.ContainsKey(ab.DCSID)) continue;

                // Airbase is the player starting airbase, always set it to the player coalition
                if (ab.DCSID == mission.InitialAirbaseID)
                {
                    mission.AirbasesCoalition.Add(ab.DCSID, mission.CoalitionPlayer);
                    continue;
                }

                // Other airbases are assigned to a coalition according to the theater and the template settings
                Coalition airbaseCoalition = ab.Coalition;
                switch (theaterAirbasesCoalitions)
                {
                    case CountryCoalition.AllBlue: airbaseCoalition = Coalition.Blue; break;
                    case CountryCoalition.AllRed: airbaseCoalition = Coalition.Red; break;
                    case CountryCoalition.Inverted: airbaseCoalition = (Coalition)(1 - (int)ab.Coalition); break;
                }

                mission.AirbasesCoalition.Add(ab.DCSID, airbaseCoalition);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
