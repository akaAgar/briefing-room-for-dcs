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

using BriefingRoom4DCS.Generator;
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

       private static readonly List<UnitFamily> SINGLE_TYPE_FAMILIES = new List<UnitFamily>{UnitFamily.VehicleMissile, UnitFamily.VehicleArtillery};


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

            return true;
        }

        internal Tuple<Country, DBEntryTemplate> GetRandomTemplate(List<UnitFamily> families, Decade decade, List<string> unitMods, MinMaxI? countMinMax = null)
        {

            var validTemplates = new Dictionary<Country, List<DBEntryTemplate>>();
            foreach (var country in Countries)
            {
                validTemplates[country] = (
                        from template in Database.GetAllEntries<DBEntryTemplate>()
                        where families.Contains(template.Family) && template.Countries.ContainsKey(country)
                            && (string.IsNullOrEmpty(template.Module) || unitMods.Contains(template.Module, StringComparer.InvariantCultureIgnoreCase) || DBEntryDCSMod.CORE_MODS.Contains(template.Module, StringComparer.InvariantCultureIgnoreCase))
                            && (template.Countries[country].start <= decade) && (template.Countries[country].end >= decade)
                            && (!countMinMax.HasValue || countMinMax.Value.Contains(template.Units.Count()))
                        select template
                    ).Distinct().ToList();
            }

            validTemplates = validTemplates.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

            if (validTemplates.Count() == 0)
            {
                 BriefingRoom.PrintToLog($"No Units of types {string.Join(", ", families)} found in coalition of {string.Join(", ", Countries.Where(x => x != Country.ALL))}");
                 return null;
            }

            var selectedCountry = Toolbox.RandomFrom(validTemplates.Keys.ToList());
            return new(selectedCountry, Toolbox.RandomFrom(validTemplates[selectedCountry]));
        }



        internal Tuple<Country, List<string>> GetRandomUnits(List<UnitFamily> families, Decade decade, int count, List<string> unitMods, bool allowLowPolly, Country? requiredCountry = null, bool lowUnitVariation = false)
        {
            // Count is zero, return an empty array.
            if (count < 1) throw new BriefingRoomException("Asking for a zero unit list");
            if (families.Select(x => x.GetDCSUnitCategory()).Any(x => x != families.First().GetDCSUnitCategory())) throw new BriefingRoomException($"Cannot mix Categories in types {string.Join(", ", families)}");

            var category = families.First().GetDCSUnitCategory();
            bool allowDifferentUnitTypes = false;

            var validUnits = SelectValidUnits(families, decade, unitMods, allowLowPolly);

            switch (category)
            {
                // Units are planes or helicopters, make sure unit count does not exceed the maximum flight group size
                case DCSUnitCategory.Helicopter:
                case DCSUnitCategory.Plane:
                    count = Toolbox.Clamp(count, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
                    break;
                // Units are ships or static buildings, only one unit per group (that's the law in DCS World, buddy)
                case DCSUnitCategory.Ship:
                    count = 1;
                    break;
                case DCSUnitCategory.Static:
                    count = 1;
                    break;
                // Units are ground vehicles, allow multiple unit types in the group
                case DCSUnitCategory.Vehicle:
                    allowDifferentUnitTypes = families.Count > 1 || !SINGLE_TYPE_FAMILIES.Contains(families.First());
                    break;
            }


            var selectableUnits = new List<string>();
            var country = Toolbox.RandomFrom(validUnits.Keys.ToList());
            if (requiredCountry.HasValue)
                if (validUnits.ContainsKey(requiredCountry.Value))
                    country = requiredCountry.Value;
                else
                    BriefingRoom.PrintToLog($"Could not find suitable units for {requiredCountry.Value} using units from other coalition members.", LogMessageErrorLevel.Info);

            selectableUnits = validUnits[country];


            // Different unit types allowed in the group, pick a random type for each unit.
            if (allowDifferentUnitTypes)
            {
                if (lowUnitVariation)
                    selectableUnits = new List<string> { Toolbox.RandomFrom(selectableUnits), Toolbox.RandomFrom(selectableUnits) };
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(selectableUnits));

                return new(country, selectedUnits.ToList());
            }

            // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            string unit = Toolbox.RandomFrom(selectableUnits);
            return new(country, Enumerable.Repeat(unit, count).ToList());
        }

        private Dictionary<Country, List<string>> SelectValidUnits(List<UnitFamily> families, Decade decade, List<string> unitMods, bool allowLowPolly)
        {
            var validUnits = new Dictionary<Country, List<string>>();

            foreach (Country country in Countries)
                validUnits[country] = (
                        from unit in Database.GetAllEntries<DBEntryJSONUnit>()
                        where unit.Families.Intersect(families).ToList().Count > 0 && unit.Operators.ContainsKey(country)
                            && (string.IsNullOrEmpty(unit.Module) || unitMods.Contains(unit.Module, StringComparer.InvariantCultureIgnoreCase) || DBEntryDCSMod.CORE_MODS.Contains(unit.Module, StringComparer.InvariantCultureIgnoreCase))
                            && (unit.Operators[country].start <= decade) && (unit.Operators[country].end >= decade)
                            && (!unit.lowPolly || allowLowPolly)
                        select unit.ID
                    ).Distinct().ToList();

            // Ensures that only countries with units listed get returned
            validUnits = validUnits.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
            // At least one unit found, return it
            if (validUnits.Count > 0)
                return validUnits;

            BriefingRoom.PrintToLog($"No Units of types {string.Join(", ", families)} found in coalition of {string.Join(", ", Countries.Where(x => x != Country.ALL))} forced to use defaults", LogMessageErrorLevel.Info);
            
            return GetDefaultUnits(Database, DefaultUnitList, families, decade, unitMods);
        }

        internal static Dictionary<Country, List<string>> GetDefaultUnits(Database database, string defaultUnitList,  List<UnitFamily> families, Decade decade, List<string> unitMods)
        {
            var defaultDict = database.GetEntry<DBEntryDefaultUnitList>(defaultUnitList).DefaultUnits;
            var validUnits = new List<string>();
            foreach (var family in families)
            {
                if(!defaultDict[family].ContainsKey(decade))
                    continue;
                var options = defaultDict[family][decade].Where(id => 
                {
                    var unit = database.GetEntry<DBEntryJSONUnit>(id);
                    return string.IsNullOrEmpty(unit.Module) || unitMods.Contains(unit.Module, StringComparer.InvariantCultureIgnoreCase) || DBEntryDCSMod.CORE_MODS.Contains(unit.Module, StringComparer.InvariantCultureIgnoreCase);
                });
                validUnits.AddRange(options);
            }
            if(validUnits.Count == 0)
                BriefingRoom.PrintToLog($"No operational units found in {decade} for types {String.Join(", ", families)}", LogMessageErrorLevel.Warning);
            return new Dictionary<Country, List<string>> { { Country.ALL, validUnits } };
        }
    }
}
