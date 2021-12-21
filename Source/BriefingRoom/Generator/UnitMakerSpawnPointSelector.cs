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

            if (TheaterDB.SpawnPoints is not null)
                SpawnPoints.AddRange(TheaterDB.SpawnPoints.Where(x => CheckNotInNoSpawnCoords(x.Coordinates)).ToList());

            foreach (DBEntryAirbase airbase in SituationDB.GetAirbases(InvertCoalition))
            {
                if (airbase.ParkingSpots.Length < 1) continue;
                if (AirbaseParkingSpots.ContainsKey(airbase.DCSID)) continue;
                AirbaseParkingSpots.Add(airbase.DCSID, airbase.ParkingSpots.ToList());
            }
        }

        internal List<DBEntryAirbaseParkingSpot> GetFreeParkingSpots(int airbaseID, int unitCount, UnitFamily unitFamily, bool requiresOpenAirParking = false)
        {

            if (!AirbaseParkingSpots.ContainsKey(airbaseID))
                throw new BriefingRoomException($"Airbase {airbaseID} not found in parking map");


            var airbaseDB = SituationDB.GetAirbases(InvertCoalition).First(x => x.DCSID == airbaseID);
            var parkingSpots = new List<DBEntryAirbaseParkingSpot>();
            DBEntryAirbaseParkingSpot? lastSpot = null;
            for (int i = 0; i < unitCount; i++)
            {
                var viableSpots = FilterAndSortSuitableSpots(AirbaseParkingSpots[airbaseID].ToArray(), unitFamily, requiresOpenAirParking);
                if (viableSpots.Count == 0) throw new BriefingRoomException("Airbase didn't have enough suitable parking spots.");
                var parkingSpot = viableSpots.First();
                if (lastSpot.HasValue) //find nearest spot distance wise in attempt to cluster
                    parkingSpot = viableSpots
                        .Aggregate((acc, x) => acc.Coordinates.GetDistanceFrom(lastSpot.Value.Coordinates) > x.Coordinates.GetDistanceFrom(lastSpot.Value.Coordinates) ? x : acc);

                lastSpot = parkingSpot;
                AirbaseParkingSpots[airbaseID].Remove(parkingSpot);
                parkingSpots.Add(parkingSpot);
            }

            return parkingSpots;
        }

        internal Coordinates? GetNearestSpawnPoint(
            SpawnPointType[] validTypes,
            Coordinates origin)
        {
            if (validTypes.Contains(SpawnPointType.Air) || validTypes.Contains(SpawnPointType.Sea))
                return Coordinates.CreateRandom(origin, new MinMaxD(1, 3));
            var sp = SpawnPoints.Where(x => validTypes.Contains(x.PointType)).Aggregate((acc, x) => origin.GetDistanceFrom(x.Coordinates) < origin.GetDistanceFrom(acc.Coordinates)? x : acc);
            SpawnPoints.Remove(sp);
            return sp.Coordinates;
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
                                          CheckNotInHostileCoords(s.Coordinates, coalition)
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
                    .Where(x => CheckNotInHostileCoords(x, coalition) && CheckNotInNoSpawnCoords(x));

                if (secondSearchRange.HasValue)
                    coordOptionsLinq = coordOptionsLinq.Where(x => secondSearchRange.Value.Contains(distanceOrigin2.Value.GetDistanceFrom(x)));

                if (validTypes.First() == SpawnPointType.Sea) //sea position
                    coordOptionsLinq = coordOptionsLinq.Where(x => ShapeManager.IsPosValid(x, TheaterDB.WaterCoordinates, TheaterDB.WaterExclusionCoordinates));

                var coordOptions = coordOptionsLinq.ToList();
                if (coordOptions.Count > 0)
                    return Toolbox.RandomFrom(coordOptions);

                searchRange = new MinMaxD(searchRange.Min * 0.9, searchRange.Max * 1.1);

                if (secondSearchRange.HasValue)
                    secondSearchRange = new MinMaxD(secondSearchRange.Value.Min * 0.9, secondSearchRange.Value.Max * 1.1);

                iterations++;
            } while (iterations < MAX_RADIUS_SEARCH_ITERATIONS);

            return null;
        }

        internal Tuple<DBEntryAirbase, List<int>, List<Coordinates>> GetAirbaseAndParking(
            MissionTemplateRecord template, Coordinates coordinates,
            int unitCount, Coalition coalition, UnitFamily unitFamily)
        {
            var targetAirbaseOptions =
                        (from DBEntryAirbase airbaseDB in SituationDB.GetAirbases(template.OptionsMission.Contains("InvertCountriesCoalitions"))
                         where airbaseDB.Coalition == coalition && ValidateAirfield(AirbaseParkingSpots[airbaseDB.DCSID], unitFamily, unitCount)
                         select airbaseDB).OrderBy(x => x.Coordinates.GetDistanceFrom(coordinates));

            if (targetAirbaseOptions.Count() == 0) throw new BriefingRoomException("No airbase found for aircraft.");

            var targetAirbase = targetAirbaseOptions.First();
            var objectiveCoordinates = targetAirbase.Coordinates;
            var airbaseID = targetAirbase.DCSID;
            var parkingSpotIDsList = new List<int>();
            var parkingSpotCoordinatesList = new List<Coordinates>();
            var parkingSpots = GetFreeParkingSpots(airbaseID, unitCount, unitFamily);

            parkingSpotIDsList = parkingSpots.Select(x => x.DCSID).ToList();
            parkingSpotCoordinatesList = parkingSpots.Select(x => x.Coordinates).ToList();

            return Tuple.Create(targetAirbase, parkingSpotIDsList, parkingSpotCoordinatesList);
        }

        private List<DBEntryAirbaseParkingSpot> FilterAndSortSuitableSpots(DBEntryAirbaseParkingSpot[] parkingspots, UnitFamily unitFamily, bool requiresOpenAirParking)
        {
            var validTypes = new List<ParkingSpotType>{
                ParkingSpotType.OpenAirSpawn,
                ParkingSpotType.HardenedAirShelter,
                ParkingSpotType.AirplaneOnly
            };

            if (unitFamily.GetUnitCategory() == UnitCategory.Helicopter)
                validTypes = new List<ParkingSpotType>{
                    ParkingSpotType.OpenAirSpawn,
                    ParkingSpotType.HelicopterOnly,
                };
            else if (IsBunkerUnsuitable(unitFamily) || requiresOpenAirParking)
                validTypes = new List<ParkingSpotType>{
                    ParkingSpotType.OpenAirSpawn
                };

            return parkingspots.Where(x => validTypes.Contains(x.ParkingType)).OrderBy(x => x.ParkingType).ToList();
        }

        private bool IsBunkerUnsuitable(UnitFamily unitFamily) =>
            new List<UnitFamily>{
                UnitFamily.PlaneAWACS,
                UnitFamily.PlaneTankerBasket,
                UnitFamily.PlaneTankerBoom,
                UnitFamily.PlaneTransport,
                UnitFamily.PlaneBomber,
            }.Contains(unitFamily) || unitFamily.GetUnitCategory() == UnitCategory.Helicopter;

        private bool ValidateAirfield(List<DBEntryAirbaseParkingSpot> parkingSpots, UnitFamily unitFamily, int unitCount)
        {
            var openSpots = parkingSpots.Count(X => X.ParkingType == ParkingSpotType.OpenAirSpawn);
            if (openSpots >= unitCount) //Is there just enough open spaces
                return true;

            // Helicopters
            if (unitFamily.GetUnitCategory() == UnitCategory.Helicopter)
                return parkingSpots.Count(X => X.ParkingType == ParkingSpotType.HelicopterOnly) + openSpots > unitCount;

            // Aircraft that can't use bunkers
            if (IsBunkerUnsuitable(unitFamily))
                return parkingSpots.Count(X => X.ParkingType == ParkingSpotType.AirplaneOnly) + openSpots > unitCount;

            // Bunkerable aircraft
            return parkingSpots.Count(X => X.ParkingType == ParkingSpotType.HardenedAirShelter) + openSpots > unitCount;
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
            if (SituationDB.NoSpawnCoordinates is null)
                return true;
            return !ShapeManager.IsPosValid(coordinates, SituationDB.NoSpawnCoordinates);
        }
    }
}
