using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class DrawingMaker
    {

        private readonly DCSMission Mission;
        private readonly MissionTemplateRecord Template;
        private readonly DBEntryTheater TheaterDB;
        private readonly DBEntrySituation SituationDB;
        private readonly List<string> LuaDrawings = new List<string>();

        internal DrawingMaker(
            DCSMission mission, MissionTemplateRecord template, DBEntryTheater theaterDB, DBEntrySituation situationDB)
        {
            Mission = mission;
            Template = template;
            TheaterDB = theaterDB;
            SituationDB = situationDB;
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
            string drawingLuaTemplate = File.ReadAllText(Path.Combine(BRPaths.INCLUDE_LUA_MISSION, "Drawing", "Free.lua"));
            string freePosLuaTemplate = File.ReadAllText(Path.Combine(BRPaths.INCLUDE_LUA_MISSION, "Drawing", "FreePos.lua"));

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
            string drawingLuaTemplate = File.ReadAllText(Path.Combine(BRPaths.INCLUDE_LUA_MISSION, "Drawing", "Circle.lua"));
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Radius", extraSettings.First(x => x.Key == "Radius").Value);
            DrawingColour colour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "Colour").Value ?? DrawingColour.Red);
            DrawingColour fillColour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "FillColour").Value ?? DrawingColour.Clear);
            AddToList(UIName, drawingLuaTemplate, coordinates, colour, fillColour);
        }

        private void AddTextBox(string UIName, Coordinates coordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            string drawingLuaTemplate = File.ReadAllText(Path.Combine(BRPaths.INCLUDE_LUA_MISSION, "Drawing", "TextBox.lua"));
            GeneratorTools.ReplaceKey(ref drawingLuaTemplate, "Text", extraSettings.First(x => x.Key == "Text").Value);
            DrawingColour colour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "Colour").Value ?? DrawingColour.Black);
            DrawingColour fillColour = (DrawingColour)(extraSettings.FirstOrDefault(x => x.Key == "FillColour").Value ?? DrawingColour.WhiteBackground);
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
            DrawWaterAndIslands();
            if (Template.OptionsMission.Contains("SpawnAnywhere") || Template.ContextSituation == "None")
                return;
            var invertCoalition = Template.OptionsMission.Contains("InvertCountriesCoalitions");
            var hideBorders = Template.OptionsMission.Contains("HideBorders");
            var red = SituationDB.GetRedZones(invertCoalition);
            var redColour = hideBorders ? DrawingColour.Clear : DrawingColour.RedFill;

            var blue = SituationDB.GetBlueZones(invertCoalition);
            var blueColour = hideBorders ? DrawingColour.Clear : DrawingColour.BlueFill;
            foreach (var zone in red)
            {
                AddFree(
                    "Red Control",
                    zone.First(),
                    "Points".ToKeyValuePair(zone.Select(coord => coord - zone.First()).ToList()),
                    "Colour".ToKeyValuePair(redColour),
                    "FillColour".ToKeyValuePair(redColour));
                Mission.MapData.Add($"RED_{red.IndexOf(zone)}", zone.Select(x => x.ToArray()).ToList());
            }
            foreach (var zone in blue)
            {

                AddFree(
                    "Blue Control",
                    zone.First(),
                    "Points".ToKeyValuePair(zone.Select(coord => coord - zone.First()).ToList()),
                    "Colour".ToKeyValuePair(blueColour),
                    "FillColour".ToKeyValuePair(blueColour));
                Mission.MapData.Add($"BLUE_{blue.IndexOf(zone)}", zone.Select(x => x.ToArray()).ToList());
            }

            if (SituationDB.NoSpawnZones.Count > 0)
            {
                var noSpawn = SituationDB.NoSpawnZones;
                var noSpawnColour = hideBorders ? DrawingColour.Clear : DrawingColour.GreenFill;
                foreach (var zone in noSpawn)
                {
                    AddFree(
                        "Neutural (NoSpawning)",
                        zone.First(),
                        "Points".ToKeyValuePair(zone.Select(coord => coord - zone.First()).ToList()),
                        "Colour".ToKeyValuePair(noSpawnColour),
                        "FillColour".ToKeyValuePair(noSpawnColour));
                    Mission.MapData.Add($"NOSPAWN_{noSpawn.IndexOf(zone)}", zone.Select(x => x.ToArray()).ToList());
                }
            }

        }

        private void DrawWaterAndIslands()
        {
            // DEBUG water
            var i = 0;
            foreach (var item in TheaterDB.WaterCoordinates)
            {
                AddFree(
                    "Water",
                    item.First(),
                    "Points".ToKeyValuePair(item.Select(coord => coord - item.First()).ToList()),
                    "Colour".ToKeyValuePair(DrawingColour.Clear),
                    "FillColour".ToKeyValuePair(DrawingColour.Clear));
                Mission.MapData.Add($"WATER_{i}", item.Select(x => x.ToArray()).ToList());
                i++;
            }
            i = 0;
            foreach (var item in TheaterDB.WaterExclusionCoordinates)
            {
                AddFree(
                    "Water Exclusion",
                    item.First(),
                    "Points".ToKeyValuePair(item.Select(coord => coord - item.First()).ToList()),
                    "Colour".ToKeyValuePair(DrawingColour.Clear),
                    "FillColour".ToKeyValuePair(DrawingColour.Clear));
                Mission.MapData.Add($"ISLAND_{i}", item.Select(x => x.ToArray()).ToList());
                i++;
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
    }
}