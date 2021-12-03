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
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGeneratorBriefing : IDisposable
    {

        internal MissionGeneratorBriefing()
        {

        }

        internal void GenerateMissionBriefingDescription(DCSMission mission, MissionTemplate template, List<UnitFamily> objectiveTargetUnitFamilies)
        {
            // Try to get the provided custom mission description.
            string briefingDescription = (template.BriefingMissionDescription ?? "").Replace("\r\n", "\n").Replace("\n", " ").Trim();

            // No custom description found, generate one from the most frequent objective task/target combination.
            if (string.IsNullOrEmpty(briefingDescription))
            {
                if (template.Objectives.Count == 0)
                    briefingDescription = "";
                else
                {
                    List<string> descriptionsList = new List<string>();
                    for (int i = 0; i < template.Objectives.Count; i++)
                    {
                        DBEntryBriefingDescription descriptionDB =
                            Database.Instance.GetEntry<DBEntryBriefingDescription>(
                                Database.Instance.GetEntry<DBEntryObjectiveTask>(template.Objectives[i].Task).BriefingDescription);
                        descriptionsList.Add(descriptionDB.DescriptionText[(int)objectiveTargetUnitFamilies[i]]);
                    }

                    briefingDescription = descriptionsList.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                    briefingDescription = GeneratorTools.ParseRandomString(briefingDescription);
                }
            }

            mission.Briefing.Description = briefingDescription;
            mission.SetValue("BRIEFINGDESCRIPTION", briefingDescription);
        }

        public void Dispose() { }
    }
}
