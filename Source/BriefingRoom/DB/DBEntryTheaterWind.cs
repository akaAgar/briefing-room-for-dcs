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
    /// Stores information about a set of wind conditions in a <see cref="DBEntry"/>.
    /// </summary>
    public struct DBEntryWind
    {
        /// <summary>
        /// Min/max wind speed (in meters/second).
        /// </summary>
        public MinMaxI Wind { get; }

        /// <summary>
        /// Min/max turbulence (in meters/second).
        /// </summary>
        public MinMaxI Turbulence { get; }

        /// <summary>
        /// Constructor. Loads data from a weather database entry .ini file.
        /// </summary>
        /// <param name="ini">The .ini file to load from.</param>
        /// <param name="key">The value key.</param>
        public DBEntryWind(INIFile ini, string key)
        {
            Wind = ini.GetValue<MinMaxI>("Wind", key + ".Wind");
            Turbulence = ini.GetValue<MinMaxI>("Wind", key + ".Turbulence");
        }
    }
}
