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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Template
{
    /// <summary>
    /// A mission template, to be used as input in the MissionGenerator class.
    /// </summary>
    public sealed class MissionTemplate : IDisposable
    {
        /// <summary>
        /// Path to the default template file storing default values.
        /// </summary>
        private static readonly string DEFAULT_TEMPLATE_FILEPATH = $"{BRPaths.ROOT}Default.brt";

        /// <summary>
        /// Maximum number of objectives.
        /// </summary>
        public const int MAX_OBJECTIVES = 5;

        /// <summary>
        /// Maximum number of player flight groups.
        /// </summary>
        public const int MAX_PLAYER_FLIGHT_GROUPS = 8;

        /// <summary>
        /// Maximum distance to the objective, in nautical miles.
        /// </summary>
        public const int MAX_OBJECTIVE_DISTANCE = 200;

        [Required]
        [Display(Name = "Mission type", Description = "Is the mission single or multiplayer?")]
        public MissionType MissionType { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Coalition)]
        [Display(Name = "Blue coalition", Description = "Which countries belong to the blue coalition?")]
        [Category("Context")]
        public string ContextCoalitionBlue { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Coalition)]
        [Display(Name = "Player coalition", Description = "Which countries belong to the red coalition?")]
        [Category("Context")]
        public string ContextCoalitionRed { get; set; }

        [Required]
        [Display(Name = "Time period", Description = "Time period during which the mission takes place.")]
        [Category("Context")]
        public Decade ContextDecade { get; set; }

        [Required]
        [Display(Name = "Player coalition", Description = "Coalition the player(s) belongs to.")]
        [Category("Context")]
        public Coalition ContextPlayerCoalition { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Theater)]
        [Display(Name = "Theater", Description = "Theater in which the mission takes place.")]
        [Category("Context")]
        public string ContextTheater { get; set; }

        [Required]
        [Display(Name = "Countries coalition", Description = "To which coalition do the countries on the map (and their airbases) belong?")]
        [Category("Context")]
        public CountryCoalition ContextTheaterCountriesCoalitions { get; set; }

        [Required]
        [Display(Name = "Season", Description = "Season during which the mission takes place.")]
        [Category("Environment")]
        public Season EnvironmentSeason { get; set; }

        [Required]
        [Display(Name = "Time of day", Description = "Time of day of mission start.")]
        [Category("Environment")]
        public TimeOfDay EnvironmentTimeOfDay { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.WeatherPreset, true)]
        [Display(Name = "Weather preset", Description = "Weather preset to use for this mission.")]
        [Category("Environment")]
        public string EnvironmentWeatherPreset { get; set; }

        [Required]
        [Display(Name = "Wind", Description = "Wind intensity.")]
        [Category("Environment")]
        public Wind EnvironmentWind { get; set; }

        [Required, Range(0, MAX_OBJECTIVE_DISTANCE, ErrorMessage = "Objective distance must be between {1} and {2} nautical miles.")]
        [Display(Name = "Objective distance", Description = "Distance to the objectives, in nautical miles. \"Zero\" means \"random\".")]
        [Category("Flight plan")]
        public int FlightPlanObjectiveDistance { get { return FlightPlanObjectiveDistance_; } set { FlightPlanObjectiveDistance_ = Toolbox.Clamp(value, 0, MAX_OBJECTIVE_DISTANCE); } }
        private int FlightPlanObjectiveDistance_;

        [Required]
        [Display(Name = "Objectives locations", Description = "In which countries should objectives and enemy units be spawned? Be aware that selecting an option other than \"Any\" can greatly increase distance to the objectives.")]
        [Category("Flight plan")]
        public ObjectiveCountryLocation FlightPlanObjectivesLocation { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.Airbase, true)]
        [Display(Name = "Starting airbase", Description = "Airbase from which the player(s) will take off. Leave empty for none")]
        [Category("Flight plan")]
        public string FlightPlanTheaterStartingAirbase { get; set; }

        [Required, DatabaseSourceType(DatabaseEntryType.MissionFeature)]
        [Display(Name = "Mission features", Description = "Special features to include in this mission.")]
        public List<string> MissionFeatures { get { return MissionFeatures_; } set { MissionFeatures_ = Database.Instance.CheckMissionFeaturesIDs(value.ToArray()).ToList(); } }
        private List<string> MissionFeatures_ = new List<string>();

        [Required, DatabaseSourceType(DatabaseEntryType.DCSMod)]
        [Display(Name = "DCS World mods", Description = "DCS unit mods to use for this mission.")]
        public List<string> Mods { get { return Mods_; } set { Mods_ = Database.Instance.CheckIDs<DBEntryDCSMod>(value.ToArray()).ToList(); } }
        private List<string> Mods_ = new List<string>();

        [Required, MinLength(1), MaxLength(MAX_OBJECTIVES)]
        [Display(Name = "Objectives", Description = "Mission objectives.")]
        public List<MissionTemplateObjective> Objectives { get; set; }

        [Required]
        [Display(Name = "Fog of war", Description = "Fog of war settings for this mission.")]
        public FogOfWar OptionsFogOfWar { get; set; }

        [Required]
        [Display(Name = "Mission options", Description = "Miscellaneous options to customize the mission's feel.")]
        [Category("Options")]
        public List<MissionOption> OptionsMission { get { return OptionsMission_; } set { OptionsMission_ = value.Distinct().ToList(); } }
        private List<MissionOption> OptionsMission_ = new List<MissionOption>();

        [Required]
        [Display(Name = "Realism options", Description = "Realism options to enforce.")]
        [Category("Options")]
        public List<RealismOption> OptionsRealism { get { return OptionsRealism_; } set { OptionsRealism_ = value.Distinct().ToList(); } }
        private List<RealismOption> OptionsRealism_ = new List<RealismOption>();

        [Required, MinLength(1), MaxLength(MAX_PLAYER_FLIGHT_GROUPS)]
        [Display(Name = "Player flight groups", Description = "All player flight groups in this mission's flight package.")]
        public List<MissionTemplateFlightGroup> PlayerFlightGroups { get { return PlayerFlightGroups_; } set { PlayerFlightGroups_ = value.Take(MAX_PLAYER_FLIGHT_GROUPS).ToList(); } }
        private List<MissionTemplateFlightGroup> PlayerFlightGroups_ = new List<MissionTemplateFlightGroup>();

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
        public MissionTemplate()
        {
            Clear();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">Path to the .ini file the template should be read from.</param>
        public MissionTemplate(string filePath)
        {
            Clear();
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

            MissionType = MissionType.SinglePlayer;

            ContextCoalitionBlue = "USA";
            ContextCoalitionRed = "Russia";
            ContextDecade = Decade.Decade2000;
            ContextPlayerCoalition = Coalition.Blue;
            ContextTheater = "Caucasus";
            ContextTheaterCountriesCoalitions = CountryCoalition.Default;

            EnvironmentSeason = Season.Random;
            EnvironmentTimeOfDay = TimeOfDay.RandomDaytime;
            EnvironmentWeatherPreset = "";
            EnvironmentWind = Wind.Random;

            FlightPlanObjectiveDistance = 80;
            FlightPlanObjectivesLocation = ObjectiveCountryLocation.Any;
            FlightPlanTheaterStartingAirbase = "";

            MissionFeatures = new List<string>();
            
            Mods = new List<string>();

            Objectives = new MissionTemplateObjective[] { new MissionTemplateObjective() }.ToList();

            OptionsFogOfWar = FogOfWar.All;
            OptionsMission = new MissionOption[] { MissionOption.ImperialUnitsForBriefing }.ToList();
            OptionsRealism = new RealismOption[] { RealismOption.DisableDCSRadioAssists, RealismOption.NoBDA }.ToList();

            PlayerFlightGroups = new MissionTemplateFlightGroup[] { new MissionTemplateFlightGroup() }.ToList();

            SituationEnemyAirDefense = AmountNR.Random;
            SituationEnemyAirForce = AmountNR.Random;
            SituationFriendlyAirDefense = AmountNR.Random;
            SituationFriendlyAirForce = AmountNR.None;
        }

        /// <summary>
        /// Loads a mission template from an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file</param>
        /// <returns></returns>
        public bool LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            using (INIFile ini = new INIFile(filePath))
            {
                MissionType = ini.GetValue("MissionType", "MissionType", MissionType);

                ContextCoalitionBlue = ini.GetValue("Context", "CoalitionBlue", ContextCoalitionBlue);
                ContextCoalitionRed = ini.GetValue("Context", "CoalitionRed", ContextCoalitionRed);
                ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
                ContextPlayerCoalition = ini.GetValue("Context", "PlayerCoalition", ContextPlayerCoalition);
                ContextTheater = ini.GetValue("Context", "Theater", ContextTheater);
                ContextTheaterCountriesCoalitions = ini.GetValue("Context", "TheaterCountriesCoalitions", ContextTheaterCountriesCoalitions);

                EnvironmentSeason = ini.GetValue("Environment", "Season", EnvironmentSeason);
                EnvironmentTimeOfDay = ini.GetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                EnvironmentWeatherPreset = ini.GetValue("Environment", "WeatherPreset", EnvironmentWeatherPreset);
                EnvironmentWind = ini.GetValue("Environment", "Wind", EnvironmentWind);

                FlightPlanObjectiveDistance = ini.GetValue("FlightPlan", "ObjectiveDistance", FlightPlanObjectiveDistance);
                FlightPlanObjectivesLocation = ini.GetValue("FlightPlan", "ObjectivesLocation", FlightPlanObjectivesLocation);
                FlightPlanTheaterStartingAirbase = ini.GetValue("FlightPlan", "TheaterStartingAirbase", FlightPlanTheaterStartingAirbase);

                MissionFeatures = ini.GetValueArray<string>("MissionFeatures", "MissionFeatures").ToList();

                Mods = ini.GetValueArray<string>("Mods", "Mods").ToList();

                Objectives.Clear();
                foreach (string key in ini.GetTopLevelKeysInSection("Objectives"))
                    Objectives.Add(new MissionTemplateObjective(ini, "Objectives", key));

                OptionsFogOfWar = ini.GetValue("Options", "FogOfWar", OptionsFogOfWar);
                OptionsMission = ini.GetValueArray<MissionOption>("Options", "Mission").ToList();
                OptionsRealism = ini.GetValueArray<RealismOption>("Options", "Realism").ToList();

                PlayerFlightGroups.Clear();
                foreach (string key in ini.GetTopLevelKeysInSection("PlayerFlightGroups"))
                    PlayerFlightGroups.Add(new MissionTemplateFlightGroup(ini, "PlayerFlightGroups", key));

                SituationEnemyAirDefense = ini.GetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
                SituationEnemyAirForce = ini.GetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);
                SituationFriendlyAirDefense = ini.GetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
                SituationFriendlyAirForce = ini.GetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);
            }

            return true;
        }

        /// <summary>
        /// Save the mission template to an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file.</param>
        public void SaveToFile(string filePath)
        {
            int i;

            using (INIFile ini = new INIFile())
            {
                ini.SetValue("MissionType", "MissionType", MissionType);

                ini.SetValue("Context", "CoalitionBlue", ContextCoalitionBlue);
                ini.SetValue("Context", "CoalitionRed", ContextCoalitionRed);
                ini.SetValue("Context", "Decade", ContextDecade);
                ini.SetValue("Context", "PlayerCoalition", ContextPlayerCoalition);
                ini.SetValue("Context", "Theater", ContextTheater);
                ini.SetValue("Context", "TheaterCountriesCoalitions", ContextTheaterCountriesCoalitions);

                ini.SetValue("Environment", "Season", EnvironmentSeason);
                ini.SetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                ini.SetValue("Environment", "WeatherPreset", EnvironmentWeatherPreset);
                ini.SetValue("Environment", "Wind", EnvironmentWind);

                ini.SetValue("FlightPlan", "ObjectiveDistance", FlightPlanObjectiveDistance);
                ini.SetValue("FlightPlan", "ObjectivesLocation", FlightPlanObjectivesLocation);
                ini.SetValue("FlightPlan", "TheaterStartingAirbase", FlightPlanTheaterStartingAirbase);

                ini.SetValueArray("MissionFeatures", "MissionFeatures", MissionFeatures.ToArray());

                ini.SetValueArray("Mods", "Mods", Mods.ToArray());

                for (i = 0; i < Objectives.Count; i++)
                    Objectives[i].SaveToFile(ini, "Objectives", $"Objective{i:000}");

                ini.SetValue("Options", "FogOfWar", OptionsFogOfWar);
                ini.SetValueArray("Options", "Mission", OptionsMission.ToArray());
                ini.SetValueArray("Options", "Realism", OptionsRealism.ToArray());

                for (i = 0; i < PlayerFlightGroups.Count; i++)
                    PlayerFlightGroups[i].SaveToFile(ini, "PlayerFlightGroups", $"PlayerFlightGroup{i:000}");

                ini.SetValue("Situation", "EnemyAirDefense", SituationEnemyAirDefense);
                ini.SetValue("Situation", "EnemyAirForce", SituationEnemyAirForce);
                ini.SetValue("Situation", "FriendlyAirDefense", SituationFriendlyAirDefense);
                ini.SetValue("Situation", "FriendlyAirForce", SituationFriendlyAirForce);

                ini.SaveToFile(filePath);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
