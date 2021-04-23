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

namespace BriefingRoom.DB
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

        internal DatabaseEntryInfo(string id, string name, string category, string description)
        {
            ID = id;
            Name = name;
            Category = category;
            Description = description;
        }
    }
}
