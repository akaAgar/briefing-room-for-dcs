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

namespace BriefingRoom.DB
{
    /// <summary>
    /// Stores information about a mission feature.
    /// </summary>
    public class DBEntryMissionFeature : DBEntry
    {
        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) single-line remarks to add to the mission briefing when this feature is enabled.
        /// </summary>
        public string[] BriefingRemarks { get; private set; }

        /// <summary>
        /// Lua scripts to include (from Include\Lua\IncludedScripts) in the mission when this feature is enabled.
        /// </summary>
        public string[] IncludeLua { get; private set; }

        /// <summary>
        /// Ogg files to include in the .miz file when this mission feature is enabled.
        /// </summary>
        public string[] IncludeOgg { get; private set; }

        /// <summary>
        /// Lua code to include just before the included files.
        /// </summary>
        public string LuaSettings { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string IncompatiblePrefix { get; private set; }

        /// <summary>
        /// Unit group to spawn when this mission feature is enabled.
        /// </summary>
        public DBUnitGroup UnitGroup { get; private set; }

        /// <summary>
        /// Initial position (index #0) and destination (index #1) of the unit group.
        /// </summary>
        public DBEntryMissionFeatureUnitGroupLocation[] UnitGroupCoordinates { get; private set; }

        /// <summary>
        /// Loads a database entry from an .ini file.
        /// </summary>
        /// <param name="iniFilePath">Path to the .ini file where entry inforation is stored</param>
        /// <returns>True is successful, false if an error happened</returns>

        protected override bool OnLoad(string iniFilePath)
        {
            using (INIFile ini = new INIFile(iniFilePath))
            {
                BriefingRemarks = ini.GetValueArray<string>("Briefing", "Remarks", ';');
                IncludeLua = ini.GetValueArray<string>("Include", "Lua");
                IncludeOgg = ini.GetValueArray<string>("Include", "Ogg");
                LuaSettings = ini.GetValue<string>("Lua", "Settings");

                IncompatiblePrefix = ini.GetValue<string>("UnitGroup", "Position.Initial").ToLowerInvariant();

                UnitGroup = new DBUnitGroup(ini, "UnitGroup");
                UnitGroupCoordinates = new DBEntryMissionFeatureUnitGroupLocation[2];
                UnitGroupCoordinates[0] = ini.GetValue<DBEntryMissionFeatureUnitGroupLocation>("UnitGroup", "Position.Initial");
                UnitGroupCoordinates[1] = ini.GetValue<DBEntryMissionFeatureUnitGroupLocation>("UnitGroup", "Position.Destination");
            }

            foreach (string f in IncludeLua)
                if (!File.Exists($"{BRPaths.INCLUDE_LUA_INCLUDEDSCRIPTS}{f}.lua"))
                    DebugLog.Instance.WriteLine($"File \"Include\\Lua\\IncludedScripts\\{f}.lua\", required by mission feature \"{ID}\", doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);

            foreach (string f in IncludeOgg)
                if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                    DebugLog.Instance.WriteLine($"File \"Include\\Ogg\\{f}.ogg\", required by mission feature \"{ID}\", doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);

            return true;
        }
    }
}
