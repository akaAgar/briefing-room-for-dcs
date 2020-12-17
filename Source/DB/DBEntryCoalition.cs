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
    /// Stores information about a coalition.
    /// </summary>
    public class DBEntryCoalition : DBEntry
    {

        public string[][] BriefingElements { get; private set; }

        public UnitSystem BriefingUnitSystem { get; private set; }

        /// <summary>
        /// IDs of <see cref="DBEntryUnit"/> carrier ships available to this coalition, by <see cref="CarrierType"/>
        /// </summary>
        public string[][] Carriers { get; private set; }

        /// <summary>
        /// Countries included in this coalition.
        /// </summary>
        public string[] Countries { get; private set; }

        /// <summary>
        /// Decade during which this coalition is active.
        /// </summary>
        public Decade Decade { get; private set; }

        /// <summary>
        /// Does this coalition use NATO callsigns for its units?
        /// </summary>
        public bool NATOCallsigns { get; private set; }

        /// <summary>
        /// Support units (AWACS, tankets...) in use by this coalition.
        /// </summary>
        public string[][] SupportUnits { get; private set; }

        /// <summary>
        /// Units in use by this coalition.
        /// </summary>
        public string[][] Units { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            int i;

            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingElements = new string[Toolbox.EnumCount<CoalitionBriefingElement>()][];
                for (i = 0; i < BriefingElements.Length; i++)
                    BriefingElements[i] = ini.GetValueArray<string>("Briefing", $"Elements.{(CoalitionBriefingElement)i}");
                BriefingUnitSystem = ini.GetValue<UnitSystem>("Briefing", "UnitSystem");

                Carriers = new string[Toolbox.EnumCount<CarrierType>()][];
                for (i = 0; i < Carriers.Length; i++)
                {
                    Carriers[i] = GetValidDBEntryIDs<DBEntryUnit>(
                        ini.GetValueArray<string>("Carriers", ((CarrierType)i).ToString()), out string[] invalidUnits);

                    foreach (string u in invalidUnits)
                        DebugLog.Instance.WriteLine($"Unit \"{u}\" not found in carriers for coalition \"{ID}\"", DebugLogMessageErrorLevel.Warning);
                }

                Decade = ini.GetValue<Decade>("Coalition", "Decade");

                NATOCallsigns = ini.GetValue("Coalition", "NATOCallsigns", false);

                SupportUnits = new string[Toolbox.EnumCount<SupportUnitRoles>()][];
                for (i = 0; i < SupportUnits.Length; i++)
                {
                    SupportUnits[i] = GetValidDBEntryIDs<DBEntryUnit>(
                        ini.GetValueArray<string>("Support", ((SupportUnitRoles)i).ToString()), out string[] invalidUnits);

                    foreach (string u in invalidUnits)
                        DebugLog.Instance.WriteLine($"Unit \"{u}\" not found in coalition \"{ID}\"", DebugLogMessageErrorLevel.Warning);
                }

                Units = new string[Toolbox.EnumCount<UnitFamily>()][];
                for (i = 0; i < Units.Length; i++)
                {
                    Units[i] = GetValidDBEntryIDs<DBEntryUnit>(
                        ini.GetValueArray<string>("Units", ((UnitFamily)i).ToString()), out string[] invalidUnits);

                    foreach (string u in invalidUnits)
                        DebugLog.Instance.WriteLine($"Unit \"{u}\" not found in coalition \"{ID}\"", DebugLogMessageErrorLevel.Warning);

                    if (Units[i].Length == 0)
                    {
                        DebugLog.Instance.WriteLine($"Coalition \"{ID}\" has no unit of family \"{(UnitFamily)i}\", coalition was ignored", DebugLogMessageErrorLevel.Warning);
                        return false;
                    }
                }
            }
         
            return true;
        }

        /// <summary>
        /// Returns the ID of a random <see cref="DBEntryUnit"/> belonging to one (or more) of the <see cref="UnitFamily"/> passed as parameters.
        /// </summary>
        /// <param name="family">Family of units to choose from</param>
        /// <param name="count">Number of units to generate</param>
        /// <returns>Array of IDs of <see cref="DBEntryUnit"/></returns>
        public string[] GetRandomUnits(UnitFamily family, int count)
        {
            // Count is zero, return an empty array.
            if (count < 1) return new string[0];
            
            UnitCategory category = Toolbox.GetUnitCategoryFromUnitFamily(family);
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

            // Different unit types allowed in the group, pick a random type for each unit.
            if (allowDifferentUnitTypes)
            {
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(Units[(int)family]));

                return selectedUnits.ToArray();
            }
            // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            else
            {
                string unit = Toolbox.RandomFrom(Units[(int)family]);
                return Enumerable.Repeat(unit, count).ToArray();
            }
        }
    }
}
