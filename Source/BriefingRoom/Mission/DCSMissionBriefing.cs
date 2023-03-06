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
using System;
using System.Collections.Generic;
using System.IO;

namespace BriefingRoom4DCS.Mission
{
    public sealed class DCSMissionBriefing
    {
        private readonly DCSMission Mission;

        public string Name { get; internal set; } = "";
        public string Description { get; internal set; } = "";

        private readonly Dictionary<DCSMissionBriefingItemType, List<string>> Items;

        internal List<DCSMissionFlightBriefing> FlightBriefings { get; set; } = new List<DCSMissionFlightBriefing>();

        internal DCSMissionBriefing(DCSMission mission)
        {
            Mission = mission;

            Items = new Dictionary<DCSMissionBriefingItemType, List<string>>{};
            var enumList = Enum.GetValues(typeof(DCSMissionBriefingItemType));
            foreach (DCSMissionBriefingItemType enumItem in enumList)
            {
                Items.Add(enumItem, new List<string>());
            }
        }

        public List<string> GetItems(DCSMissionBriefingItemType briefingItemType)
        {
            return Items[briefingItemType];
        }

        public void UpdateItem(DCSMissionBriefingItemType briefingItemType, int index, string str)
        {
            Items[briefingItemType][index] = str;
        }

        internal void AddItem(DCSMissionBriefingItemType briefingItemType, string briefingItem, bool insertFirst = false)
        {
            if (insertFirst)
                Items[briefingItemType].Insert(0, briefingItem);
            else
                Items[briefingItemType].Add(briefingItem);
        }

        public string GetBriefingAsHTML(bool htmlHeaderAndFooter = true)
        {
            string html = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "Briefing.html"));
            if (htmlHeaderAndFooter)
                html = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "BriefingHeader.html")) + html + Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "BriefingFooter.html"));
            html = BriefingRoom.LanguageDB.ReplaceValues(html);
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
            string html = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "KneeboardHeader.html")) +
                Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "KneeboardTasksRemarks.html")) +
                Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "BriefingFooter.html"));

            html = BriefingRoom.LanguageDB.ReplaceValues(html);
            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingRemarks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Remark)));
            GeneratorTools.ReplaceKey(ref html, "BriefingTasks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Task)));

            return html;
        }

        public string GetBriefingKneeBoardFlightsHTML()
        {
            string html = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "KneeboardHeader.html")) +
                Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "KneeboardFlights.html")) +
                Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "BriefingFooter.html"));

            html = BriefingRoom.LanguageDB.ReplaceValues(html);
            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingFlightGroups", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.FlightGroup)));

            return html;
        }

        public string GetBriefingAsRawText(string newLine = "\n")
        {
            string text = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "Briefing.txt"));
            text = BriefingRoom.LanguageDB.ReplaceValues(text);
            text = Mission.ReplaceValues(text);

            GeneratorTools.ReplaceKey(ref text, "BriefingAirbases", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.Airbase)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingFlightGroups", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.FlightGroup)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingRemarks", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.Remark)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingTasks", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.Task)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingWaypoints", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.Waypoint)).Replace("\t", "    "));
            GeneratorTools.ReplaceKey(ref text, "BriefingJTAC", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.JTAC)).Replace("\t", "    "));

            return text.Replace("\r\n", "\n").Replace("\n", newLine).Replace("\"", "''");
        }

        public string GetEditorNotes(string newLine = "\n")
        {
            string text = Toolbox.ReadAllTextIfFileExists(Path.Combine(BRPaths.INCLUDE_HTML, "EditorNotes.txt"));
            text = BriefingRoom.LanguageDB.ReplaceValues(text);
            GeneratorTools.ReplaceKey(ref text, "TARGETGROUPNAMES", GeneratorTools.MakeRawTextList(GetItems(DCSMissionBriefingItemType.TargetGroupName)).Replace("\t", "    "));
            return text.Replace("\r\n", "\n").Replace("\n", newLine).Replace("\"", "''");
        }

    }
}
