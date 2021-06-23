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


using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a default unit list.
    /// Used by <see cref="DBEntryCoalition.GetRandomUnits(UnitFamily, Decade, int, string[], bool)"/> when no matching unit are found for a Coalition/Decade/UnitFamily combination.
    /// </summary>
    internal class DBEntryDefaultUnitList : DBEntry
    {
        /// <summary>
        /// Default units for each decade and unit category
        /// </summary>
        internal string[,][] DefaultUnits { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            int i, j;

            DefaultUnits = new string[Toolbox.EnumCount<UnitFamily>(), Toolbox.EnumCount<Decade>()][];
            for (i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
                for (j = 0; j < Toolbox.EnumCount<Decade>(); j++)
                    DefaultUnits[i, j] = new string[0];

            using (INIFile ini = new INIFile(iniFilePath))
            {
                foreach (UnitFamily family in Toolbox.GetEnumValues<UnitFamily>())
                    foreach (Decade decade in Toolbox.GetEnumValues<Decade>())
                    {
                        string[] units = GetValidDBEntryIDs<DBEntryUnit>(ini.GetValueArray<string>($"{decade}", $"{family}"), out string[] invalidUnits);

                        foreach (string u in invalidUnits)
                            BriefingRoom.PrintToLog($"Unit \"{u}\" not found in default unit list \"{ID}\".", LogMessageErrorLevel.Warning);

                        if (units.Length == 0) continue;

                        for (i = (int)decade; i <= (int)Decade.Decade2020; i++)
                            DefaultUnits[(int)family, i] = units.ToArray();
                    }
            }

            for (i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
                for (j = 0; j < Toolbox.EnumCount<Decade>(); j++)
                    if (DefaultUnits[i, j].Length == 0)
                    {
                        BriefingRoom.PrintToLog($"Default unit list \"{ID}\" has no unit of family \"{(UnitFamily)i}\" during {(Decade)j}, unit list was ignored.", LogMessageErrorLevel.Warning);
                        return false;
                    }


            return true;
        }
    }
}
