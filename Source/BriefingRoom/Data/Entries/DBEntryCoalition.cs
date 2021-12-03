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

            using (INIFile ini = new INIFile(iniFilePath))
            {
                //BriefingElements = new string[Toolbox.EnumCount<CoalitionBriefingElement>()][];
                //for (i = 0; i < BriefingElements.Length; i++)
                //    BriefingElements[i] = ini.GetValueArray<string>("Briefing", $"Elements.{(CoalitionBriefingElement)i}");

                string[] badCountries = (from country in ini.GetValueArray<string>("Coalition", "Countries").Distinct() where !Enum.TryParse<Country>(country, true, out _) select country).ToArray();
                if (badCountries.Length > 0)
                    BriefingRoom.PrintToLog($"Bad countr{(badCountries.Length == 1 ? "y" : "ies")} in coalition \"{ID}\": {string.Join(", ", badCountries)}", LogMessageErrorLevel.Warning);

                Countries = ini.GetValueArray<Country>("Coalition", "Countries").Distinct().OrderBy(x => x).ToArray();
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
            }

            return true;
        }

        internal string[] GetRandomUnits(UnitFamily family, Decade decade, int count, List<string> unitMods, bool useDefaultList = true)
        {
            // Count is zero, return an empty array.
            if (count < 1) return new string[0];

            UnitCategory category = family.GetUnitCategory();
            bool allowDifferentUnitTypes = false;

            switch (category)
            {
                // Units are planes or helicopters, make sure unit count does not exceed the maximum flight group size
                case UnitCategory.Helicopter:
                case UnitCategory.Plane:
                    count = Toolbox.Clamp(count, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
                    break;
                // Units are ships or static buildings, only one unit per group (that's the law in DCS World, buddy)
                case UnitCategory.Ship:
                case UnitCategory.Static:
                    count = 1;
                    break;
                // Units are ground vehicles, allow multiple unit types in the group
                case UnitCategory.Vehicle:
                    allowDifferentUnitTypes = true;
                    break;
            }

            string[] validUnits = SelectValidUnits(family, decade, unitMods, useDefaultList);

            // Different unit types allowed in the group, pick a random type for each unit.
            if (allowDifferentUnitTypes)
            {
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(validUnits));

                return selectedUnits.ToArray();
            }
            // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            else
            {
                string unit = Toolbox.RandomFrom(validUnits);
                return Enumerable.Repeat(unit, count).ToArray();
            }
        }

        private string[] SelectValidUnits(UnitFamily family, Decade decade, List<string> unitMods, bool useDefaultList)
        {
            List<string> validUnits = new List<string>();

            foreach (Country country in Countries)
                validUnits.AddRange(
                    from DBEntryUnit unit in Database.GetAllEntries<DBEntryUnit>()
                    where unit.Families.Contains(family) && unit.Operators.ContainsKey(country) &&
                    (string.IsNullOrEmpty(unit.RequiredMod) || unitMods.Contains(unit.RequiredMod, StringComparer.InvariantCultureIgnoreCase)) &&
                    (unit.Operators[country][0] <= decade) && (unit.Operators[country][1] >= decade)
                    select unit.ID);

            validUnits = validUnits.Distinct().ToList();

            // At least one unit found, return it
            if (validUnits.Count > 0)
                return validUnits.ToArray();

            // No unit found
            if (!useDefaultList) return new string[0];
            return Database.GetEntry<DBEntryDefaultUnitList>(DefaultUnitList).DefaultUnits[(int)family, (int)decade];
        }
    }
}
