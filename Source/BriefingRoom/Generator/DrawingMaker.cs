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
            string UIName,
            DrawingType type,
            Coordinates coordinates,
            params KeyValuePair<string, object>[] extraSettings)
        {
            switch (type)
            {
                case DrawingType.Free:
                    AddFree(UIName, coordinates, extraSettings);
                    break;
                case DrawingType.Circle:
                    AddOval(UIName, coordinates, extraSettings);
                    break;
                case DrawingType.TextBox:
                    AddTextBox(UIName, coordinates, extraSettings);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Not expected drawing value: {type}");

            };
        }

        private void AddFree(string UIName, Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
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
            DrawingColour colour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "Colour").Value ?? DrawingColour.Red);
            DrawingColour fillColour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "FillColour").Value ?? DrawingColour.RedFill);
            AddToList(UIName, drawingLuaTemplate, coordinates, colour, fillColour);
        }

        private void AddOval(string UIName, Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\Circle.lua");
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Radius", extraSettings.First(x => x.Key == "Radius").Value);
            DrawingColour colour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "Colour").Value ?? DrawingColour.Red);
            DrawingColour fillColour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "FillColour").Value ?? DrawingColour.Clear);
            AddToList(UIName, drawingLuaTemplate, coordinates, colour, fillColour);
        }

        private void AddTextBox(string UIName, Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_MISSION}\\Drawing\\TextBox.lua");
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Text", extraSettings.First(x => x.Key == "Text").Value);
            DrawingColour colour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "Colour").Value ?? DrawingColour.Red);
            DrawingColour fillColour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "FillColour").Value ?? DrawingColour.Clear);
            AddToList(UIName, drawingLuaTemplate, coordinates, colour, fillColour);
        }

        private void AddToList(string UIName, string template, Coordinates coordinates, DrawingColour colour, DrawingColour fillColour)
        {
            GeneratorTools.ReplaceKey(ref template, "UIName", UIName);
            GeneratorTools.ReplaceKey(ref template, "X", coordinates.X);
            GeneratorTools.ReplaceKey(ref template, "Y", coordinates.Y);
            GeneratorTools.ReplaceKey(ref template, "Colour", colour.ToValue());
            GeneratorTools.ReplaceKey(ref template, "FillColour", fillColour.ToValue());
            LuaDrawings.Add(template);
        }

        private void AddTheaterZones()
        {
            AddFree("Red Control", TheaterDB.RedCoordinates.First(), "Points".ToKeyValuePair(TheaterDB.RedCoordinates.Select(coord => coord - TheaterDB.RedCoordinates.First()).ToList()), "Colour".ToKeyValuePair(DrawingColour.RedFill));
            AddFree(
                "Blue Control",
                TheaterDB.BlueCoordinates.First(),
                "Points".ToKeyValuePair(TheaterDB.BlueCoordinates.Select(coord => coord - TheaterDB.BlueCoordinates.First()).ToList()),
                "Colour".ToKeyValuePair(DrawingColour.BlueFill),
                "FillColour".ToKeyValuePair(DrawingColour.BlueFill));

            // DEBUG water
            AddFree(
                "Water",
                TheaterDB.WaterCoordinates.First(),
                "Points".ToKeyValuePair(TheaterDB.WaterCoordinates.Select(coord => coord - TheaterDB.WaterCoordinates.First()).ToList()),
                "Colour".ToKeyValuePair(DrawingColour.Clear),
                "FillColour".ToKeyValuePair(DrawingColour.Clear));

            foreach (var item in TheaterDB.WaterExclusionCoordinates)
            {
                AddFree(
                    "Water Exclusion",
                    item.First(),
                    "Points".ToKeyValuePair(item.Select(coord => coord - item.First()).ToList()),
                    "Colour".ToKeyValuePair(DrawingColour.Clear),
                    "FillColour".ToKeyValuePair(DrawingColour.Clear));
            }

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