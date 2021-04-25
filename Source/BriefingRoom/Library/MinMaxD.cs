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

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Stores a minimum and a maximum double value.
    /// </summary>
    internal struct MinMaxD
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        internal readonly double Min;

        /// <summary>
        /// The maximum value.
        /// </summary>
        internal readonly double Max;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        internal MinMaxD(double min, double max) { Min = Math.Min(min, max); Max = Math.Max(min, max); }

        /// <summary>
        /// Constructor. Parses the min and max coordinates from a string (format is "1.2345,6.7890")
        /// </summary>
        /// <param name="minMaxString">The string containing the min and max value.</param>
        internal MinMaxD(string minMaxString)
        {
            try
            {
                string[] minAndMaxString = minMaxString.Split(',');

                double val1 = Toolbox.StringToDouble(minAndMaxString[0]);
                double val2 = Toolbox.StringToDouble(minAndMaxString[1]);

                Min = Math.Min(val1, val2);
                Max = Math.Max(val1, val2);
            }
            catch (Exception)
            {
                Min = 0; Max = 0;
            }
        }

        /// <summary>
        /// Returns a random value between Min and Max.
        /// </summary>
        /// <returns>A random value.</returns>
        internal double GetValue() { return Toolbox.RandomDouble(Min, Max); }

        /// <summary>
        /// Returns the MinMax value as a string.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString() { return ToString(null); }

        /// <summary>
        /// Returns the MinMax value as a string.
        /// </summary>
        /// <param name="stringFormat">A format string.</param>
        /// <returns>A string</returns>
        internal string ToString(string stringFormat)
        { return Toolbox.ValToString(Min, stringFormat) + "," + Toolbox.ValToString(Max, stringFormat); }

        public static MinMaxD operator *(MinMaxD mm, double mult) { return new MinMaxD(mm.Min * mult, mm.Max * mult); }
        public static MinMaxD operator /(MinMaxD mm, double mult) { return new MinMaxD(mm.Min / mult, mm.Max / mult); }

        /// <summary>
        /// Is the value passed as parameter between the min and the max values?
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the value passed as parameter is between the min and the max values, false otherwise.</returns>
        internal bool Contains(double value) { return (value >= Min) && (value <= Max); }
    }
}
