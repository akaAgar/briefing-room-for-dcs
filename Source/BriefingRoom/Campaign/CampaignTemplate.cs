/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace BriefingRoom4DCS.Campaign
{
    /// <summary>
    /// A campaign template.
    /// </summary>
    public sealed class CampaignTemplate : IDisposable
    {
        /// <summary>
        /// Path to the default campaign template file storing default values.
        /// </summary>
        private static readonly string DEFAULT_TEMPLATE_FILEPATH = $"{BRPaths.ROOT}Default.cbrt";

        /// <summary>
        /// Minimum number of missions in a campaign.
        /// </summary>
        private const int MIN_CAMPAIGN_MISSIONS = 2;

        /// <summary>
        /// Maximum number of missions in a campaign.
        /// </summary>
        private const int MAX_CAMPAIGN_MISSIONS = 20;

        [Display(Name = "Campaign name", Description = "Name of the campaign. If left empty, a random name will be generated.")]
        [Category("Briefing")]
        public string BriefingCampaignName { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Coalition)]
        [Display(Name = "Blue coalition", Description = "Which countries belong to the blue coalition?")]
        [Category("Context")]
        public string ContextCoalitionsBlue { get; set; }

        [Required]
        [Display(Name = "Player coalition", Description = "Coalition the player belongs to.")]
        [Category("Context")]
        public Coalition ContextCoalitionPlayer { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Coalition)]
        [Display(Name = "Red coalition", Description = "Which countries belong to the red coalition?")]
        [Category("Context")]
        public string ContextCoalitionsRed { get; set; }

        [Required]
        [Display(Name = "Time period", Description = "Time period during which the campaign takes place.")]
        [Category("Context")]
        public Decade ContextDecade { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Theater)]
        [Display(Name = "Theater", Description = "Theater in which the mission takes place.")]
        [Category("Context")]
        public string ContextTheater { get; set; }

        [Required]
        [Display(Name = "Bad weather chance", Description = "Chance for a mission of this campaign to take place in bad weather.")]
        [Category("Environment")]
        public Amount EnvironmentBadWeatherChance { get; set; }

        [Required]
        [Display(Name = "Night mission chance", Description = "Chance for a mission of this campaign to take place during the night.")]
        [Category("Environment")]
        public Amount EnvironmentNightMissionChance { get; set; }

        [Required]
        [Display(Name = "Mission count", Description = "Number of missions in the campaign.")]
        [Category("Missions")]
        public int MissionsCount { get { return MissionsCount_; } set { MissionsCount_ = Toolbox.Clamp(value, MIN_CAMPAIGN_MISSIONS, MAX_CAMPAIGN_MISSIONS); } }
        private int MissionsCount_ = MIN_CAMPAIGN_MISSIONS;

        [Required, DatabaseSourceType(DatabaseEntryType.MissionFeature)]
        [Display(Name = "Mission features", Description = "Special features to include in this mission.")]
        public List<string> MissionsFeatures { get { return MissionFeatures_; } set { MissionFeatures_ = Database.Instance.CheckIDs<DBEntryFeatureMission>(value.ToArray()).ToList(); } }
        private List<string> MissionFeatures_ = new List<string>();

        [Required]
        [Display(Name = "Mission difficulty variation", Description = "How the situation evolves during the campaign. Campaign can get harder and harder with enemy power increasing as player completes missions (like in a game) or easier and easier as enemy power decreased (as it tend to happens to the losing side in a real conflict), etc.")]
        [Category("Missions")]
        public CampaignDifficultyVariation MissionsDifficultyVariation { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.ObjectivePreset)]
        [Display(Name = "Objectives", Description = "Possible mission objectives during the campaign.")]
        [Category("Missions")]
        public List<string> MissionsObjectives { get { return MissionObjectives_; } set { MissionObjectives_ = value.Distinct().ToList(); } }
        private List<string> MissionObjectives_ = new List<string>();

        [Required]
        [Display(Name = "Objectives count", Description = "How many objectives/targets will be present in the mission.")]
        [Category("Missions")]
        public Amount MissionsObjectiveCount { get; set; }

        [Required]
        [Display(Name = "Objectives distance", Description = "How far from the player's starting location will the objectives be.")]
        [Category("Missions")]
        public Amount MissionsObjectiveDistance { get; set; }

        [Required]
        [Display(Name = "Fog of war", Description = "Fog of war settings for this mission.")]
        [Category("Options")]
        public FogOfWar OptionsFogOfWar { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.OptionsMission)]
        [Display(Name = "Mission options", Description = "Miscellaneous options to customize the mission's feel.")]
        [Category("Options")]
        public List<string> OptionsMission { get { return OptionsMission_; } set { OptionsMission_ = Database.Instance.CheckIDs<DBEntryOptionsMission>(value.ToArray()).ToList(); } }
        private List<string> OptionsMission_ = new List<string>();

        [Required, DatabaseSourceType(DatabaseEntryType.DCSMod)]
        [Display(Name = "DCS World mods", Description = "DCS unit mods to use for this mission.")]
        [Category("Options")]
        public List<string> OptionsMods { get { return OptionsMods_; } set { OptionsMods_ = Database.Instance.CheckIDs<DBEntryDCSMod>(value.ToArray()).ToList(); } }
        private List<string> OptionsMods_ = new List<string>();

        [Required]
        [Display(Name = "Realism options", Description = "Realism options to enforce.")]
        [Category("Options")]
        public List<RealismOption> OptionsRealism { get { return OptionsRealism_; } set { OptionsRealism_ = value.Distinct().ToList(); } }
        private List<RealismOption> OptionsRealism_ = new List<RealismOption>();

        [Required, DatabaseSourceType(DatabaseEntryType.UnitFlyableAircraft)]
        [Display(Name = "Player aircraft", Description = "Type of aircraft the player will fly.")]
        [Category("Player")]
        public string PlayerAircraft { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.UnitCarrier)]
        [Display(Name = "Player carrier", Description = "Type of aircraft carrier the player will be spawned on.")]
        [Category("Player")]
        public string PlayerCarrier { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Airbase, true)]
        [Display(Name = "Starting airbase", Description = "Airbase from which the player(s) will take off. Leave empty for none")]
        [Category("Flight plan")]
        public string PlayerStartingAirbase { get; set; }

        [Required]
        [Display(Name = "Start location", Description = "Start location for the player flight group.")]
        [Category("Flight plan")]
        public PlayerStartLocation PlayerStartLocation { get; set; }

        [Required]
        [Display(Name = "Enemy air defense", Description = "Quality and quantity of enemy surface-to-air defense.")]
        [Category("Situation")]
        public AmountNR SituationEnemyAirDefense { get; set; }

        [Required]
        [Display(Name = "Enemy air force", Description = "Quality and quantity of enemy fighter patrols.")]
        [Category("Situation")]
        public AmountNR SituationEnemyAirForce { get; set; }

        [Required]
        [Display(Name = "Friendly air defense", Description = "Quality and quantity of enemy surface-to-air defense.")]
        [Category("Situation")]
        public AmountNR SituationFriendlyAirDefense { get; set; }

        [Required]
        [Display(Name = "Friendly air force", Description = "Quality and quantity of friendly fighter patrols.")]
        [Category("Situation")]
        public AmountNR SituationFriendlyAirForce { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CampaignTemplate()
        {
            Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">Path to the .ini file the campaign template should be read from.</param>
        public CampaignTemplate(string filePath)
        {
            LoadFromFile(filePath);
        }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            // If the default template is found, load it.
            if (File.Exists(DEFAULT_TEMPLATE_FILEPATH))
            {
                LoadFromFile(DEFAULT_TEMPLATE_FILEPATH);
                return;
            }

            BriefingCampaignName = "";

            ContextCoalitionsBlue = "USA";
            ContextCoalitionPlayer = Coalition.Blue;
            ContextCoalitionsRed = "Russia";
            ContextDecade = Decade.Decade2000;
            ContextTheater = "Caucasus";

            EnvironmentBadWeatherChance = Amount.VeryLow;
            EnvironmentNightMissionChance = Amount.VeryLow;

            MissionsCount = 5;
            MissionsDifficultyVariation = CampaignDifficultyVariation.Random;
            MissionsFeatures = new List<string>();
            MissionsObjectives = BriefingRoom.GetDatabaseEntriesIDs(DatabaseEntryType.ObjectivePreset).ToList();
            MissionsObjectiveCount = Amount.Average;
            MissionsObjectiveDistance = Amount.Average;

            OptionsFogOfWar = FogOfWar.All;
            OptionsMods = new List<string>();
            OptionsMission = new List<string>{ "ImperialUnitsForBriefing" };
            OptionsRealism = new RealismOption[] { RealismOption.DisableDCSRadioAssists, RealismOption.NoBDA }.ToList();

            PlayerAircraft = "Su-25T";
            PlayerCarrier = "";
            PlayerStartingAirbase = "";
            PlayerStartLocation = PlayerStartLocation.Runway;

            SituationEnemyAirDefense = AmountNR.Random;
            SituationEnemyAirForce = AmountNR.Random;
            SituationFriendlyAirDefense = AmountNR.Random;
            SituationFriendlyAirForce = AmountNR.Random;
        }

        /// <summary>
        /// Loads a campaign template from an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file</param>
        /// <returns></returns>
        public bool LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            using (INIFile ini = new INIFile(filePath))
            {
                BriefingCampaignName = ini.GetValue("Briefing", "CampaignName", BriefingCampaignName);

                ContextCoalitionsBlue = ini.GetValue("Context", "Coalitions.Blue", ContextCoalitionsBlue);
                ContextCoalitionPlayer = ini.GetValue("Context", "Coalitions.Player", ContextCoalitionPlayer);
                ContextCoalitionsRed = ini.GetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
                ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
                ContextTheater = ini.GetValue("Context", "Theater", ContextTheater);

                EnvironmentBadWeatherChance = ini.GetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
                EnvironmentNightMissionChance = ini.GetValue("Environment", "NightMissionChance", EnvironmentBadWeatherChance);

                MissionsCount = ini.GetValue("Missions", "Count", MissionsCount);
                MissionsDifficultyVariation = ini.GetValue("Missions", "DifficultyVariation", MissionsDifficultyVariation);
                MissionsFeatures = ini.GetValueDistinctList<string>("Missions", "Features");
                MissionsObjectives = ini.GetValueList<string>("Missions", "Objectives");
                MissionsObjectiveCount = ini.GetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
                MissionsObjectiveDistance = ini.GetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);

                OptionsFogOfWar = ini.GetValue("Options", "FogOfWar", OptionsFogOfWar);
                OptionsMods = ini.GetValueDistinctList<string>("Options", "Mods");
                OptionsMission = ini.GetValueDistinctList<string>("Options", "Mission");
                OptionsRealism = ini.GetValueDistinctList<RealismOption>("Options", "Realism");

                PlayerAircraft = ini.GetValue("Player", "Aircraft", PlayerAircraft);
                PlayerCarrier = ini.GetValue("Player", "Carrier", PlayerCarrier);
                PlayerStartingAirbase = ini.GetValue("Player", "StartingAirbase", PlayerStartingAirbase);
                PlayerStartLocation = ini.GetValue("Player", "StartLocation", PlayerStartLocation);

                SituationEnemyAirDefense = ini.GetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
                SituationEnemyAirForce = ini.GetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);
                SituationFriendlyAirDefense = ini.GetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
                SituationFriendlyAirForce = ini.GetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);
            }

            return true;
        }

        /// <summary>
        /// Save the campaign template to an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file.</param>
        public void SaveToFile(string filePath)
        {
            var ini = GetAsIni();
            ini.SaveToFile(filePath);
        }

        public byte[] GetIniBytes()
        {
            var ini = GetAsIni();
            return Encoding.ASCII.GetBytes(ini.GetFileData());
        }

        private INIFile GetAsIni()
        {
            var ini = new INIFile();
            ini.SetValue("Briefing", "CampaignName", BriefingCampaignName);

            ini.SetValue("Context", "Coalitions.Blue", ContextCoalitionsBlue);
            ini.SetValue("Context", "Coalitions.Player", ContextCoalitionPlayer);
            ini.SetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
            ini.SetValue("Context", "Decade", ContextDecade);
            ini.SetValue("Context", "Theater", ContextTheater);

            ini.SetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
            ini.SetValue("Environment", "NightMissionChance", EnvironmentBadWeatherChance);

            ini.SetValue("Missions", "Count", MissionsCount);
            ini.SetValue("Missions", "DifficultyVariation", MissionsDifficultyVariation);
            ini.SetValueArray("Missions", "Features", MissionsFeatures.ToArray());
            ini.SetValueArray("Missions", "Objectives", MissionsObjectives.ToArray());
            ini.SetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
            ini.SetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);

            ini.SetValue("Options", "FogOfWar", OptionsFogOfWar);
            ini.SetValueArray("Options", "Mods", OptionsMods.ToArray());
            ini.SetValueArray("Options", "Mission", OptionsMission.ToArray());
            ini.SetValueArray("Options", "Realism", OptionsRealism.ToArray());

            ini.SetValue("Player", "Aircraft", PlayerAircraft);
            ini.SetValue("Player", "Carrier", PlayerCarrier);
            ini.SetValue("Player", "StartingAirbase", PlayerStartingAirbase);
            ini.SetValue("Player", "StartLocation", PlayerStartLocation);

            ini.SetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
            ini.SetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);
            ini.SetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
            ini.SetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);

            return ini;
        }

        /// <summary>
        /// "Shortcut" method to get <see cref="ContextCoalitionsBlue"/> or <see cref="ContextCoalitionsRed"/> by using a <see cref="Coalition"/> parameter.
        /// </summary>
        /// <param name="coalition">Color of the coalition to return</param>
        /// <returns><see cref="ContextCoalitionsBlue"/> or <see cref="ContextCoalitionsRed"/></returns>
        public string GetCoalition(Coalition coalition)
        {
            if (coalition == Coalition.Red) return ContextCoalitionsRed;
            return ContextCoalitionsBlue;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
