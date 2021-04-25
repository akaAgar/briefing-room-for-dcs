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

using BriefingRoom.Attributes;
using BriefingRoom.DB;
using BriefingRoom.Template;
using System;
//using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;

namespace BriefingRoom.Campaign
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

        [TreeViewProperty("Coalition, blue", "Context", typeof(DBEntryCoalition), "Who belongs to the blue coalition?")]
        public string ContextCoalitionsBlue { get; set; }

        [TreeViewProperty("Player coalition", "Context", typeof(Coalition), "Which coalition does the player(s) belong to?")]
        public Coalition ContextCoalitionPlayer { get; set; }

        [TreeViewProperty("Coalition, red", "Context", typeof(DBEntryCoalition), "Who belongs to the red coalition?")]
        public string ContextCoalitionsRed { get; set; }

        [TreeViewProperty("Time period", "Context", typeof(Decade), "Decade during which the campaign will take place.")]
        public Decade ContextDecade { get; set; }

        [TreeViewProperty("Theater", "Context", typeof(DBEntryTheater), "DCS World theater in which the mission will take place.")]
        public string ContextTheaterID { get; set; }

        [TreeViewProperty("Bad weather chance", "Environment", typeof(AmountN), "Chance for a mission of this campaign to take place in bad weather.")]
        public AmountN EnvironmentBadWeatherChance { get; set; }

        [TreeViewProperty("Night mission chance", "Environment", typeof(AmountN), "Chance for a mission of this campaign to take place during the night.")]
        public AmountN EnvironmentNightMissionChance { get; set; }

        [TreeViewProperty("Count", "Missions", typeof(int), "Number of missions in the campaign.")]
        [TreeViewPropertyInt(MIN_CAMPAIGN_MISSIONS, MAX_CAMPAIGN_MISSIONS)]
        public int MissionsCount { get; set; }

        [TreeViewProperty("Difficulty variation", "Missions", typeof(CampaignDifficultyVariation), "How the situation evolves during the campaign. Campaign can get harder and harder with enemy power increasing as player completes missions (like in a game) or easier and easier as enemy power decreased (as it tend to happens to the losing side in a real conflict), etc.")]
        public CampaignDifficultyVariation MissionsDifficultyVariation { get; set; }

        [TreeViewProperty("Objective count", "Missions", typeof(Amount), "How many objectives/targets will be present in the mission.")]
        public Amount MissionsObjectiveCount { get; set; }

        [TreeViewProperty("Objective distance", "Missions", typeof(Amount), "How far from the player's starting location will the objectives be.")]
        public Amount MissionsObjectiveDistance { get; set; }

        //[TreeViewProperty("Objectives", null, typeof(DBEntryObjective), "Allowed objectives in this campaign.")]
        //public string[] Objectives { get; set; }

        [TreeViewProperty("Civilian road traffic", "Options", typeof(CivilianTraffic), "Amount of civilian traffic on the roads. Can affect performance if set too high.")]
        public CivilianTraffic OptionsCivilianTraffic { get; set; }

        [TreeViewProperty("Theater countries coalitions", "Options", typeof(CountryCoalition), "To which coalitions should the countries on the map (and their airbases) belong to?")]
        public CountryCoalition OptionsTheaterCountriesCoalitions { get; set; }

        [TreeViewProperty("Realism", null, typeof(RealismOption), "Realism options to apply to this mission.")]
        public RealismOption[] Realism { get; set; }

        [TreeViewProperty("Unit mods", null, typeof(DBEntryDCSMod), "Which unit mods should be enabled in this mission? Make sure units mods are installed and active in your version of DCS World or the units won't be spawned.")]
        public string[] UnitMods { get; set; }

        [TreeViewProperty("Aircraft", "Player", typeof(DBPseudoEntryPlayerAircraft), "Type of aircraft the player will fly.")]
        public string PlayerAircraft { get; set; }

        [TreeViewProperty("Carrier", "Player", typeof(DBPseudoEntryCarrier), "Type of aircraft carrier the player will be spawned on.")]
        public string PlayerCarrier { get; set; }

        [TreeViewProperty("Start location", "Player", typeof(PlayerStartLocation), "Where should the player take off from?")]
        public PlayerStartLocation PlayerStartLocation { get; set; }

        [TreeViewProperty("Starting airbase", "Player", typeof(DBPseudoEntryAirbase), "Name of the airbase the player must take off from. If left empty, or if the airbase doesn't exist in this theater, a random airbase will be selected. Be aware that if the selected airbase doesn't have enough parking spots for the player mission package, some units may not spawn properly.", TreeViewPropertyAttributeFlags.EmptyIsRandom)]
        public string PlayerStartingAirbase { get; set; }

        [TreeViewProperty("Enemy air defense", "Situation", typeof(AmountN), "Intensity and quality of enemy air defense.")]
        public AmountN SituationEnemyAirDefense { get; set; }

        [TreeViewProperty("Enemy air force", "Situation", typeof(AmountN), "Relative power of the enemy air force.")]
        public AmountN SituationEnemyAirForce { get; set; }

        [TreeViewProperty("Friendly air defense", "Situation", typeof(AmountN), "Intensity and quality of friendly air defense.")]
        public AmountN SituationFriendlyAirDefense { get; set; }

        [TreeViewProperty("Friendly air force", "Situation", typeof(AmountN), "Intensity and quality of friendly air force.")]
        public AmountN SituationFriendlyAirForce { get; set; }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            ContextCoalitionsBlue = "USA"; // Database.Instance.CheckValue<DBEntryCoalition>("USA");
            ContextCoalitionPlayer = Coalition.Blue;
            ContextCoalitionsRed = "Russia"; // Database.Instance.CheckValue<DBEntryCoalition>("Russia");
            ContextDecade = Decade.Decade2000;
            ContextTheaterID = "Caucasus"; // Database.Instance.CheckValue<DBEntryTheater>("Caucasus");

            EnvironmentBadWeatherChance = AmountN.Random;
            EnvironmentNightMissionChance = AmountN.Random;
            
            MissionsCount = 5;
            MissionsDifficultyVariation = CampaignDifficultyVariation.Random;
            MissionsObjectiveCount = Amount.Average;
            MissionsObjectiveDistance = Amount.Average;
            
            //Objectives = new string[0];
            
            OptionsTheaterCountriesCoalitions = CountryCoalition.Default;
            OptionsCivilianTraffic = CivilianTraffic.Low;

            PlayerAircraft = "Su-25T"; // Database.Instance.CheckValue<DBPseudoEntryPlayerAircraft>("Su-25T");
            PlayerStartingAirbase = "";
            PlayerCarrier = "";
            PlayerStartLocation = PlayerStartLocation.Runway;

            Realism = new RealismOption[] { RealismOption.DisableDCSRadioAssists, RealismOption.NoBDA };

            SituationEnemyAirDefense = AmountN.Random;
            SituationEnemyAirForce = AmountN.Random;
            SituationFriendlyAirDefense = AmountN.Random;
            SituationFriendlyAirForce = AmountN.Random;

            UnitMods = new string[0];
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
                ContextCoalitionPlayer = ini.GetValue("Context", "Coalition.Player", ContextCoalitionPlayer);
                ContextCoalitionsRed = ini.GetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
                ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
                ContextTheaterID = ini.GetValue("Context", "TheaterID", ContextTheaterID);

                EnvironmentBadWeatherChance = ini.GetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
                EnvironmentNightMissionChance = ini.GetValue("Environment", "NightMissionChance", EnvironmentNightMissionChance);

                MissionsCount = ini.GetValue("Missions", "Count", MissionsCount);
                MissionsDifficultyVariation = ini.GetValue("Missions", "DifficultyVariation", MissionsDifficultyVariation);
                MissionsObjectiveCount = ini.GetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
                MissionsObjectiveDistance = ini.GetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);

                //Objectives = ini.GetValueArray<string>("Objectives", "Objectives");

                OptionsCivilianTraffic = ini.GetValue("Options", "CivilianTraffic", OptionsCivilianTraffic);
                OptionsTheaterCountriesCoalitions = ini.GetValue("Options", "TheaterRegionsCoalitions", OptionsTheaterCountriesCoalitions);

                PlayerAircraft = ini.GetValue("Player", "Aircraft", PlayerAircraft);
                PlayerStartingAirbase = ini.GetValue("Player", "StartingAirbase", PlayerStartingAirbase);
                PlayerCarrier = ini.GetValue("Player", "StartingCarrier", PlayerCarrier);
                PlayerStartLocation = ini.GetValue("Player", "StartLocation", PlayerStartLocation);

                Realism = ini.GetValueArray<RealismOption>("Realism", "Realism");
                
                SituationEnemyAirDefense = ini.GetValue("Situation", "Enemy.AirDefense", SituationEnemyAirDefense);
                SituationEnemyAirForce = ini.GetValue("Situation", "Enemy.AirForce", SituationEnemyAirForce);
                SituationFriendlyAirDefense = ini.GetValue("Situation", "Friendly.AirDefense", SituationFriendlyAirDefense);
                SituationFriendlyAirForce = ini.GetValue("Situation", "Friendly.AirForce", SituationFriendlyAirForce);

                UnitMods = ini.GetValueArray<string>("UnitMods", "UnitMods");
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
                ini.SetValue("Context", "Coalition.Player", ContextCoalitionPlayer);
                ini.SetValue("Context", "Coalitions.Red", ContextCoalitionsRed);
                ini.SetValue("Context", "Decade", ContextDecade);
                ini.SetValue("Context", "TheaterID", ContextTheaterID);

                ini.SetValue("Environment", "BadWeatherChance", EnvironmentBadWeatherChance);
                ini.SetValue("Environment", "NightMissionChance", EnvironmentNightMissionChance);

                ini.SetValue("Missions", "Count", MissionsCount);
                ini.SetValue("Missions", "DifficultyVariation", MissionsDifficultyVariation);
                ini.SetValue("Missions", "ObjectiveCount", MissionsObjectiveCount);
                ini.SetValue("Missions", "ObjectiveDistance", MissionsObjectiveDistance);

                //ini.SetValueArray("Objectives", "Objectives", Objectives);

                ini.SetValue("Options", "CivilianTraffic", OptionsCivilianTraffic);
                ini.SetValue("Options", "TheaterRegionsCoalitions", OptionsTheaterCountriesCoalitions);

                ini.SetValue("Player", "Aircraft", PlayerAircraft);
                ini.SetValue("Player", "StartingAirbase", PlayerStartingAirbase);
                ini.SetValue("Player", "StartingCarrier", PlayerCarrier);
                ini.SetValue("Player", "StartLocation", PlayerStartLocation);

                ini.SetValueArray("Realism", "Realism", Realism);

                ini.SetValue("Situation", "Enemy.AirDefense", SituationEnemyAirDefense);
                ini.SetValue("Situation", "Enemy.AirForce", SituationEnemyAirForce);
                ini.SetValue("Situation", "Friendly.AirDefense", SituationFriendlyAirDefense);
                ini.SetValue("Situation", "Friendly.AirForce", SituationFriendlyAirForce);

                ini.SetValueArray("UnitMods", "UnitMods", UnitMods);

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
