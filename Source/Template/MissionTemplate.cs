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

using BriefingRoom4DCSWorld.Attributes;
using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCSWorld.Template
{
    /// <summary>
    /// A mission template, to be used as input in the MissionGenerator class.
    /// </summary>
    public class MissionTemplate : IDisposable
    {
        /// <summary>
        /// Increment between two proposed objective distances in the menu (in nautical miles).
        /// </summary>
        public const int OBJECTIVE_DISTANCE_INCREMENT = 20;

        /// <summary>
        /// Maximum number of objectives.
        /// </summary>
        public const int MAX_OBJECTIVES = 5;

        /// <summary>
        /// Maximum distance to the objective, in nautical miles.
        /// </summary>
        public const int MAX_OBJECTIVE_DISTANCE = 200;

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
            LoadFromFile(filePath);
        }

        [TreeViewProperty("Mission type", null, typeof(MissionType), "The type of mission (single player or cooperative)")]
        public MissionType MissionType { get; set; }

        [TreeViewProperty("Coalition, blue", "Context", typeof(DBEntryCoalition), "Who belongs to the blue coalition?")]
        public string ContextCoalitionBlue { get; set; }

        [TreeViewProperty("Player coalition", "Context", typeof(Coalition), "Which coalition does the player(s) belong to?")]
        public Coalition ContextCoalitionPlayer { get; set; }

        [TreeViewProperty("Coalition, red", "Context", typeof(DBEntryCoalition), "Who belongs to the red coalition?")]
        public string ContextCoalitionRed { get; set; }

        [TreeViewProperty("Time period", "Context", typeof(Decade), "During which decade will this mission take place? This value is ignored if Briefing/Mission date is set.")]
        public Decade ContextDecade { get; set; }

        [TreeViewProperty("Theater", "Context", typeof(DBEntryTheater), "DCS World theater in which the mission will take place.")]
        public string ContextTheater { get; set; }

        [TreeViewProperty("Season", "Environment", typeof(Season), "Season during which the mission will take place.")]
        public Season EnvironmentSeason { get; set; }

        [TreeViewProperty("Time of day", "Environment", typeof(TimeOfDay), "Starting time of the mission.")]
        public TimeOfDay EnvironmentTimeOfDay { get; set; }

        [TreeViewProperty("Weather", "Environment", typeof(Weather), "What the weather be like during the mission.")]
        public Weather EnvironmentWeather { get; set; }

        [TreeViewProperty("Cloud preset", "Environment", typeof(CloudPreset), "Cloud preset to use (for DCS World 2.7+ new cloud sytem)")]
        public CloudPreset EnvironmentCloudPreset { get; set; }

        [TreeViewProperty("Wind", "Environment", typeof(Wind), "How windy will the weather be during the mission. \"Auto\" means \"choose according to the weather\".")]
        public Wind EnvironmentWind { get; set; }

        [TreeViewProperty("Add extra waypoints", "Flight plan", typeof(YesNo), "Should extra ingress/egress waypoints be generated in addition to the objective waypoints?")]
        public YesNo FlightPlanAddExtraWaypoints { get; set; }

        [TreeViewProperty("Starting airbase", "Flight plan", typeof(DBPseudoEntryAirbase), "Name of the airbase the player must take off from. If left empty, or if the airbase doesn't exist in this theater, a random airbase will be selected. Be aware that if the selected airbase doesn't have enough parking spots for the player mission package, some units may not spawn properly.", TreeViewPropertyAttributeFlags.EmptyIsRandom)]
        public string FlightPlanTheaterStartingAirbase { get; set; }

        [TreeViewProperty("Objective count", "Objectives", typeof(int), "How many objectives/targets will be present in the mission.")]
        [TreeViewPropertyInt(1, MAX_OBJECTIVES)]
        public int ObjectiveCount { get; set; }

        [TreeViewProperty("Objective distance", "Objectives", typeof(int), "How far from the player's starting location will the objectives be.")]
        [TreeViewPropertyInt(0, MAX_OBJECTIVE_DISTANCE, 20, "%i nm", "Random")]
        public int ObjectiveDistance { get; set; }

        [TreeViewProperty("Objective type", "Objectives", typeof(DBEntryObjective), "The type of task player must accomplish in this mission.", TreeViewPropertyAttributeFlags.EmptyIsRandom)]
        public string ObjectiveType { get; set; }

        [TreeViewProperty("Briefing unit system", "Options", typeof(UnitSystem), "Unit system to use in the mission briefing.")]
        public UnitSystem OptionsBriefingUnitSystem { get; set; }

        [TreeViewProperty("Civilian road traffic", "Options", typeof(CivilianTraffic), "Amount of civilian traffic on the roads. Can affect performance if set too high.")]
        public CivilianTraffic OptionsCivilianTraffic { get; set; }

        [TreeViewProperty("Mission auto-ending", "Options", typeof(MissionEndMode), "When (and if) should the mission automatically end after all objectives are complete?")]
        public MissionEndMode OptionsEndMode { get; set; }

        [TreeViewProperty("Enemy units location", "Options", typeof(SpawnPointPreferredCoalition), "Can enemy units be spawned in any country (recommended) or only in countries aligned with a given coalition? Be aware that when choosing an option other than \"Any\", depending on the theater and the \"Theater region coalitions\" setting, objectives may end up VERY far from the player(s) starting location, no matter the value of \"Objective distance\". Keep in mind that \"Theaters regions coalitions\" has a influence on this setting.")]
        public SpawnPointPreferredCoalition OptionsEnemyUnitsLocation { get; set; }

        [TreeViewProperty("Audio radio messages", "Options", typeof(YesNo), "Should audio radio messages be enabled? If not, messages will only be displayed as text.")]
        public YesNo OptionsRadioSounds { get; set; }

        [TreeViewProperty("Theater countries coalitions", "Options", typeof(CountryCoalition), "To which coalitions should the countries on the map (and their airbases) belong to?")]
        public CountryCoalition OptionsTheaterCountriesCoalitions { get; set; }

        [TreeViewProperty("Air defense", "Situation, enemies", typeof(AmountN), "Intensity and quality of enemy air defense.")]
        public AmountN SituationEnemyAirDefense { get; set; }
        
        [TreeViewProperty("Air force", "Situation, enemies", typeof(AmountN), "Relative power of the enemy air force. Enemy air force will always be proportional to the number and air-to-air efficiency of aircraft in the player mission package, so more player/AI friendly aircraft means more enemy aircraft, regardless of this setting.")]
        public AmountN SituationEnemyAirForce { get; set; }
        
        [TreeViewProperty("CAP on station", "Situation, enemies", typeof(AmountN), "Chance that enemy fighter planes will already be patrolling on mission start rather than popping up during the mission on objective completion.")]
        public AmountN SituationEnemyCAPOnStationChance { get; set; }

        [TreeViewProperty("Support Aircraft", "Situation, enemies", typeof(AmountN), "Enemy forces have support aircraft such as AWACS and Tankers.")]
        public AmountN SituationEnemySupportAircraft { get; set; }
        
        [TreeViewProperty("Aircraft skill level", "Situation, enemies", typeof(BRSkillLevel), "Skill level of enemy planes and helicopters.")]
        public BRSkillLevel SituationEnemySkillLevelAir { get; set; }
        
        [TreeViewProperty("Ground forces skill level", "Situation, enemies", typeof(BRSkillLevel), "Skill level of enemy ground units and air defense.")]
        public BRSkillLevel SituationEnemySkillLevelGround { get; set; }

        [TreeViewProperty("Air defense", "Situation, friendlies", typeof(AmountN), "Intensity and quality of friendly air defense.")]
        public AmountN SituationFriendlyAirDefense { get; set; }
        
        [TreeViewProperty("Skill level", "Situation, friendlies", typeof(BRSkillLevel), "Skill level of friendly units.")]
        public BRSkillLevel SituationFriendlyAISkillLevel { get; set; }
        
        [TreeViewProperty("AI CAP escort", "Situation, friendlies", typeof(int), "Number of AI aircraft tasked with escorting the player against enemy fighters. In single-player missions, escorts will be spawned on the ramp if the player starts from the ramp (cold or hot), or in the air above the airbase if the player starts on the runway. In multiplayer missions, escorts will be spawned as soon as one player takes off.")]
        [TreeViewPropertyInt(0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE)]
        public int SituationFriendlyEscortCAP { get; set; }

        [TreeViewProperty("AI SEAD escort", "Situation, friendlies", typeof(int), "Number of AI aircraft tasked with escorting the player against enemy air defense. In single-player missions, escorts will be spawned on the ramp if the player starts from the ramp (cold or hot), or in the air above the airbase if the player starts on the runway. In multiplayer missions, escorts will be spawned as soon as one player takes off.")]
        [TreeViewPropertyInt(0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE)]
        public int SituationFriendlyEscortSEAD { get; set; }

        [TreeViewProperty("Player flight groups", null, typeof(MissionTemplateFlightGroup), "Player flight groups.")]
        public MissionTemplateFlightGroup[] PlayerFlightGroups { get; set; } = new MissionTemplateFlightGroup[0];

        [TreeViewProperty("Realism", null, typeof(RealismOption), "Realism options to apply to this mission.")]
        public RealismOption[] Realism { get; set; }

        [TreeViewProperty("Script extensions", null, typeof(DBEntryExtension), "Script extensions to include in this mission to provide additional features.")]
        public string[] ScriptExtensions { get; set; }

        [TreeViewProperty("Unit mods", null, typeof(DBEntryDCSMod), "Which unit mods should be enabled in this mission? Make sure units mods are installed and active in your version of DCS World or the units won't be spawned.")]
        public string[] UnitMods { get; set; }

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            MissionType = MissionType.SinglePlayer;

            ContextCoalitionBlue = Database.Instance.CheckValue<DBEntryCoalition>("USA");
            ContextCoalitionPlayer = Coalition.Blue;
            ContextCoalitionRed = Database.Instance.CheckValue<DBEntryCoalition>("Russia");
            ContextDecade = Decade.Decade2000;
            ContextTheater = Database.Instance.CheckValue<DBEntryTheater>("Caucasus");

            EnvironmentSeason = Season.Random;
            EnvironmentTimeOfDay = TimeOfDay.RandomDaytime;
            EnvironmentWeather = Weather.Random;
            EnvironmentCloudPreset = CloudPreset.Random;
            EnvironmentWind = Wind.Auto;

            FlightPlanAddExtraWaypoints = YesNo.No;
            FlightPlanTheaterStartingAirbase = "";

            ObjectiveCount = 2;
            ObjectiveDistance = 0;
            ObjectiveType = "";

            OptionsBriefingUnitSystem = UnitSystem.Imperial;
            OptionsCivilianTraffic = CivilianTraffic.Low;
            OptionsEndMode = MissionEndMode.Never;
            OptionsEnemyUnitsLocation = SpawnPointPreferredCoalition.Any;
            OptionsRadioSounds = YesNo.Yes;
            OptionsTheaterCountriesCoalitions = CountryCoalition.Default;

            SituationEnemyAirDefense = AmountN.Random;
            SituationEnemyAirForce = AmountN.Random;
            SituationEnemyCAPOnStationChance = AmountN.Random;
            SituationEnemySkillLevelAir = BRSkillLevel.Random;
            SituationEnemySkillLevelGround = BRSkillLevel.Random;
            SituationEnemySupportAircraft = AmountN.Random;

            SituationFriendlyAISkillLevel = BRSkillLevel.Random;
            SituationFriendlyAirDefense = AmountN.Random;
            SituationFriendlyEscortCAP = 0;
            SituationFriendlyEscortSEAD = 0;

            PlayerFlightGroups = new MissionTemplateFlightGroup[] { new MissionTemplateFlightGroup() };

            Realism = new RealismOption[] { RealismOption.DisableDCSRadioAssists, RealismOption.NoBDA };

            ScriptExtensions = new string[0];

            UnitMods = new string[0];

            CheckValues(out string[] _);
        }

        public bool CheckValues(out string[] errorMessages)
        {
            bool changesMade = false;
            List<string> errorMessagesList = new List<string>();

            if (PlayerFlightGroups.Length == 0)
            {
                PlayerFlightGroups = new MissionTemplateFlightGroup[] { new MissionTemplateFlightGroup() };
                changesMade = true;
                errorMessagesList.Add("No player flight group in template, added default flight group.");
            }

            if ((MissionType == MissionType.SinglePlayer) && (PlayerFlightGroups.Length > 1))
            {
                MissionType = MissionType.Multiplayer;
                changesMade = true;
                errorMessagesList.Add("Multiple player flight groups, mission type was changed to multiplayer.");
            }

            foreach (string msg in errorMessagesList)
                DebugLog.Instance.WriteLine(msg, DebugLogMessageErrorLevel.Warning);

            errorMessages = errorMessagesList.ToArray();
            return changesMade;
        }

        /// <summary>
        /// Loads a mission template from an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file</param>
        /// <returns></returns>
        public bool LoadFromFile(string filePath)
        {
            Clear();
            if (!File.Exists(filePath)) return false;

            using (INIFile ini = new INIFile(filePath))
            {
                MissionType = ini.GetValue("Common", "MissionType", MissionType);
                
                ContextCoalitionBlue = Database.Instance.CheckValue<DBEntryCoalition>(ini.GetValue("Context", "Coalition.Blue", ContextCoalitionBlue));
                ContextCoalitionPlayer = ini.GetValue("Context", "Coalition.Player", ContextCoalitionPlayer);
                ContextCoalitionRed = Database.Instance.CheckValue<DBEntryCoalition>(ini.GetValue("Context", "Coalition.Red", ContextCoalitionRed));
                ContextDecade = ini.GetValue("Context", "Decade", ContextDecade);
                ContextTheater = Database.Instance.CheckValue<DBEntryTheater>(ini.GetValue("Context", "Theater", ContextTheater));

                EnvironmentSeason = ini.GetValue("Environment", "Season", EnvironmentSeason);
                EnvironmentTimeOfDay = ini.GetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                EnvironmentWeather = ini.GetValue("Environment", "Weather", EnvironmentWeather);
                EnvironmentCloudPreset = ini.GetValue("Environment", "CloudPreset", EnvironmentCloudPreset);
                EnvironmentWind = ini.GetValue("Environment", "Wind", EnvironmentWind);

                FlightPlanAddExtraWaypoints = ini.GetValue("FlightPlan", "ExtraWaypoints", FlightPlanAddExtraWaypoints);
                FlightPlanTheaterStartingAirbase = ini.GetValue("FlightPlan", "TheaterStartingAirbase", FlightPlanTheaterStartingAirbase);

                ObjectiveCount = Toolbox.Clamp(ini.GetValue("Objective", "Count", ObjectiveCount), 1, MAX_OBJECTIVES);
                ObjectiveDistance = Toolbox.Clamp(ini.GetValue("Objective", "Distance", ObjectiveDistance), 0, MAX_OBJECTIVE_DISTANCE);
                ObjectiveType = Database.Instance.CheckValue<DBEntryObjective>(ini.GetValue("Objective", "Type", ObjectiveType), "", true);

                OptionsBriefingUnitSystem = ini.GetValue("Options", "BriefingUnitSystem", OptionsBriefingUnitSystem);
                OptionsCivilianTraffic = ini.GetValue("Options", "CivilianTraffic", OptionsCivilianTraffic);
                OptionsEndMode = ini.GetValue("Options", "EndMode", OptionsEndMode);
                OptionsEnemyUnitsLocation = ini.GetValue("Options", "EnemyUnitsLocation", OptionsEnemyUnitsLocation);
                OptionsRadioSounds = ini.GetValue("Options", "RadioSounds", OptionsRadioSounds);
                OptionsTheaterCountriesCoalitions = ini.GetValue("Options", "TheaterCountriesCoalitions", OptionsTheaterCountriesCoalitions);

                SituationEnemyAirDefense = ini.GetValue("SituationEnemy", "AirDefense", SituationEnemyAirDefense);
                SituationEnemyAirForce = ini.GetValue("SituationEnemy", "AirForce", SituationEnemyAirForce);
                SituationEnemyCAPOnStationChance = ini.GetValue("SituationEnemy", "CAPOnStationChance", SituationEnemyCAPOnStationChance);
                SituationEnemySkillLevelAir = ini.GetValue("SituationEnemy", "SkillLevelAir", SituationEnemySkillLevelAir);
                SituationEnemySkillLevelGround = ini.GetValue("SituationEnemy", "SkillLevelGround", SituationEnemySkillLevelGround);
                SituationEnemySupportAircraft = ini.GetValue("SituationEnemy", "SupportAircraft", SituationEnemySupportAircraft);

                SituationFriendlyAISkillLevel = ini.GetValue("SituationFriendly", "AISkillLevel", SituationFriendlyAISkillLevel);
                SituationFriendlyAirDefense = ini.GetValue("SituationFriendly", "AirDefense", SituationFriendlyAirDefense);
                SituationFriendlyEscortCAP = Toolbox.Clamp(ini.GetValue("SituationFriendly", "EscortCAP", SituationFriendlyEscortCAP), 0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
                SituationFriendlyEscortSEAD = Toolbox.Clamp(ini.GetValue("SituationFriendly", "EscortSEAD", SituationFriendlyEscortSEAD), 0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);

                int fgFlightGroupCount = Math.Max(0, ini.GetValue<int>("PlayerFlightGroups", "Count"));
                PlayerFlightGroups = new MissionTemplateFlightGroup[fgFlightGroupCount];
                for (int i = 0; i < fgFlightGroupCount; i++)
                    PlayerFlightGroups[i] = new MissionTemplateFlightGroup(ini, "PlayerFlightGroups", $"FG{i:000}");
                if (PlayerFlightGroups.Length == 0)
                    PlayerFlightGroups = new MissionTemplateFlightGroup[] { new MissionTemplateFlightGroup() };

                Realism = ini.GetValueArray<RealismOption>("Realism", "Realism").Distinct().ToArray();

                ScriptExtensions = ini.GetValueArray<string>("ScriptExtensions", "ScriptExtensions").Distinct().ToArray();

                UnitMods = ini.GetValueArray<string>("UnitMods", "UnitMods").Distinct().ToArray();
            }

            CheckValues(out string[] _);

            return true;
        }

        /// <summary>
        /// Save the mission template to an .ini file.
        /// </summary>
        /// <param name="filePath">Path to the .ini file.</param>
        public void SaveToFile(string filePath)
        {
            using (INIFile ini = new INIFile())
            {
                ini.SetValue("Common", "MissionType", MissionType);

                ini.SetValue("Context", "Coalition.Blue", ContextCoalitionBlue);
                ini.SetValue("Context", "Coalition.Player", ContextCoalitionPlayer);
                ini.SetValue("Context", "Coalition.Red", ContextCoalitionRed);
                ini.SetValue("Context", "Decade", ContextDecade);
                ini.SetValue("Context", "Theater", ContextTheater);

                ini.SetValue("Environment", "Season", EnvironmentSeason);
                ini.SetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                ini.SetValue("Environment", "Weather", EnvironmentWeather);
                ini.SetValue("Environment", "CloudPreset", EnvironmentCloudPreset);
                ini.SetValue("Environment", "Wind", EnvironmentWind);

                ini.SetValue("FlightPlan", "ExtraWaypoints", FlightPlanAddExtraWaypoints);
                ini.SetValue("FlightPlan", "TheaterStartingAirbase", FlightPlanTheaterStartingAirbase);

                ini.SetValue("Objective", "Count", ObjectiveCount);
                ini.SetValue("Objective", "Distance", ObjectiveDistance);
                ini.SetValue("Objective", "Type", ObjectiveType);

                ini.SetValue("Options", "BriefingUnitSystem", OptionsBriefingUnitSystem);
                ini.SetValue("Options", "CivilianTraffic", OptionsCivilianTraffic);
                ini.SetValue("Options", "EndMode", OptionsEndMode);
                ini.SetValue("Options", "EnemyUnitsLocation", OptionsEnemyUnitsLocation);
                ini.SetValue("Options", "RadioSounds", OptionsRadioSounds);
                ini.SetValue("Options", "TheaterCountriesCoalitions", OptionsTheaterCountriesCoalitions);

                ini.SetValue("SituationEnemy", "AirDefense", SituationEnemyAirDefense);
                ini.SetValue("SituationEnemy", "AirForce", SituationEnemyAirForce);
                ini.SetValue("SituationEnemy", "CAPOnStationChance", SituationEnemyCAPOnStationChance);
                ini.SetValue("SituationEnemy", "SkillLevelAir", SituationEnemySkillLevelAir);
                ini.SetValue("SituationEnemy", "SkillLevelGround", SituationEnemySkillLevelGround);
                ini.SetValue("SituationEnemy", "SupportAircraft", SituationEnemySupportAircraft);

                ini.SetValue("SituationFriendly", "AISkillLevel", SituationFriendlyAISkillLevel);
                ini.SetValue("SituationFriendly", "AirDefense", SituationFriendlyAirDefense);
                ini.SetValue("SituationFriendly", "EscortCAP", SituationFriendlyEscortCAP);
                ini.SetValue("SituationFriendly", "EscortSEAD", SituationFriendlyEscortSEAD);

                ini.SetValue("PlayerFlightGroups", "Count", PlayerFlightGroups.Length);

                for (int i = 0; i < PlayerFlightGroups.Length; i++)
                    PlayerFlightGroups[i].SaveToFile(ini, "PlayerFlightGroups", $"FG{i:000}");

                ini.SetValueArray("Realism", "Realism", Realism);

                ini.SetValueArray("ScriptExtensions", "ScriptExtensions", ScriptExtensions);

                ini.SetValueArray("UnitMods", "UnitMods", UnitMods);

                ini.SaveToFile(filePath);
            }
        }

        /// <summary>
        /// "Shortcut" method to get <see cref="ContextCoalitionBlue"/> or <see cref="ContextCoalitionRed"/> by using a <see cref="Coalition"/> parameter.
        /// </summary>
        /// <param name="coalition">Color of the coalition to return</param>
        /// <returns><see cref="ContextCoalitionBlue"/> or <see cref="ContextCoalitionRed"/></returns>
        public string GetCoalition(Coalition coalition)
        {
            if (coalition == Coalition.Red) return ContextCoalitionRed;
            return ContextCoalitionBlue;
        }

        /// <summary>
        /// Returns the total number of player-controllable aircraft in the mission.
        /// </summary>
        /// <returns>The number of player-controllable aircraft</returns>
        public int GetPlayerCount()
        {
            if (MissionType == MissionType.SinglePlayer) return 1;

            return (from MissionTemplateFlightGroup pfg in PlayerFlightGroups select pfg.Count).Sum();
        }

        /// <summary>
        /// Returns the total number of parking spots required for the misison package aircraft.
        /// </summary>
        /// <returns>Number of parking spots required</returns>
        public int GetMissionPackageRequiredParkingSpots()
        {
            if (MissionType == MissionType.SinglePlayer)
            {
                if (PlayerFlightGroups[0].StartLocation == PlayerStartLocation.Runway) return 0; // Player and wingmen start on the runway, AI escort start in air above the airbase
                return PlayerFlightGroups[0].Count + SituationFriendlyEscortCAP + SituationFriendlyEscortSEAD;
            }

            return GetPlayerCount(); // AI escorts start in the air
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
