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

namespace BriefingRoom4DCSWorld
{

    // Define an extension method in a non-nested static class.
    public static class Extensions
    {
        /// <summary>
        /// Gets True value if random selects a random option
        /// </summary>
        public static AmountN Get(this AmountN amountN)
        {
            return AmountN.Random == amountN ? (AmountN)(new Random().Next((int)AmountN.VeryHigh) + 1)  : amountN;
        }
    }
    
    /// <summary>
    /// Enumerates various relative amount value, from "very low" to "very high", with a "None" value
    /// </summary>
    public enum AmountN
    {
        /// <summary>
        /// Random Value
        /// </summary>
        Random,

        /// <summary>
        /// Nothing at all
        /// </summary>
        None,
        /// <summary>
        /// Very low amount
        /// </summary>
        VeryLow,
        /// <summary>
        /// Low amount
        /// </summary>
        Low,
        /// <summary>
        /// Average amount
        /// </summary>
        Average,
        /// <summary>
        /// High amount
        /// </summary>
        High,
        /// <summary>
        /// Very high amount
        /// </summary>
        VeryHigh
    }
}
