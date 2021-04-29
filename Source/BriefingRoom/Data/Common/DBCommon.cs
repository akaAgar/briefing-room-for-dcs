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


using System;
using System.IO;

namespace BriefingRoom4DCS.Data
{
    /// <summary>
    /// Stores miscellaneous shared settings for mission generation.
    /// </summary>
    internal class DatabaseCommon : IDisposable
    {
        /// <summary>
        /// Maximum number of name parts to use in random mission names.
        /// </summary>
        internal const int MISSION_NAMES_PART_COUNT = 4;

        /// <summary>
        /// Ogg files to include in every mission.
        /// </summary>
        internal string[] CommonOGG { get; private set; }

        /// <summary>
        /// Ogg files to include in every mission of a given game mode.
        /// </summary>
        internal string[][] CommonOGGForGameMode { get; private set; }

        /// <summary>
        /// Settings for enemy air defense, according to mission template <see cref="Template.MissionTemplate.SituationEnemyAirDefense"/> setting.
        /// </summary>
        internal DBCommonAirDefenseInfo[] EnemyAirDefense { get; }

        /// <summary>
        /// Minimum distance (in nautical miles) between players take off location and enemy surface-to-air defense, for each air defense range category.
        /// </summary>
        internal int[] EnemyAirDefenseDistanceFromTakeOffLocation { get; }

        /// <summary>
        /// Min/max distance (in nautical miles) between objectives and enemy surface-to-air defense, for each air defense range category.
        /// </summary>
        internal MinMaxD[] EnemyAirDefenseDistanceFromObjectives { get; }

        /// <summary>
        /// Min/max distance between enemy CAP and the mission objectives.
        /// </summary>
        internal MinMaxD EnemyCAPDistanceFromObjectives { get; private set; }

        /// <summary>
        /// Min distance (in nautical miles) between enemy CAP and players take off location.
        /// </summary>
        internal int EnemyCAPMinDistanceFromTakeOffLocation { get; private set; }

        /// <summary>
        /// Relative power (percentage) of enemy CAP relative to the players' flight package.
        /// </summary>
        internal double[] EnemyCAPRelativePower { get; }

        /// <summary>
        /// Settings for enemy air defense, according to mission template <see cref="Template.MissionTemplate.SituationFriendlyAirDefense"/> setting.
        /// </summary>
        internal DBCommonAirDefenseInfo[] AllyAirDefense { get; }

        /// <summary>
        /// Minimum distance (in nautical miles) between players take off location and ally surface-to-air defense, for each air defense range category.
        /// </summary>
        internal MinMaxD[] AllyAirDefenseDistanceFromTakeOffLocation { get; }

        /// <summary>
        /// Min/max distance (in nautical miles) between objectives and ally surface-to-air defense, for each air defense range category.
        /// </summary>
        internal int[] AllyAirDefenseDistanceFromObjectives { get; }

        /// <summary>
        /// Random mission names part.
        /// </summary>
        internal string[][] MissionNameParts { get; } = new string[MISSION_NAMES_PART_COUNT][];

        /// <summary>
        /// Random mission name template, where $P1$, $P2$, $P3$, $P4$ is remplaced with a random mission name part.
        /// </summary>
        internal string MissionNameTemplate { get; private set; }

        /// <summary>
        /// Name (singular -index #0- and plural -index #1) to display in the briefings for each unit family.
        /// </summary>
        internal string[][] UnitBriefingNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()][];

        /// <summary>
        /// Random-parsable (<see cref="Generator.GeneratorTools.ParseRandomString(string)"/>) string for unit group names of each <see cref="UnitFamily"/>.
        /// </summary>
        internal string[] UnitGroupNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()];

        /// <summary>
        /// Name of the final (landing) player waypoint.
        /// </summary>
        internal string WPNameFinal { get; private set; }

        /// <summary>
        /// Name of the initial (takeoff) player waypoint.
        /// </summary>
        internal string WPNameInitial { get; private set; }

        /// <summary>
        /// Name of the navigation player waypoints, where $0$, $00$, $000$... is replaced with the waypoint number.
        /// </summary>
        internal string WPNameNavigation { get; private set; }

        /// <summary>
        /// Names to use for objectives and objective waypoints.
        /// </summary>
        internal string[] WPNamesObjectives { get; private set; }

        //internal DBEntryWeather[] Weather { get; private set; }

        internal DBCommonWind[] Wind { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DatabaseCommon()
        {
            EnemyAirDefense = new DBCommonAirDefenseInfo[Toolbox.EnumCount<AmountNR>()];
            EnemyAirDefenseDistanceFromObjectives = new MinMaxD[Toolbox.EnumCount<AirDefenseRange>()];
            EnemyAirDefenseDistanceFromTakeOffLocation = new int[Toolbox.EnumCount<AirDefenseRange>()];
            AllyAirDefense = new DBCommonAirDefenseInfo[Toolbox.EnumCount<AmountNR>()];
            AllyAirDefenseDistanceFromObjectives = new int[Toolbox.EnumCount<AirDefenseRange>()]; 
            AllyAirDefenseDistanceFromTakeOffLocation = new MinMaxD[Toolbox.EnumCount<AirDefenseRange>()];
            EnemyCAPRelativePower = new double[Toolbox.EnumCount<AmountNR>()];
        }

        /// <summary>
        /// Loads common settings from <see cref="COMMON_SETTINGS_FILE"/>
        /// </summary>
        internal void Load()
        {
            int i;

            BriefingRoom.PrintToLog("Loading common global settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
            {
                CommonOGG = ini.GetValueArray<string>("Include", "CommonOgg");
                foreach (string f in CommonOGG)
                    if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                        BriefingRoom.PrintToLog($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", LogMessageErrorLevel.Warning);

                CommonOGGForGameMode = new string[Toolbox.EnumCount<MissionType>()][];
                for (i = 0; i < CommonOGGForGameMode.Length; i++)
                {
                    CommonOGGForGameMode[i] = ini.GetValueArray<string>("Include", $"CommonOgg.{(MissionType)i}");
                    foreach (string f in CommonOGGForGameMode[i])
                        if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                            BriefingRoom.PrintToLog($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", LogMessageErrorLevel.Warning);
                }
            }

            BriefingRoom.PrintToLog("Loading common enemy air defense settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}EnemyAirDefense.ini"))
            {
                EnemyCAPDistanceFromObjectives = ini.GetValue<MinMaxD>("CombatAirPatrols", "DistanceFromObjectives");
                EnemyCAPMinDistanceFromTakeOffLocation = ini.GetValue<int>("CombatAirPatrols", "MinDistanceFromTakeOffLocation");

                for (i = 0; i < Toolbox.EnumCount<AmountNR>(); i++)
                {
                    EnemyAirDefense[i] = new DBCommonAirDefenseInfo(ini, (AmountNR)i);

                    EnemyCAPRelativePower[i] = (i == 0) ? 0.0 :
                        Toolbox.Clamp(ini.GetValue<int>("CombatAirPatrols", $"RelativePower.{(AmountNR)i}"), 0, 100) / 100.0;
                }

                for (i = 0; i < Toolbox.EnumCount<AirDefenseRange>(); i++)
                {
                    EnemyAirDefenseDistanceFromTakeOffLocation[i] = ini.GetValue<int>("AirDefenseRange", $"{(AirDefenseRange)i}.MinDistanceFromTakeOffLocation");
                    EnemyAirDefenseDistanceFromObjectives[i] = ini.GetValue<MinMaxD>("AirDefenseRange", $"{(AirDefenseRange)i}.DistanceFromObjectives");
                }
            }

            BriefingRoom.PrintToLog("Loading common ally air defense settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}AllyAirDefense.ini"))
            {
                for (i = 0; i < Toolbox.EnumCount<AmountNR>(); i++)
                {
                    AllyAirDefense[i] = new DBCommonAirDefenseInfo(ini, (AmountNR)i);
                }

                for (i = 0; i < Toolbox.EnumCount<AirDefenseRange>(); i++)
                {
                    AllyAirDefenseDistanceFromTakeOffLocation[i] = ini.GetValue<MinMaxD>("AirDefenseRange", $"{(AirDefenseRange)i}.DistanceFromTakeOffLocation");
                    AllyAirDefenseDistanceFromObjectives[i] = ini.GetValue<int>("AirDefenseRange", $"{(AirDefenseRange)i}.MinDistanceFromObjectives");
                }
            }

            BriefingRoom.PrintToLog("Loading common names settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Names.ini"))
            {
                MissionNameTemplate = ini.GetValue<string>("Mission", "Template");
                for (i = 0; i < MISSION_NAMES_PART_COUNT; i++)
                    MissionNameParts[i] = ini.GetValueArray<string>("Mission", $"Part{i + 1}");

                for (i = 0; i < Toolbox.EnumCount<UnitFamily>(); i++)
                {
                    UnitBriefingNames[i] = ini.GetValueArray<string>("UnitBriefing", ((UnitFamily)i).ToString());
                    Array.Resize(ref UnitBriefingNames[i], 2);
                    UnitGroupNames[i] = ini.GetValue<string>("UnitGroup", ((UnitFamily)i).ToString());
                }

                WPNameFinal = ini.GetValue<string>("Waypoints", "Final");
                WPNameInitial = ini.GetValue<string>("Waypoints", "Initial");
                WPNameNavigation = ini.GetValue<string>("Waypoints", "Navigation");
                WPNamesObjectives = ini.GetValueArray<string>("Waypoints", "Objectives");
            }

            BriefingRoom.PrintToLog("Loading common wind settings...");
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Wind.ini"))
            {
                //Weather = new DBEntryWeather[Toolbox.EnumCount<Weather>() - 1]; // -1 because we don't want "Random"
                //for (i = 0; i < Weather.Length; i++)
                //    Weather[i] = new DBEntryWeather(ini, ((Weather)i).ToString());
                
                Wind = new DBCommonWind[Toolbox.EnumCount<Wind>() - 1]; // -1 because we don't want "Random"
                for (i = 0; i < Wind.Length; i++)
                    Wind[i] = new DBCommonWind(ini, ((Wind)i).ToString());
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}