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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Mission
{
    public sealed class DCSMissionBriefing 
    {
        private readonly DCSMission Mission;

        public string Name { get; internal set; } = "";
        public string Description { get; internal set; } = "";

        private readonly List<string>[] Items;
        //BRIEFINGWAYPOINTS

        internal DCSMissionBriefing(DCSMission mission)
        {
            Mission = mission;

            Items = new List<string>[Toolbox.EnumCount<DCSMissionBriefingItemType>()];
            for (int i = 0; i < Items.Length; i++)
                Items[i] = new List<string>();
        }

        public string[] GetItems(DCSMissionBriefingItemType briefingItemType) { return Items[(int)briefingItemType].ToArray(); }
        
        internal void AddItem(DCSMissionBriefingItemType briefingItemType, string task) { Items[(int)briefingItemType].Add(task); }

        internal void AddRemarkFromFeature(DBEntryFeature featureDB, bool useEnemyRemarkIfAvailable, params KeyValuePair<string, string>[] stringReplacements)
        {
            string[] remarks;
            if (useEnemyRemarkIfAvailable && featureDB.BriefingRemarks[(int)Side.Enemy].Length > 0)
                remarks = featureDB.BriefingRemarks[(int)Side.Enemy].ToArray();
            else
                remarks = featureDB.BriefingRemarks[(int)Side.Ally].ToArray();
            if (remarks.Length == 0) return; // No briefing for this feature

            string remark = Toolbox.RandomFrom(remarks);
            foreach (KeyValuePair<string, string> stringReplacement in stringReplacements)
                GeneratorTools.ReplaceKey(ref remark, stringReplacement.Key, stringReplacement.Value);

            AddItem(DCSMissionBriefingItemType.Remark, remark);
        }

        public string GetBriefingAsHTML(bool htmlHeaderAndFooter = true)
        {
            string html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}Briefing.html");
            if (htmlHeaderAndFooter)
                html = Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingHeader.html") + html + Toolbox.ReadAllTextIfFileExists($"{BRPaths.INCLUDE_HTML}BriefingFooter.html");

            html = Mission.ReplaceValues(html);

            GeneratorTools.ReplaceKey(ref html, "BriefingAirbases", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.Airbase)));
            GeneratorTools.ReplaceKey(ref html, "BriefingCarriers", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.Carrier)));
            GeneratorTools.ReplaceKey(ref html, "BriefingFlightGroups", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.FlightGroup)));
            GeneratorTools.ReplaceKey(ref html, "BriefingRemarks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Remark)));
            GeneratorTools.ReplaceKey(ref html, "BriefingTasks", GeneratorTools.MakeHTMLList(GetItems(DCSMissionBriefingItemType.Task)));
            GeneratorTools.ReplaceKey(ref html, "BriefingWaypoints", GeneratorTools.MakeHTMLTable(GetItems(DCSMissionBriefingItemType.Waypoint)));

            return html;
        }

        public string GetBriefingAsRawText(string newLine = "\n")
        {
            return ""; // TODO
        }
    }
}
