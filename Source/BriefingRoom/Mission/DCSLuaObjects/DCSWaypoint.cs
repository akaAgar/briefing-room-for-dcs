using System;
using System.Collections.Generic;
using System.Linq;
using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Generator;
using LuaTableSerializer;

namespace BriefingRoom4DCS.Mission.DCSLuaObjects
{
    public class DCSWaypoint
    {

        public int Alt { get; set; }
        public string AltType { get; set; } = "BARO";
        public string Action { get; set; }
        public double Speed { get; set; }
        private List<DCSWaypointTask> _tasks = new List<DCSWaypointTask>();
        public List<DCSWaypointTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = SortTasks(value); }
        }
        public string Type { get; set; }
        public bool EtaLocked { get; set; }

        public bool SpeedLocked { get; set; }
        public double Y { get; set; }
        public double X { get; set; }
        public string Name { get; set; }

        public int AirdromeId { get; set; }

        public int LinkUnit { get; set; }

        public int HelipadId { get; set; }

        internal Coordinates Coordinates { get { return new Coordinates(X, Y); } }

        private List<DCSWaypointTask> SortTasks(List<DCSWaypointTask> tasks) => tasks.Select(x => x.parameters.ContainsKey("wrapped") ? new DCSWrappedWaypointTask(x) : x).ToList();

        public string ToLuaString(int number)
        {
            var obj = new Dictionary<string, object> {
                {"alt", Alt},
                {"action", Action},
                {"alt_type", AltType},
                {"speed", Speed},
                {"task", new Dictionary<string, object>{
                    {"id", "ComboTask"},
                    {"params", new Dictionary<string, object>{
                        {"tasks", Tasks.OrderBy(x => x.Priority).ToList()}
                    }}
                    }
                },
                {"type", Type},
                {"ETA", 0},
                {"ETA_locked", EtaLocked},
                {"x", X},
                {"y", Y},
                {"name", Name},
                {"formation_template", ""},
                {"speed_locked", SpeedLocked}
            };
            if (AirdromeId != default && LinkUnit == default)
                obj.Add("airdromeId", AirdromeId);
            if (LinkUnit != default)
                obj.Add("linkUnit", LinkUnit);
            if (HelipadId != default)
                obj.Add("helipadId", HelipadId);
            return LuaSerializer.Serialize(obj);
        }

        internal static List<DCSWaypoint> CreateExtraWaypoints(List<DCSWaypoint> waypoints, UnitFamily unitFamily, UnitMakerSpawnPointSelector spawnPointSelector)
        {
            var firstWP = waypoints.First();
            var lastWP = waypoints.Last();
            if (firstWP == lastWP)
                return waypoints;
            var L = CalculateParallelVector(waypoints);
            var mid1 = OffsetWaypoint(waypoints, firstWP.Coordinates, L, unitFamily);
            var mid3 = OffsetWaypoint(waypoints, lastWP.Coordinates, L, unitFamily);
            var mid2 = OffsetWaypoint(waypoints, Coordinates.Lerp(firstWP.Coordinates, lastWP.Coordinates, new MinMaxD(0.2, 0.7).GetValue()), L, unitFamily);
            var lastWaypoint = waypoints.Last();
            var extraWaypoints = new List<DCSWaypoint>();

            foreach (var waypointCoords in new Coordinates[] { mid1, mid2, mid3 })
            {
                var tempWaypointCoords = waypointCoords;
                if (new Random().NextDouble() <= 0.5)
                    continue; 
                
                if(unitFamily.GetUnitCategory() == UnitCategory.Vehicle || unitFamily.GetUnitCategory() == UnitCategory.Infantry)
                {
                    var waypointCoordsSpawn = spawnPointSelector.GetNearestSpawnPoint(new SpawnPointType[] { SpawnPointType.LandLarge, SpawnPointType.LandMedium, SpawnPointType.LandSmall }, tempWaypointCoords, false);
                    if(!waypointCoordsSpawn.HasValue)
                        continue;
                    tempWaypointCoords = waypointCoordsSpawn.Value;
                }
            
                extraWaypoints.Add(new DCSWaypoint
                {
                    Alt = lastWaypoint.Alt,
                    AltType = lastWaypoint.AltType,
                    Action = "Turning Point",
                    Speed = lastWaypoint.Speed,
                    Type = "Turning Point",
                    EtaLocked = false,
                    SpeedLocked = true,
                    X = tempWaypointCoords.X,
                    Y = tempWaypointCoords.Y,
                });
            }

            waypoints.InsertRange(waypoints.Count - 1, extraWaypoints);
            return waypoints;
        }

        private static double CalculateParallelVector(List<DCSWaypoint> waypoints)
        {
            var firstWP = waypoints.First();
            var lastWp = waypoints.Last();
            return Math.Sqrt((firstWP.X - lastWp.X) * (firstWP.X - lastWp.X) + (firstWP.Y - lastWp.Y) * (firstWP.Y - lastWp.Y));
        }

        private static Coordinates OffsetWaypoint(List<DCSWaypoint> waypoints, Coordinates waypoint, double L, UnitFamily unitFamily)
        {

            var offsetRange = unitFamily.GetUnitCategory() switch
            {
                UnitCategory.Plane => new MinMaxD(-30, 30),
                UnitCategory.Helicopter => new MinMaxD(-20, 20),
                _ => unitFamily == UnitFamily.InfantryMANPADS || unitFamily == UnitFamily.Infantry ? new MinMaxD(-5, 5) :  new MinMaxD(-8, 8)
            };
            var randomRange = unitFamily.GetUnitCategory() switch
            {
                UnitCategory.Plane => 20,
                UnitCategory.Helicopter => 10,
                _ => unitFamily == UnitFamily.InfantryMANPADS || unitFamily == UnitFamily.Infantry ? 1 :  3
            };

            var offsetPixels = offsetRange.GetValue() * Toolbox.NM_TO_METERS;
            var firstWP = waypoints.First();
            var lastWp = waypoints.Last();
            var x1p = waypoint.X + offsetPixels * (lastWp.Y - firstWP.Y) / L;
            var y1p = waypoint.Y + offsetPixels * (firstWP.X - lastWp.X) / L;
            return new Coordinates(x1p, y1p).CreateNearRandom(0, randomRange * Toolbox.NM_TO_METERS);
        }
    }
}