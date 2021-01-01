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

using BriefingRoom4DCSWorld.Generator;
using BriefingRoom4DCSWorld.Template;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Miz;
using System;
using System.IO;

namespace BriefingRoom4DCSWorld.Campaign
{
    public class CampaignGenerator : IDisposable
    {
        public CampaignGenerator()
        {

        }

        public void Generate(CampaignTemplate campaignTemplate, string campaignFilePath)
        {
            string campaignName = Path.GetFileNameWithoutExtension(campaignFilePath);
            string campaignDirectory = Path.GetDirectoryName(campaignFilePath);
            DCSMissionDateTime date = new DCSMissionDateTime();

            using (MissionGenerator generator = new MissionGenerator())
            {
                for (int i = 0; i < campaignTemplate.MissionsCount; i++)
                {
                    MissionTemplate template = CreateMissionTemplate(campaignTemplate, i, campaignName, ref date);

                    DCSMission mission = generator.Generate(template);
                    MizFile miz = mission.ExportToMiz();
                    miz.SaveToFile(Path.Combine(campaignDirectory, $"{campaignName}{i + 1:00}.miz"));
                }
            }

            CreateCMPFile(campaignTemplate, campaignFilePath);
        }

        private void CreateCMPFile(CampaignTemplate campaignTemplate, string campaignFilePath)
        {
            string campaignName = Path.GetFileNameWithoutExtension(campaignFilePath);

            string lua = LuaTools.ReadIncludeLuaFile("Campaign\\Campaign.lua");
            LuaTools.ReplaceKey(ref lua, "Name", campaignName);
            LuaTools.ReplaceKey(ref lua, "Description",
                $"This is a ${campaignTemplate.CoalitionsBlue} vs ${campaignTemplate.CoalitionsRed} randomly-generated campaign created by an early version of the campaign generator of BriefingRoom, a mission generator for DCS World ({BriefingRoom.WEBSITE_URL}).");
            LuaTools.ReplaceKey(ref lua, "Units", "");

            string stagesLua = "";
            for (int i = 0; i < campaignTemplate.MissionsCount; i++)
            {
                string nextStageLua = LuaTools.ReadIncludeLuaFile("Campaign\\CampaignStage.lua");
                LuaTools.ReplaceKey(ref nextStageLua, "Index", i + 1);
                LuaTools.ReplaceKey(ref nextStageLua, "Name", $"Stage {i + 1}");
                LuaTools.ReplaceKey(ref nextStageLua, "Description", $"");
                LuaTools.ReplaceKey(ref nextStageLua, "File", $"{campaignName}{i + 1:00}.miz");

                stagesLua += nextStageLua + "\r\n";
            }
            LuaTools.ReplaceKey(ref lua, "Stages", stagesLua);

            File.WriteAllText(campaignFilePath, lua.Replace("\r\n", "\n"));
        }

        private MissionTemplate CreateMissionTemplate(CampaignTemplate campaignTemplate, int index, string campaignName, ref DCSMissionDateTime currentDate)
        {
            MissionTemplate template = new MissionTemplate();
            template.BriefingDate.Enabled = true;
            if (index > 0) currentDate = IncrementDate(currentDate);
            template.BriefingDate.Day = currentDate.Day;
            template.BriefingDate.Month = currentDate.Month;
            template.BriefingDate.Year = currentDate.Year;
            template.BriefingName = $"{campaignName}, phase {index + 1}";
            template.CoalitionBlue = campaignTemplate.CoalitionsBlue;
            template.CoalitionPlayer = campaignTemplate.PlayerCoalition;
            template.CoalitionRed = campaignTemplate.CoalitionsRed;
            template.EnvironmentTimeOfDay = TimeOfDay.Dawn; // TODO
            template.EnvironmentWeather = Weather.Clear; // TODO
            template.EnvironmentWind = Wind.Auto;
            template.ObjectiveCount = Toolbox.RandomMinMax(1, 3);
            template.ObjectiveDistance = Toolbox.RandomFrom(Amount.Average, Amount.High);
            template.ObjectiveType = Toolbox.RandomFrom(campaignTemplate.MissionsTypes);
            //template.OppositionAirDefense;
            //template.OppositionAirForce;
            //template.OppositionSkillLevelAir;
            //template.OppositionSkillLevelGround;
            //template.OppositionUnitsLocation;

            return template;
        }

        private DCSMissionDateTime IncrementDate(DCSMissionDateTime date)
        {
            date.Day += Toolbox.RandomMinMax(1, 3);
            if (date.Day > Toolbox.GetDaysPerMonth(date.Month, date.Year))
            {
                if (date.Month == Month.December)
                {
                    date.Month = Month.January;
                    date.Year++;
                }
                else
                    date.Month++;
            }

            return date;
        }

        public void Dispose()
        {

        }
    }
}
