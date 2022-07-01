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
using BriefingRoom4DCS.Media;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BriefingRoom4DCS.Campaign
{
    internal class CampaignGenerator
    {
        private static readonly string CAMPAIGN_LUA_TEMPLATE = $"{BRPaths.INCLUDE_LUA}Campaign\\Campaign.lua";
        private static readonly string CAMPAIGN_STAGE_LUA_TEMPLATE = $"{BRPaths.INCLUDE_LUA}Campaign\\CampaignStage.lua";

        internal static async Task<DCSCampaign> GenerateAsync(CampaignTemplate campaignTemplate)
        {
            DCSCampaign campaign = new();

            campaign.Name = GeneratorTools.GenerateMissionName(campaignTemplate.BriefingCampaignName, campaignTemplate.Language); ;
            string baseFileName = Toolbox.RemoveInvalidPathCharacters(campaign.Name);

            DateTime date = GenerateCampaignDate(campaignTemplate);

            for (int i = 0; i < campaignTemplate.MissionsCount; i++)
            {
                if (i > 0) date = IncrementDate(date);

                var template = CreateMissionTemplate(campaignTemplate, campaign.Name, i, (int)campaignTemplate.MissionsObjectiveCount);

                var mission = await MissionGenerator.GenerateRetryableAsync(template, true);
            
                if (mission == null)
                {
                    BriefingRoom.PrintToLog($"Failed to generate mission {i + 1} in the campaign.", LogMessageErrorLevel.Warning);
                    continue;
                }

                mission.SetValue("DateDay", date.Day);
                mission.SetValue("DateMonth", date.Month);
                mission.SetValue("DateYear", date.Year);
                mission.SetValue("BriefingDate", $"{date.Day:00}/{date.Month:00}/{date.Year:0000}");

                campaign.AddMission(mission);
            }

            if (campaign.MissionCount < 1) // No missions generated, something went very wrong.
                throw new BriefingRoomException($"Campaign has no valid mission.");


            CreateImageFiles(campaignTemplate, campaign, baseFileName);

            campaign.CMPFile = GetCMPFile(campaignTemplate, campaign.Name);

            return campaign;
        }

        private static DateTime GenerateCampaignDate(CampaignTemplate campaignTemplate)
        {
            int year = Toolbox.GetRandomYearFromDecade(campaignTemplate.ContextDecade);
            Month month = Toolbox.RandomFrom(Toolbox.GetEnumValues<Month>());
            int day = Toolbox.RandomMinMax(1, GeneratorTools.GetDaysPerMonth(month, year));

            DateTime date = new DateTime(year, (int)month + 1, day);
            return date;
        }

        private static void CreateImageFiles(CampaignTemplate campaignTemplate, DCSCampaign campaign, string baseFileName)
        {
            string allyFlagName = campaignTemplate.GetCoalition(campaignTemplate.ContextPlayerCoalition);
            string enemyFlagName = campaignTemplate.GetCoalition((Coalition)(1 - (int)campaignTemplate.ContextPlayerCoalition));

            ImageMaker imgMaker = new();
            string theaterImage;
            string[] theaterImages = Directory.GetFiles($"{BRPaths.INCLUDE_JPG}Theaters\\", $"{campaignTemplate.ContextTheater}*.jpg");
            if (theaterImages.Length == 0)
                theaterImage = "_default.jpg";
            else
                theaterImage = "Theaters\\" + Path.GetFileName(Toolbox.RandomFrom(theaterImages));

            // Print the name of the campaign over the campaign "title picture"
            imgMaker.TextOverlay.Text = campaign.Name;
            imgMaker.TextOverlay.Alignment = ContentAlignment.TopCenter;
            campaign.AddMediaFile($"{baseFileName}_Title.jpg",
                imgMaker.GetImageBytes(
                    new ImageMakerLayer(theaterImage),
                    new ImageMakerLayer($"Flags\\{enemyFlagName}.png", ContentAlignment.MiddleCenter, -32, -32),
                    new ImageMakerLayer($"Flags\\{allyFlagName}.png", ContentAlignment.MiddleCenter, 32, 32)));

            // Reset background and text overlay
            imgMaker.BackgroundColor = Color.Black;
            imgMaker.TextOverlay.Text = "";

            campaign.AddMediaFile($"{baseFileName}_Success.jpg", imgMaker.GetImageBytes("Sky.jpg", $"Flags\\{allyFlagName}.png"));
            campaign.AddMediaFile($"{baseFileName}_Failure.jpg", imgMaker.GetImageBytes("Fire.jpg", $"Flags\\{allyFlagName}.png", "Burning.png"));
        }

        private static string GetCMPFile(CampaignTemplate campaignTemplate, string campaignName)
        {
            string lua = File.ReadAllText(CAMPAIGN_LUA_TEMPLATE);
            GeneratorTools.ReplaceKey(ref lua, "Name", campaignName);
            GeneratorTools.ReplaceKey(ref lua, "Description",
                $"This is a {campaignTemplate.ContextCoalitionBlue} vs {campaignTemplate.ContextCoalitionRed} randomly-generated campaign created by an early version of the campaign generator of BriefingRoom, a mission generator for DCS World ({BriefingRoom.WEBSITE_URL}).");
            GeneratorTools.ReplaceKey(ref lua, "Units", "");

            string stagesLua = "";
            for (int i = 0; i < campaignTemplate.MissionsCount; i++)
            {
                string nextStageLua = File.ReadAllText(CAMPAIGN_STAGE_LUA_TEMPLATE);
                GeneratorTools.ReplaceKey(ref nextStageLua, "Index", i + 1);
                GeneratorTools.ReplaceKey(ref nextStageLua, "Name", $"Stage {i + 1}");
                GeneratorTools.ReplaceKey(ref nextStageLua, "Description", $"");
                GeneratorTools.ReplaceKey(ref nextStageLua, "File", $"{campaignName}{i + 1:00}.miz");

                stagesLua += nextStageLua + "\r\n";
            }
            GeneratorTools.ReplaceKey(ref lua, "Stages", stagesLua);

            return lua.Replace("\r\n", "\n");
        }

        private static MissionTemplate CreateMissionTemplate(CampaignTemplate campaignTemplate, string campaignName, int missionIndex, int missionCount)
        {
            string weatherPreset = GetWeatherForMission(campaignTemplate.EnvironmentBadWeatherChance);

            MissionTemplate template = new MissionTemplate
            {
                BriefingMissionName = $"{campaignName}, phase {missionIndex + 1}",
                BriefingMissionDescription = "",

                ContextCoalitionBlue = campaignTemplate.ContextCoalitionBlue,
                ContextCoalitionRed = campaignTemplate.ContextCoalitionRed,
                ContextDecade = campaignTemplate.ContextDecade,
                ContextPlayerCoalition = campaignTemplate.ContextPlayerCoalition,
                ContextTheater = campaignTemplate.ContextTheater,
                ContextSituation = campaignTemplate.ContextSituation,

                EnvironmentSeason = Season.Random,
                EnvironmentTimeOfDay = GetTimeOfDayForMission(campaignTemplate.EnvironmentNightMissionChance),
                EnvironmentWeatherPreset = weatherPreset,
                EnvironmentWind = GetWindForMission(campaignTemplate.EnvironmentBadWeatherChance, weatherPreset),

                FlightPlanObjectiveDistance = GetObjectiveDistance(campaignTemplate.MissionsObjectiveDistance),
                FlightPlanTheaterStartingAirbase = campaignTemplate.PlayerStartingAirbase,

                MissionFeatures = campaignTemplate.MissionsFeatures.ToList(),

                Mods = campaignTemplate.OptionsMods.ToList(),

                Objectives = new(),

                OptionsFogOfWar = campaignTemplate.OptionsFogOfWar,
                OptionsMission = campaignTemplate.OptionsMission.ToList(),
                OptionsRealism = campaignTemplate.OptionsRealism.ToList(),

                PlayerFlightGroups = campaignTemplate.PlayerFlightGroups,

                SituationEnemySkill = GetPowerLevel(campaignTemplate.SituationEnemySkill, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount),
                SituationEnemyAirDefense = GetPowerLevel(campaignTemplate.SituationEnemyAirDefense, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount),
                SituationEnemyAirForce = GetPowerLevel(campaignTemplate.SituationEnemyAirForce, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount),

                SituationFriendlySkill = GetPowerLevel(campaignTemplate.SituationFriendlySkill, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount, true),
                SituationFriendlyAirDefense = GetPowerLevel(campaignTemplate.SituationFriendlyAirDefense, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount, true),
                SituationFriendlyAirForce = GetPowerLevel(campaignTemplate.SituationFriendlyAirForce, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount, true),

                CombinedArmsCommanderBlue = campaignTemplate.CombinedArmsCommanderBlue,
                CombinedArmsCommanderRed = campaignTemplate.CombinedArmsCommanderRed,
                CombinedArmsJTACBlue = campaignTemplate.CombinedArmsJTACBlue,
                CombinedArmsJTACRed = campaignTemplate.CombinedArmsJTACRed,
            };

            int objectiveCount = GetObjectiveCountForMission(campaignTemplate.MissionsObjectiveCount);
            for (int i = 0; i < objectiveCount; i++)
                template.Objectives.Add(new MissionTemplateObjective(Toolbox.RandomFrom(campaignTemplate.MissionsObjectives)));

            return template;
        }

        private static int GetObjectiveDistance(Amount objectiveDistance)
        {
            switch (objectiveDistance)
            {
                case Amount.VeryLow: return Toolbox.RandomMinMax(40, 80);
                case Amount.Low: return Toolbox.RandomMinMax(60, 100);
                default: return Toolbox.RandomMinMax(80, 120); // case Amount.Average
                case Amount.High: return Toolbox.RandomMinMax(100, 140);
                case Amount.VeryHigh: return Toolbox.RandomMinMax(120, 160);
            }
        }

        private static Wind GetWindForMission(Amount badWeatherChance, string weatherPreset)
        {
            // Pick a max wind force
            Wind maxWind;
            switch (badWeatherChance)
            {
                case Amount.VeryLow: maxWind = Wind.Calm; break;
                case Amount.Low: maxWind = Wind.LightBreeze; break;
                default: maxWind = Wind.ModerateBreeze; break; // case Amount.Average
                case Amount.High: maxWind = Wind.ModerateBreeze; break;
                case Amount.VeryHigh: maxWind = Wind.StrongBreeze; break;
            }

            // Select a random wind force
            Wind wind = (Wind)Toolbox.RandomMinMax((int)Wind.Calm, (int)maxWind);

            // Makes the wind stronger if the weather preset is classified as "bad weather"
            if (Database.Instance.GetEntry<DBEntryWeatherPreset>(weatherPreset).BadWeather)
                wind += Toolbox.RandomMinMax(0, 2);

            return (Wind)Toolbox.Clamp((int)wind, (int)Wind.Calm, (int)Wind.Storm);
        }

        private static AmountNR GetPowerLevel(AmountNR amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount, bool reverseVariation = false)
        {
            if (amount == AmountNR.Random) return AmountNR.Random;
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
            if (reverseVariation) amountDouble = -amountDouble;

            return (AmountNR)Toolbox.Clamp((int)amountDouble, (int)AmountNR.VeryLow, (int)AmountNR.VeryHigh);
        }

        private static string GetWeatherForMission(Amount badWeatherChance)
        {
            // Chance to have bad weather
            int chance;
            switch (badWeatherChance)
            {
                case Amount.VeryLow: chance = 0; break;
                case Amount.Low: chance = 10; break;
                default: chance = 25; break; // case Amount.Average
                case Amount.High: chance = 40; break;
                case Amount.VeryHigh: chance = 60; break;
            }

            // Pick a random weather preset matching the good/bad weather chance
            string weather =
                (from DBEntryWeatherPreset weatherDB
                 in Database.Instance.GetAllEntries<DBEntryWeatherPreset>()
                 where weatherDB.BadWeather == (Toolbox.RandomInt(100) < chance)
                 select weatherDB.ID).OrderBy(x => Toolbox.RandomInt()).FirstOrDefault();

            // Just to make sure weather ID is not null
            if (weather == null)
                return Toolbox.RandomFrom(Database.Instance.GetAllEntriesIDs<DBEntryWeatherPreset>());

            return weather;
        }

        private static TimeOfDay GetTimeOfDayForMission(Amount nightMissionChance)
        {
            int chance;
            switch (nightMissionChance)
            {
                case Amount.VeryLow: chance = 0; break;
                case Amount.Low: chance = 10; break;
                default: chance = 25; break; // case Amount.Average
                case Amount.High: chance = 50; break;
                case Amount.VeryHigh: chance = 80; break;
            }

            if (Toolbox.RandomInt(100) < chance)
                return TimeOfDay.Night;
            else
                return TimeOfDay.RandomDaytime;
        }

        private static int GetObjectiveCountForMission(Amount amount)
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

        private static DateTime IncrementDate(DateTime dateTime) => dateTime.AddDays(Toolbox.RandomMinMax(1, 3));
    }
}
