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

using System;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores information about common mission names/wording.
    /// </summary>
    internal class DBCommonNames
    {
        /// <summary>
        /// Maximum number of name parts to use in random mission names.
        /// </summary>
        internal const int MISSION_NAMES_PART_COUNT = 4;

        /// <summary>
        /// Random mission names part.
        /// </summary>
        internal string[][] MissionNameParts { get; } = new string[MISSION_NAMES_PART_COUNT][];

        /// <summary>
        /// Random mission name template, where $P1$, $P2$, $P3$, $P4$ is remplaced with a random mission name part.
        /// </summary>
        internal string MissionNameTemplate { get; private set; }

        /// <summary>
        /// Name (singular -index #0- and plural -index #1) to display in the briefings for each unit family.
        /// </summary>
        internal string[][] UnitBriefingNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()][];

        /// <summary>
        /// Random-parsable (<see cref="Generator.GeneratorTools.ParseRandomString(string)"/>) string for unit group names of each <see cref="UnitFamily"/>.
        /// </summary>
        internal string[] UnitGroupNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()];

        /// <summary>
        /// Name of the final (landing) player waypoint.
        /// </summary>
        internal string WPFinalName { get; private set; }

        /// <summary>
        /// Name of the initial (takeoff) player waypoint.
        /// </summary>
        internal string WPInitialName { get; private set; }

        /// <summary>
        /// Name of the navigation player waypoints, where $0$, $00$, $000$... is replaced with the waypoint number.
        /// </summary>
        internal string WPNavigationName { get; private set; }

        /// <summary>
        /// Names to use for objectives and objective waypoints.
        /// </summary>
        internal string[] WPObjectivesNames { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DBCommonNames()
        {
            int i;

            BriefingRoom.PrintToLog("Loading common global settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Names.ini"))
            {
                MissionNameTemplate = ini.GetValue<string>("Mission", "Template");
                for (i = 0; i < MISSION_NAMES_PART_COUNT; i++)
                    MissionNameParts[i] = ini.GetValueArray<string>("Mission", $"Part{i + 1}");

                for (i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
                {
                    UnitBriefingNames[i] = ini.GetValueArray<string>("UnitBriefing", ((UnitFamily)i).ToString());
                    Array.Resize(ref UnitBriefingNames[i], 2);
                    UnitGroupNames[i] = ini.GetValue<string>("UnitGroup", ((UnitFamily)i).ToString());
                }

                WPFinalName = ini.GetValue<string>("Waypoints", "Final");
                WPInitialName = ini.GetValue<string>("Waypoints", "Initial");
                WPNavigationName = ini.GetValue<string>("Waypoints", "Navigation");
                WPObjectivesNames = ini.GetValueArray<string>("Waypoints", "Objectives");
            }
        }
    }
}