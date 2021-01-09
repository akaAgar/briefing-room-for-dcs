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

using BriefingRoom4DCSWorld.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.DB
{
    public class DatabaseLoader : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseLoader() { }

        /// <summary>
        /// Load all files from the Library directory into the library.
        /// Called by <see cref="=BriefingRoom"/> when application starts.
        /// </summary>
        /// <param name="dbEntries">Dictionary in which to store the database entries</param>
        /// <returns>True if everything went right, false otherwise</returns>
        public void LoadAll(Dictionary<Type, Dictionary<string, DBEntry>> dbEntries)
        {
            dbEntries.Clear();
            LoadDatabaseEntries<DBEntryExtension>(dbEntries, "Extensions");
            LoadDatabaseEntries<DBEntryMissionFeature>(dbEntries, "MissionFeatures");
            LoadDatabaseEntries<DBEntryObjective>(dbEntries, "Objectives"); // Must be loaded after DBEntryMissionFeature as it depends on it
            LoadDatabaseEntries<DBEntryTheater>(dbEntries, "Theaters");
            LoadDatabaseEntries<DBEntryUnit>(dbEntries, "Units");
            CreateCountriesArrayFromUnitOperators(); // Must be called after DBEntryUnit is loaded as it depends on it
            CreateUnitModsArrayFromUnits(); // Must be called after DBEntryUnit is loaded as it depends on it
            LoadDatabaseEntries<DBEntryDefaultUnitList>(dbEntries, "DefaultUnitLists"); // Must be loaded after DBEntryUnit as it depends on it
            LoadDatabaseEntries<DBEntryCoalition>(dbEntries, "Coalitions"); // Must be loaded after DBEntryUnit and DBEntryDefaultUnitList as it depends on them
        }

        /// <summary>
        /// Creates the list of available countries from the operators found in <see cref="DBEntryUnit"/> .ini files.
        /// </summary>
        private void CreateCountriesArrayFromUnitOperators()
        {
            List<string> countries = new List<string>();

            foreach (DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>())
                countries.AddRange(unit.Operators.Keys);

            Database.Instance.Countries =
                (from string c in countries select c.ToLowerInvariant()).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
        }

        private void CreateUnitModsArrayFromUnits()
        {
            List<string> unitMods = new List<string>();

            foreach (DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>())
                if (!string.IsNullOrEmpty(unit.RequiredMod.Trim()))
                    unitMods.Add(unit.RequiredMod);

            Database.Instance.UnitsMods = unitMods.Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();
        }

        /// <summary>
        /// Loads database entries from all .ini files in a directory (and its subdirectories).
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DBEntry"/> class to load</typeparam>
        /// <param name="dbEntries">Dictionary in which to store the database entries</param>
        /// <param name="subDirectory"><see cref="DATABASE_PATH"/> subdirectory in which to look for files</param>
        private void LoadDatabaseEntries<T>(Dictionary<Type, Dictionary<string, DBEntry>> dbEntries, string subDirectory) where T : DBEntry, new()
        {
            DebugLog.Instance.WriteLine($"Loading {subDirectory.ToLowerInvariant()}...");

            string directory = $"{BRPaths.DATABASE}{subDirectory}";
            if (!Directory.Exists(directory))
                throw new Exception($"Directory Database\\{subDirectory} not found.");

            Type dbType = typeof(T);
            string shortTypeName = dbType.Name.Substring(7).ToLowerInvariant();

            if (!dbEntries.ContainsKey(dbType))
                dbEntries.Add(dbType, new Dictionary<string, DBEntry>(StringComparer.InvariantCultureIgnoreCase));

            dbEntries[dbType].Clear();

            foreach (string filePath in Directory.EnumerateFiles(directory, "*.ini", SearchOption.AllDirectories))
            {
                string id = Path.GetFileNameWithoutExtension(filePath).Trim();

                // Extensions, mission features and units may not have commas in their IDs as these will be used in comma-separated arrays.
                if ((dbType == typeof(DBEntryExtension)) || (dbType == typeof(DBEntryMissionFeature)) || (dbType == typeof(DBEntryUnit)))
                    id = id.Replace(",", "").Trim();

                if (dbEntries[dbType].ContainsKey(id)) continue;
                T entry = new T();
                if (!entry.Load(id, filePath)) continue;
                dbEntries[dbType].Add(id, entry);
                DebugLog.Instance.WriteLine($"Loaded {shortTypeName} \"{id}\"", 1);
            }
            DebugLog.Instance.WriteLine($"Found {dbEntries[dbType].Count} database entries of type \"{typeof(T).Name}\"");

            // If a required database type has no entries, raise an error.
            if ((dbEntries[dbType].Count == 0) && TypeMustHaveAtLeastOneEntry<T>())
                DebugLog.Instance.WriteLine($"No valid database entries found in the \"{subDirectory}\" directory", DebugLogMessageErrorLevel.Error);
        }

        /// <summary>
        /// Checks if this database entry type requires at least one value.
        /// </summary>
        /// <typeparam name="T">A type derived from <see cref="DBEntry"/></typeparam>
        /// <returns>True if this type requires at lease one value, false if it doesn't</returns>
        private bool TypeMustHaveAtLeastOneEntry<T>() where T: DBEntry
        {
            Type type = typeof(T);

            if (
                (type == typeof(DBEntryDefaultUnitList)) ||
                (type == typeof(DBEntryExtension)) ||
                (type == typeof(DBEntryMissionFeature))
                )
                return false;

            return true;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
