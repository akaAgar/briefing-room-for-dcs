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
    internal class UnitMakerSpawnPointSelector
    {
        private const int MAX_RADIUS_SEARCH_ITERATIONS = 32;

        private readonly Dictionary<int, List<DBEntryAirbaseParkingSpot>> AirbaseParkingSpots;

        private readonly List<DBEntryTheaterSpawnPoint> SpawnPoints;

        private readonly DBEntryTheater TheaterDB;

        private readonly DBEntrySituation SituationDB;

        private readonly bool InvertCoalition;

        internal UnitMakerSpawnPointSelector(DBEntryTheater theaterDB, DBEntrySituation situationDB, bool invertCoalition)
        {
            TheaterDB = theaterDB;
            SituationDB = situationDB;
            AirbaseParkingSpots = new Dictionary<int, List<DBEntryAirbaseParkingSpot>>();
            SpawnPoints = new List<DBEntryTheaterSpawnPoint>();
            InvertCoalition = invertCoalition;

            Clear();
        }

        internal List<DBEntryAirbaseParkingSpot> GetFreeParkingSpots(int airbaseID, int unitCount, bool requiresOpenAirParking = false)
        {
            if (!AirbaseParkingSpots.ContainsKey(airbaseID) ||
                (AirbaseParkingSpots[airbaseID].Count(x => !requiresOpenAirParking || x.ParkingType != ParkingSpotType.HardenedAirShelter) < unitCount))
                throw new BriefingRoomException("Airbase didn't have enough parking spots. ");

            var airbaseDB = SituationDB.GetAirbases(InvertCoalition).First(x => x.DCSID == airbaseID);
            var parkingSpots = new List<DBEntryAirbaseParkingSpot>();
            Coordinates? lastSpotCoordinates = null;
            for (int i = 0; i < unitCount; i++)
            {
                var viableSpots = AirbaseParkingSpots[airbaseID].FindAll(x => (!requiresOpenAirParking || x.ParkingType != ParkingSpotType.HardenedAirShelter)).ToList();
                if (viableSpots.Count == 0) throw new BriefingRoomException("Airbase didn't have enough parking spots. POST CHECK!");
                var parkingSpot = Toolbox.RandomFrom(viableSpots);
                if (lastSpotCoordinates.HasValue) //find nearest spot distance wise in attempt to cluster
                    parkingSpot = viableSpots
                        .Aggregate((acc, x) => acc.Coordinates.GetDistanceFrom(lastSpotCoordinates.Value) > x.Coordinates.GetDistanceFrom(lastSpotCoordinates.Value) && x.Coordinates.GetDistanceFrom(lastSpotCoordinates.Value) > 3 ? x : acc);

                lastSpotCoordinates = parkingSpot.Coordinates;
                AirbaseParkingSpots[airbaseID].Remove(parkingSpot);
                parkingSpots.Add(parkingSpot);
            }

            return parkingSpots;
        }

        internal void Clear()
        {
            AirbaseParkingSpots.Clear();
            SpawnPoints.Clear();
            if (TheaterDB.SpawnPoints is not null)
                SpawnPoints.AddRange(TheaterDB.SpawnPoints);

            foreach (DBEntryAirbase airbase in SituationDB.GetAirbases(InvertCoalition))
            {
                if (airbase.ParkingSpots.Length < 1) continue;
                if (AirbaseParkingSpots.ContainsKey(airbase.DCSID)) continue;
                AirbaseParkingSpots.Add(airbase.DCSID, airbase.ParkingSpots.ToList());
            }
        }

        internal Coordinates? GetRandomSpawnPoint(
            SpawnPointType[] validTypes,
            Coordinates distanceOrigin1, MinMaxD distanceFrom1,
            Coordinates? distanceOrigin2 = null, MinMaxD? distanceFrom2 = null,
            Coalition? coalition = null)
        {
            if (validTypes.Contains(SpawnPointType.Air) || validTypes.Contains(SpawnPointType.Sea))
                return GetAirOrSeaCoordinates(validTypes, distanceOrigin1, distanceFrom1, distanceOrigin2, distanceFrom2, coalition);
            return GetLandCoordinates(validTypes, distanceOrigin1, distanceFrom1, distanceOrigin2, distanceFrom2, coalition);
        }

        private Coordinates? GetLandCoordinates(
            SpawnPointType[] validTypes,
            Coordinates distanceOrigin1, MinMaxD distanceFrom1,
            Coordinates? distanceOrigin2 = null, MinMaxD? distanceFrom2 = null,
            Coalition? coalition = null
        )
        {
            var validSP = (from DBEntryTheaterSpawnPoint pt in SpawnPoints where validTypes.Contains(pt.PointType) select pt);
            Coordinates?[] distanceOrigin = new Coordinates?[] { distanceOrigin1, distanceOrigin2 };
            MinMaxD?[] distanceFrom = new MinMaxD?[] { distanceFrom1, distanceFrom2 };

            for (int i = 0; i < 2; i++) // Remove spawn points too far or too close from distanceOrigin1 and distanceOrigin2
            {
                if (validSP.Count() == 0) return null;
                if (!distanceFrom[i].HasValue || !distanceOrigin[i].HasValue) continue;

                var searchRange = distanceFrom[i].Value * Toolbox.NM_TO_METERS; // convert distance to meters

                IEnumerable<DBEntryTheaterSpawnPoint> validSPInRange = (from DBEntryTheaterSpawnPoint s in validSP select s);

                int iterationsLeft = MAX_RADIUS_SEARCH_ITERATIONS;

                do
                {
                    Coordinates origin = distanceOrigin[i].Value;

                    validSPInRange = (from DBEntryTheaterSpawnPoint s in validSP
                                        where 
                                            searchRange.Contains(origin.GetDistanceFrom(s.Coordinates)) &&
                                            CheckNotInHostileCoords(s.Coordinates, coalition) && 
                                            CheckNotInNoSpawnCoords(s.Coordinates)
                                      select s);
                    searchRange = new MinMaxD(searchRange.Min * 0.9, Math.Max(100, searchRange.Max * 1.1));
                    validSP = (from DBEntryTheaterSpawnPoint s in validSPInRange select s);
                    iterationsLeft--;
                } while ((validSPInRange.Count() == 0) && (iterationsLeft > 0));
            }

            if (validSP.Count() == 0) return null;
            DBEntryTheaterSpawnPoint selectedSpawnPoint = Toolbox.RandomFrom(validSP.ToArray());
            SpawnPoints.Remove(selectedSpawnPoint); // Remove spawn point so it won't be used again
            return selectedSpawnPoint.Coordinates;
        }

        private Coordinates? GetAirOrSeaCoordinates(
            SpawnPointType[] validTypes,
            Coordinates distanceOrigin1, MinMaxD distanceFrom1,
            Coordinates? distanceOrigin2 = null, MinMaxD? distanceFrom2 = null,
            Coalition? coalition = null)
        {
            var searchRange = distanceFrom1 * Toolbox.NM_TO_METERS;

            MinMaxD? secondSearchRange = null;
            if (distanceOrigin2.HasValue && distanceFrom2.HasValue)
                secondSearchRange = distanceFrom2.Value * Toolbox.NM_TO_METERS;

            var iterations = 0;
            do
            {
                var coordOptionsLinq = Enumerable.Range(0, 50)
                    .Select(x => Coordinates.CreateRandom(distanceOrigin1, searchRange))
                    .Where(x => CheckNotInHostileCoords(x) && CheckNotInNoSpawnCoords(x));

                if (secondSearchRange.HasValue)
                    coordOptionsLinq = coordOptionsLinq.Where(x => secondSearchRange.Value.Contains(distanceOrigin2.Value.GetDistanceFrom(x)));

                if (validTypes.First() == SpawnPointType.Sea) //sea position
                    coordOptionsLinq = coordOptionsLinq.Where(x => ShapeManager.IsPosValid(x, TheaterDB.WaterCoordinates, TheaterDB.WaterExclusionCoordinates));

                var coordOptions = coordOptionsLinq.ToArray();
                if (coordOptions.Count() > 0)
                    return Toolbox.RandomFrom(coordOptions);

                searchRange = new MinMaxD(searchRange.Min * 0.9, searchRange.Max * 1.1);

                if (secondSearchRange.HasValue)
                    secondSearchRange = new MinMaxD(secondSearchRange.Value.Min * 0.9, secondSearchRange.Value.Max * 1.1);

                iterations++;
            } while (iterations < MAX_RADIUS_SEARCH_ITERATIONS);

            return null;
        }

        public Tuple<DBEntryAirbase, List<int>, List<Coordinates>> GetAirbaseAndParking(MissionTemplate template, Coordinates coordinates, int unitCount, Coalition coalition, bool requiresOpenAirParking)
        {
            var targetAirbaseOptions =
                        (from DBEntryAirbase airbaseDB in SituationDB.GetAirbases(template.OptionsMission.Contains("InvertCountriesCoalitions"))
                         where airbaseDB.Coalition == coalition
                         select airbaseDB).OrderBy(x => x.Coordinates.GetDistanceFrom(coordinates));

            if (targetAirbaseOptions.Count() == 0) throw new BriefingRoomException("No airbase found for aircraft.");

            DBEntryAirbase targetAirbase = targetAirbaseOptions.First(x => AirbaseParkingSpots[x.DCSID].Count() >= unitCount);

            var objectiveCoordinates = targetAirbase.Coordinates;
            var airbaseID = targetAirbase.DCSID;
            List<int> parkingSpotIDsList = new List<int>();
            List<Coordinates> parkingSpotCoordinatesList = new List<Coordinates>();

            var parkingSpots = GetFreeParkingSpots(airbaseID, unitCount, requiresOpenAirParking);
            parkingSpotIDsList = parkingSpots.Select(x => x.DCSID).ToList();
            parkingSpotCoordinatesList = parkingSpots.Select(x => x.Coordinates).ToList();

            return Tuple.Create(targetAirbase, parkingSpotIDsList, parkingSpotCoordinatesList);
        }

        private bool CheckNotInHostileCoords(Coordinates coordinates, Coalition? coalition = null)
        {
            if (!coalition.HasValue)
                return true;

            var red = SituationDB.GetRedZone(InvertCoalition);
            var blue = SituationDB.GetBlueZone(InvertCoalition);

            if (coalition.Value == Coalition.Blue)
                return !ShapeManager.IsPosValid(coordinates, red);
            return !ShapeManager.IsPosValid(coordinates, blue);
        }

        private bool CheckNotInNoSpawnCoords(Coordinates coordinates)
        {
            if(SituationDB.NoSpawnCoordinates is null)
                return true;
            return !ShapeManager.IsPosValid(coordinates, SituationDB.NoSpawnCoordinates);
        }
    }
}
