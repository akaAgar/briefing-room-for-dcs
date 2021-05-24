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

namespace BriefingRoom4DCS.Mission
{
    /// <summary>
    /// Stores a DCS World mission, with all its unit groups, Lua scripts and parameters.
    /// Can be exported to a <see cref="Miz.MizFile"/> through use of the <see cref="ExportToMiz"/> method.
    /// </summary>
    public sealed class DCSMission : IDisposable
    {
        /// <summary>
        /// If a value is longer than this, <see cref="SetValue(string, string, bool)" /> will not output it to the debug log.
        /// </summary>
        private const int MAX_VALUE_LENGTH_DISPLAY = 16;

        /// <summary>
        /// Unique ID for the mission. Appended to certain filenames so they don't have the same name in every mission and
        /// get confused with one another in the DCS World cache.
        /// </summary>
        internal string UniqueID { get; }

        public DCSMissionBriefing Briefing { get; }

        private Dictionary<string, string> Values { get; }

        private Dictionary<string, byte[]> MediaFiles { get; }

        internal Dictionary<int, Coalition> Airbases { get; }

        public string Name { get { return Values.ContainsKey("MISSION_NAME") ? Values["MISSION_NAME"] : ""; } }

        internal string ReplaceValues(string rawText)
        {
            if (rawText == null) return null;

            foreach (KeyValuePair<string, string> keyPair in Values)
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
            Values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            MediaFiles = new Dictionary<string, byte[]>(StringComparer.InvariantCultureIgnoreCase);
            UniqueID = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).ToLowerInvariant();
            SetValue("MissionID", UniqueID);
            Briefing = new DCSMissionBriefing(this);
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
            SetValue(key, value.ToString(NumberFormatInfo.InvariantInfo), false);
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
            if (MediaFiles.ContainsKey(fileName)) return;
            if (!File.Exists(sourceFilePath)) return;
            MediaFiles.Add(fileName, File.ReadAllBytes(sourceFilePath));
        }

        internal void AddMediaFile(string fileName, byte[] mediaFileBytes)
        {
            if (MediaFiles.ContainsKey(fileName)) return;
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
            return MediaFiles[mediaFileName];
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}

        //        /// <summary>
        //        /// The coalition each airbase on the map belongs to. Key is the airbase ID in DCS World, value is the coalition.
        //        /// </summary>
        //        public Dictionary<int, Coalition> AirbasesCoalition { get; internal set; } = new Dictionary<int, Coalition>();

//        /// <summary>
//        /// List of aircraft groups to spawn during the mission.
//        /// Aircraft groups are activated later on because (1) they would run out of fuel if spawned on start and (2) it's nice to face new enemy CAP units as the mission goes on.
//        /// </summary>
//        public List<DCSMissionAircraftSpawnQueueItem> AircraftSpawnQueue { get; internal set; } = new List<DCSMissionAircraftSpawnQueueItem>();

//        /// <summary>
//        /// Mission briefing, in HTML format.
//        /// </summary>
//        public string BriefingHTML { get; internal set; } = "";

//        /// <summary>
//        /// Mission briefing, in raw text format, as it will be displayed in DCS World.
//        /// </summary>
//        public string BriefingTXT { get; internal set; } = "";

//        /// <summary>
//        /// Location of blue (index #0) and red (index #1) coalition bullseye.
//        /// </summary>
//        public Coordinates[] Bullseye { get; internal set; } = new Coordinates[] { new Coordinates(), new Coordinates() };

//        /// <summary>
//        /// DCS IDs of the carrier(s) the player will take off from.
//        /// </summary>
//        public DCSMissionUnitGroup[] Carriers { get; internal set; }

//        /// <summary>
//        /// ID of the <see cref="DB.DBEntryCoalition"/> of blue (index #0) and red (index #1) coalitions.
//        /// </summary>
//        public string[] Coalitions { get; internal set; } = new string[] { "", "" };

//        /// <summary>
//        /// Countries belonging to the blue (index #0) and red (index #1) coalitions.
//        /// </summary>
//        public List<Country>[] CoalitionCountries { get; internal set; } = new List<Country>[] { new List<Country>(), new List<Country>() };

//        /// <summary>
//        /// Enemy coalition. Read-only, generated from <see cref="CoalitionPlayer"/>.
//        /// </summary>
//        public Coalition CoalitionEnemy { get { return (Coalition)(1 - (int)CoalitionPlayer); } }

//        /// <summary>
//        /// Coalition the player(s) belongs to.
//        /// </summary>
//        public Coalition CoalitionPlayer { get; internal set; } = Coalition.Blue;

//        /// <summary>
//        /// Date and time during which this mission takes place.
//        /// </summary>
//        public DCSMissionDateTime DateTime { get; internal set; } = new DCSMissionDateTime();

//        /// <summary>
//        /// Lua script files to include in the mission script. Will replace $INCLUDEDLUA$ in l10n\DEFAULT\script.lua.
//        /// </summary>
//        public List<string> IncludedLuaScripts { get; internal set; } = new List<string>();

//        /// <summary>
//        /// DCS ID of the airbase the player will take off from.
//        /// </summary>
//        public int InitialAirbaseID { get; internal set; }

//        /// <summary>
//        /// Initial coordinates of the players, used to measure the distance to objectives.
//        /// </summary>
//        public Coordinates InitialPosition { get; internal set; }

//        /// <summary>
//        /// Lua script for mission features. Replaces $MISSION_FEATURES_LUA$ in Script.lua.
//        /// </summary>
//        public string LuaScriptMissionFeatures { get; internal set; } = "";

//        /// <summary>
//        /// Lua script for mission features settings. Replaces $MISSION_FEATURES_SETTINGS_LUA$ in Script.lua.
//        /// </summary>
//        public string LuaScriptMissionFeaturesSettings { get; internal set; } = "";

//        /// <summary>
//        /// Lua script for mission objectives. Replaces $MISSION_OBJECTIVES_LUA$ in Script.lua.
//        /// </summary>
//        public string LuaScriptObjectives { get; internal set; } = "";

//        /// <summary>
//        /// Name/title of the mission/sortie.
//        /// </summary >
//        public string MissionName { get; internal set; } = "";

//        /// <summary>
//        /// An array of <see cref="DCSMissionObjective"/> describing all objectives in the mission.
//        /// </summary>
//        public DCSMissionObjective[] Objectives { get; internal set; } = new DCSMissionObjective[0];

//        /// <summary>
//        /// "Center position" of the mission objectives (average of all objectives positions)
//        /// </summary>
//        public Coordinates ObjectivesCenter
//        {
//            get
//            {
//                if (Objectives.Length == 0) return InitialPosition;

//                Coordinates center = new Coordinates();
//                foreach (DCSMissionObjective objective in Objectives)
//                    center += objective.Coordinates;

//                return center / Objectives.Length;
//            }
//        }

//        /// <summary>
//        /// The mission options to enable in this mission.
//        /// </summary>
//        public MissionOption[] OptionsMission { get; internal set; } = new MissionOption[0];

//        /// <summary>
//        /// The realism options to enable in this mission.
//        /// </summary>
//        public RealismOption[] OptionsRealism { get; internal set; } = new RealismOption[0];

//        /// <summary>
//        /// True if the objective is static object, false if it isn't (which means it's an unit)
//        /// Replaces $STATICOBJECTIVE$ in Script.lua
//        /// </summary>
//        public bool ObjectiveIsStatic = false;

//        /// <summary>
//        /// Ogg vorbis audio files to include in the .miz file.
//        /// </summary>
//        public List<string> OggFiles { get; internal set; } = new List<string>();

//        /// <summary>
//        /// Returns the ID of the first group containing players.
//        /// Mostly used to direct escort flight groups in single-player mission, should probably not be used in MP missions
//        /// as not clients groups will always be populated.
//        /// </summary>
//        public int PlayerGroupID
//        {
//            get
//            {
//                foreach (DCSMissionUnitGroup g in UnitGroups)
//                    if (g.IsAPlayerGroup()) return g.GroupID;

//                return 0;
//            }
//        }

//        /// <summary>
//        /// The ID of the <see cref="TheaterBP"/> (NOT THE DCS WORLD INTERNAL THEATER ID!) for this mission.
//        /// </summary>
//        public string Theater { get; internal set; } = "";

//        /// <summary>
//        /// Unique ID for the mission. Appended to certain filenames so they don't have the same name in every mission and
//        /// get confused with one another by the DCS cache.
//        /// </summary>
//        public string UniqueID { get; } = "";

//        /// <summary>
//        /// A list of <see cref="DCSMissionUnitGroup"/> describing all unit groups in the mission.
//        /// </summary>
//        public List<DCSMissionUnitGroup> UnitGroups { get; internal set; } = new List<DCSMissionUnitGroup>();

//        /// <summary>
//        /// A list of <see cref="DCSMissionWaypoint"/> describing all waypoints in the mission.
//        /// </summary>
//        public List<DCSMissionWaypoint> Waypoints { get; internal set; } = new List<DCSMissionWaypoint>();

//        /// <summary>
//        /// Total flight plan distance, in meters.
//        /// </summary>
//        public double WaypointsDistance
//        {
//            get
//            {
//                double distance = 0.0;
//                Coordinates lastCoordinates = InitialPosition;
//                for (int i = 0; i < Waypoints.Count; i++)
//                {
//                    distance += lastCoordinates.GetDistanceFrom(Waypoints[i].Coordinates);
//                    lastCoordinates = Waypoints[i].Coordinates;
//                }
//                distance += lastCoordinates.GetDistanceFrom(InitialPosition); // Distance from last waypoint to home airbase

//                return distance;
//            }
//        }

//        /// <summary>
//        /// Weather information for this mission.
//        /// </summary>
//        public DCSMissionWeatherInfo Weather { get; internal set; } = new DCSMissionWeatherInfo();

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public DCSMission()
//        {
//            // Generate a unique ID for the mission.
//            UniqueID = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()).ToLowerInvariant();
//        }


//        public MizFile ExportToMiz()
//        {
//            MizFile miz;

//            using (MizMaker mizMaker = new MizMaker())
//                miz = mizMaker.ExportToMizFile(this);

//            return miz;
//        }
