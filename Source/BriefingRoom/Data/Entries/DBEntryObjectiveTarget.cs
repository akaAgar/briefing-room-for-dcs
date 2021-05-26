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

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a possible target for an objective.
    /// </summary>
    internal class DBEntryObjectiveTarget : DBEntry
    {
        /// <summary>
        /// Name as displayed .
        /// Index #0 is singular form, index #1 is plural form.
        /// </summary>
        internal string[] BriefingName { get; private set; }

        /// <summary>
        /// Unit category these target units belong to.
        /// </summary>
        internal UnitCategory UnitCategory { get { return UnitFamilies[0].GetUnitCategory(); } }

        /// <summary>
        /// Minimum/maximum number of units in this group, for each "target group size" setting.
        /// </summary>
        internal MinMaxI[] UnitCount { get; private set; }

        /// <summary>
        /// Valid unit families for this target.
        /// </summary>
        internal UnitFamily[] UnitFamilies { get; private set; }

        /// <summary>
        /// Valid spawn points on which to spawn this item.
        /// </summary>
        internal SpawnPointType[] ValidSpawnPoints { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>
        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingName = new string[2]
                {
                        ini.GetValue<string>("ObjectiveTarget", "Briefing.UnitName.Singular"),
                        ini.GetValue<string>("ObjectiveTarget", "Briefing.UnitName.Plural")
                };

                UnitFamilies = Toolbox.SetSingleCategoryFamilies(ini.GetValueArray<UnitFamily>("ObjectiveTarget", "Units.Families"));
                if (UnitFamilies.Length == 0)
                {
                    BriefingRoom.PrintToLog($"No unit categories for objective target \"{ID}\"", LogMessageErrorLevel.Warning);
                    return false;
                }

                UnitCount = new MinMaxI[Toolbox.EnumCount<Amount>()];
                foreach (Amount amount in Toolbox.GetEnumValues<Amount>())
                    UnitCount[(int)amount] = ini.GetValue<MinMaxI>("ObjectiveTarget", $"Units.Count.{amount}");

                ValidSpawnPoints = DatabaseTools.CheckSpawnPoints(ini.GetValueArray<SpawnPointType>("ObjectiveTarget", "ValidSpawnPoints"));
            }

            return true;
        }
    }
}
