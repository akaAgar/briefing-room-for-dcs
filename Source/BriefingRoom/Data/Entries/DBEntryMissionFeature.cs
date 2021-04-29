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

using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about a mission feature.
    /// </summary>
    internal class DBEntryMissionFeature : DBEntry
    {
        /// <summary>
        /// Randomly-parsed (<see cref="Toolbox.ParseRandomString(string)"/>) single-line remarks to add to the mission briefing when this feature is enabled.
        /// </summary>
        internal string[] BriefingRemarks { get; private set; }

        /// <summary>
        /// Other features whose ID begins with this string are incompatible with this one.
        /// E.g. If you create an "TimeLimit30Minutes" and you don't want any other features beginning with "TimeLimit" to be allowed, set the value to "TimeLimit"
        /// </summary>
        internal string FeatureIncompatiblePrefix { get; private set; }

        /// <summary>
        /// Lua scripts to include (from Include\Lua\MissionFeatures) when this feature is enabled.
        /// Scripts will be included IN THE ORDER THEY ARE IN THIS ARRAY.
        /// </summary>
        internal string[] IncludeLua { get; private set; }

        /// <summary>
        /// Raw Lua code to include just BEFORE the included files.
        /// Keep it short. It should mostly be used to pass info to the included files.
        /// </summary>
        internal string IncludeLuaSettings { get; private set; }

        /// <summary>
        /// Ogg files to include in the .miz file when this mission feature is enabled.
        /// </summary>
        internal string[] IncludeOgg { get; private set; }

        /// <summary>
        /// Unit families the spawned unit group may belong to.
        /// An empty array means "no unit group".
        /// </summary>
        internal UnitFamily[] UnitGroupFamilies { get; private set; }

        /// <summary>
        /// Special flags for this unit group.
        /// </summary>
        internal MissionFeatureUnitGroupFlags[] UnitGroupFlags { get; private set; }

        /// <summary>
        /// Lua script (loaded from Include\Lua\Units) to use for the spawned unit group.
        /// Empty/null means "no unit group".
        /// </summary>
        internal string UnitGroupLuaGroup { get; private set; }

        /// <summary>
        /// Lua script (loaded from Include\Lua\Units) to use for each unit of the spawned unit group.
        /// Empty/null means "no unit group".
        /// </summary>
        internal string UnitGroupLuaUnit { get; private set; }

        /// <summary>
        /// Min/max number of units in the spawned unit group.
        /// Zero means "no unit group".
        /// </summary>
        internal MinMaxI UnitGroupSize { get; private set; }

        /// <summary>
        /// Where should the unit group be spawned?
        /// </summary>
        internal MissionFeatureUnitGroupSpawnPoint UnitGroupSpawnLocation { get; private set; }

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

                FeatureIncompatiblePrefix = ini.GetValue<string>("Feature", "IncompatiblePrefix").ToLowerInvariant();

                // Included files
                IncludeLua = Toolbox.AddMissingFileExtensions(ini.GetValueArray<string>("Include", "Lua"), ".lua");
                IncludeLuaSettings = ini.GetValue<string>("Lua", "LuaSettings");
                IncludeOgg = Toolbox.AddMissingFileExtensions(ini.GetValueArray<string>("Include", "Ogg"), ".ogg");

                foreach (string f in IncludeLua)
                    if (!File.Exists($"{BRPaths.INCLUDE_LUA_MISSIONFEATURES}{f}.lua"))
                        BriefingRoom.PrintToLog($"File \"Include\\Lua\\MissionFeatures\\{f}.lua\", required by feature \"{ID}\", doesn't exist.", LogMessageErrorLevel.Warning);

                foreach (string f in IncludeOgg)
                    if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                        BriefingRoom.PrintToLog($"File \"Include\\Ogg\\{f}.ogg\", required by feature \"{ID}\", doesn't exist.", LogMessageErrorLevel.Warning);

                // Unit group
                UnitGroupFamilies = Toolbox.SetSingleCategoryFamilies(ini.GetValueArray<UnitFamily>("UnitGroup", "Families"));
                UnitGroupFlags = ini.GetValueArray<MissionFeatureUnitGroupFlags>("UnitGroup", "Flags").Distinct().ToArray();
                UnitGroupLuaGroup = Toolbox.AddMissingFileExtension(ini.GetValue<string>("UnitGroup", "Lua.Group"), ".lua");
                UnitGroupLuaUnit = Toolbox.AddMissingFileExtension(ini.GetValue<string>("UnitGroup", "Lua.Unit"), ".lua");
                UnitGroupSize = ini.GetValue<MinMaxI>("UnitGroup", "Size");
                UnitGroupSpawnLocation = ini.GetValue<MissionFeatureUnitGroupSpawnPoint>("UnitGroup", "SpawnLocation");
            }

            return true;
        }
    }
}

