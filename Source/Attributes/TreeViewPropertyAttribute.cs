/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;

namespace BriefingRoom4DCSWorld.Attributes
{
    /// <summary>
    /// Stores information about the way to display a property in a <see cref="GUI.TreeViewPropertyEditor{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TreeViewPropertyAttribute : Attribute
    {
        /// <summary>
        /// Human-readable name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Description, to display as a tooltip.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Type of data source to use. Can be an enum, a <see cref="DB.DBEntry"/> from the database, an integer, etc.
        /// </summary>
        public Type DataSourceType { get; }

        /// <summary>
        /// Parent node to use (or create if it doesn't exist) in the <see cref="GUI.TreeViewPropertyEditor{T}"/>.
        /// </summary>
        public string ParentNode { get; }

        /// <summary>
        /// Special behavior flags.
        /// </summary>
        public TreeViewPropertyAttributeFlags Flags { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="displayName">Human-readable name.</param>
        /// <param name="parentNode">Description, to display as a tooltip.</param>
        /// <param name="dataSourceType">Type of data source to use. Can be an enum, a <see cref="DB.DBEntry"/> from the database, an integer, etc.</param>
        /// <param name="description">Description, to display as a tooltip.</param>
        /// <param name="flags">Special behavior flags.</param>
        public TreeViewPropertyAttribute(string displayName, string parentNode, Type dataSourceType, string description, TreeViewPropertyAttributeFlags flags = 0)
        {
            DisplayName = displayName;
            ParentNode = parentNode;
            DataSourceType = dataSourceType;
            Description = description;
            Flags = flags;
        }
    }
}
