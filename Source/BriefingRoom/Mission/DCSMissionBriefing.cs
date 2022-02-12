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

using BriefingRoom4DCS.Generator;
using System.Collections.Generic;

namespace BriefingRoom4DCS.Mission
{
    public sealed class DCSMissionBriefing
    {
        private readonly DCSMission Mission;

        public string Name { get; internal set; } = "";
        public string Description { get; internal set; } = "";

        private readonly List<string>[] Items;

        internal List<DCSMissionFlightBriefing> FlightBriefings {get; set;} = new List<DCSMissionFlightBriefing>();

        internal DCSMissionBriefing(DCSMission mission)
        {
            Mission = mission;

            Items = new List<string>[Toolbox.EnumCount<DCSMissionBriefingItemType>()];
            for (int i = 0; i < Items.Length; i++)
                Items[i] = new List<string>();
        }

        public string[] GetItems(DCSMissionBriefingItemType briefingItemType)
        {
            return Items[(int)briefingItemType].ToArray();
        }

        internal void AddItem(DCSMissionBriefingItemType briefingItemType, string briefingItem, bool insertFirst = false)
        {
            if (insertFirst)
                Items[(int)briefingItemType].Insert(0, briefingItem);
            else
                Items[(int)briefingItemType].Add(briefingItem);
        }

        public string GetBriefingAsHTML(bool htmlHeaderAndFooter = true)
        {
            string html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}Briefing.html");
            if (htmlHeaderAndFooter)
                html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingHeader.html") + html + Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingFooter.html");

            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingAirbases", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.Airbase)));
            GeneratorTools.ReplaceKey(ref html, "BriefingFlightGroups", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.FlightGroup)));
            GeneratorTools.ReplaceKey(ref html, "BriefingRemarks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Remark)));
            GeneratorTools.ReplaceKey(ref html, "BriefingTasks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Task)));
            GeneratorTools.ReplaceKey(ref html, "BriefingJTAC", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.JTAC)));
            GeneratorTools.ReplaceKey(ref html, "BriefingWaypoints", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.Waypoint)));

            return html;
        }

        public string GetBriefingKneeBoardTasksAndRemarksHTML()
        {
            string html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}KneeboardHeader.html") +
                Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}KneeboardTasksRemarks.html") +
                Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingFooter.html");

            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingRemarks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Remark)));
            GeneratorTools.ReplaceKey(ref html, "BriefingTasks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Task)));

            return html;
        }

        public string GetBriefingKneeBoardFlightsHTML()
        {
            string html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}KneeboardHeader.html") +
                Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}KneeboardFlights.html") +
                Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingFooter.html");

            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingFlightGroups", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.FlightGroup)));

            return html;
        }

        public string GetBriefingAsRawText(string newLine = "\n")
        {
            string text = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}Briefing.txt");
            text = Mission.ReplaceValues(text);

            GeneratorTools.ReplaceKey(ref text, "BriefingAirbases", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.Airbase)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingFlightGroups", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.FlightGroup)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingRemarks", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.Remark)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingTasks", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.Task)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingWaypoints", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.Waypoint)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingJTAC", GeneratorTools.MakeRawTextList("\n", GetItems(DCSMissionBriefingItemType.JTAC)).Replace("\t", "    "));

            return text.Replace("\r\n", "\n").Replace("\n", newLine).Replace("\"", "''");
        }
    }
}
