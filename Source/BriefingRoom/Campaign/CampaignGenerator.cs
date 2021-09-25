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

namespace BriefingRoom4DCS.Campaign
{
    /// <summary>
    /// The main campaign generator class.
    /// </summary>
    internal class CampaignGenerator : IDisposable
    {
        private static readonly string CAMPAIGN_LUA_TEMPLATE = $"{BRPaths.INCLUDE_LUA}Campaign\\Campaign.lua";
        private static readonly string CAMPAIGN_STAGE_LUA_TEMPLATE = $"{BRPaths.INCLUDE_LUA}Campaign\\CampaignStage.lua";

        /// <summary>
        /// Mission generator to use for mission generations.
        /// </summary>
        private readonly MissionGenerator MissionGenerator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="missionGenerator">Mission generator to use for mission generations.</param>
        internal CampaignGenerator(MissionGenerator missionGenerator)
        {
            MissionGenerator = missionGenerator;
        }

        /// <summary>
        /// Generates a campaign from a campaign template.
        /// </summary>
        /// <param name="campaignTemplate">Campaign template to use.</param>
        /// <returns>A <see cref="DCSCampaign"/> or null is something went wrong</returns>
        internal DCSCampaign Generate(CampaignTemplate campaignTemplate)
        {
            DCSCampaign campaign = new();

            campaign.Name = GeneratorTools.GenerateMissionName(campaignTemplate.BriefingCampaignName);;
            string baseFileName = Toolbox.RemoveInvalidPathCharacters(campaign.Name);

            DateTime date = GenerateCampaignDate(campaignTemplate);

            for (int i = 0; i < campaignTemplate.MissionsCount; i++)
            {
                // Increment the date by a few days for each mission after the first
                if (i > 0) date = IncrementDate(date);

                MissionTemplate template = CreateMissionTemplate(campaignTemplate, campaign.Name,  i, (int)campaignTemplate.MissionsObjectiveCount);

                DCSMission mission = MissionGenerator.Generate(template, true);
                // TODO: mission.DateTime.Day = date.Day; mission.DateTime.Month = date.Month; mission.DateTime.Year = date.Year;
                if (mission == null)
                {
                    BriefingRoom.PrintToLog($"Failed to generate mission {i + 1} in the campaign.", LogMessageErrorLevel.Warning);
                    continue;
                }

                campaign.AddMission(mission);
            }

            if (campaign.MissionCount < 1) // No missions generated, something went very wrong.
            {
                BriefingRoom.PrintToLog($"Campaign has no valid mission.", LogMessageErrorLevel.Error);
                return null;
            }

            CreateImageFiles(campaignTemplate, campaign, baseFileName);

            campaign.CMPFile = GetCMPFile(campaignTemplate, campaign.Name);

            return campaign;
        }

        private DateTime GenerateCampaignDate(CampaignTemplate campaignTemplate)
        {
            int year = Toolbox.GetRandomYearFromDecade(campaignTemplate.ContextDecade);
            Month month = Toolbox.RandomFrom(Toolbox.GetEnumValues<Month>());
            int day = Toolbox.RandomMinMax(1, GeneratorTools.GetDaysPerMonth(month, year));

            DateTime date = new DateTime(year, (int)month + 1, day);
            return date;
        }

        private void CreateImageFiles(CampaignTemplate campaignTemplate, DCSCampaign campaign, string baseFileName)
        {
            string allyFlagName = campaignTemplate.GetCoalition(campaignTemplate.ContextCoalitionPlayer);
            string enemyFlagName = campaignTemplate.GetCoalition((Coalition)(1 - (int)campaignTemplate.ContextCoalitionPlayer));

            using (ImageMaker imgMaker = new())
            {
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
        }

        private string GetCMPFile(CampaignTemplate campaignTemplate, string campaignName)
        {
            string lua = File.ReadAllText(CAMPAIGN_LUA_TEMPLATE);
            GeneratorTools.ReplaceKey(ref lua, "Name", campaignName);
            GeneratorTools.ReplaceKey(ref lua, "Description",
                $"This is a {campaignTemplate.ContextCoalitionsBlue} vs {campaignTemplate.ContextCoalitionsRed} randomly-generated campaign created by an early version of the campaign generator of BriefingRoom, a mission generator for DCS World ({BriefingRoom.WEBSITE_URL}).");
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

        private MissionTemplate CreateMissionTemplate(CampaignTemplate campaignTemplate, string campaignName, int missionIndex, int missionCount)
        {
            string weatherPreset = GetWeatherForMission(campaignTemplate.EnvironmentBadWeatherChance);

            MissionTemplate template = new MissionTemplate
            {
                BriefingMissionName = $"{campaignName}, phase {missionIndex + 1}",
                BriefingMissionDescription = "",

                ContextCoalitionBlue = campaignTemplate.ContextCoalitionsBlue,
                ContextCoalitionRed = campaignTemplate.ContextCoalitionsRed,
                ContextDecade = campaignTemplate.ContextDecade,
                ContextPlayerCoalition = campaignTemplate.ContextCoalitionPlayer,
                ContextTheater = campaignTemplate.ContextTheater,

                EnvironmentSeason = Season.Random,
                EnvironmentTimeOfDay = GetTimeOfDayForMission(campaignTemplate.EnvironmentNightMissionChance),
                EnvironmentWeatherPreset = weatherPreset,
                EnvironmentWind = GetWindForMission(campaignTemplate.EnvironmentBadWeatherChance, weatherPreset),

                FlightPlanObjectiveDistance = GetObjectiveDistance(campaignTemplate.MissionsObjectiveDistance),
                FlightPlanTheaterStartingAirbase = campaignTemplate.PlayerStartingAirbase,

                MissionFeatures = new List<string>(),

                Mods = campaignTemplate.OptionsMods.ToList(),

                Objectives = new(),

                OptionsFogOfWar = campaignTemplate.OptionsFogOfWar,
                OptionsMission = campaignTemplate.OptionsMission.ToList(),
                OptionsRealism = campaignTemplate.OptionsRealism.ToList(),

                PlayerFlightGroups = new(),

                SituationEnemyAirDefense = GetPowerLevel(campaignTemplate.SituationEnemyAirDefense, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount),
                SituationEnemyAirForce = GetPowerLevel(campaignTemplate.SituationEnemyAirForce, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount),
                SituationFriendlyAirDefense = GetPowerLevel(campaignTemplate.SituationFriendlyAirDefense, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount, true),
                SituationFriendlyAirForce = GetPowerLevel(campaignTemplate.SituationFriendlyAirForce, campaignTemplate.MissionsDifficultyVariation, missionIndex, missionCount, true),
            };

            int objectiveCount = GetObjectiveCountForMission(campaignTemplate.MissionsObjectiveCount);
            for (int i = 0; i < objectiveCount; i++)
                template.Objectives.Add(new MissionTemplateObjective(Toolbox.RandomFrom(campaignTemplate.MissionsObjectives)));

            for (int i = 0; i < objectiveCount; i++)
                template.PlayerFlightGroups.Add(
                    new(campaignTemplate.PlayerAircraft, Toolbox.RandomFrom(2, 2, 2, 2, 3, 4, 4),
                    "default", campaignTemplate.PlayerCarrier,
                    campaignTemplate.ContextCoalitionPlayer == Coalition.Red ? Country.CJTFRed : Country.CJTFBlue,
                    campaignTemplate.PlayerStartLocation, true, "default"));

            return template;
        }

        /// <summary>
        /// Returns a random distance to the mission objective(s).
        /// </summary>
        /// <param name="objectiveDistance">Objective distance setting in the campaign template.</param>
        /// <returns>A distance, in nautical miles.</returns>
        private int GetObjectiveDistance(Amount objectiveDistance)
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

        private Wind GetWindForMission(Amount badWeatherChance, string weatherPreset)
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

        private AmountNR GetPowerLevel(AmountNR amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount, bool reverseVariation = false)
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

        /// <summary>
        /// Gets a random weather preset for missions of this campaign.
        /// </summary>
        /// <param name="badWeatherChance">Chance to have bad weather during this campaign's missions.</param>
        /// <returns>A weather preset ID</returns>
        private string GetWeatherForMission(Amount badWeatherChance)
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

        private TimeOfDay GetTimeOfDayForMission(Amount nightMissionChance)
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

        /// <summary>
        /// Increment the date after each mission.
        /// </summary>
        /// <param name="dateTime">Date of the current mission.</param>
        /// <returns>Date of the new mission.</returns>
        private DateTime IncrementDate(DateTime dateTime) => dateTime.AddDays(Toolbox.RandomMinMax(1, 3));

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
