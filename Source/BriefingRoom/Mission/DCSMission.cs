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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.Mission
{
    public sealed class DCSMission
    {
        private const int MAX_VALUE_LENGTH_DISPLAY = 16;

        internal string UniqueID { get; }

        public string TheaterID { get { return GetValue("TheaterID"); } }

        public DCSMissionBriefing Briefing { get; }

        internal Dictionary<string, List<double[]>> MapData { get; }

        private readonly Dictionary<string, string> Values;

        private readonly Dictionary<string, object> MediaFiles;

        internal HashSet<string> SingletonSet { get; }

        internal Dictionary<int, Coalition> Airbases { get; }

        internal Dictionary<Coalition, List<int>> PopulatedAirbaseIds { get; }

        internal List<DCSMissionPackage> MissionPackages { get; }


        internal string ReplaceValues(string rawText)
        {
            if (rawText == null) return null;

            foreach (KeyValuePair<string, string> keyPair in Values) // Replace included scripts first so later replacements (objective count, player coalition, etc) will be applied to them too
            {
                if (!keyPair.Key.ToLower().StartsWith("script")) continue;
                rawText = rawText.Replace($"${keyPair.Key.ToUpper()}$", keyPair.Value);
            }

            foreach (KeyValuePair<string, string> keyPair in Values) // Replace other values
                rawText = rawText.Replace($"${keyPair.Key.ToUpper()}$", keyPair.Value);

            return rawText;
        }

        internal void SetAirbase(int airbaseID, Coalition airbaseCoalition)
        {
            if (Airbases.ContainsKey(airbaseID))
                Airbases[airbaseID] = airbaseCoalition;
            else
                Airbases.Add(airbaseID, airbaseCoalition);
        }

        internal DCSMission()
        {
            Airbases = new Dictionary<int, Coalition>();
            Briefing = new DCSMissionBriefing(this);
            MissionPackages = new List<DCSMissionPackage>();
            MediaFiles = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            SingletonSet = new HashSet<string>();
            MapData = new Dictionary<string, List<double[]>>();
            PopulatedAirbaseIds = new Dictionary<Coalition, List<int>>{
                    {Coalition.Blue, new List<int>()},
                    {Coalition.Red, new List<int>()},
                    {Coalition.Neutral, new List<int>()}
                };

            UniqueID = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).ToLower();
            SetValue("MissionID", UniqueID);
            SetValue("ScriptMIST", Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_LUA, "MIST.lua")));
            SetValue("ScriptSingletons", "");
        }

        internal void SetValue(string key, int value)
        {
            SetValue(key, value.ToString(NumberFormatInfo.InvariantInfo), false);
        }

        internal void SetValue(string key, double value)
        {
            SetValue(key, value.ToString(NumberFormatInfo.InvariantInfo), false);
        }

        internal void SetValue(string key, bool value)
        {
            SetValue(key, value.ToString(NumberFormatInfo.InvariantInfo).ToLower(), false);
        }

        internal void SetValue(string key, string value)
        {
            SetValue(key, value, false);
        }

        internal void AppendValue(string key, string value)
        {
            SetValue(key, value, true);
        }

        internal void AppendSingletonValue(string id, string key, string value)
        {
            if (SingletonSet.Contains(id))
                return;
            AppendValue(key, value);
            SingletonSet.Add(id);
        }

        private void SetValue(string key, string value, bool append)
        {
            if (string.IsNullOrEmpty(key)) return;
            key = key.ToUpper();
            value = value ?? "";
            value = value.Replace("\r\n", "\n");

            string displayedValue = value.Replace("\n", " ");
            if (displayedValue.Length > MAX_VALUE_LENGTH_DISPLAY) displayedValue = displayedValue.Substring(0, MAX_VALUE_LENGTH_DISPLAY) + "...";

            BriefingRoom.PrintToLog($"Mission parameter \"{key.ToLower()}\" {(append ? "appended with" : "set to")} \"{displayedValue}\".");

            if (!Values.ContainsKey(key))
                Values.Add(key, value);
            else
                Values[key] = append ? Values[key] + value : value;
        }

        internal string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            if (!Values.ContainsKey(key)) return "";
            return Values[key];
        }

        internal void AddMediaFile(string fileName, string sourceFilePath)
        {
            if (string.IsNullOrEmpty(fileName) || MediaFiles.ContainsKey(fileName)) return;
            if (!File.Exists(sourceFilePath)) return;
            MediaFiles.Add(fileName, sourceFilePath);
        }

        internal void AddMediaFolder(string folderName, string sourceFolderPath)
        {
            if (!Directory.Exists(sourceFolderPath)) return;
            foreach (string file in Directory.EnumerateFiles(sourceFolderPath, "*.*", SearchOption.AllDirectories))
            {
                var filePath = file.Split(sourceFolderPath)[1];
                if (!MediaFiles.ContainsKey($"{folderName}{filePath}"))
                {
                    var endPath = $"{folderName}{filePath}".Replace("\\", "/");
                    MediaFiles.Add(endPath, sourceFolderPath + filePath);
                }
            }

        }

        internal void AddMediaFile(string fileName, byte[] mediaFileBytes)
        {
            if (string.IsNullOrEmpty(fileName) || MediaFiles.ContainsKey(fileName)) return;
            MediaFiles.Add(fileName, mediaFileBytes);
        }

        public bool SaveToMizFile(string mizFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(mizFilePath)) return false;
                if (!Toolbox.IsFilePathValid(mizFilePath)) return false;

                byte[] mizBytes = SaveToMizBytes();
                if (mizBytes == null) return false; // Something went wrong

                if (File.Exists(mizFilePath)) File.Delete(mizFilePath);

                File.WriteAllBytes(mizFilePath, mizBytes);
                return true;
            }
            catch (Exception ex)
            {
                throw new(ex.Message);
            }
        }

        public byte[] SaveToMizBytes(MissionTemplate template = null)
        {
            return MizMaker.ExportToMizBytes(this, template);
        }

        internal string[] GetMediaFileNames()
        {
            return MediaFiles.Keys.ToArray();
        }

        internal byte[] GetMediaFile(string mediaFileName)
        {
            if (!MediaFiles.ContainsKey(mediaFileName)) return null;
            if (MediaFiles[mediaFileName] is byte[] fileBytes) return fileBytes;
            if (MediaFiles[mediaFileName] is string filePath)
            {
                if (!File.Exists(filePath)) return null;
                return File.ReadAllBytes(filePath);
            }
            return null;
        }

        internal bool IsExtremeDistance(MissionTemplate template, out double distance)
        {
            var objCenter = new Coordinates(
                double.Parse(GetValue("MissionCenterX"), CultureInfo.InvariantCulture),
                double.Parse(GetValue("MissionCenterY"), CultureInfo.InvariantCulture));
            var playerAirbase = new Coordinates(
                double.Parse(GetValue("MissionAirbaseX"), CultureInfo.InvariantCulture),
                double.Parse(GetValue("MissionAirbaseY"), CultureInfo.InvariantCulture));
            distance = objCenter.GetDistanceFrom(playerAirbase) * Toolbox.METERS_TO_NM;
            var extremeLimit = template.FlightPlanObjectiveDistanceMax * 1.7;
            var isTooFar = distance > extremeLimit;
            if (isTooFar)
                BriefingRoom.PrintToLog($"Distance too far {distance}NM > {extremeLimit}NM", LogMessageErrorLevel.Warning);
            return isTooFar;
        }

        public Dictionary<string, List<double[]>> GetMapData()
        {
            return MapData;
        }
    }
}

