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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Template;
using BriefingRoom4DCSWorld.TypeConverters;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.Campaign
{
    /// <summary>
    /// A campaign template
    /// </summary>
    public class CampaignTemplate : IDisposable
    {
        /// <summary>
        /// Minimum number of missions in a campaign.
        /// </summary>
        private const int MIN_CAMPAIGN_MISSIONS = 2;

        /// <summary>
        /// Maximum number of missions in a campaign.
        /// </summary>
        private const int MAX_CAMPAIGN_MISSIONS = 20;

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
        /// Who belongs to the blue coalition?
        /// </summary>
        [Category("Context"), DisplayName("Coalition, blue")]
        [Description("Who belongs to the blue coalition?")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string ContextCoalitionsBlue { get { return ContextCoalitionBlue_; } set { ContextCoalitionBlue_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string ContextCoalitionBlue_;

        /// <summary>
        /// Who belongs to the red coalition?
        /// </summary>
        [Category("Context"), DisplayName("Coalition, red")]
        [Description("Who belongs to the red coalition?")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string ContextCoalitionsRed { get { return ContextCoalitionRed_; } set { ContextCoalitionRed_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string ContextCoalitionRed_;

        /// <summary>
        /// Decade during which the campaign will take place.
        /// </summary>
        [Category("Context"), DisplayName("Decade")]
        [Description("Decade during which the campaign will take place.")]
        [TypeConverter(typeof(EnumTypeConverter<Decade>))]
        public Decade ContextDecade { get; set; }

        /// <summary>
        /// DCS World theater in which the mission will take place.
        /// </summary>
        [Category("Context"), DisplayName("Theater")]
        [Description("DCS World theater in which the mission will take place.")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryTheater>))]
        public string ContextTheaterID { get { return ContextTheaterID_; } set { ContextTheaterID_ = TemplateTools.CheckValue<DBEntryTheater>(value); } }
        private string ContextTheaterID_;

        /// <summary>
        /// To which coalitions should the countries on the map (and their airbases) belong to?
        /// </summary>
        [Category("Context"), DisplayName("Theater, regions alignment")]
        [Description("To which coalitions should the countries on the map (and their airbases) belong to?")]
        [TypeConverter(typeof(EnumTypeConverter<CountryCoalition>))]
        public CountryCoalition ContextTheaterRegionsCoalitions { get; set; }

        /// <summary>
        /// Chance for a mission of this campaign to take place in bad weather.
        /// </summary>
        [Category("Environment"), DisplayName("Bad weather chance")]
        [Description("Chance for a mission of this campaign to take place in bad weather.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN EnvironmentBadWeatherChance { get; set; }

        /// <summary>
        /// Chance for a mission of this campaign to take place during the night.
        /// </summary>
        [Category("Environment"), DisplayName("Night mission chance")]
        [Description("Chance for a mission of this campaign to take place during the night.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN EnvironmentNightMissionChance { get; set; }

        /// <summary>
        /// Number of missions in the campaign.
        /// </summary>
        [Category("Missions"), DisplayName("Number of missions")]
        [Description("Number of missions in the campaign.")]
        public int MissionsCount { get { return MissionsCount_; } set { MissionsCount_ = Toolbox.Clamp(value, MIN_CAMPAIGN_MISSIONS, MAX_CAMPAIGN_MISSIONS); } }
        private int MissionsCount_;

        /// <summary>
        /// How many objectives/targets will be present in each mission.
        /// </summary>
        [Category("Missions"), DisplayName("Objective count")]
        [Description("How many objectives/targets will be present in the mission.")]
        [TypeConverter(typeof(EnumTypeConverter<Amount>))]
        public Amount MissionsObjectiveCount { get; set; }

        /// <summary>
        /// How far from the player's starting location will the objectives be.
        /// </summary>
        [Category("Missions"), DisplayName("Objective distance")]
        [Description("How far from the player's starting location will the objectives be.")]
        [TypeConverter(typeof(EnumTypeConverter<Amount>))]
        public Amount MissionsObjectiveDistance { get; set; }

        /// <summary>
        /// The type of tasking player may recieve in this campaign's missions.
        /// </summary>
        [Category("Missions"), DisplayName("Mission tasking")]
        [Description("The type of tasking player may recieve in this campaign's missions.")]
        [TypeConverter(typeof(StringArrayTypeConverter))]
        [Editor(typeof(CheckedListBoxUIEditorDBEntry<DBEntryObjective>), typeof(UITypeEditor))]
        public string[] MissionsTypes { get { return MissionsTypes_; } set { MissionsTypes_ = TemplateTools.CheckValues<DBEntryObjective>(value, Database.Instance.Common.DefaultObjective); } }
        private string[] MissionsTypes_;

        /// <summary>
        /// Should enemy units be shown on the F10 map, mission planning, MFD SA pages, etc?
        /// </summary>
        [Category("Options"), DisplayName("Show enemy units on map")]
        [Description("Should enemy units be shown on the F10 map, mission planning, MFD SA pages, etc?")]
        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool OptionsShowEnemyUnits { get; set; }

        /// <summary>
        /// Type of aircraft the player will fly.
        /// </summary>
        [Category("Player"), DisplayName("Aircraft")]
        [Description("Type of aircraft the player will fly.")]
        [TypeConverter(typeof(DBEntryPlayerAircraftTypeConverter))]
        public string PlayerAircraft { get { return PlayerAircraft_; } set { PlayerAircraft_ = TemplateTools.CheckValuePlayerAircraft(value); } }
        private string PlayerAircraft_;

        /// <summary>
        /// Which coalition does the player belong to?
        /// </summary>
        [Category("Player"), DisplayName("Coalition")]
        [Description("Which coalition does the player belong to?")]
        [TypeConverter(typeof(EnumTypeConverter<Coalition>))]
        public Coalition PlayerCoalition { get; set; }

        /// <summary>
        /// Where should the player take off from?
        /// </summary>
        [Category("Player"), DisplayName("Start location")]
        [Description("Where should the player take off from?")]
        [TypeConverter(typeof(EnumTypeConverter<PlayerStartLocation>))]
        public PlayerStartLocation PlayerStartLocation { get; set; }

        /// <summary>
        /// Initial quality of enemy air defense.
        /// </summary>
        [Category("Situation"), DisplayName("Enemy air defense")]
        [Description("Initial quality of enemy air defense.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN SituationEnemyAirDefense { get; set; }

        /// <summary>
        /// Initial quality of enemy air force.
        /// </summary>
        [Category("Situation"), DisplayName("Enemy air force")]
        [Description("Initial quality of enemy air force.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN SituationEnemyAirForce { get; set; }

        /// <summary>
        /// Initial quality of friendly air defense.
        /// </summary>
        [Category("Situation"), DisplayName("Friendly air defense")]
        [Description("Initial quality of friendly air defense.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN SituationFriendlyAirDefense { get; set; }

        /// <summary>
        /// Initial quality of friendly air force.
        /// </summary>
        [Category("Situation"), DisplayName("Friendly air force")]
        [Description("Initial quality of friendly air force.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN SituationFriendlyAirForce { get; set; }

        /// <summary>
        /// How the situation evolves during the campaign. Campaign can get harder and harder with enemy power increasing as player completes missions (like in a game) or easier and easier as enemy power decreased (as it tend to happens to the losing side in a real conflict), etc.
        /// </summary>
        [Category("Situation"), DisplayName("Variation")]
        [Description("How the situation evolves during the campaign. Campaign can get harder and harder with enemy power increasing as player completes missions (like in a game) or easier and easier as enemy power decreased (as it tend to happens to the losing side in a real conflict), etc.")]
        [TypeConverter(typeof(EnumTypeConverter<CampaignDifficultyVariation>))]
        public CampaignDifficultyVariation SituationVariation { get; set; }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            ContextCoalitionsBlue = TemplateTools.CheckValue<DBEntryCoalition>(Database.Instance.Common.DefaultCoalitionBlue);
            ContextCoalitionsRed = TemplateTools.CheckValue<DBEntryCoalition>(Database.Instance.Common.DefaultCoalitionRed);
            ContextDecade = Decade.Decade2000;
            ContextTheaterID = TemplateTools.CheckValue<DBEntryTheater>(Database.Instance.Common.DefaultTheater);
            ContextTheaterRegionsCoalitions = CountryCoalition.Default;

            EnvironmentBadWeatherChance = AmountN.Random;
            EnvironmentNightMissionChance = AmountN.Random;
            
            MissionsCount = 5;
            MissionsObjectiveCount = Amount.Average;
            MissionsObjectiveDistance = Amount.Average;
            MissionsTypes = new string[] { Database.Instance.Common.DefaultObjective };

            OptionsShowEnemyUnits = true;

            PlayerAircraft = TemplateTools.CheckValuePlayerAircraft(Database.Instance.Common.DefaultPlayerAircraft);
            PlayerCoalition = Coalition.Blue;
            PlayerStartLocation = PlayerStartLocation.Runway;

            SituationEnemyAirDefense = AmountN.Random;
            SituationEnemyAirForce = AmountN.Random;
            SituationFriendlyAirDefense = AmountN.Random;
            SituationFriendlyAirForce = AmountN.Random;
            SituationVariation = CampaignDifficultyVariation.Random;
        }

        /// <summary>
        /// Loads a campaign template from an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file</param>
        /// <returns></returns>
        public bool LoadFromFile(string filePath)
        {
            Clear();
            if (!File.Exists(filePath)) return false;

            using (INIFile ini = new INIFile(filePath))
            {
                ContextCoalitionsBlue = ini.GetValue("Context", "Coalitions.Blue", ContextCoalitionsBlue);
                ContextCoalitionsRed = ini.GetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
                ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
                ContextTheaterID = ini.GetValue("Context", "TheaterID", ContextTheaterID);
                ContextTheaterRegionsCoalitions = ini.GetValue("Context", "TheaterRegionsCoalitions", ContextTheaterRegionsCoalitions);

                EnvironmentBadWeatherChance = ini.GetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
                EnvironmentNightMissionChance = ini.GetValue("Environment", "NightMissionChance", EnvironmentNightMissionChance);

                MissionsCount = ini.GetValue("Missions", "Count", MissionsCount);
                MissionsObjectiveCount = ini.GetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
                MissionsObjectiveDistance = ini.GetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);
                MissionsTypes = ini.GetValueArray<string>("Missions", "Types");

                OptionsShowEnemyUnits = ini.GetValue("Options", "ShowEnemyUnits", OptionsShowEnemyUnits);

                PlayerAircraft = ini.GetValue("Player", "Aircraft", PlayerAircraft);
                PlayerCoalition = ini.GetValue("Player", "Coalition", PlayerCoalition);
                PlayerStartLocation = ini.GetValue("Player", "StartLocation", PlayerStartLocation);

                SituationEnemyAirDefense = ini.GetValue("Situation", "Enemy.AirDefense", SituationEnemyAirDefense);
                SituationEnemyAirForce = ini.GetValue("Situation", "Enemy.AirForce", SituationEnemyAirForce);
                SituationFriendlyAirDefense = ini.GetValue("Situation", "Friendly.AirDefense", SituationFriendlyAirDefense);
                SituationFriendlyAirForce = ini.GetValue("Situation", "Friendly.AirForce", SituationFriendlyAirForce);
                SituationVariation = ini.GetValue("Situation", "Variation", SituationVariation);
            }

            return true;
        }

        /// <summary>
        /// Save the campaign template to an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file.</param>
        public void SaveToFile(string filePath)
        {
            using (INIFile ini = new INIFile())
            {
                ini.SetValue("Context", "Coalitions.Blue", ContextCoalitionsBlue);
                ini.SetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
                ini.SetValue("Context", "Decade", ContextDecade);
                ini.SetValue("Context", "TheaterID", ContextTheaterID);
                ini.SetValue("Context", "TheaterRegionsCoalitions", ContextTheaterRegionsCoalitions);

                ini.SetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
                ini.SetValue("Environment", "NightMissionChance", EnvironmentNightMissionChance);

                ini.SetValue("Missions", "Count", MissionsCount);
                ini.SetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
                ini.SetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);
                ini.SetValueArray("Missions", "Types", MissionsTypes);

                ini.SetValue("Options", "ShowEnemyUnits", OptionsShowEnemyUnits);

                ini.SetValue("Player", "Aircraft", PlayerAircraft);
                ini.SetValue("Player", "Coalition", PlayerCoalition);
                ini.SetValue("Player", "StartLocation", PlayerStartLocation);

                ini.SetValue("Situation", "Enemy.AirDefense", SituationEnemyAirDefense);
                ini.SetValue("Situation", "Enemy.AirForce", SituationEnemyAirForce);
                ini.SetValue("Situation", "Friendly.AirDefense", SituationFriendlyAirDefense);
                ini.SetValue("Situation", "Friendly.AirForce", SituationFriendlyAirForce);
                ini.SetValue("Situation", "Variation", SituationVariation);

                ini.SaveToFile(filePath);
            }
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
