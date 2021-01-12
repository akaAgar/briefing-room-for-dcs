/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCSWorld.Debug;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores data about an unit group to be used in an <see cref="DBEntryFeature">.
    /// </summary>
    public struct DBUnitGroup
    {
        /// <summary>
        /// Unit category of this unit group, if any.
        /// Null means group is empty/should not be spawned.
        /// </summary>
        public UnitCategory? Category { get { return (Families.Length > 0) ? (UnitCategory?)Toolbox.GetUnitCategoryFromUnitFamily(Families[0]) : null; } }

        /// <summary>
        /// Min/max number of units in the group.
        /// </summary>
        public MinMaxI Count { get; }

        /// <summary>
        /// Unit families this group may belong to.
        /// </summary>
        public UnitFamily[] Families { get; }

        /// <summary>
        /// Special flags for this group.
        /// </summary>
        public DBUnitGroupFlags Flags { get; }

        /// <summary>
        /// Lua script (loaded from Include\Lua\Mission) to use for this group.
        /// </summary>
        public string[] LuaGroup { get; }

        /// <summary>
        /// Lua script (loaded from Include\Lua\Mission) to use for each unit in this group.
        /// </summary>
        public string LuaUnit { get; }

        /// <summary>
        /// Spawn points type where this unit group can be spawned.
        /// </summary>
        public TheaterLocationSpawnPointType[] SpawnPoints { get; }

        /// <summary>
        /// Find spawn point within this min max from original spawn point (for DestinationObjective) flagged units
        /// </summary>
        public MinMaxD DistanceFromPoint { get; }

        /// <summary>
        /// Should should the units have extra waypoints
        /// </summary>
        public bool ExtraWaypoints { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ini">Objective database entry ini file</param>
        /// <param name="section">Ini section to load from</param>
        public DBUnitGroup(INIFile ini, string section)
        {
            Count = ini.GetValue<MinMaxI>(section, "Count");
            DistanceFromPoint = ini.GetValue<MinMaxD>(section, "DistanceFromPoint");
            ExtraWaypoints = ini.GetValue<bool>(section, "ExtraWaypoints", false);

            Families = ini.GetValueArray<UnitFamily>(section, "Families");
            if (Families.Length > 0) // If the group has at least one unit family (no families mean the group is disabled)...
            {
                // ...make sure all families belong to the same category
                UnitFamily family0 = Families[0];
                // Families don't have to be Distinct() because one may want to mention the same familiy several times
                // so it appears more often
                Families = (from UnitFamily family in Families where Toolbox.GetUnitCategoryFromUnitFamily(family) == Toolbox.GetUnitCategoryFromUnitFamily(family0) select family).ToArray();
            }

            Flags = ini.GetValueArrayAsEnumFlags<DBUnitGroupFlags>(section, "Flags");
            LuaGroup = ini.GetValueArray<string>(section, "Lua.Group");
            LuaUnit = ini.GetValue<string>(section, "Lua.Unit");
            // TODO: check LuaGroup and LuaUnit Lua files exist in Include/Lua

            List<TheaterLocationSpawnPointType> spList = new List<TheaterLocationSpawnPointType>(ini.GetValueArray<TheaterLocationSpawnPointType>(section, "SpawnPoints"));
            if (spList.Count == 0) spList.AddRange(Toolbox.GetEnumValues<TheaterLocationSpawnPointType>()); // No spawn point type means all spawn point types
            // LandSmall implies LandLarge and LandMedium, LandMedium implies LandLarge (larger spots can handle smaller units)
            if (spList.Contains(TheaterLocationSpawnPointType.LandSmall)) spList.AddRange(new TheaterLocationSpawnPointType[] { TheaterLocationSpawnPointType.LandMedium, TheaterLocationSpawnPointType.LandLarge });
            if (spList.Contains(TheaterLocationSpawnPointType.LandMedium)) spList.Add(TheaterLocationSpawnPointType.LandLarge);
            SpawnPoints = spList.Distinct().ToArray();

            if (Families.Length > 0)
            {
                foreach (string lGroup in LuaGroup)
                    if (!File.Exists($"{BRPaths.INCLUDE_LUA_UNITS}{lGroup}.lua"))
                        DebugLog.Instance.WriteLine($"File \"Include\\Lua\\Units\\{lGroup}.lua\" doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);

                if (!File.Exists($"{BRPaths.INCLUDE_LUA_UNITS}{LuaUnit}.lua"))
                    DebugLog.Instance.WriteLine($"File \"Include\\Lua\\Units\\{LuaUnit}.lua\" doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);
            }
        }
    }
}
