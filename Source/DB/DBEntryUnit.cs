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
using System.Linq;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores information about a DCS World unit.
    /// </summary>
    public class DBEntryUnit : DBEntry
    {
        /// <summary>
        /// Specific data for aircraft units.
        /// </summary>
        public DBEntryUnitAircraftData AircraftData { get; private set; }

        /// <summary>
        /// DCS World unit category this unit belongs to.
        /// </summary>
        public UnitCategory Category { get { return Toolbox.GetUnitCategoryFromUnitFamily(DefaultFamily); } }
        
        /// <summary>
        /// Internal DCS World IDs for this unit.
        /// A vehicle/static unit database entry can be made of multiple units (e.g. SAM sites, factories...)
        /// </summary>
        public string[] DCSIDs { get; private set; }

        /// <summary>
        /// Internal DCS World shapes for this unit. Only used by static units (aka buildings).
        /// </summary>
        public string[] DCSShapes { get; private set; }

        /// <summary>
        /// Default unit family for this unit, mainly used to get random unit group names etc.
        /// </summary>
        public UnitFamily DefaultFamily { get; private set; }

        /// <summary>
        /// Extra lua to add for units of this type.
        /// </summary>
        public string ExtraLua { get; private set; }

        /// <summary>
        /// Special flags for this unit.
        /// </summary>
        public DBEntryUnitFlags Flags { get; private set; }

        /// <summary>
        /// True if unit is a helicopter or plane, false otherwise.
        /// </summary>
        public bool IsAircraft { get { return (Category == UnitCategory.Helicopter) || (Category == UnitCategory.Plane); } }

        /// <summary>
        /// Coordinates offset for each unit in the group.
        /// </summary>
        public Coordinates[] OffsetCoordinates { get; private set; }

        /// <summary>
        /// Heading offset for each unit in the group.
        /// </summary>
        public double[] OffsetHeading { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                // Unit info
                DCSIDs = (from string u in ini.GetValueArray<string>("Unit", "DCSID") select u.Trim()).ToArray();
                if (DCSIDs.Length < 1)
                {
                    DebugLog.Instance.WriteLine($"Unit {ID} contains no DCS unit ID, unit was ignored.", DebugLogMessageErrorLevel.Warning);
                    return false;
                }
                DefaultFamily = ini.GetValue<UnitFamily>("Unit", "DefaultFamily");
                ExtraLua = ini.GetValue<string>("Unit", "ExtraLua");
                Flags = ini.GetValueArrayAsEnumFlags<DBEntryUnitFlags>("Unit", "Flags");
                OffsetCoordinates = (from string s in ini.GetValueArray<string>("Unit", "Offset.Coordinates", ';') select new Coordinates(s)).ToArray();
                OffsetHeading = ini.GetValueArray<double>("Unit", "Offset.Heading");

                AircraftData = new DBEntryUnitAircraftData();

                if (IsAircraft) // Load aircraft-specific data, if required
                {
                    DCSIDs = new string[] { DCSIDs[0] }; // Aircraft can not have multiple unit types in their group
                    AircraftData = new DBEntryUnitAircraftData(ini);
                }
                else if (Category == UnitCategory.Static) // Load static-specific data, if required
                {
                    DCSShapes = ini.GetValueArray<string>("Unit", "DCSID.Shape");
                }
            }

            return true;
        }
    }
}
