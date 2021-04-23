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

using BriefingRoom.Debug;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom.DB
{
    /// <summary>
    /// Stores all database entries and settings from the .ini files in the <see cref="DATABASE_PATH"/> subdirectory.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Singleton of the library.
        /// </summary>
        public static Database Instance
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
        /// Misc. common settings for mission generation.
        /// </summary>
        public DatabaseCommon Common { get; set; }

        /// <summary>
        /// Array of countries ID used by <see cref="DBEntryCoalition"/> and <see cref="DBEntryUnit"/>
        /// </summary>
        public string[] Countries { get; set; }

        /// <summary>
        /// Database entries are stored by type in a dictionary of dictionaries.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, DBEntry>> DBEntries;

        /// <summary>
        /// Targeted DCS world version (just for info, doesn't mean that the program will not work with another version)
        /// </summary>
        public string TARGETED_DCS_WORLD_VERSION { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Database()
        {
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
                TARGETED_DCS_WORLD_VERSION = ini.GetValue("Versions", "DCSVersion", "2.5");

            Common = new DatabaseCommon();
            Countries = new string[0];
            DBEntries = new Dictionary<Type, Dictionary<string, DBEntry>>();
        }

        /// <summary>
        /// Initialize the database and loads all entries from .ini files in <see cref="DATABASE_PATH"/>
        /// </summary>
        public void Initialize()
        {
            DebugLog.Instance.Clear();
            Common.Load();

            DBEntries.Clear();
            using (DatabaseLoader loader = new DatabaseLoader())
                loader.LoadAll(DBEntries);

            if (GetAllPlayerAircraftID().Length == 0) // Can't start without at least one player-controllable aircraft
                DebugLog.Instance.WriteLine("No player-controllable aircraft found.", DebugLogMessageErrorLevel.Error);
        }

        /// <summary>
        /// Does a entry exist?
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="id">The unique ID of the entry (case insensitive)</param>
        /// <returns>True if the entry exist, false if it doesn't</returns>
        public bool EntryExists<T>(string id) where T : DBEntry
        {
            return DBEntries[typeof(T)].ContainsKey(id ?? "");
        }

        /// <summary>
        /// Returns all entries of a certain type.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <returns>An array of entries.</returns>
        public T[] GetAllEntries<T>() where T : DBEntry
        {
            return (from entry in DBEntries[typeof(T)].Values select (T)entry).ToArray();
        }

        public DBEntry[] GetAllEntries(Type entryType)
        {
            return (from entry in DBEntries[entryType].Values select entry).ToArray();
        }

        /// <summary>
        /// Returns all IDs of entries of a certain type.
        /// </summary>
        /// <typeparam name="T">Database entry type</typeparam>
        /// <returns>IDs, in an array of strings</returns>
        public string[] GetAllEntriesIDs<T>() where T : DBEntry
        {
            return GetAllEntriesIDs(typeof(T));
        }

        /// <summary>
        /// Returns all IDs of entries of a certain type.
        /// </summary>
        /// <param name="t">Database entry type</param>
        /// <returns>IDs, in an array of strings</returns>
        public string[] GetAllEntriesIDs(Type t)
        {
            if (!DBEntries.ContainsKey(t)) return null;
            return DBEntries[t].Keys.ToArray();
        }

        /// <summary>
        /// Returns IDs of all <see cref="DBEntryUnit"/> describing a player-controllable aircraft.
        /// </summary>
        /// <returns>Array of IDs</returns>
        public string[] GetAllPlayerAircraftID()
        {
            return
                (from DBEntryUnit unit in GetAllEntries<DBEntryUnit>()
                 where unit.AircraftData.PlayerControllable
                 select unit.ID).OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Returns all airfields of all theaters as an array of strings in the "TheaterID, AirbaseID"
        /// </summary>
        /// <returns>An array of strings</returns>
        public string[] GetAllTheaterAirfields()
        {
            List<string> airfields = new List<string>();

            foreach (DBEntryTheater theater in GetAllEntries<DBEntryTheater>())
                airfields.AddRange(from DBEntryTheaterAirbase airbase in theater.Airbases select $"{theater.ID}, {airbase.Name}");

            return airfields.ToArray();
        }

        /// <summary>
        /// Returns the entry of type T with unique ID id.
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="id">The unique ID of the entry (case insensitive)</param>
        /// <returns>The entry, or null is no entry with this ID exists</returns>
        public T GetEntry<T>(string id) where T : DBEntry
        {
            id = id ?? "";
            if (!DBEntries[typeof(T)].ContainsKey(id)) return null;
            return (T)DBEntries[typeof(T)][id];
        }

        public DBEntry GetEntry(Type entryType, string id)
        {
            id = id ?? "";
            if (!DBEntries[entryType].ContainsKey(id)) return null;
            return DBEntries[entryType][id];
        }

        /// <summary>
        /// Returns all existing entries of type T with an ID in array ids;
        /// </summary>
        /// <typeparam name="T">The type of the entry</typeparam>
        /// <param name="ids">An array of unique entry IDs (case insensitive)</param>
        /// <returns>An array of entries</returns>
        public T[] GetEntries<T>(params string[] ids) where T : DBEntry
        {
            return (from T entry in GetAllEntries<T>() where ids.Distinct().OrderBy(x => x).Contains(entry.ID) select entry).ToArray();
        }

        /// <summary>
        /// Checks if the proposed ID exists in the database, or return a default value if it doesn't.
        /// </summary>
        /// <typeparam name="T">Database entry type to look for</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="defaultValue">Default value to return if the checked value is not found</param>
        /// <param name="allowEmpty">If true, null or empty values will be allowed (and "" will be returned)</param>
        /// <returns>The value if it is found in the library, or a default value if desired value is not found</returns>
        public string CheckValue<T>(string value, string defaultValue = "", bool allowEmpty = false) where T : DBEntry
        {
            if (string.IsNullOrEmpty(value) && allowEmpty) return "";

            if (EntryExists<T>(value)) return value; // Value exists, return it
            if (EntryExists<T>(defaultValue)) return defaultValue; // Value doesn't exist, return the default value
            if (string.IsNullOrEmpty(defaultValue) && allowEmpty) return "";
            return GetAllEntriesIDs<T>()[0]; // Neither value exist, return the first value found
        }

        /// <summary>
        /// Checks if the proposed IDs exists in the database, and remove invalid ones from the array.
        /// </summary>
        /// <typeparam name="T">Database entry type to look for</typeparam>
        /// <param name="value">Array of IDs to check</param>
        /// <param name="defaultValue">Default ID to return if no valid ID is found in the array. Leave empty to return an empty array if no valid ID is found.</param>
        /// <returns>An array of IDs</returns>
        public string[] CheckValues<T>(string[] values, string defaultValue = "") where T : DBEntry
        {
            values = (from string v in values where EntryExists<T>(v) select v).Distinct().OrderBy(x => x).ToArray();

            if ((values.Length == 0) && EntryExists<T>(defaultValue))
                values = new string[] { defaultValue };

            return values;
        }

    }
}
