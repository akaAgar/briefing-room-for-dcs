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
using BriefingRoom4DCSWorld.TypeConverters;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
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

        /// <summary>
        /// The exact date at which the mission should take place.
        /// If enabled, the <see cref="EnvironmentSeason"/> setting will be ignored.
        /// If not enabled, a random year will be selected according to the selected coalitions.
        /// </summary>
        [Category("Briefing"), DisplayName("Mission date")]
        [Description("The exact date at which the mission should take place. If enabled, the \"Season\" setting from the \"Environment\" category will be ignored. If not enabled, a random year will be selected according to the selected coalitions.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MissionTemplateDate BriefingDate { get; set; }

        /// <summary>
        /// Description of the mission to display in the briefing. If left empty, a random description will be generated.
        /// </summary>
        [Category("Briefing"), DisplayName("Mission description")]
        [Description("Description of the mission to display in the briefing. If left empty, a random description will be generated.")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string BriefingDescription { get { return BriefingDescription_; } set { BriefingDescription_ = value.Trim(); } }
        private string BriefingDescription_;

        /// <summary>
        /// Name/title of the mission. If left empty, a random name will be generated.
        /// </summary>
        [Category("Briefing"), DisplayName("Mission name")]
        [Description("Name/title of the mission. If left empty, a random name will be generated.")]
        public string BriefingName { get { return BriefingName_; } set { BriefingName_ = value.Trim(); } }
        private string BriefingName_;

        /// <summary>
        /// Who belongs to the blue coalition?
        /// </summary>
        [Category("Coalitions"), DisplayName("Coalition, blue")]
        [Description("Who belongs to the blue coalition?")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string CoalitionBlue { get { return CoalitionBlue_; } set { CoalitionBlue_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string CoalitionBlue_;

        /// <summary>
        /// Which coalition does the player(s) belong to?
        /// </summary>
        [Category("Coalitions"), DisplayName("Player coalition")]
        [Description("Which coalition does the player(s) belong to?")]
        [TypeConverter(typeof(EnumTypeConverter<Coalition>))]
        public Coalition CoalitionPlayer { get; set; }

        /// <summary>
        /// Who belongs to the red coalition?
        /// </summary>
        [Category("Coalitions"), DisplayName("Coalition, red")]
        [Description("Who belongs to the red coalition?")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryCoalition>))]
        public string CoalitionRed { get { return CoalitionRed_; } set { CoalitionRed_ = TemplateTools.CheckValue<DBEntryCoalition>(value); } }
        private string CoalitionRed_;

        /// <summary>
        /// Season during which the mission will take place.
        /// </summary>
        [Category("Environment"), DisplayName("Season")]
        [Description("Season during which the mission will take place.")]
        [TypeConverter(typeof(EnumTypeConverter<Season>))]
        public Season EnvironmentSeason { get; set; }

        /// <summary>
        /// Starting time of the mission.
        /// </summary>
        [Category("Environment"), DisplayName("Time of day")]
        [Description("Starting time of the mission.")]
        [TypeConverter(typeof(EnumTypeConverter<TimeOfDay>))]
        public TimeOfDay EnvironmentTimeOfDay { get; set; }

        /// <summary>
        /// What the weather be like during the mission.
        /// </summary>
        [Category("Environment"), DisplayName("Weather")]
        [Description("What the weather be like during the mission.")]
        [TypeConverter(typeof(EnumTypeConverter<Weather>))]
        public Weather EnvironmentWeather { get; set; }

        /// <summary>
        /// How windy will the weather be during the mission. "Auto" means "choose according to <see cref="EnvironmentWeather"/>".
        /// </summary>
        [Category("Environment"), DisplayName("Wind")]
        [Description("How windy will the weather be during the mission. \"Auto\" means \"choose according to the weather\".")]
        [TypeConverter(typeof(EnumTypeConverter<Wind>))]
        public Wind EnvironmentWind { get; set; }

        /// <summary>
        /// How many objectives/targets will be present in the mission.
        /// </summary>
        [Category("Objectives"), DisplayName("Objective count")]
        [Description("How many objectives/targets will be present in the mission.")]
        public int ObjectiveCount { get { return ObjectiveCount_; } set { ObjectiveCount_ = Toolbox.Clamp(value, 1, TemplateTools.MAX_OBJECTIVES); } }
        private int ObjectiveCount_;

        /// <summary>
        /// How far from the player's starting location will the objectives be.
        /// </summary>
        [Category("Objectives"), DisplayName("Objective distance")]
        [Description("How far from the player's starting location will the objectives be.")]
        [TypeConverter(typeof(EnumTypeConverter<Amount>))]
        public Amount ObjectiveDistance { get; set; }

        /// <summary>
        /// The type of task player must accomplish in this mission.
        /// </summary>
        [Category("Objectives"), DisplayName("Objective type")]
        [Description("The type of task player must accomplish in this mission.")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryObjective>))]
        public string ObjectiveType { get { return ObjectiveType_; } set { ObjectiveType_ = TemplateTools.CheckValue<DBEntryObjective>(value); } }
        private string ObjectiveType_;

        /// <summary>
        /// Intensity and quality of enemy air defense.
        /// </summary>
        [Category("Opposition"), DisplayName("Air defense")]
        [Description("Intensity and quality of enemy air defense.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN OppositionAirDefense { get; set; }

        /// <summary>
        /// Relative power of the enemy air force.
        /// Enemy air force will always be proportional to the number and air-to-air efficiency of aircraft in the player mission package,
        /// so more player/AI friendly aircraft means more enemy aircraft, regardless of this setting.
        /// </summary>
        [Category("Opposition"), DisplayName("Air force")]
        [Description("Relative power of the enemy air force. Enemy air force will always be proportional to the number and air-to-air efficiency of aircraft in the player mission package, so more player/AI friendly aircraft means more enemy aircraft, regardless of this setting.")]
        [TypeConverter(typeof(EnumTypeConverter<AmountN>))]
        public AmountN OppositionAirForce { get; set; }

        /// <summary>
        /// Can enemy units be spawned in any country (recommended) or only in countries aligned with a
        /// given coalition? Be aware that when choosing an option other than "Any", depending on the theater and the
        /// <see cref="TheaterRegionsCoalitions"/> setting, objectives may end up VERY far from the player(s) starting 
        /// location, no matter the value of <see cref="ObjectiveDistance"/>.
        /// Keep in mind that <see cref="TheaterRegionsCoalitions"/> has a influence on this setting.
        /// </summary>
        [Category("Opposition"), DisplayName("Enemy units location")]
        [Description("Can enemy units be spawned in any country (recommended) or only in countries aligned with a given coalition? Be aware that when choosing an option other than \"Any\", depending on the theater and the \"Theater region coalitions\" setting, objectives may end up VERY far from the player(s) starting location, no matter the value of \"Objective distance\". Keep in mind that \"Theaters regions coalitions\" has a influence on this setting.")]
        [TypeConverter(typeof(EnumTypeConverter<SpawnPointPreferredCoalition>))]
        public SpawnPointPreferredCoalition OppositionUnitsLocation { get; set; }

        ///// <summary>
        ///// Special preferences and options to apply to this mission.
        ///// </summary>
        //[Category("Options"), DisplayName("Preferences")]
        //[Description("Special preferences and options to apply to this mission.")]
        //[TypeConverter(typeof(EnumArrayTypeConverter<MissionTemplatePreferences>))]
        //[Editor(typeof(CheckedListBoxUIEditorEnum<MissionTemplatePreferences>), typeof(UITypeEditor))]
        [Browsable(false)]
        public MissionTemplatePreferences[] OptionsPreferences { get; set; }

        /// <summary>
        /// Script extensions to include in this mission to provide additional features.
        /// </summary>
        [Category("Options"), DisplayName("Script extensions")]
        [Description("Script extensions to include in this mission to provide additional features.")]
        [TypeConverter(typeof(StringArrayTypeConverter))]
        [Editor(typeof(CheckedListBoxUIEditorDBEntry<DBEntryExtension>), typeof(UITypeEditor))]
        public string[] OptionsScriptExtensions { get; set; }

        /// <summary>
        /// Should enemy units be shown on the F10 map, mission planning, MFD SA pages, etc?
        /// </summary>
        [Category("Options"), DisplayName("Show enemy units on map")]
        [Description("Should enemy units be shown on the F10 map, mission planning, MFD SA pages, etc?")]
        [TypeConverter(typeof(BooleanYesNoTypeConverter))]
        public bool OptionsShowEnemyUnits { get; set; }

        /// <summary>
        /// Multiplayer flight groups.
        /// If any flight group is specified here, the mission then becomes a multiplayer mission and all values
        /// in the "Player, single player only" are ignored.
        /// </summary>
        [Category("Player, multiplayer only"), DisplayName("MP flight groups")]
        [Description("Multiplayer flight groups. If any flight group is specified here, the mission then becomes a multiplayer mission and all values in the \"Player, single player only\" are ignored.")]
        [TypeConverter(typeof(MissionTemplateFlightGroupConverter))]
        [Editor(typeof(DescriptionArrayEditor), typeof(UITypeEditor))]
        public MissionTemplateMPFlightGroup[] PlayerMPFlightGroups { get; set; } = new MissionTemplateMPFlightGroup[0];

        /// <summary>
        /// Type of aircraft the player will fly.
        /// As with all values in the "Player, single player only" category, this value is ignored if any
        /// flight group is specified in <see cref="PlayerMPFlightGroups" />, the multiplayer flight groups
        /// are then used instead.
        /// </summary>
        [Category("Player, single player only"), DisplayName("Aircraft")]
        [Description("Type of aircraft the player will fly. As with all values in the \"Player, single player only\" category, this value is ignored if any flight group is specified in \"MP flight groups\", the multiplayer flight groups are then used instead.")]
        [TypeConverter(typeof(DBEntryPlayerAircraftTypeConverter))]
        public string PlayerSPAircraft { get { return PlayerSPAircraft_; } set { PlayerSPAircraft_ = TemplateTools.CheckValuePlayerAircraft(value); } }
        private string PlayerSPAircraft_;

        /// <summary>
        /// Number of AI aircraft tasked with escorting the player against enemy fighters.
        /// As with all values in the "Player, single player only" category, this value is ignored if any
        /// flight group is specified in <see cref="PlayerMPFlightGroups" />, the multiplayer flight groups
        /// are then used instead.
        /// </summary>
        [Category("Friendly AI"), DisplayName("Escort aircraft, CAP")]
        [Description("Number of AI aircraft tasked with escorting the player against enemy fighters.")]
        public int AIEscortCAP { get { return AIEscortCAP_; } set { AIEscortCAP_ = Toolbox.Clamp(value, 0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int AIEscortCAP_;

        /// <summary>
        /// Number of AI aircraft tasked with escorting the player against enemy SAMs.
        /// As with all values in the "Player, single player only" category, this value is ignored if any
        /// flight group is specified in <see cref="PlayerMPFlightGroups" />, the multiplayer flight groups
        /// are then used instead.
        /// </summary>
        [Category("Friendly AI"), DisplayName("Escort aircraft, SEAD")]
        [Description("Number of AI aircraft tasked with escorting the player against enemy SAMs.")]
        public int AIEscortSEAD { get { return AIEscortSEAD_; } set { AIEscortSEAD_ = Toolbox.Clamp(value, 0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE); } }
        private int AIEscortSEAD_;

        /// <summary>
        /// Number of AI wingmen in the player's flight group.
        /// As with all values in the "Player, single player only" category, this value is ignored if any
        /// flight group is specified in <see cref="PlayerMPFlightGroups" />, the multiplayer flight groups
        /// are then used instead.
        /// </summary>
        [Category("Player, single player only"), DisplayName("Wingmen")]
        [Description("Number of AI wingmen in the player's flight group. As with all values in the \"Player, single player only\" category, this value is ignored if any flight group is specified in \"MP flight groups\", the multiplayer flight groups are then used instead.")]
        public int PlayerSPWingmen { get { return PlayerSPWingmen_; } set { PlayerSPWingmen_ = Toolbox.Clamp(value, 0, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE - 1); } }
        private int PlayerSPWingmen_;

        /// <summary>
        /// Where should the player take off from?
        /// As with all values in the "Player, single player only" category, this value is ignored if any
        /// flight group is specified in <see cref="PlayerMPFlightGroups" />, the multiplayer flight groups
        /// are then used instead.
        /// </summary>
        [Category("Player"), DisplayName("Start location")]
        [Description("Where should the player take off from? As with all values in the \"Player, single player only\" category, this value is ignored if any flight group is specified in \"MP flight groups\", the multiplayer flight groups are then used instead.")]
        [TypeConverter(typeof(EnumTypeConverter<PlayerStartLocation>))]
        public PlayerStartLocation PlayerStartLocation { get; set; }

        /// <summary>
        /// DCS World theater in which the mission will take place.
        /// </summary>
        [Category("Theater"), DisplayName("Theater ID")]
        [Description("DCS World theater in which the mission will take place.")]
        [TypeConverter(typeof(DBEntryTypeConverter<DBEntryTheater>))]
        public string TheaterID { get { return TheaterID_; } set { TheaterID_ = TemplateTools.CheckValue<DBEntryTheater>(value); } }
        private string TheaterID_;

        /// <summary>
        /// To which coalitions should the countries on the map (and their airbases) belong to?
        /// </summary>
        [Category("Theater"), DisplayName("Theater regions coalitions")]
        [Description("To which coalitions should the countries on the map (and their airbases) belong to?")]
        [TypeConverter(typeof(EnumTypeConverter<CountryCoalition>))]
        public CountryCoalition TheaterRegionsCoalitions { get; set; }

        /// <summary>
        /// Name of the airbase the player must take off from.
        /// If left empty, or if the airbase doesn't exist in this theater, a random airbase will be selected.
        /// </summary>
        [Category("Theater"), DisplayName("Theater starting airbase")]
        [Description("Name of the airbase the player must take off from. If left empty, or if the airbase doesn't exist in this theater, a random airbase will be selected. Be aware that if the selected airbase doesn't have enough parking spots for the player mission package, some units may not spawn properly.")]
        [TypeConverter(typeof(DBEntryTheaterAirbaseArrayTypeConverter))]
        public string TheaterStartingAirbase { get { return TheaterStartingAirbase_; } set { TheaterStartingAirbase_ = TemplateTools.CheckValueTheaterStartingAirbase(value); } }
        private string TheaterStartingAirbase_;

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public void Clear()
        {
            BriefingDate = new MissionTemplateDate();
            BriefingDescription = "";
            BriefingName = "";

            CoalitionBlue = TemplateTools.CheckValue<DBEntryCoalition>(Database.Instance.Common.DefaultCoalitionBlue);
            CoalitionPlayer = Coalition.Blue;
            CoalitionRed = TemplateTools.CheckValue<DBEntryCoalition>(Database.Instance.Common.DefaultCoalitionRed);

            EnvironmentSeason = Season.Random;
            EnvironmentTimeOfDay = TimeOfDay.RandomDaytime;
            EnvironmentWeather = Weather.Random;
            EnvironmentWind = Wind.Auto;

            ObjectiveCount = Database.Instance.Common.DefaultObjectiveCount;
            ObjectiveDistance = Amount.Average;
            ObjectiveType = TemplateTools.CheckValue<DBEntryObjective>(Database.Instance.Common.DefaultObjective);

            OppositionAirDefense = AmountN.Average;
            OppositionAirForce = AmountN.Average;
            OppositionUnitsLocation = SpawnPointPreferredCoalition.Any;

            OptionsPreferences = new MissionTemplatePreferences[0];
            OptionsScriptExtensions = new string[0];
            OptionsShowEnemyUnits = true;

            PlayerStartLocation = PlayerStartLocation.Runway;
            PlayerMPFlightGroups = new MissionTemplateMPFlightGroup[0];
            PlayerSPAircraft = TemplateTools.CheckValuePlayerAircraft(Database.Instance.Common.DefaultPlayerAircraft);
            AIEscortCAP = 0;
            AIEscortSEAD = 0;
            PlayerSPWingmen = 1;

            TheaterID = TemplateTools.CheckValue<DBEntryTheater>(Database.Instance.Common.DefaultTheater);
            TheaterRegionsCoalitions = CountryCoalition.Default;
            TheaterStartingAirbase = "";
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
                BriefingDate.LoadFromFile(ini);
                BriefingDescription = ini.GetValue("Briefing", "Description", BriefingDescription);
                BriefingName = ini.GetValue("Briefing", "Name", BriefingName);

                CoalitionBlue = ini.GetValue("Coalition", "Blue", CoalitionBlue);
                CoalitionPlayer = ini.GetValue("Coalition", "Player", CoalitionPlayer);
                CoalitionRed = ini.GetValue("Coalition", "Red", CoalitionRed);

                EnvironmentSeason = ini.GetValue("Environment", "Season", EnvironmentSeason);
                EnvironmentTimeOfDay = ini.GetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                EnvironmentWeather = ini.GetValue("Environment", "Weather", EnvironmentWeather);
                EnvironmentWind = ini.GetValue("Environment", "Wind", EnvironmentWind);

                ObjectiveCount = ini.GetValue("Objective", "Count", ObjectiveCount);
                ObjectiveDistance = ini.GetValue("Objective", "Distance", ObjectiveDistance);
                ObjectiveType = ini.GetValue("Objective", "Type", ObjectiveType);

                OppositionAirDefense = ini.GetValue("Opposition", "AirDefense", OppositionAirDefense);
                OppositionAirForce = ini.GetValue("Opposition", "AirForce", OppositionAirForce);
                OppositionUnitsLocation = ini.GetValue("Opposition", "UnitsLocation", OppositionUnitsLocation);

                OptionsPreferences = ini.GetValueArray<MissionTemplatePreferences>("Options", "Preferences");
                OptionsScriptExtensions = ini.GetValueArray<string>("Options", "ScriptExtensions");
                OptionsShowEnemyUnits = ini.GetValue("Options", "ShowEnemyUnits", OptionsShowEnemyUnits);

                PlayerStartLocation = ini.GetValue("Player", "StartLocation", PlayerStartLocation);

                int fgFlightGroupCount = Math.Max(0, ini.GetValue<int>("PlayerMP", "FGCount"));
                PlayerMPFlightGroups = new MissionTemplateMPFlightGroup[fgFlightGroupCount];
                for (int i = 0; i < fgFlightGroupCount; i++)
                    PlayerMPFlightGroups[i] = new MissionTemplateMPFlightGroup(ini, "PlayerMP", $"FG{i:000}");

                PlayerSPAircraft = ini.GetValue("PlayerSP", "Aircraft", PlayerSPAircraft);
                AIEscortCAP = ini.GetValue("PlayerSP", "EscortCAP", AIEscortCAP);
                AIEscortSEAD = ini.GetValue("PlayerSP", "EscortSEAD", AIEscortSEAD);
                PlayerSPWingmen = ini.GetValue("PlayerSP", "Wingmen", PlayerSPWingmen);

                TheaterID = ini.GetValue("Theater", "ID", TheaterID);
                TheaterRegionsCoalitions = ini.GetValue("Theater", "RegionsCoalitions", TheaterRegionsCoalitions);
                TheaterStartingAirbase = ini.GetValue("Theater", "StartingAirbase", TheaterStartingAirbase);
            }

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
                BriefingDate.SaveToFile(ini);
                ini.SetValue("Briefing", "Description", BriefingDescription);
                ini.SetValue("Briefing", "Name", BriefingName);

                ini.SetValue("Coalition", "Blue", CoalitionBlue);
                ini.SetValue("Coalition", "Player", CoalitionPlayer);
                ini.SetValue("Coalition", "Red", CoalitionRed);

                ini.SetValue("Environment", "Season", EnvironmentSeason);
                ini.SetValue("Environment", "TimeOfDay", EnvironmentTimeOfDay);
                ini.SetValue("Environment", "Weather", EnvironmentWeather);
                ini.SetValue("Environment", "Wind", EnvironmentWind);

                ini.SetValue("Objective", "Count", ObjectiveCount);
                ini.SetValue("Objective", "Distance", ObjectiveDistance);
                ini.SetValue("Objective", "Type", ObjectiveType);

                ini.SetValue("Opposition", "AirDefense", OppositionAirDefense);
                ini.SetValue("Opposition", "AirForce", OppositionAirForce);
                ini.SetValue("Opposition", "UnitsLocation", OppositionUnitsLocation);

                ini.SetValueArray("Options", "Preferences", OptionsPreferences);
                ini.SetValueArray("Options", "ScriptExtensions", OptionsScriptExtensions);
                ini.SetValue("Options", "ShowEnemyUnits", OptionsShowEnemyUnits);

                ini.SetValue("Player", "StartLocation", PlayerStartLocation);
                ini.SetValue("Player", "EscortCAP", AIEscortCAP);
                ini.SetValue("Player", "EscortSEAD", AIEscortSEAD);

                ini.SetValue("PlayerSP", "Aircraft", PlayerSPAircraft);
                ini.SetValue("PlayerSP", "Wingmen", PlayerSPWingmen);

                ini.SetValue("PlayerMP", "FGCount", PlayerMPFlightGroups.Length);
                for (int i = 0; i < PlayerMPFlightGroups.Length; i++)
                    PlayerMPFlightGroups[i].SaveToFile(ini, "PlayerMP", $"FG{i:000}");

                ini.SetValue("Theater", "ID", TheaterID);
                ini.SetValue("Theater", "RegionsCoalitions", TheaterRegionsCoalitions);
                ini.SetValue("Theater", "StartingAirbase", TheaterStartingAirbase);

                ini.SaveToFile(filePath);
            }
        }

        /// <summary>
        /// "Shortcut" method to get <see cref="CoalitionBlue"/> or <see cref="CoalitionRed"/> by using a <see cref="Coalition"/> parameter.
        /// </summary>
        /// <param name="coalition">Color of the coalition to return</param>
        /// <returns><see cref="CoalitionBlue"/> or <see cref="CoalitionRed"/></returns>
        public string GetCoalition(Coalition coalition)
        {
            if (coalition == Coalition.Red) return CoalitionRed;
            return CoalitionBlue;
        }

        /// <summary>
        /// Returns the total number of player-controllable aircraft in the mission.
        /// </summary>
        /// <returns>The number of player-controllable aircraft</returns>
        public int GetPlayerCount()
        {
            if (GetMissionType() == MissionType.SinglePlayer) return 1;

            return (from MissionTemplateMPFlightGroup pfg in PlayerMPFlightGroups select pfg.Count).Sum();
        }

        /// <summary>
        /// Returns the type of mission (single player, cooperative...) this template will generate.
        /// </summary>
        /// <returns>The type of mission</returns>
        public MissionType GetMissionType()
        {
            if (PlayerMPFlightGroups.Length == 0)
                return MissionType.SinglePlayer;

            return MissionType.Cooperative;
        }

        /// <summary>
        /// Returns the total number of parking spots required for the misison package aircraft.
        /// </summary>
        /// <returns>Number of parking spots required</returns>
        public int GetMissionPackageRequiredParkingSpots()
        {
            if (GetMissionType() == MissionType.SinglePlayer)
            {
                if (PlayerStartLocation == PlayerStartLocation.Runway) return 0; // Player and wingmen start on the runway, AI escort start in air above the airbase
                return PlayerSPWingmen_ + 1 + AIEscortCAP_ + AIEscortSEAD_;
            }

            return GetPlayerCount() + AIEscortCAP_ + AIEscortSEAD_;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
