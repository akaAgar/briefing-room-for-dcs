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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Sotres location about a spawn point: a set of X,Y coordinates where a group of unit can be spawned.
    /// </summary>
    internal struct DBEntryTheaterSpawnPoint
    {
        /// <summary>
        /// ID of this spawn point. Must be unique at each location.
        /// </summary>
        internal string UniqueID { get; private set; }

        /// <summary>
        /// DCS map coordinates of this spawn point.
        /// </summary>
        internal Coordinates Coordinates { get; private set; }

        /// <summary>
        /// The type of spawn point.
        /// </summary>
        internal TheaterLocationSpawnPointType PointType { get; private set; }

        /// <summary>
        /// Default coalition the country this point is located in belongs to.
        /// </summary>
        internal Coalition Coalition { get; private set; }

        /// <summary>
        /// Load data for this spawn point.
        /// </summary>
        /// <param name="ini">Theater database entry ini file</param>
        /// <param name="key">.Ini key to load the spawnpoint from</param>
        /// <returns>True if data is valid, false otherwise</returns>
        internal bool Load(INIFile ini, string key)
        {
            string[] vals = ini.GetValueArray<string>("SpawnPoints", key, ',');
            UniqueID = key;

            if (vals.Length < 4) return false;

            try
            {
                Coordinates = new Coordinates(Toolbox.StringToDouble(vals[0]), Toolbox.StringToDouble(vals[1]));
                PointType = (TheaterLocationSpawnPointType)Enum.Parse(typeof(TheaterLocationSpawnPointType), vals[2], true);
                Coalition = (Coalition)Enum.Parse(typeof(Coalition), vals[3], true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
