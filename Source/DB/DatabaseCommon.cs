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

using BriefingRoom4DCSWorld.Debug;
using System;
using System.IO;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Stores miscellaneous shared settings for mission generation.
    /// </summary>
    public class DatabaseCommon : IDisposable
    {
        /// <summary>
        /// Maximum number of name parts to use in random mission names.
        /// </summary>
        public const int MISSION_NAMES_PART_COUNT = 4;

        /// <summary>
        /// Ogg files to include in every mission.
        /// </summary>
        public string[] CommonOGG { get; private set; }

        /// <summary>
        /// Ogg files to include in every mission of a given game mode.
        /// </summary>
        public string[][] CommonOGGForGameMode { get; private set; }

        /// <summary>
        /// Template default: blue coalition.
        /// </summary>
        public string DefaultCoalitionBlue { get; private set; }

        /// <summary>
        /// Template default: red coalition.
        /// </summary>
        public string DefaultCoalitionRed { get; private set; }

        /// <summary>
        /// Template default: objective.
        /// </summary>
        public string DefaultObjective { get; private set; }

        /// <summary>
        /// Template default: objective count.
        /// </summary>
        public int DefaultObjectiveCount { get; private set; }

        /// <summary>
        /// Template default: player aircraft.
        /// </summary>
        public string DefaultPlayerAircraft { get; private set; }

        /// <summary>
        /// Template default: theater.
        /// </summary>
        public string DefaultTheater { get; private set; }

        /// <summary>
        /// Distance between two objectives, according to mission template <see cref="Template.MissionTemplate.ObjectiveDistance"/> setting, in nautical miles.
        /// </summary>
        public int[] DistanceBetweenObjectives { get; }

        /// <summary>
        /// Distance between the players take off location and the first objective, according to mission template <see cref="Template.MissionTemplate.ObjectiveDistance"/> setting, in nautical miles.
        /// </summary>
        public int[] DistanceFromTakeOffLocation { get; }

        /// <summary>
        /// Settings for enemy air defense, according to mission template <see cref="Template.MissionTemplate.OppositionAirDefense"/> setting.
        /// </summary>
        public DatabaseCommonAirDefenseInfo[] EnemyAirDefense { get; }

        /// <summary>
        /// Minimum distance (in nautical miles) between players take off location and enemy surface-to-air defense, for each air defense range category.
        /// </summary>
        public int[] EnemyAirDefenseDistanceFromTakeOffLocation { get; }

        /// <summary>
        /// Min/max distance (in nautical miles) between objectives and enemy surface-to-air defense, for each air defense range category.
        /// </summary>
        public MinMaxD[] EnemyAirDefenseDistanceFromObjectives { get; }

        /// <summary>
        /// Min/max distance between enemy CAP and the mission objectives.
        /// </summary>
        public MinMaxD EnemyCAPDistanceFromObjectives { get; private set; }

        /// <summary>
        /// Min distance (in nautical miles) between enemy CAP and players take off location.
        /// </summary>
        public int EnemyCAPMinDistanceFromTakeOffLocation { get; private set; }

        /// <summary>
        /// Relative power (percentage) of enemy CAP relative to the players' flight package.
        /// </summary>
        public double[] EnemyCAPRelativePower { get; }

        /// <summary>
        /// Settings for enemy air defense, according to mission template <see cref="Template.MissionTemplate.AllyAirDefense"/> setting.
        /// </summary>
        public DatabaseCommonAirDefenseInfo[] AllyAirDefense { get; }

        /// <summary>
        /// Minimum distance (in nautical miles) between players take off location and ally surface-to-air defense, for each air defense range category.
        /// </summary>
        public MinMaxD[] AllyAirDefenseDistanceFromTakeOffLocation { get; }

        /// <summary>
        /// Min/max distance (in nautical miles) between objectives and ally surface-to-air defense, for each air defense range category.
        /// </summary>
        public int[] AllyAirDefenseDistanceFromObjectives { get; }

        /// <summary>
        /// Random mission names part.
        /// </summary>
        public string[][] MissionNameParts { get; } = new string[MISSION_NAMES_PART_COUNT][];

        /// <summary>
        /// Random mission name template, where $P1$, $P2$, $P3$, $P4$ is remplaced with a random mission name part.
        /// </summary>
        public string MissionNameTemplate { get; private set; }

        /// <summary>
        /// Name (singular -index #0- and plural -index #1) to display in the briefings for each unit family.
        /// </summary>
        public string[][] UnitBriefingNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()][];

        /// <summary>
        /// Random-parsable (<see cref="Generator.GeneratorTools.ParseRandomString(string)"/>) string for unit group names of each <see cref="UnitFamily"/>.
        /// </summary>
        public string[] UnitGroupNames { get; } = new string[Toolbox.EnumCount<UnitFamily>()];

        /// <summary>
        /// Name of the final (landing) player waypoint.
        /// </summary>
        public string WPNameFinal { get; private set; }

        /// <summary>
        /// Name of the initial (takeoff) player waypoint.
        /// </summary>
        public string WPNameInitial { get; private set; }

        /// <summary>
        /// Name of the navigation player waypoints, where $0$, $00$, $000$... is replaced with the waypoint number.
        /// </summary>
        public string WPNameNavigation { get; private set; }

        /// <summary>
        /// Names to use for objectives and objective waypoints.
        /// </summary>
        public string[] WPNamesObjectives { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseCommon()
        {
            DistanceBetweenObjectives = new int[Toolbox.EnumCount<Amount>()];
            DistanceFromTakeOffLocation = new int[Toolbox.EnumCount<Amount>()];
            EnemyAirDefense = new DatabaseCommonAirDefenseInfo[Toolbox.EnumCount<AmountN>()];
            EnemyAirDefenseDistanceFromObjectives = new MinMaxD[Toolbox.EnumCount<AirDefenseRange>()];
            EnemyAirDefenseDistanceFromTakeOffLocation = new int[Toolbox.EnumCount<AirDefenseRange>()];
            AllyAirDefense = new DatabaseCommonAirDefenseInfo[Toolbox.EnumCount<AmountN>()];
            AllyAirDefenseDistanceFromObjectives = new int[Toolbox.EnumCount<AirDefenseRange>()]; 
            AllyAirDefenseDistanceFromTakeOffLocation = new MinMaxD[Toolbox.EnumCount<AirDefenseRange>()];
            EnemyCAPRelativePower = new double[Toolbox.EnumCount<AmountN>()];
        }

        /// <summary>
        /// Loads common settings from <see cref="COMMON_SETTINGS_FILE"/>
        /// </summary>
        public void Load()
        {
            int i;

            DebugLog.Instance.WriteLine("Loading common global settings...", 1);
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
            {
                CommonOGG = ini.GetValueArray<string>("Include", "CommonOgg");
                foreach (string f in CommonOGG)
                    if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                        DebugLog.Instance.WriteLine($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);

                CommonOGGForGameMode = new string[Toolbox.EnumCount<MissionType>()][];
                for (i = 0; i < CommonOGGForGameMode.Length; i++)
                {
                    CommonOGGForGameMode[i] = ini.GetValueArray<string>("Include", $"CommonOgg.{(MissionType)i}");
                    foreach (string f in CommonOGGForGameMode[i])
                        if (!File.Exists($"{BRPaths.INCLUDE_OGG}{f}.ogg"))
                            DebugLog.Instance.WriteLine($"File \"Include\\Ogg\\{f}.ogg\" doesn't exist.", 1, DebugLogMessageErrorLevel.Warning);
                }
            }

            DebugLog.Instance.WriteLine("Loading template defaults...", 1);
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Defaults.ini"))
            {
                DefaultCoalitionBlue = ini.GetValue<string>("TemplateDefaults", "Coalition.Blue");
                DefaultCoalitionRed = ini.GetValue<string>("TemplateDefaults", "Coalition.Red");
                DefaultObjective = ini.GetValue<string>("TemplateDefaults", "Objective");
                DefaultObjectiveCount = ini.GetValue<int>("TemplateDefaults", "ObjectiveCount");
                DefaultPlayerAircraft = ini.GetValue<string>("TemplateDefaults", "PlayerAircraft");
                DefaultTheater = ini.GetValue<string>("TemplateDefaults", "Theater");
            }

            DebugLog.Instance.WriteLine("Loading common enemy air defense settings...", 1);
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}EnemyAirDefense.ini"))
            {
                EnemyCAPDistanceFromObjectives = ini.GetValue<MinMaxD>("CombatAirPatrols", "DistanceFromObjectives");
                EnemyCAPMinDistanceFromTakeOffLocation = ini.GetValue<int>("CombatAirPatrols", "MinDistanceFromTakeOffLocation");

                for (i = 0; i < Toolbox.EnumCount<AmountN>(); i++)
                {
                    EnemyAirDefense[i] = new DatabaseCommonAirDefenseInfo(ini, (AmountN)i);

                    EnemyCAPRelativePower[i] = (i == 0) ? 0.0 :
                        Toolbox.Clamp(ini.GetValue<int>("CombatAirPatrols", $"RelativePower.{(AmountN)i}"), 0, 100) / 100.0;
                }

                for (i = 0; i < Toolbox.EnumCount<AirDefenseRange>(); i++)
                {
                    EnemyAirDefenseDistanceFromTakeOffLocation[i] = ini.GetValue<int>("AirDefenseRange", $"{(AirDefenseRange)i}.MinDistanceFromTakeOffLocation");
                    EnemyAirDefenseDistanceFromObjectives[i] = ini.GetValue<MinMaxD>("AirDefenseRange", $"{(AirDefenseRange)i}.DistanceFromObjectives");
                }
            }

            DebugLog.Instance.WriteLine("Loading common ally air defense settings...", 1);
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}AllyAirDefense.ini"))
            {


                for (i = 0; i < Toolbox.EnumCount<AmountN>(); i++)
                {
                    AllyAirDefense[i] = new DatabaseCommonAirDefenseInfo(ini, (AmountN)i);
                }

                for (i = 0; i < Toolbox.EnumCount<AirDefenseRange>(); i++)
                {
                    AllyAirDefenseDistanceFromTakeOffLocation[i] = ini.GetValue<MinMaxD>("AirDefenseRange", $"{(AirDefenseRange)i}.DistanceFromTakeOffLocation");
                    AllyAirDefenseDistanceFromObjectives[i] = ini.GetValue<int>("AirDefenseRange", $"{(AirDefenseRange)i}.MinDistanceFromObjectives");
                }
            }

            DebugLog.Instance.WriteLine("Loading common names settings...", 1);
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

            DebugLog.Instance.WriteLine("Loading common objective settings...", 1);
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Objectives.ini"))
            {
                for (i = 0; i < Toolbox.EnumCount<Amount>(); i++)
                {
                    DistanceBetweenObjectives[i] = Math.Max(0, ini.GetValue<int>("DistanceToObjective", $"{(Amount)i}.DistanceBetweenObjectives"));
                    DistanceFromTakeOffLocation[i] = Math.Max(0, ini.GetValue<int>("DistanceToObjective", $"{(Amount)i}.DistanceFromTakeOffLocation"));
                }
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}