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

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Stores a summary of a database entry to display in an user interface.
    /// </summary>
    public struct DatabaseEntryInfo
    {
        /// <summary>
        /// Category in which to sort this database entry in the user interface.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Description to show for this database entry in the user interface.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Unique ID used by this entry in the database.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Display name to show for this database entry in the user interface. If null or empty, <see cref="ID"/> will be used instead.
        /// </summary>
        public string Name { get; }

        public DatabaseEntryInfo(string id, string name, string category, string description)
        {
            ID = id;
            Name = name;
            Category = category;
            Description = description;
        }

        /// <summary>
        /// Returns a single string with both name and description.
        /// </summary>
        /// <param name="separator">The separator between name and description.</param>
        /// <param name="upperCaseName">Should the name be cast to upper case?</param>
        /// <returns>A string.</returns>
        public string GetNameAndDescription(string separator = " - ", bool upperCaseName = false)
        {
            string casedName = upperCaseName ? Name.ToUpperInvariant() : Name;

            if (string.IsNullOrEmpty(Description)) return casedName;
            return $"{casedName}{separator}{Description}";
        }
    }
}
