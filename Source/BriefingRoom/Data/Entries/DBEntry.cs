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
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal abstract class DBEntry : IDisposable
    {
        protected Database Database { get; set; }

        internal string ID { get; set; }

        internal string UIDisplayName { get; set; }

        internal string UICategory { get; set; }

        internal string UIDescription { get; set; }

        internal DBEntry() { }

        internal virtual DatabaseEntryInfo GetDBEntryInfo()
        {
            return new DatabaseEntryInfo(ID, UIDisplayName, UICategory, UIDescription);
        }

        internal bool Load(Database database, string id, string iniFilePath)
        {
            Database = database;

            ID = id;
            using (INIFile ini = new INIFile(iniFilePath))
            {
                UIDisplayName = ini.GetValue<string>("GUI", "DisplayName");
                if (string.IsNullOrEmpty(UIDisplayName)) UIDisplayName = ID;
                UICategory = ini.GetValue<string>("GUI", "Category");
                UIDescription = ini.GetValue<string>("GUI", "Description");
            }
            return OnLoad(iniFilePath);
        }

        protected abstract bool OnLoad(string iniFilePath);

        public virtual void Dispose() { }

        protected string[] GetValidDBEntryIDs<T>(string[] values, out string[] rejected) where T : DBEntry
        {
            values = values ?? new string[0]; // Make sure values is not null

            // Accepted values IDs are neither null nor empty AND exist in the database
            string[] accepted =
                (from string v in values
                 where !string.IsNullOrEmpty(v.Trim()) && Database.EntryExists<T>(v.Trim())
                 select v.Trim()).ToArray();

            // Accepted values IDs are neither null nor empty but DO NOT exist in the database
            rejected =
                (from string v in values
                 where !string.IsNullOrEmpty(v.Trim()) && !Database.EntryExists<T>(v.Trim())
                 select v.Trim()).ToArray();

            return accepted;
        }
    }
}
