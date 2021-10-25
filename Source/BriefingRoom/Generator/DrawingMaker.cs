using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class DrawingMaker : IDisposable
    {

        private readonly DCSMission Mission;
        private readonly MissionTemplate Template;
        private readonly DBEntryTheater TheaterDB;
        private readonly List<string> LuaDrawings = new List<string>();

        internal DrawingMaker(
            DCSMission mission, MissionTemplate template, DBEntryTheater theaterDB)
        {
            Mission = mission;
            Template = template;
            TheaterDB = theaterDB;
            Clear();
            AddTheaterZones();
        }

        internal void Clear()
        {
            LuaDrawings.Clear();
        }


        internal void AddDrawing(
            DrawingType type,
            Coordinates coordinates,
            params KeyValuePair<string, object>[] extraSettings)
        {
            switch (type)
            {
                case DrawingType.Free:
                    AddFree(coordinates, extraSettings);
                    break;
                case DrawingType.Circle:
                    AddOval(coordinates, extraSettings);
                    break;
                case DrawingType.TextBox:
                    AddTextBox(coordinates, extraSettings);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Not expected drawing value: {type}");

            };
        }

        private void AddFree(Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\Free.lua");
            string freePosLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\FreePos.lua");
            
            var points = "";
            var index = 1;
            foreach (Coordinates pos in (List<Coordinates>)extraSettings.First(x => x.Key == "Points").Value)
            {
                var templateLua = new String(freePosLuaTemplate);
                GeneratorTools.ReplaceKey(ref templateLua, "Index", index);
                GeneratorTools.ReplaceKey(ref templateLua, "X", pos.X);
                GeneratorTools.ReplaceKey(ref templateLua, "Y", pos.Y);
                points += $"{templateLua}\n";
                index++;
            }
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "POINTS", points);
            AddToList(drawingLuaTemplate, coordinates);
        }

        private void AddOval(Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\Circle.lua");
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Radius", extraSettings.First(x => x.Key == "Radius").Value);  
            AddToList(drawingLuaTemplate, coordinates); 
        }

        private void AddTextBox(Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\TextBox.lua");
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Text", extraSettings.First(x => x.Key == "Text").Value);  
            AddToList(drawingLuaTemplate, coordinates);  
        }

        private void AddToList(string template, Coordinates coordinates)
        {
            GeneratorTools.ReplaceKey(ref template, "X", coordinates.X);
            GeneratorTools.ReplaceKey(ref template, "Y", coordinates.Y);
            LuaDrawings.Add(template);
        }

        private void AddTheaterZones()
        {
            if(!TheaterDB.ShapeSpawnSystem || Template.OptionsMission.Contains(MissionOption.ForceOldSpawning))
                return;
            AddFree(TheaterDB.RedCoordinates.First(), "Points".ToKeyValuePair(TheaterDB.RedCoordinates));
            AddFree(TheaterDB.RedCoordinates.First(), "Points".ToKeyValuePair(TheaterDB.BlueCoordinates));
        }

        internal string GetLuaDrawings()
        {
            string luaDrawings = "";

            var index = 1;
            foreach (var drawing in LuaDrawings)
            {
                var drawingLua = drawing;
                GeneratorTools.ReplaceKey(ref drawingLua, "Index", index);
                luaDrawings += $"{drawingLua}\n";
                index++;
            }
            return luaDrawings;
        }

        public void Dispose()
        {
        }
    }
}