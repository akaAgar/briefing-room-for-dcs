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
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCSWorld.DB;
using System.Linq;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// A series of static tools to help with mission template validation.
    /// </summary>
    public static class TemplateTools
    {
        ///// <summary>
        ///// Default value for the blue coalition.
        ///// </summary>
        //public const string DEFAULT_COALITION_BLUE = "USA, 2000s";

        ///// <summary>
        ///// Default value for the red coalition.
        ///// </summary>
        //public const string DEFAULT_COALITION_RED = "Russia, 2000s";

        ///// <summary>
        ///// Default value for the objective type.
        ///// </summary>
        //public const string DEFAULT_OBJECTIVE = "Deep air support";

        ///// <summary>
        ///// Default number of objectives.
        ///// </summary>
        //public const int DEFAULT_OBJECTIVE_COUNT = 2;

        ///// <summary>
        ///// Default number of objectives.
        ///// </summary>
        //public const string DEFAULT_PLAYER_AIRCRAFT = "Su-25T Frogfoot";

        ///// <summary>
        ///// Default value for the theater.
        ///// </summary>
        //public const string DEFAULT_THEATER = "Caucasus";

        /// <summary>
        /// Maximum number of objectives.
        /// </summary>
        public const int MAX_OBJECTIVES = 5;

        /// <summary>
        /// Checks if the proposed ID exists in the database, or return a default value if it doesn't.
        /// </summary>
        /// <typeparam name="T">Database entry type to look for</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="defaultValue">Default value to return if the checked value is not found</param>
        /// <returns>The value if it is found in the library, or a default value if desired value is not found</returns>
        public static string CheckValue<T>(string value, string defaultValue = "") where T : DBEntry
        {
            if (Database.Instance.EntryExists<T>(value)) return value; // Value exists, return it
            if (Database.Instance.EntryExists<T>(defaultValue)) return defaultValue; // Value doesn't exist, return the default value
            return Database.Instance.GetAllEntriesIDs<T>()[0]; // Neither value exist, return the first value found
        }

        /// <summary>
        /// Checks if the proposed IDs exists in the database, and remove invalid ones from the array.
        /// </summary>
        /// <typeparam name="T">Database entry type to look for</typeparam>
        /// <param name="value">Array of IDs to check</param>
        /// <param name="defaultValue">Default ID to return if no valid ID is found in the array. Leave empty to return an empty array if no valid ID is found.</param>
        /// <returns>An array of IDs</returns>
        public static string[] CheckValues<T>(string[] values, string defaultValue = "") where T : DBEntry
        {
            values = (from string v in values where Database.Instance.EntryExists<T>(v) select v).Distinct().OrderBy(x => x).ToArray();

            if ((values.Length == 0) && Database.Instance.EntryExists<T>(defaultValue))
                values = new string[] { defaultValue };

            return values;
        }

        /// <summary>
        /// Checks the proposed aircraft type exists and is player controllable.
        /// </summary>
        /// <param name="value">Aircraft type to check</param>
        /// <returns>The aircraft type, a default value if proposed aircraft type doesn't exist or is invalid.</returns>
        public static string CheckValuePlayerAircraft(string value)
        {
            string[] playerAircraft = Database.Instance.GetAllPlayerAircraftID();

            if (playerAircraft.Contains(value)) return value;
            if (playerAircraft.Contains(Database.Instance.Common.DefaultPlayerAircraft)) return Database.Instance.Common.DefaultPlayerAircraft;
            return playerAircraft[0];
        }

        /// <summary>
        /// Checks the proposed airfield exists.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>The value, or an empty string if the airfield doesn't exist or is invalid.</returns>
        public static string CheckValueTheaterStartingAirbase(string value)
        {
            if (Database.Instance.GetAllTheaterAirfields().Contains(value)) return value;
            return "";
        }
    }
}
