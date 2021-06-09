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
    /// BriefingRoom specific exception.
    /// Basically the same as a normal <see cref="Exception"/>, but with a different type so exceptions thrown by BriefingRoom
    /// (e.g. mission generation failures, etc) can be easily differenciated from a "regular" exception (file access error, etc.) in a try...catch loop.
    /// </summary>
    public class BriefingRoomException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message to display in the exception.</param>
        public BriefingRoomException(string message) : base(message) { }
    }
}
