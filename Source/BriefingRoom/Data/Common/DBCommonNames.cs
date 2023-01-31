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
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Data
{
    internal class DBCommonNames
    {
        internal const int MISSION_NAMES_PART_COUNT = 4;

        internal LanguageString[] MissionNameParts { get; } = new LanguageString[MISSION_NAMES_PART_COUNT];

        internal LanguageString MissionNameTemplate { get; }

        internal LanguageString[] UnitFamilies { get; } = new LanguageString[Toolbox.EnumCount<UnitFamily>()];

        internal LanguageString[] UnitGroups { get; } = new LanguageString[Toolbox.EnumCount<UnitFamily>()];

        internal LanguageString WPEgressName { get; }

        internal LanguageString WPFinalName { get; }

        internal LanguageString WPIngressName { get; }

        internal LanguageString WPInitialName { get; }

        internal LanguageString WPObjectivesNames { get; }

        public DBCommonNames()
        {
            int i;

            BriefingRoom.PrintToLog("Loading common global settings...");
            INIFile ini = new(Path.Combine(BRPaths.DATABASE, "Names.ini"));
            MissionNameTemplate = ini.GetLangStrings("Mission", "Template");
            for (i = 0; i < MISSION_NAMES_PART_COUNT; i++)
                MissionNameParts[i] = ini.GetLangStrings("Mission", $"Part{i + 1}");

            for (i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
            {
                UnitFamilies[i] = ini.GetLangStrings("UnitFamilies", ((UnitFamily)i).ToString());
                UnitGroups[i] = ini.GetLangStrings("UnitGroups", ((UnitFamily)i).ToString());
            }

            WPEgressName = ini.GetLangStrings("Waypoints", "Egress");
            WPFinalName = ini.GetLangStrings("Waypoints", "Final");
            WPIngressName = ini.GetLangStrings("Waypoints", "Ingress");
            WPInitialName = ini.GetLangStrings("Waypoints", "Initial");
            WPObjectivesNames = ini.GetLangStrings("Waypoints", "Objectives");
        }
    }
}