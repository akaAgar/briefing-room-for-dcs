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
        /// Generates a campaign and output .miz files and campaign data to the provided path.
        /// </summary>
        /// <param name="campaignTemplate">Campaign template to use.</param>
        /// <param name="campaignName">Campaign name.</param>
        /// <param name="campaignFilePath">Path in which campaign files must be written.</param>
        internal void Generate(CampaignTemplate campaignTemplate, string campaignName, string campaignFilePath)
        {
            string campaignDirectory = Path.GetDirectoryName(campaignFilePath);

            DateTime date = GenerateCampaignDate(campaignTemplate);

            for (int i = 0; i < campaignTemplate.MissionsCount; i++)
            {
                // Increment the date by a few days for each mission after the first
                if (i > 0) date = IncrementDate(date);

                MissionTemplate template = CreateMissionTemplate(campaignTemplate, campaignName,  i);

                DCSMission mission = MissionGenerator.Generate(template);
                // TODO: mission.DateTime.Day = date.Day; mission.DateTime.Month = date.Month; mission.DateTime.Year = date.Year;

                string missionFilePath = Path.Combine(campaignDirectory, $"{campaignName}{i + 1:00}.miz");
                mission.SaveToMizFile(missionFilePath);
            }

            CreateImageFiles(campaignTemplate, campaignFilePath);
            //CreateCMPFile(campaignTemplate, campaignFilePath);
        }

        private DateTime GenerateCampaignDate(CampaignTemplate campaignTemplate)
        {
            int year = Toolbox.GetRandomYearFromDecade(campaignTemplate.ContextDecade);
            Month month = Toolbox.RandomFrom(Toolbox.GetEnumValues<Month>());
            int day = Toolbox.RandomMinMax(1, GeneratorTools.GetDaysPerMonth(month, year));

            DateTime date = new DateTime(year, (int)month + 1, day);
            return date;
        }

        private void CreateImageFiles(CampaignTemplate campaignTemplate, string campaignFilePath)
        {
            string baseFileName = Path.Combine(Path.GetDirectoryName(campaignFilePath), Path.GetFileNameWithoutExtension(campaignFilePath));
            string allyFlagName = campaignTemplate.GetCoalition(campaignTemplate.ContextCoalitionPlayer);
            string enemyFlagName = campaignTemplate.GetCoalition((Coalition)(1 - (int)campaignTemplate.ContextCoalitionPlayer));

            using (ImageMaker imgMaker = new ImageMaker())
            {
                string theaterImage;
                string[] theaterImages = Directory.GetFiles($"{BRPaths.INCLUDE_JPG}Theaters\\", $"{campaignTemplate.ContextTheater}*.jpg");
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
                        new ImageMakerLayer($"Flags\\{enemyFlagName}.png", ContentAlignment.MiddleCenter, -32, -32),
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

        //private void CreateCMPFile(CampaignTemplate campaignTemplate, string campaignFilePath)
        //{
        //    string campaignName = Path.GetFileNameWithoutExtension(campaignFilePath);

        //    string lua = LuaTools.ReadIncludeLuaFile("Campaign\\Campaign.lua");
        //    GeneratorTools.ReplaceKey(ref lua, "Name", campaignName);
        //    GeneratorTools.ReplaceKey(ref lua, "Description",
        //        $"This is a {campaignTemplate.ContextCoalitionsBlue} vs {campaignTemplate.ContextCoalitionsRed} randomly-generated campaign created by an early version of the campaign generator of BriefingRoom, a mission generator for DCS World ({BriefingRoom.WEBSITE_URL}).");
        //    GeneratorTools.ReplaceKey(ref lua, "Units", "");

        //    string stagesLua = "";
        //    for (int i = 0; i < campaignTemplate.MissionsCount; i++)
        //    {
        //        string nextStageLua = LuaTools.ReadIncludeLuaFile("Campaign\\CampaignStage.lua");
        //        GeneratorTools.ReplaceKey(ref nextStageLua, "Index", i + 1);
        //        GeneratorTools.ReplaceKey(ref nextStageLua, "Name", $"Stage {i + 1}");
        //        GeneratorTools.ReplaceKey(ref nextStageLua, "Description", $"");
        //        GeneratorTools.ReplaceKey(ref nextStageLua, "File", $"{campaignName}{i + 1:00}.miz");

        //        stagesLua += nextStageLua + "\r\n";
        //    }
        //    GeneratorTools.ReplaceKey(ref lua, "Stages", stagesLua);

        //    File.WriteAllText(campaignFilePath, lua.Replace("\r\n", "\n"));
        //}

        private MissionTemplate CreateMissionTemplate(CampaignTemplate campaignTemplate, string campaignName, int missionIndex)
        {
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
                EnvironmentWeatherPreset = "",
                EnvironmentWind = Wind.Random,

                FlightPlanObjectiveDistance = 80,
                FlightPlanTheaterStartingAirbase = "",

                MissionFeatures = new List<string>(),

                Mods = campaignTemplate.OptionsMods.ToList(),

                Objectives = new MissionTemplateObjective[] { new MissionTemplateObjective() }.ToList(),

                OptionsFogOfWar = campaignTemplate.OptionsFogOfWar,
                OptionsMission = campaignTemplate.OptionsMission.ToList(),
                OptionsRealism = campaignTemplate.OptionsRealism.ToList(),

                PlayerFlightGroups = new MissionTemplateFlightGroup[] { new MissionTemplateFlightGroup() }.ToList(),

                SituationEnemyAirDefense = campaignTemplate.SituationEnemyAirDefense,
                SituationEnemyAirForce = campaignTemplate.SituationEnemyAirForce,
                SituationFriendlyAirDefense = campaignTemplate.SituationFriendlyAirDefense,
                SituationFriendlyAirForce = campaignTemplate.SituationFriendlyAirForce,
            };

            int objectiveCount = GetObjectiveCountForMission(campaignTemplate.MissionsObjectiveCount);
            for (int i = 0; i < objectiveCount; i++)
                template.Objectives.Add(MissionTemplateObjective.FromObjectivePreset(Toolbox.RandomFrom(campaignTemplate.MissionsObjectives)));

            /*
            template.PlayerFlightGroups = new MissionTemplateFlightGroup[]
                { new MissionTemplateFlightGroup(
                    campaignTemplate.PlayerAircraft, Toolbox.RandomFrom(2, 2, 2, 2, 3, 4, 4),
                    MissionTemplateFlightGroupTask.Objectives, campaignTemplate.PlayerCarrier, Country.CJTFBlue, PlayerStartLocation.Runway) }; //TODO Make country selectable

            template.ContextTheater = campaignTemplate.ContextTheaterID;
            template.OptionsTheaterCountriesCoalitions = campaignTemplate.OptionsTheaterCountriesCoalitions;
            template.FlightPlanTheaterStartingAirbase = campaignTemplate.PlayerStartingAirbase;
            */

            return template;
        }

        //private DCSSkillLevel GetSkillLevel(AmountNR amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount)
        //{
        //    if (amount == AmountNR.Random) return DCSSkillLevel.Random;
        //    double campaignProgress = missionIndex / (double)(Math.Max(2, missionsCount) - 1.0);

        //    DCSSkillLevel baseSkillLevel = DCSSkillLevel.Regular;
        //    switch (amount)
        //    {
        //        case AmountNR.VeryLow: baseSkillLevel = DCSSkillLevel.Rookie; break;
        //        case AmountNR.Low: baseSkillLevel = Toolbox.RandomFrom(DCSSkillLevel.Rookie, DCSSkillLevel.Regular); break;
        //        case AmountNR.Average: baseSkillLevel = Toolbox.RandomFrom(DCSSkillLevel.Rookie, DCSSkillLevel.Regular, DCSSkillLevel.Veteran); break;
        //        case AmountNR.High: baseSkillLevel = Toolbox.RandomFrom(DCSSkillLevel.Veteran, DCSSkillLevel.Ace); break;
        //        case AmountNR.VeryHigh: baseSkillLevel = DCSSkillLevel.Ace; break;
        //    }

        //    if (variation == CampaignDifficultyVariation.Steady) return baseSkillLevel;

        //    double amountOffset = 0;
        //    switch (variation)
        //    {
        //        case CampaignDifficultyVariation.ConsiderablyEasier: amountOffset = -2.25; break;
        //        case CampaignDifficultyVariation.MuchEasier: amountOffset = -1.5; break;
        //        case CampaignDifficultyVariation.SomewhatEasier: amountOffset = -1.0; break;
        //        case CampaignDifficultyVariation.SomewhatHarder: amountOffset = 1.0; break;
        //        case CampaignDifficultyVariation.MuchHarder: amountOffset = 1.5; break;
        //        case CampaignDifficultyVariation.ConsiderablyHarder: amountOffset = 2.25; break;
        //    }
        //    double skillDouble = (double)baseSkillLevel + amountOffset * campaignProgress;

        //    return (DCSSkillLevel)Toolbox.Clamp((int)skillDouble, (int)DCSSkillLevel.Regular, (int)DCSSkillLevel.Ace);
        //}

        private AmountNR GetPowerLevel(AmountNR amount, CampaignDifficultyVariation variation, int missionIndex, int missionsCount)
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

            return (AmountNR)Toolbox.Clamp((int)amountDouble, (int)AmountNR.VeryLow, (int)AmountNR.VeryHigh);
        }

        //private Weather GetWeatherForMission(AmountNR badWeatherChance)
        //{
        //    int chance;
        //    switch (badWeatherChance.Get())
        //    {
        //        case AmountNR.VeryLow: chance = 0; break;
        //        case AmountNR.Low: chance = 10; break;
        //        default: chance = 25; break; // case AmountN.Average
        //        case AmountNR.High: chance = 40; break;
        //        case AmountNR.VeryHigh: chance = 60; break;
        //    }

        //    if (Toolbox.RandomInt(100) < chance)
        //        return Toolbox.RandomFrom(Weather.Precipitation, Weather.Precipitation, Weather.Precipitation, Weather.Storm);
        //    else
        //        return Toolbox.RandomFrom(
        //            Weather.Clear, Weather.Clear, Weather.Clear, Weather.Clear,
        //            Weather.LightClouds, Weather.LightClouds, Weather.LightClouds,
        //            Weather.SomeClouds, Weather.Overcast);
        //}

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
        private DateTime IncrementDate(DateTime dateTime)
        {
            int day = dateTime.Day + Toolbox.RandomMinMax(1, 3);
            int month = dateTime.Month;
            int year = dateTime.Year;

            if (day > GeneratorTools.GetDaysPerMonth((Month)(month - 1), year))
            {
                if (month == 12)
                {
                    month = 1;
                    year++;
                }
                else
                    month++;
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
