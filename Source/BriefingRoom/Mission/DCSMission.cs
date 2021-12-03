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
    public sealed class DCSMission : IDisposable
    {
        private const int MAX_VALUE_LENGTH_DISPLAY = 16;

        internal string UniqueID { get; }

        public string TheaterID { get { return GetValue("TheaterID"); } }

        public DCSMissionBriefing Briefing { get; }

        private readonly Dictionary<string, string> Values;

        private readonly Dictionary<string, object> MediaFiles;

        internal Dictionary<int, Coalition> Airbases { get; }

        internal string ReplaceValues(string rawText)
        {
            if (rawText == null) return null;

            foreach (KeyValuePair<string, string> keyPair in Values) // Replace included scripts first so later replacements (objective count, player coalition, etc) will be applied to them too
            {
                if (!keyPair.Key.ToLowerInvariant().StartsWith("script")) continue;
                rawText = rawText.Replace($"${keyPair.Key.ToUpperInvariant()}$", keyPair.Value);
            }

            foreach (KeyValuePair<string, string> keyPair in Values) // Replace other values
                rawText = rawText.Replace($"${keyPair.Key.ToUpperInvariant()}$", keyPair.Value);

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
            MediaFiles = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            UniqueID = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).ToLowerInvariant();
            SetValue("MissionID", UniqueID);
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
            SetValue(key, value.ToString(NumberFormatInfo.InvariantInfo).ToLowerInvariant(), false);
        }

        internal void SetValue(string key, string value)
        {
            SetValue(key, value, false);
        }

        internal void AppendValue(string key, string value)
        {
            SetValue(key, value, true);
        }

        private void SetValue(string key, string value, bool append)
        {
            if (string.IsNullOrEmpty(key)) return;
            key = key.ToUpperInvariant();
            value = value ?? "";
            value = value.Replace("\r\n", "\n");

            string displayedValue = value.Replace("\n", " ");
            if (displayedValue.Length > MAX_VALUE_LENGTH_DISPLAY) displayedValue = displayedValue.Substring(0, MAX_VALUE_LENGTH_DISPLAY) + "...";

            BriefingRoom.PrintToLog($"Mission parameter \"{key.ToLowerInvariant()}\" {(append ? "appended with" : "set to")} \"{displayedValue}\".");

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
                BriefingRoom.PrintToLog(ex.Message, LogMessageErrorLevel.Error);
                return false;
            }
        }

        public byte[] SaveToMizBytes()
        {
            using (MizMaker mizMaker = new MizMaker())
                return mizMaker.ExportToMizBytes(this);
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
                double.Parse(GetValue("MissionCenterX")),
                double.Parse(GetValue("MissionCenterY")));
            var playerAirbase = new Coordinates(
                double.Parse(GetValue("MissionAirbaseX")),
                double.Parse(GetValue("MissionAirbaseY")));
            distance = objCenter.GetDistanceFrom(playerAirbase) * Toolbox.METERS_TO_NM;
            var extremeLimit = template.FlightPlanObjectiveDistance * 1.7;
            return distance > extremeLimit;
        }

        public void Dispose()
        {

        }
    }
}

