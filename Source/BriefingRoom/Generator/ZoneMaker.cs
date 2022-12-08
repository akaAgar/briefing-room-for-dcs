using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class ZoneMaker
    {
        private readonly UnitMaker _unitMaker;
        private readonly DrawingMaker _drawingMaker;
        private readonly DCSMission _mission;
        private readonly List<string> LuaZones = new List<string>();
        private int CTLDZoneCount = 1;

        internal ZoneMaker(UnitMaker unitMaker, DrawingMaker drawingMaker, DCSMission mission)
        {
            _unitMaker = unitMaker;
            _drawingMaker = drawingMaker;
            _mission = mission;
            CTLDZoneCount = 1;
            Clear();
        }

        internal void Clear()
        {
            LuaZones.Clear();
        }

        internal void AddZone(
            string UIName,
            Coordinates coordinates, int radius) => AddToList(UIName, coordinates, radius);

        internal void AddCTLDPickupZone(Coordinates coordinates, bool onTop = false)
        {
            var coords = coordinates;
            if (!onTop)
            {
                var spawnPoints = new List<SpawnPointType> { SpawnPointType.LandLarge }.ToArray();
                Coordinates? newCoords = _unitMaker.SpawnPointSelector.GetNearestSpawnPoint(spawnPoints, coordinates);
                if (!newCoords.HasValue)
                    throw new BriefingRoomException("Can't find suitable zone Coordinates!");
                coords = newCoords.Value;
                _drawingMaker.AddDrawing($"Supply_{CTLDZoneCount}", DrawingType.TextBox, coords, "Text".ToKeyValuePair($"Supply Base"));
                _drawingMaker.AddDrawing($"Supply_zone_{CTLDZoneCount}", DrawingType.Circle, coords, "Radius".ToKeyValuePair(500), "Colour".ToKeyValuePair(DrawingColour.White));
                _mission.MapData.Add($"SUPPLY_{CTLDZoneCount}", new List<double[]> { coords.ToArray() });
                var group = _unitMaker.AddUnitGroup(UnitFamily.StaticStructureMilitary, 1, Side.Ally, "Static", "Static", coords, UnitMakerGroupFlags.Inert, new Dictionary<string, object>());
                group.Value.DCSGroup.Units[0].Name = $"logistic{CTLDZoneCount}";
            }
            AddToList($"pickzone{CTLDZoneCount}", coords, 500);
            CTLDZoneCount++;
        }

        internal void AddAirbaseZones(List<string> missionFeatures, DBEntryAirbase homeBase, List<DCSMissionPackage> missionPackages)
        {
            if (!missionFeatures.Contains("CTLD"))
                return;
            AddCTLDPickupZone(homeBase.Coordinates);
            missionPackages.ForEach(x => AddCTLDPickupZone(x.Airbase.Coordinates));
        }


        private void AddToList(string UIName, Coordinates coordinates, int radius)
        {
            string template = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Zone.lua");
            GeneratorTools.ReplaceKey(ref template, "NAME", UIName);
            GeneratorTools.ReplaceKey(ref template, "RADIUS", radius);
            GeneratorTools.ReplaceKey(ref template, "X", coordinates.X);
            GeneratorTools.ReplaceKey(ref template, "Y", coordinates.Y);
            GeneratorTools.ReplaceKey(ref template, "zoneId", new Random().Next(100, 500));
            LuaZones.Add(template);
        }

        internal string GetLuaZones()
        {
            string luaDrawings = "";

            var index = 1;
            foreach (var zone in LuaZones)
            {
                var zoneLua = zone;
                GeneratorTools.ReplaceKey(ref zoneLua, "Index", index);
                luaDrawings += $"{zoneLua}\n";
                index++;
            }
            return luaDrawings;
        }
    }
}