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
using System.IO;
using System.Linq;
using System.Text;

namespace BriefingRoom4DCS.Campaign
{
    public sealed class CampaignTemplate
    {
        private static readonly string DEFAULT_TEMPLATE_FILEPATH = $"{BRPaths.ROOT}Default.cbrt";
        private const int MIN_CAMPAIGN_MISSIONS = 2;
        private const int MAX_CAMPAIGN_MISSIONS = 20;
        public const int MAX_COMBINED_ARMS_SLOTS = 100;

        public string Language {get; set;} = "EN";
        public string BriefingCampaignName { get; set; }
        public string ContextCoalitionBlue { get; set; }
        public Coalition ContextPlayerCoalition { get; set; }
        public string ContextCoalitionRed { get; set; }
        public Decade ContextDecade { get; set; }
        public string ContextTheater { get; set; }
        public string ContextSituation { get; set; }
        public Amount EnvironmentBadWeatherChance { get; set; }
        public Amount EnvironmentNightMissionChance { get; set; }
        public int MissionsCount { get { return MissionsCount_; } set { MissionsCount_ = Toolbox.Clamp(value, MIN_CAMPAIGN_MISSIONS, MAX_CAMPAIGN_MISSIONS); } }
        private int MissionsCount_ = MIN_CAMPAIGN_MISSIONS;
        public List<string> MissionsFeatures { get { return MissionFeatures_; } set { MissionFeatures_ = Database.Instance.CheckIDs<DBEntryFeatureMission>(value.ToArray()).ToList(); } }
        private List<string> MissionFeatures_ = new List<string>();
        public CampaignDifficultyVariation MissionsDifficultyVariation { get; set; }
        public List<string> MissionsObjectives { get { return MissionObjectives_; } set { MissionObjectives_ = value.Distinct().ToList(); } }
        private List<string> MissionObjectives_ = new List<string>();
        public Amount MissionsObjectiveCount { get; set; }
        public Amount MissionsObjectiveDistance { get; set; }
        public FogOfWar OptionsFogOfWar { get; set; }
        public List<string> OptionsMission { get { return OptionsMission_; } set { OptionsMission_ = Database.Instance.CheckIDs<DBEntryOptionsMission>(value.ToArray()).ToList(); } }
        private List<string> OptionsMission_ = new List<string>();
        public List<string> OptionsMods { get { return OptionsMods_; } set { OptionsMods_ = Database.Instance.CheckIDs<DBEntryDCSMod>(value.ToArray()).ToList(); } }
        private List<string> OptionsMods_ = new List<string>();
        public List<RealismOption> OptionsRealism { get { return OptionsRealism_; } set { OptionsRealism_ = value.Distinct().ToList(); } }
        private List<RealismOption> OptionsRealism_ = new List<RealismOption>();
        public List<MissionTemplateFlightGroup> PlayerFlightGroups { get { return PlayerFlightGroups_; } set { PlayerFlightGroups_ = value.Take(MissionTemplate.MAX_PLAYER_FLIGHT_GROUPS).ToList(); } }
        private List<MissionTemplateFlightGroup> PlayerFlightGroups_ = new List<MissionTemplateFlightGroup>();
        public string PlayerStartingAirbase { get; set; }
        public AmountNR SituationEnemySkill { get; set; }
        public AmountNR SituationEnemyAirDefense { get; set; }
        public AmountNR SituationEnemyAirForce { get; set; }
        public AmountNR SituationFriendlySkill { get; set; }
        public AmountNR SituationFriendlyAirDefense { get; set; }
        public AmountNR SituationFriendlyAirForce { get; set; }
        public int CombinedArmsCommanderBlue  { get { return CombinedArmsCommanderBlue_; } set { CombinedArmsCommanderBlue_ = Toolbox.Clamp(value, 0, MAX_COMBINED_ARMS_SLOTS); } }
        private int CombinedArmsCommanderBlue_;
        public int CombinedArmsCommanderRed  { get { return CombinedArmsCommanderRed_; } set { CombinedArmsCommanderRed_ = Toolbox.Clamp(value, 0, MAX_COMBINED_ARMS_SLOTS); } }
        private int CombinedArmsCommanderRed_;
        public int CombinedArmsJTACBlue  { get { return CombinedArmsJTACBlue_; } set { CombinedArmsJTACBlue_ = Toolbox.Clamp(value, 0, MAX_COMBINED_ARMS_SLOTS); } }
        private int CombinedArmsJTACBlue_;
        public int CombinedArmsJTACRed  { get { return CombinedArmsJTACRed_; } set { CombinedArmsJTACRed_ = Toolbox.Clamp(value, 0, MAX_COMBINED_ARMS_SLOTS); } }
        private int CombinedArmsJTACRed_;

        public CampaignTemplate()
        {
            Clear();
        }

        public CampaignTemplate(string filePath)
        {
            LoadFromFile(filePath);
        }

        public void Clear()
        {
            // If the default template is found, load it.
            if (File.Exists(DEFAULT_TEMPLATE_FILEPATH))
            {
                LoadFromFile(DEFAULT_TEMPLATE_FILEPATH);
                return;
            }

            Language = "EN";
            BriefingCampaignName = "";

            ContextCoalitionBlue = "USA";
            ContextPlayerCoalition = Coalition.Blue;
            ContextCoalitionRed = "Russia";
            ContextDecade = Decade.Decade2000;
            ContextTheater = "Caucasus";
            ContextSituation = "";

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
            OptionsMission = new List<string> { "ImperialUnitsForBriefing" };
            OptionsRealism = new RealismOption[] { RealismOption.DisableDCSRadioAssists, RealismOption.NoBDA }.ToList();
            var fg = new MissionTemplateFlightGroup();
            fg.AIWingmen = true;
            PlayerFlightGroups = new List<MissionTemplateFlightGroup>{fg};
            PlayerStartingAirbase = "";

            SituationEnemySkill = AmountNR.Random;
            SituationEnemyAirDefense = AmountNR.Random;
            SituationEnemyAirForce = AmountNR.Random;

            SituationFriendlySkill = AmountNR.Random;
            SituationFriendlyAirDefense = AmountNR.Random;
            SituationFriendlyAirForce = AmountNR.Random;

            CombinedArmsCommanderBlue = 0;
            CombinedArmsCommanderRed = 0;
            CombinedArmsJTACBlue = 0;
            CombinedArmsJTACRed = 0;

            AssignAliases();
        }

        public bool LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            return Load(new INIFile(filePath));
        }

        public bool LoadFromString(string data)
        {
            return Load(INIFile.CreateFromRawINIString(data));
        }

        private bool Load(INIFile ini)
        {
            BriefingCampaignName = ini.GetValue("Briefing", "CampaignName", BriefingCampaignName);

            ContextCoalitionBlue = ini.GetValue("Context", "Coalitions.Blue", ContextCoalitionBlue);
            ContextPlayerCoalition = ini.GetValue("Context", "Coalitions.Player", ContextPlayerCoalition);
            ContextCoalitionRed = ini.GetValue("Context", "Coalitions.Red", ContextCoalitionRed);
            ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
            ContextTheater = ini.GetValue("Context", "Theater", ContextTheater);
            ContextSituation = ini.GetValue("Context", "Situation", ContextSituation);

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

            PlayerFlightGroups.Clear();
            foreach (string key in ini.GetTopLevelKeysInSection("PlayerFlightGroups"))
                PlayerFlightGroups.Add(new MissionTemplateFlightGroup(ini, "PlayerFlightGroups", key));

            if(PlayerFlightGroups.Count == 0) // Redundancy vs old single player version
                PlayerFlightGroups.Add(new MissionTemplateFlightGroup(ini, "PlayerFlightGroups", "Player"));

            PlayerStartingAirbase = ini.GetValue("Player", "StartingAirbase", PlayerStartingAirbase);

            SituationEnemySkill = ini.GetValue("Situation", "EnemySkill", SituationEnemySkill);
            SituationEnemyAirDefense = ini.GetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
            SituationEnemyAirForce = ini.GetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);

            SituationFriendlySkill = ini.GetValue("Situation", "FriendlySkill", SituationFriendlySkill);
            SituationFriendlyAirDefense = ini.GetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
            SituationFriendlyAirForce = ini.GetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);

            CombinedArmsCommanderBlue = ini.GetValue("CombinedArms", "CommanderBlue", CombinedArmsCommanderBlue);
            CombinedArmsCommanderRed = ini.GetValue("CombinedArms", "CommanderRed", CombinedArmsCommanderRed);
            CombinedArmsJTACBlue = ini.GetValue("CombinedArms", "JTACBlue", CombinedArmsJTACBlue);
            CombinedArmsJTACRed = ini.GetValue("CombinedArms", "JTACRed", CombinedArmsJTACRed);

            AssignAliases();
            return true;
        }

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
            int i;
            var ini = new INIFile();
            ini.SetValue("Briefing", "CampaignName", BriefingCampaignName);

            ini.SetValue("Context", "Coalitions.Blue", ContextCoalitionBlue);
            ini.SetValue("Context", "Coalitions.Player", ContextPlayerCoalition);
            ini.SetValue("Context", "Coalitions.Red", ContextCoalitionRed);
            ini.SetValue("Context", "Decade", ContextDecade);
            ini.SetValue("Context", "Theater", ContextTheater);
            ini.SetValue("Context", "Situation", ContextSituation);

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

            for (i = 0; i < PlayerFlightGroups.Count; i++)
                PlayerFlightGroups[i].SaveToFile(ini, "PlayerFlightGroups", $"PlayerFlightGroup{i:000}");
            ini.SetValue("Player", "StartingAirbase", PlayerStartingAirbase);

            ini.SetValue("Situation", "EnemySkill", SituationEnemySkill);
            ini.SetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
            ini.SetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);

            ini.SetValue("Situation", "FriendlySkill", SituationFriendlySkill);
            ini.SetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
            ini.SetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);

            ini.SetValue("CombinedArms", "CommanderBlue", CombinedArmsCommanderBlue);
            ini.SetValue("CombinedArms", "CommanderRed", CombinedArmsCommanderRed);
            ini.SetValue("CombinedArms", "JTACBlue", CombinedArmsJTACBlue);
            ini.SetValue("CombinedArms", "JTACRed", CombinedArmsJTACRed);

            AssignAliases();
            return ini;
        }

        public string GetCoalition(Coalition coalition)
        {
            if (coalition == Coalition.Red) return ContextCoalitionRed;
            return ContextCoalitionBlue;
        }

        internal void AssignAliases()
        {
            foreach (var item in PlayerFlightGroups)
                item.AssignAlias(PlayerFlightGroups.IndexOf(item));
        }

    }
}
