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
using BriefingRoom4DCSWorld.Media;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Miz;
using System;
using System.Drawing;
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

            DCSMissionDateTime date = GenerateCampaignDate(campaignTemplate);

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

            CreateImageFiles(campaignTemplate, campaignFilePath);
            CreateCMPFile(campaignTemplate, campaignFilePath);
        }

        private DCSMissionDateTime GenerateCampaignDate(CampaignTemplate campaignTemplate)
        {
            DCSMissionDateTime date = new DCSMissionDateTime
            {
                Year = Toolbox.GetRandomYearFromDecade(campaignTemplate.ContextDecade),
                Month = Toolbox.RandomFrom(Toolbox.GetEnumValues<Month>())
            };
            date.Day = Toolbox.RandomMinMax(1, Toolbox.GetDaysPerMonth(date.Month, date.Year));
            return date;
        }

        private void CreateImageFiles(CampaignTemplate campaignTemplate, string campaignFilePath)
        {
            string baseFileName = Path.Combine(Path.GetDirectoryName(campaignFilePath), Path.GetFileNameWithoutExtension(campaignFilePath));
            string allyFlagName = GeneratorTools.RemoveAfterComma(campaignTemplate.GetCoalition(campaignTemplate.PlayerCoalition));
            string enemyFlagName = GeneratorTools.RemoveAfterComma(campaignTemplate.GetCoalition((Coalition)(1 - (int)campaignTemplate.PlayerCoalition)));

            using (ImageMaker imgMaker = new ImageMaker())
            {
                string theaterImage;
                string[] theaterImages = Directory.GetFiles($"{BRPaths.INCLUDE_JPG}Theaters\\", $"{campaignTemplate.ContextTheaterID}*.jpg");
                if (theaterImages.Length == 0)
                    theaterImage = "_default.jpg";
                else
                    theaterImage = "Theaters\\" + Path.GetFileName(Toolbox.RandomFrom(theaterImages));

                // Print the name of the campaign over the campaign "title picture"
                imgMaker.TextOverlay.Text = Path.GetFileNameWithoutExtension(campaignFilePath);
                imgMaker.TextOverlay.Alignment = ContentAlignment.TopCenter;
                File.WriteAllBytes($"{baseFileName}_Title.jpg",
                    imgMaker.GetImageBytes(
                        new ImageMakerLayer(theaterImage),
                        new ImageMakerLayer($"Flags\\{enemyFlagName}.png", ContentAlignment.MiddleCenter, -32, - 32),
                        new ImageMakerLayer($"Flags\\{allyFlagName}.png", ContentAlignment.MiddleCenter, 32, 32)));

                // Reset background and text overlay
                imgMaker.BackgroundColor = Color.Black;
                imgMaker.TextOverlay.Text = "";

                File.WriteAllBytes($"{baseFileName}_Success.jpg",
                    imgMaker.GetImageBytes("Sky.jpg", $"Flags\\{allyFlagName}.png"));

                File.WriteAllBytes($"{baseFileName}_Failure.jpg",
                    imgMaker.GetImageBytes("Fire.jpg", $"Flags\\{allyFlagName}.png", "Burning.png"));
            }
        }

        private void CreateCMPFile(CampaignTemplate campaignTemplate, string campaignFilePath)
        {
            string campaignName = Path.GetFileNameWithoutExtension(campaignFilePath);

            string lua = LuaTools.ReadIncludeLuaFile("Campaign\\Campaign.lua");
            LuaTools.ReplaceKey(ref lua, "Name", campaignName);
            LuaTools.ReplaceKey(ref lua, "Description",
                $"This is a {campaignTemplate.ContextCoalitionsBlue} vs {campaignTemplate.ContextCoalitionsRed} randomly-generated campaign created by an early version of the campaign generator of BriefingRoom, a mission generator for DCS World ({BriefingRoom.WEBSITE_URL}).");
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
            // Increment the date by a few days for each mission after the first
            if (index > 0) currentDate = IncrementDate(currentDate);

            MissionTemplate template = new MissionTemplate();

            template.BriefingDate.Enabled = true;
            template.BriefingDate.Day = currentDate.Day;
            template.BriefingDate.Month = currentDate.Month;
            template.BriefingDate.Year = currentDate.Year;
            template.BriefingDescription = "";
            template.BriefingName = $"{campaignName}, phase {index + 1}";

            template.CoalitionBlue = campaignTemplate.ContextCoalitionsBlue;
            template.CoalitionPlayer = campaignTemplate.PlayerCoalition;
            template.CoalitionRed = campaignTemplate.ContextCoalitionsRed;

            template.EnvironmentTimeOfDay = GetTimeOfDayForMission(campaignTemplate.EnvironmentNightMissionChance);
            template.EnvironmentWeather = GetWeatherForMission(campaignTemplate.EnvironmentBadWeatherChance);
            template.EnvironmentWind = Wind.Auto;

            template.ObjectiveCount = GetObjectiveCountForMission(campaignTemplate.MissionsObjectiveCount);
            template.ObjectiveDistance = GetRandomAmountForMission(campaignTemplate.MissionsObjectiveDistance);
            template.ObjectiveType = Toolbox.RandomFrom(campaignTemplate.MissionsTypes);

            template.OppositionAirDefense = GetPowerLevel(campaignTemplate.SituationEnemyAirDefense, campaignTemplate.SituationVariation, index, campaignTemplate.MissionsCount);
            template.OppositionAirForce = GetPowerLevel(campaignTemplate.SituationEnemyAirForce, campaignTemplate.SituationVariation, index, campaignTemplate.MissionsCount);
            template.OppositionSkillLevelAir = GetSkillLevel(campaignTemplate.SituationEnemyAirForce, campaignTemplate.SituationVariation, index, campaignTemplate.MissionsCount);
            template.OppositionSkillLevelGround = GetSkillLevel(campaignTemplate.SituationEnemyAirDefense, campaignTemplate.SituationVariation, index, campaignTemplate.MissionsCount);
            template.OppositionUnitsLocation = SpawnPointPreferredCoalition.Any;

            template.OptionsPreferences = new MissionTemplatePreferences[0];

            template.OptionsRadioSounds = true;
            template.OptionsScriptExtensions = new string[0];
            template.OptionsShowEnemyUnits = campaignTemplate.OptionsShowEnemyUnits;

            template.PlayerAISkillLevel = GetSkillLevel(campaignTemplate.SituationFriendlyAirForce, CampaignDifficultyVariation.Steady, 0, 0);
            template.PlayerEscortCAP = Toolbox.RandomFrom(2, 2, 2, 2, 3, 4, 4);
            template.PlayerEscortSEAD = Toolbox.RandomFrom(2, 2, 2, 2, 3, 4, 4);
            template.PlayerMPFlightGroups = new MissionTemplateMPFlightGroup[0];
            template.PlayerSPAircraft = campaignTemplate.PlayerAircraft;
            template.PlayerSPWingmen = Toolbox.RandomFrom(1, 1, 1, 1, 2, 3, 3);
            template.PlayerStartLocation = campaignTemplate.PlayerStartLocation;

            template.TheaterID = campaignTemplate.ContextTheaterID;
            template.TheaterRegionsCoalitions = campaignTemplate.ContextTheaterRegionsCoalitions;
            template.TheaterStartingAirbase = "";

            return template;
        }

        private BRSkillLevel GetSkillLevel(AmountN amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount)
        {
            if (amount == AmountN.Random) return BRSkillLevel.Random;
            double campaignProgress = missionIndex / (double)(Math.Max(2, missionsCount) - 1.0);

            BRSkillLevel baseSkillLevel = BRSkillLevel.Regular;
            switch (amount)
            {
                case AmountN.VeryLow: baseSkillLevel = BRSkillLevel.Rookie; break;
                case AmountN.Low: baseSkillLevel = Toolbox.RandomFrom(BRSkillLevel.Rookie, BRSkillLevel.Regular); break;
                case AmountN.Average: baseSkillLevel = Toolbox.RandomFrom(BRSkillLevel.Rookie, BRSkillLevel.Regular, BRSkillLevel.Veteran); break;
                case AmountN.High: baseSkillLevel = Toolbox.RandomFrom(BRSkillLevel.Veteran, BRSkillLevel.Ace); break;
                case AmountN.VeryHigh: baseSkillLevel = BRSkillLevel.Ace; break;
            }

            if (variation == CampaignDifficultyVariation.Steady) return baseSkillLevel;

            double amountOffset = 0;
            switch (variation)
            {
                case CampaignDifficultyVariation.ConsiderablyEasier: amountOffset = -2.25; break;
                case CampaignDifficultyVariation.MuchEasier: amountOffset = -1.5; break;
                case CampaignDifficultyVariation.SomewhatEasier: amountOffset = -1.0; break;
                case CampaignDifficultyVariation.SomewhatHarder: amountOffset = 1.0; break;
                case CampaignDifficultyVariation.MuchHarder: amountOffset = 1.5; break;
                case CampaignDifficultyVariation.ConsiderablyHarder: amountOffset = 2.25; break;
            }
            double skillDouble = (double)baseSkillLevel + amountOffset * campaignProgress;

            return (BRSkillLevel)Toolbox.Clamp((int)skillDouble, (int)BRSkillLevel.Regular, (int)BRSkillLevel.Ace);
        }


        private AmountN GetPowerLevel(AmountN amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount)
        {
            if (amount == AmountN.Random) return AmountN.Random;
            if (variation == CampaignDifficultyVariation.Steady) return amount;

            double campaignProgress = missionIndex / (double)(Math.Max(2, missionsCount) - 1.0);

            double amountOffset = 0;
            switch (variation)
            {
                case CampaignDifficultyVariation.ConsiderablyEasier: amountOffset = -3.5; break;
                case CampaignDifficultyVariation.MuchEasier: amountOffset = -2.25; break;
                case CampaignDifficultyVariation.SomewhatEasier: amountOffset = -1.5; break;
                case CampaignDifficultyVariation.SomewhatHarder: amountOffset = 1.5; break;
                case CampaignDifficultyVariation.MuchHarder: amountOffset = 2.25; break;
                case CampaignDifficultyVariation.ConsiderablyHarder: amountOffset = 3.5; break;
            }
            double amountDouble = (double)amount + amountOffset * campaignProgress;

            return (AmountN)Toolbox.Clamp((int)amountDouble, (int)AmountN.VeryLow, (int)AmountN.VeryHigh);
        }

        private Weather GetWeatherForMission(AmountN badWeatherChance)
        {
            int chance;
            switch (badWeatherChance.Get())
            {
                case AmountN.VeryLow: chance = 0; break;
                case AmountN.Low: chance = 10; break;
                default: chance = 25; break; // case AmountN.Average
                case AmountN.High: chance = 40; break;
                case AmountN.VeryHigh: chance = 60; break;
            }

            if (Toolbox.RandomInt(100) < chance)
                return Toolbox.RandomFrom(Weather.Precipitation, Weather.Precipitation, Weather.Precipitation, Weather.Storm);
            else
                return Toolbox.RandomFrom(
                    Weather.Clear, Weather.Clear, Weather.Clear, Weather.Clear,
                    Weather.LightClouds, Weather.LightClouds, Weather.LightClouds,
                    Weather.SomeClouds, Weather.Overcast);
        }

        private TimeOfDay GetTimeOfDayForMission(AmountN nightMissionChance)
        {
            int chance;
            switch (nightMissionChance.Get())
            {
                case AmountN.VeryLow: chance = 0; break;
                case AmountN.Low: chance = 10; break;
                default: chance = 25; break; // case AmountN.Average
                case AmountN.High: chance = 50; break;
                case AmountN.VeryHigh: chance = 80; break;
            }

            if (Toolbox.RandomInt(100) < chance)
                return TimeOfDay.Night;
            else
                return TimeOfDay.RandomDaytime;
        }

        private int GetObjectiveCountForMission(Amount amount)
        {
            switch (amount)
            {
                case Amount.VeryLow:
                    return 1;
                case Amount.Low:
                    return Toolbox.RandomFrom(1, 1, 2);
                default: // case Amount.Average:
                    return Toolbox.RandomFrom(1, 2, 2, 2, 3);
                case Amount.High:
                    return Toolbox.RandomFrom(2, 3, 3, 4);
                case Amount.VeryHigh:
                    return Toolbox.RandomFrom(3, 4, 4, 4, 5);
            }
        }

        private Amount GetRandomAmountForMission(Amount amount)
        {
            switch (amount)
            {
                case Amount.VeryLow:
                    return Toolbox.RandomFrom(Amount.VeryLow, Amount.VeryLow, Amount.Low);
                case Amount.Low:
                    return Toolbox.RandomFrom(Amount.VeryLow, Amount.Low, Amount.Average);
                default: // case Amount.Average:
                    return Toolbox.RandomFrom(Amount.Low, Amount.Average, Amount.High);
                case Amount.High:
                    return Toolbox.RandomFrom(Amount.Average, Amount.High, Amount.VeryHigh);
                case Amount.VeryHigh:
                    return Toolbox.RandomFrom(Amount.High, Amount.VeryHigh, Amount.VeryHigh);
            }
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
