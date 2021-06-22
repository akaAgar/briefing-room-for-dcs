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
    /// Stores information about a briefing random description.
    /// </summary>
    internal class DBEntryBriefingDescription : DBEntry
    {
        /// <summary>
        /// Description text for each target unit family.
        /// </summary>
        internal string[] DescriptionText { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                string defaultText = ini.GetValue<string>("BriefingDescription", "Description");

                DescriptionText = new string[Toolbox.EnumCount<UnitFamily>()];
                for (int i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
                {
                    DescriptionText[i] = ini.GetValue<string>("BriefingDescription", $"Description.{(UnitFamily)i}");
                    if (string.IsNullOrEmpty(DescriptionText[i]))
                        DescriptionText[i] = defaultText;
                }
            }

            return true;
        }
    }
}
