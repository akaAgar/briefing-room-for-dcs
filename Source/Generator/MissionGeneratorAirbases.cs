using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
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
            DBEntryTheaterAirbase[] airbases;

            // If a particular airbase name has been specified and an airbase with this name exists, pick it
            if (!string.IsNullOrEmpty(template.TheaterStartingAirbase))
            {
                string airbaseName = template.TheaterStartingAirbase.Trim();
                if (airbaseName.Contains(",")) airbaseName = airbaseName.Substring(airbaseName.IndexOf(',')).Trim(' ', ',');
                airbases =
                    (from DBEntryTheaterAirbase airbase in theaterDB.Airbases
                     where airbase.Name == airbaseName
                     select airbase).ToArray();

                if (airbases.Length > 0)
                    return airbases[0];
            }

            // Still no airbase found? Pick a random airbase from the player coalition
            airbases =
                    (from DBEntryTheaterAirbase ab in theaterDB.Airbases
                     where ab.Coalition == template.CoalitionPlayer select ab).ToArray();

            // Still no airbase found? Pick a random airbase belonging to any coalition
            if (airbases.Length == 0)
                airbases = (from DBEntryTheaterAirbase ab in theaterDB.Airbases select ab).ToArray();

            // Still nothing found? We're outta luck, abort mission generation.
            if (airbases.Length == 0)
                throw new Exception($"Failed to find a starting airbase on theater \"{template.TheaterID}\".");

            int requiredParkingSpots = template.GetMissionPackageRequiredParkingSpots();

            // Player(s) must start near water, try to find an airbase located near the ocean/sea if possible
            if (objectiveDB.Flags.HasFlag(DBEntryObjectiveFlags.MustStartNearWater))
            {
                DBEntryTheaterAirbase[] airbasesOnShore = 
                    (from DBEntryTheaterAirbase ab in airbases
                     where ab.Flags.Contains(DBEntryTheaterAirbaseFlag.NearWater) &&
                     ab.ParkingSpots.Length >= requiredParkingSpots
                     select ab).ToArray();

                // Found (at least) one, use the list of shore airbases instead of the general airbase list
                if (airbasesOnShore.Length > 0)
                    airbases = airbasesOnShore;
            }

            // Remove airbases with insufficient parking spots
            airbases =
                (from DBEntryTheaterAirbase ab in airbases
                 where ab.ParkingSpots.Length >= requiredParkingSpots
                 select ab).ToArray();

            if (airbases.Length == 0)
                throw new Exception($"No airbase found with {requiredParkingSpots} parking spots, cannot spawn all player aircraft.");

            return Toolbox.RandomFrom(airbases);
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
