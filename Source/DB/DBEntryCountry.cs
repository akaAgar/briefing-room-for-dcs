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

using BriefingRoom4DCSWorld.Debug;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores information about a country.
    /// </summary>
    public class DBEntryCountry : DBEntry
    {
        /// <summary>
        /// Decades during which this country existed.
        /// </summary>
        public Decade[] Decades { get; private set; }

        /// <summary>
        /// Default unit list to use when no units are found for a given decade and family.
        /// </summary>
        public string DefaultUnitList { get; private set; }

        /// <summary>
        /// Does this country use NATO callsigns for its units?
        /// </summary>
        public bool NATOCallsigns { get; private set; }

        /// <summary>
        /// Unit operator IDs for this country.
        /// </summary>
        public string[] UnitOperators { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                Decades = ini.GetValueArrayAsMinMaxEnum<Decade>("Country", "Decades");
                DefaultUnitList = ini.GetValue<string>("Country", "DefaultUnitList");
                NATOCallsigns = ini.GetValue("Country", "NATOCallsigns", false);
                UnitOperators = (from string c in ini.GetValueArray<string>("Country", "UnitCountries") select c.ToLowerInvariant()).Distinct().ToArray();

                if (!Database.Instance.EntryExists<DBEntryDefaultUnitList>(DefaultUnitList))
                {
                    DebugLog.Instance.WriteLine($"Country \"{ID}\" references a default unit list \"{DefaultUnitList}\" which doesn't exist, country was ignored.", DebugLogMessageErrorLevel.Warning);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the IDs of random <see cref="DBEntryUnit"/> belonging to the <see cref="UnitFamily"/> passed as parameter.
        /// </summary>
        /// <param name="family">Family unit to search for</param>
        /// <param name="decade">Decade during with the unit must be in use</param>
        /// <param name="count">Number of units to return</param>
        /// <param name="useDefaultUnitList">Should the default unit list be used if no unit is found</param>
        /// <returns>An array of <see cref="DBEntryUnit"/> IDs</returns>
        public string[] GetRandomUnits(UnitFamily family, Decade decade, int count = 1, bool useDefaultUnitList = true)
        {
            if (count < 1) return new string[0];

            string[] validUnits =
                (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>()
                 where unit.IsValidForFamilyCountryAndPeriod(family, UnitOperators, decade)
                 select unit.ID).ToArray();

            if (validUnits.Length == 0)
            {
                if (!useDefaultUnitList)
                    return new string[0];
            }
                validUnits = new string[] { }; // TODO: load from default unit list

            UnitCategory category = Toolbox.GetUnitCategoryFromUnitFamily(family);
            if ((category == UnitCategory.Helicopter) || (category == UnitCategory.Plane))
                count = Toolbox.Clamp(count, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
            else if (category != UnitCategory.Vehicle)
                count = 1;

            // Different unit types allowed in the group, pick a random type for each unit.
            if (category != UnitCategory.Vehicle)
            {
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(validUnits));

                return selectedUnits.ToArray();
            }
            else // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            {
                string unit = Toolbox.RandomFrom(validUnits);
                return Enumerable.Repeat(unit, count).ToArray();
            }
        }
    }
}
