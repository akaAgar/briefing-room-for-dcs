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
    /// Stores information about acceptable values for an Int32 property in a <see cref="GUI.TreeViewPropertyEditor{T}"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TreeViewPropertyIntAttribute : Attribute
    {
        /// <summary>
        /// Minimum value.
        /// </summary>
        public int IntValueMin { get; }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public int IntValueMax { get; }

        /// <summary>
        /// Interval between two values in the ContextMenu display.
        /// </summary>
        public int IntValueIncrement { get; }
        
        /// <summary>
        /// String used to format integer values ("%i" is replaced by the integer).
        /// </summary>
        public string IntValueFormat { get; }
        
        /// <summary>
        /// String to display when the integer value is equal to zero.
        /// </summary>
        public string IntValueZeroText { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="intValueMin">Minimum value (for an integer data source).</param>
        /// <param name="intValueMax">Maximum value (for an integer data source).</param>
        /// <param name="intValueIncrement">Interval between two values (for an integer data source).</param>
        /// <param name="intValueFormat">String used to format integer values ("%i" is replaced by the integer).</param>
        /// <param name="intValueZeroText">String to display when the integer value is equal to zero.</param>
        public TreeViewPropertyIntAttribute(int intValueMin, int intValueMax, int intValueIncrement = 1, string intValueFormat = "%i", string intValueZeroText = null)
        {
            IntValueMin = intValueMin;
            IntValueMax = intValueMax;
            IntValueIncrement = intValueIncrement;
            IntValueFormat = intValueFormat;
            IntValueZeroText = intValueZeroText;
        }

        /// <summary>
        /// Gets a formatted display of an integer value.
        /// </summary>
        /// <param name="value">Integer value to display.</param>
        /// <returns>A formatted string.</returns>
        public string FormatIntValue(int value)
        {
            if ((value == 0) && !string.IsNullOrEmpty(IntValueZeroText))
                return IntValueZeroText;

            return IntValueFormat.Replace("%i", value.ToString());
        }
    }
}
