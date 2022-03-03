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

using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal class DBEntryCoalition : DBEntry
    {

        internal Country[] Countries { get; private set; }

        internal string DefaultUnitList { get; private set; }

        internal bool NATOCallsigns { get; private set; }

        protected override bool OnLoad(string iniFilePath)
        {
            //int i;

            var ini = new INIFile(iniFilePath);

            string[] badCountries = (from country in ini.GetValueArray<string>("Coalition", "Countries").Distinct() where !Enum.TryParse<Country>(country, true, out _) select country).ToArray();
            if (badCountries.Length > 0)
                BriefingRoom.PrintToLog($"Bad countr{(badCountries.Length == 1 ? "y" : "ies")} in coalition \"{ID}\": {string.Join(", ", badCountries)}", LogMessageErrorLevel.Warning);

            Countries = ini.GetValueArray<Country>("Coalition", "Countries").Append(Country.ALL).Distinct().OrderBy(x => x).ToArray();
            if (Countries.Length == 0)
            {
                BriefingRoom.PrintToLog($"No country in coalition \"{ID}\", coalition was ignored.", LogMessageErrorLevel.Warning);
                return false;
            }

            DefaultUnitList = ini.GetValue<string>("Coalition", "DefaultUnitList");
            if (!Database.EntryExists<DBEntryDefaultUnitList>(DefaultUnitList))
            {
                BriefingRoom.PrintToLog($"Default unit list \"{DefaultUnitList}\" required by coalition \"{ID}\" doesn't exist. Coalition was ignored.", LogMessageErrorLevel.Warning);
                return false;
            }

            NATOCallsigns = ini.GetValue("Coalition", "NATOCallsigns", false);

            return true;
        }

        internal Tuple<Country, List<string>> GetRandomUnits(List<UnitFamily> families, Decade decade, int count, List<string> unitMods, Country? requiredCountry = null, MinMaxI? countMinMax = null)
        {
            // Count is zero, return an empty array.
            if (count < 1) throw new BriefingRoomException("Asking for a zero unit list");
            if(families.Select(x => x.GetUnitCategory()).Any(x => x != families.First().GetUnitCategory())) throw new BriefingRoomException($"Cannot mix Categories in types {string.Join(", ", families)}");

            UnitCategory category = families.First().GetUnitCategory();
            bool allowDifferentUnitTypes = false;

            var validUnits = SelectValidUnits(families, decade, unitMods);

            switch (category)
            {
                // Units are planes or helicopters, make sure unit count does not exceed the maximum flight group size
                case UnitCategory.Helicopter:
                case UnitCategory.Plane:
                    count = Toolbox.Clamp(count, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
                    break;
                // Units are ships or static buildings, only one unit per group (that's the law in DCS World, buddy)
                case UnitCategory.Ship:
                    count = 1;
                    break;
                case UnitCategory.Static:
                    validUnits = countMinMax.HasValue ? LimitValidUnitsByRequestedUnitCount(countMinMax.Value, validUnits): validUnits;
                    count = 1;
                    break;
                // Units are ground vehicles, allow multiple unit types in the group
                case UnitCategory.Vehicle:
                    allowDifferentUnitTypes = true;
                    break;
            }


            var selectableUnits = new List<string>();
            var country = Toolbox.RandomFrom(validUnits.Keys.ToList());
            if(requiredCountry.HasValue)
                if(validUnits.ContainsKey(requiredCountry.Value))
                    country = requiredCountry.Value;
                else
                    BriefingRoom.PrintToLog($"Could not find suitable units for {requiredCountry.Value} using units from other coalition members.", LogMessageErrorLevel.Info);
            
            selectableUnits = validUnits[country];


            // Different unit types allowed in the group, pick a random type for each unit.
            if (allowDifferentUnitTypes)
            {
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(selectableUnits));

                return new (country, selectedUnits.ToList());
            }

            // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            string unit = Toolbox.RandomFrom(selectableUnits);
            return new (country, Enumerable.Repeat(unit, count).ToList());
        }

        private Dictionary<Country, List<string>> LimitValidUnitsByRequestedUnitCount(
            MinMaxI countMinMax,
            Dictionary<Country, List<string>> validUnits)
        {
            var validUnitsGroupSizeBetweenMinAndMax = new Dictionary<Country, List<string>>();
            var validUnitsDCSIDsLengths = new Dictionary<Country, List<DBEntryUnit>>();

            foreach (Country country in validUnits.Keys)
            {
                validUnitsDCSIDsLengths[country] = Database.GetEntries<DBEntryUnit>(validUnits[country].ToArray()).ToList();

                // check if the list of units can satisfy the min/max requirement
                int countMinTemp = countMinMax.Min;
                bool logged = false;
                do
                {
                    validUnitsGroupSizeBetweenMinAndMax[country] = validUnits[country]
                        .Where(
                            unitID => LimitValidUnitsByMinMax(
                                unitID,
                                validUnitsDCSIDsLengths[country],
                                countMinTemp,
                                countMinMax.Max))
                        .ToList();
                    if (countMinTemp < 1) break; // if min units is now 0 and we still have no units, then an error is needed
                    if (countMinTemp < countMinMax.Max && !logged)
                    {
                        logged = true;
                        BriefingRoom.PrintToLog("Minimum Target Count is lower than requested!", LogMessageErrorLevel.Warning);
                    }
                    countMinTemp -= 1;
                }
                while (!(validUnitsGroupSizeBetweenMinAndMax[country].Count > 0));

            }

            if (validUnitsGroupSizeBetweenMinAndMax.Count < 1)
                throw new BriefingRoomException("Requested Minimum Target Count greater than Maximum Configured Target Count");
            
            return validUnitsGroupSizeBetweenMinAndMax;
        }

        private bool LimitValidUnitsByMinMax(string unitID, List<DBEntryUnit> potentiallyValidUnits, int minUnitCount, int maxUnitCount)
        {
            var potentiallyValidUnit = potentiallyValidUnits.Where(unit => unit.ID == unitID).First();
            return potentiallyValidUnit.DCSIDs.Length >= minUnitCount && potentiallyValidUnit.DCSIDs.Length <= maxUnitCount;
        }

        private Dictionary<Country, List<string>> SelectValidUnits(List<UnitFamily> families, Decade decade, List<string> unitMods)
        {
            var validUnits = new Dictionary<Country, List<string>>();

            foreach (Country country in Countries)
                validUnits[country] = (
                        from DBEntryUnit unit in Database.GetAllEntries<DBEntryUnit>()
                        where unit.Families.Intersect(families).ToList().Count > 0 && unit.Operators.ContainsKey(country) &&
                            (string.IsNullOrEmpty(unit.RequiredMod) || unitMods.Contains(unit.RequiredMod, StringComparer.InvariantCultureIgnoreCase)) &&
                            (unit.Operators[country][0] <= decade) && (unit.Operators[country][1] >= decade)
                        select unit.ID
                    ).Distinct().ToList();

            // Ensures that only countries with units listed get returned
            validUnits = validUnits.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
            // At least one unit found, return it
            if (validUnits.Count > 0)
            return validUnits;

            BriefingRoom.PrintToLog($"No Units of types {string.Join(", ", families)} found in coalition of {string.Join(", ", Countries.Where(x => x != Country.ALL))} forced to use defaults", LogMessageErrorLevel.Info);
            return new Dictionary<Country, List<string>>{{Country.ALL,Database.GetEntry<DBEntryDefaultUnitList>(DefaultUnitList).DefaultUnits[(int)families.First(), (int)decade].ToList()}};
        }

        internal void Merge(DBEntryCoalition entry)
        {
           Countries = entry.Countries.Append(Country.ALL).ToArray();
        }
    }
}
