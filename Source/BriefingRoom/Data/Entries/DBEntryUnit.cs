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
    internal class DBEntryUnit : DBEntry
    {
        internal DBEntryUnitAircraftData AircraftData { get; private set; }

        internal UnitCategory Category { get { return Families[0].GetUnitCategory(); } }

        internal string[] DCSIDs { get; private set; }

        internal UnitFamily[] Families { get; private set; }


        internal DBEntryUnitFlags Flags { get; private set; }

        internal bool IsAircraft { get { return (Category == UnitCategory.Helicopter) || (Category == UnitCategory.Plane); } }

        internal Coordinates[] OffsetCoordinates { get; private set; }

        internal double[] OffsetHeading { get; private set; }


        internal string[] Shape { get; private set; }

        internal Dictionary<Country, Decade[]> Operators { get; private set; }

        internal Dictionary<Country, string> OperatorLiveries { get; private set; }

        internal string RequiredMod { get; private set; }

        internal int ParkingSpots { get; private set; }

        internal void Merge(DBEntryUnit entry)
        {
            AircraftData.Merge(entry.AircraftData);
        }


        protected override bool OnLoad(string iniFilePath)
        {
            var ini = new INIFile(iniFilePath);
            // Unit info
            DCSIDs = (from string u in ini.GetValueArray<string>("Unit", "DCSID") select u.Trim()).ToArray();
            if (DCSIDs.Length < 1)
            {
                BriefingRoom.PrintToLog($"Unit {ID} contains no DCS unit ID, unit was ignored.", LogMessageErrorLevel.Warning);
                return false;
            }
            Families = ini.GetValueArray<UnitFamily>("Unit", "Families");
            if (Families.Length == 0)
            {
                BriefingRoom.PrintToLog($"Unit {ID} has no family, unit was ignored.", LogMessageErrorLevel.Warning);
                return false;
            }
            // Make sure all unit families belong to same category (unit cannot be a helicopter and a ground vehicle at the same time, for instance)
            Families = (from UnitFamily f in Families where f.GetUnitCategory() == Category select f).Distinct().ToArray();
            Flags = ini.GetValueArrayAsEnumFlags<DBEntryUnitFlags>("Unit", "Flags");
            OffsetCoordinates = (from string s in ini.GetValueArray<string>("Unit", "Offset.Coordinates", ';') select new Coordinates(s)).ToArray();
            OffsetHeading = ini.GetValueArray<double>("Unit", "Offset.Heading");
            Shape = ini.GetValueArray<string>("Unit", "Shape");
            RequiredMod = ini.GetValue<string>("Unit", "RequiredMod");
            ParkingSpots = ini.GetValue<int>("Unit", "ParkingSpots");

            AircraftData = new DBEntryUnitAircraftData();

            // Load the list of operators
            Operators = new Dictionary<Country, Decade[]>();
            foreach (string k in ini.GetKeysInSection("Operators"))
            {
                if (!Enum.TryParse(k, true, out Country country))
                {
                    BriefingRoom.PrintToLog($"Country {k} in unit {ID} doesn't exist.", LogMessageErrorLevel.Warning);
                    continue;
                }

                if (Operators.ContainsKey(country)) continue;
                Operators.Add(country, ini.GetValueArrayAsMinMaxEnum<Decade>("Operators", k));
            }

            // Load the list of operators
            OperatorLiveries = new Dictionary<Country, string>();
            foreach (string k in ini.GetKeysInSection("OperatorLiveries"))
            {
                if (!Enum.TryParse(k, true, out Country country))
                {
                    BriefingRoom.PrintToLog($"Country {k} in unit {ID} doesn't exist.", LogMessageErrorLevel.Warning);
                    continue;
                }

                if (OperatorLiveries.ContainsKey(country)) continue;
                OperatorLiveries.Add(country, ini.GetValue<string>("OperatorLiveries", k));
            }

            if (IsAircraft) // Load aircraft-specific data, if required
            {
                DCSIDs = new string[] { DCSIDs[0] }; // Aircraft can not have multiple unit types in their group
                AircraftData = new DBEntryUnitAircraftData(ini, iniFilePath.Contains(BRPaths.CUSTOMDATABASE));
            }

            return true;
        }

        internal bool IsValidForFamilyCountryAndPeriod(UnitFamily family, Country[] countries, Decade decade)
        {
            // Unit does not belong to the required family
            if (!Families.Contains(family)) return false;

            foreach (Country country in countries)
            {
                if (!Operators.ContainsKey(country)) continue;
                if ((Operators[country][0] <= decade) && (Operators[country][1] >= decade))
                    return true; // Found one operator operating the unit during the required decade
            }

            return false;
        }
    }
}
