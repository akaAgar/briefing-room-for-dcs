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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores all database entries and settings from the .ini files in the <see cref="DATABASE_PATH"/> subdirectory.
    /// </summary>
    internal class Database
    {
        /// <summary>
        /// Singleton of the library.
        /// </summary>
        internal static Database Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Database();

                return _Instance;
            }
        }

        /// <summary>
        /// Static singleton field.
        /// </summary>
        private static Database _Instance = null;

        /// <summary>
        /// Miscellaneous common settings used for mission generation.
        /// </summary>
        internal DatabaseCommon Common { get; set; }

        /// <summary>
        /// Database entries are stored by type in a dictionary of dictionaries.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, DBEntry>> DBEntries;

        /// <summary>
        /// Is the database initialized?
        /// </summary>
        private bool Initialized = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal Database()
        {
            Common = new DatabaseCommon();
            DBEntries = new Dictionary<Type, Dictionary<string, DBEntry>>();
        }

        /// <summary>
        /// Initializes/load the database if it's not done already.
        /// </summary>
        internal void Initialize()
        {
            if (Initialized) return;

            Common.Load();

            // Load entries into the database
            DBEntries.Clear();
            LoadEntries<DBEntryBriefingDescription>("BriefingDescriptions");
            LoadEntries<DBEntryFeatureMission>("MissionFeatures");
            LoadEntries<DBEntryFeatureObjective>("ObjectiveFeatures");
            LoadEntries<DBEntryObjectiveTarget>("ObjectiveTargets");
            LoadEntries<DBEntryObjectiveTargetBehavior>("ObjectiveTargetsBehaviors");
            LoadEntries<DBEntryObjectiveTask>("ObjectiveTasks"); // Must be loaded after other DBEntryBriefingDescription, as it depends on it
            LoadEntries<DBEntryObjectivePreset>("ObjectivePresets"); // Must be loaded after other DBEntryObjective*, as it depends on them
            LoadEntries<DBEntryTheater>("Theaters");
            LoadEntries<DBEntryAirbase>("TheatersAirbases"); // Must be loaded after DBEntryTheater, as it depends on it
            LoadEntries<DBEntryDCSMod>("DCSMods");
            LoadEntries<DBEntryUnit>("Units"); // Must be loaded after DBEntryDCSMod, as it depends on it
            LoadCustomUnitEntries("Units");
            LoadEntries<DBEntryDefaultUnitList>("DefaultUnitLists"); // Must be loaded after DBEntryUnit, as it depends on it
            LoadEntries<DBEntryCoalition>("Coalitions"); // Must be loaded after DBEntryUnit and DBEntryDefaultUnitList, as it depends on them
            LoadEntries<DBEntryWeatherPreset>("WeatherPresets");

            // Can't start without at least one player-controllable aircraft
            if ((from DBEntryUnit unit in GetAllEntries<DBEntryUnit>()
                 where unit.AircraftData.PlayerControllable select unit.ID).Count() == 0)
                BriefingRoom.PrintToLog("No player-controllable aircraft found.", LogMessageErrorLevel.Error);

            Initialized = true;
        }

        /// <summary>
        /// Loads database entries from all .ini files in a directory (and its subdirectories).
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DBEntry"/> class to load</typeparam>
        /// <param name="dbEntries">Dictionary in which to store the database entries</param>
        /// <param name="subDirectory"><see cref="DATABASE_PATH"/> subdirectory in which to look for files</param>
        private void LoadEntries<T>(string subDirectory) where T : DBEntry, new()
        {
            BriefingRoom.PrintToLog($"Loading {subDirectory.ToLowerInvariant()}...");

            string directory = $"{BRPaths.DATABASE}{subDirectory}";
            if (!Directory.Exists(directory))
                throw new Exception($"Directory {directory} not found.");

            Type dbType = typeof(T);
            string shortTypeName = dbType.Name.Substring(7).ToLowerInvariant();

            if (!DBEntries.ContainsKey(dbType))
                DBEntries.Add(dbType, new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase));

            DBEntries[dbType].Clear();

            foreach (string filePath in Directory.EnumerateFiles(directory, "*.ini", SearchOption.AllDirectories))
            {
                string id = Path.GetFileNameWithoutExtension(filePath).Replace(",", "").Trim(); // No commas in file names, so we don't break comma-separated arrays

                if (DBEntries[dbType].ContainsKey(id)) continue;
                T entry = new T();
                if (!entry.Load(this, id, filePath)) continue;
                DBEntries[dbType].Add(id, entry);
                BriefingRoom.PrintToLog($"Loaded {shortTypeName} \"{id}\"");
            }
            BriefingRoom.PrintToLog($"Found {DBEntries[dbType].Count} database entries of type \"{typeof(T).Name}\"");

            bool mustHaveAtLeastOneEntry = true;
            if ((dbType == typeof(DBEntryDefaultUnitList)) ||
                (dbType == typeof(DBEntryFeatureMission)) ||
                (dbType == typeof(DBEntryFeatureObjective)))
                mustHaveAtLeastOneEntry = false;

            // If a required database type has no entries, raise an error.
            if ((DBEntries[dbType].Count == 0) && mustHaveAtLeastOneEntry)
                BriefingRoom.PrintToLog($"No valid database entries found in the \"{subDirectory}\" directory", LogMessageErrorLevel.Error);
        }

        private void LoadCustomUnitEntries(string subDirectory)
        {
            BriefingRoom.PrintToLog($"Loading {subDirectory.ToLowerInvariant()}...");

            string directory = $"{BRPaths.CUSTOMDATABASE}{subDirectory}";
            if (!Directory.Exists(directory))
                return;

            Type dbType = typeof(DBEntryUnit);
            string shortTypeName = dbType.Name.Substring(7).ToLowerInvariant();

            foreach (string filePath in Directory.EnumerateFiles(directory, "*.ini", SearchOption.AllDirectories))
            {
                string id = Path.GetFileNameWithoutExtension(filePath).Replace(",", "").Trim(); // No commas in file names, so we don't break comma-separated arrays

                var entry = new DBEntryUnit();
                if (!entry.Load(this, id, filePath)) continue;
                if (DBEntries[dbType].ContainsKey(id))
                {
                    ((DBEntryUnit)DBEntries[dbType][id]).Merge(entry);
                    BriefingRoom.PrintToLog($"Updated {shortTypeName} \"{id}\"");

                } else {
                    DBEntries[dbType].Add(id, entry);
                    BriefingRoom.PrintToLog($"Loaded {shortTypeName} \"{id}\"");
                }
            }
            BriefingRoom.PrintToLog($"Found {DBEntries[dbType].Count} custom database entries of type \"{typeof(DBEntryUnit).Name}\"");

            bool mustHaveAtLeastOneEntry = true;
            if ((dbType == typeof(DBEntryDefaultUnitList)) ||
                (dbType == typeof(DBEntryFeatureMission)) ||
                (dbType == typeof(DBEntryFeatureObjective)))
                mustHaveAtLeastOneEntry = false;

            // If a required database type has no entries, raise an error.
            if ((DBEntries[dbType].Count == 0) && mustHaveAtLeastOneEntry)
                BriefingRoom.PrintToLog($"No valid database entries found in the \"{subDirectory}\" directory", LogMessageErrorLevel.Error);
        }


        /// <summary>
        /// Checks if the proposed ID exists in the database, or return a default ID if it doesn't.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <param name="id">ID to check</param>
        /// <param name="defaultID">Default ID to return if ID is invalid. If null, returns the first ID found.</param>
        /// <returns>A valid ID, or an empty string if none was found.</returns>
        internal string CheckID<T>(string id, string defaultID = null) where T : DBEntry
        {
            if (EntryExists<T>(id)) return id;
            if (!string.IsNullOrEmpty(defaultID) && EntryExists<T>(defaultID)) return CheckID<T>(defaultID);
            if (GetAllEntriesIDs<T>().Length == 0) return "";
            return GetAllEntriesIDs<T>()[0];
        }

        /// <summary>
        /// Checks if the proposed IDs exists in the database, and remove invalid ones from the array.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <param name="ids">Array of IDs to check</param>
        /// <returns>An array of valid IDs</returns>
        internal string[] CheckIDs<T>(params string[] ids) where T : DBEntry
        {
            return ids.Intersect(GetAllEntriesIDs<T>(), StringComparer.OrdinalIgnoreCase).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Does a entry exist?
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="id">The unique ID of the entry (case insensitive)</param>
        /// <returns>True if the entry exist, false if it doesn't</returns>
        internal bool EntryExists<T>(string id) where T : DBEntry
        {
            return DBEntries[typeof(T)].ContainsKey(id ?? "");
        }

        /// <summary>
        /// Returns all entries of a certain type.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <returns>An array of entries.</returns>
        internal T[] GetAllEntries<T>() where T : DBEntry
        {
            return (from entry in DBEntries[typeof(T)].Values select (T)entry).ToArray();
        }

        /// <summary>
        /// Returns all IDs of entries of a certain type.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <returns>IDs, in an array of strings</returns>
        internal string[] GetAllEntriesIDs<T>() where T : DBEntry
        {
            if (!DBEntries.ContainsKey(typeof(T))) return null;
            return DBEntries[typeof(T)].Keys.ToArray();
        }

        /// <summary>
        /// Returns the entry of type T with unique ID id.
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="id">The unique ID of the entry (case insensitive)</param>
        /// <returns>The entry, or null is no entry with this ID exists</returns>
        internal T GetEntry<T>(string id) where T : DBEntry
        {
            id = id ?? "";
            if (!DBEntries[typeof(T)].ContainsKey(id)) return null;
            return (T)DBEntries[typeof(T)][id];
        }

        /// <summary>
        /// Returns all existing entries of type T with an ID in array ids;
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="ids">An array of unique entry IDs (case insensitive)</param>
        /// <returns>An array of entries</returns>
        internal T[] GetEntries<T>(params string[] ids) where T : DBEntry
        {
            return (from T entry in GetAllEntries<T>() where ids.Distinct().OrderBy(x => x).Contains(entry.ID) select entry).ToArray();
        }
    }
}
