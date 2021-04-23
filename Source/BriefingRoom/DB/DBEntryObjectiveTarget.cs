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

using BriefingRoom.Debug;
using System.IO;
using System.Linq;

namespace BriefingRoom.DB
{
    /// <summary>
    /// Stores information about a possible target for an objective.
    /// </summary>
    public class DBEntryObjectiveTarget : DBEntry
    {
        public UnitCategory UnitCategory { get { return UnitFamilies[0].GetCategory(); } }

        public UnitFamily[] UnitFamilies { get; private set; }

        public MinMaxI[] UnitCount { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                UnitFamilies = ini.GetValueArray<UnitFamily>("ObjectiveTarget", "Units.Families");
                if (UnitFamilies.Length == 0)
                {
                    DebugLog.Instance.WriteLine($"No unit categories for objective target \"{ID}\"", DebugLogMessageErrorLevel.Warning);
                    return false;
                }
                
                // Make sure all unit families belong to the same category
                UnitFamilies = (from UnitFamily unitFamily in UnitFamilies where unitFamily.GetCategory() == UnitCategory select unitFamily).ToArray();

                UnitCount = new MinMaxI[Toolbox.GetEnumValuesCount<Amount>()];
                foreach (Amount amount in Toolbox.GetEnumValues<Amount>())
                    UnitCount[(int)amount] = ini.GetValue<MinMaxI>("ObjectiveTarget", $"Units.Count.{amount}");
            }

            return true;
        }
    }
}
