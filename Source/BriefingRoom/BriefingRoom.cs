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

// using BriefingRoom4DCS.Campaign;
using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Generator;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Main class for the BriefingRoom library.
    /// </summary>
    public sealed class BriefingRoom : IDisposable
    {
        /// <summary>
        /// Targeted DCS world version (just for info, doesn't mean that the program will not work with another version)
        /// </summary>
        public static string TARGETED_DCS_WORLD_VERSION { get; private set; }

        /// <summary>
        /// Absolute URL to the project source code repository.
        /// </summary>
        public const string REPO_URL = "https://github.com/akaAgar/briefing-room-for-dcs";

        /// <summary>
        /// Absolute URL to the project website.
        /// </summary>
        public const string WEBSITE_URL = "https://akaagar.itch.io/briefing-room-for-dcs";

        /// <summary>
        /// The current version of BriefingRoom.
        /// </summary>
        public const string VERSION = "0.4.104.21";

        public delegate void LogHandler(string message, LogMessageErrorLevel errorLevel);

        internal static event LogHandler OnLog;

        internal static void PrintToLog(string message, LogMessageErrorLevel errorLevel = LogMessageErrorLevel.Info)
        {
            OnLog?.Invoke(message, errorLevel);
        }

        //private readonly CampaignGenerator CampaignGen;
        private readonly MissionGenerator Generator;

        public BriefingRoom(LogHandler logHandler = null)
        {
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
                TARGETED_DCS_WORLD_VERSION = ini.GetValue("Versions", "DCSVersion", "2.7");

            OnLog += logHandler;
            Database.Instance.Initialize();

            Generator = new MissionGenerator();
            //CampaignGen = new CampaignGenerator(Database, Generator);
        }

        public DCSMission GenerateMission(string templateFilePath)
        {
            return Generator.Generate(new MissionTemplate(templateFilePath));
        }

        public DCSMission GenerateMission(MissionTemplate template)
        {
            return Generator.Generate(template);
        }

        // public MizFile MissionToMiz(DCSMission mission)
        // {
        //     MizFile miz;

        //     using (MizMaker exporter = new MizMaker(Database))
        //         miz = exporter.ExportToMizFile(mission);

        //     return miz;
        // }

        public static List<DatabaseEntryInfo> GetDatabaseEntriesInfo(DatabaseEntryType entryType, string parameter = "")
        {
            switch (entryType)
            {
                case DatabaseEntryType.Airbase:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return all airbases
                        return (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>() select airbase.GetDBEntryInfo()).ToArray();
                    else // A parameter was provided, return all airbases from specified theater
                        return (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>() where airbase.Theater == parameter.ToLowerInvariant() select airbase.GetDBEntryInfo()).ToArray();

                case DatabaseEntryType.DCSMod:
                    return (from DBEntryDCSMod dcsMod in Database.Instance.GetAllEntries<DBEntryDCSMod>() select dcsMod.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.FlyableAircraft:
                    return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>() where unit.AircraftData.PlayerControllable select unit.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.ObjectiveTarget:
                    return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryObjectiveTarget>() where unit.AircraftData.PlayerControllable select unit.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.ObjectiveTargetBehavior:
                    return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryObjectiveTargetBehavior>() where unit.AircraftData.PlayerControllable select unit.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.ObjectiveTask:
                    return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryObjectiveTask>() where unit.AircraftData.PlayerControllable select unit.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.Theater:
                    return (from DBEntryTheater theater in Database.Instance.GetAllEntries<DBEntryTheater>() select theater.GetDBEntryInfo()).ToList();

                case DatabaseEntryType.WeatherPreset:
                    return (from DBEntryWeatherPreset weatherPreset in Database.Instance.GetAllEntries<DBEntryWeatherPreset>() select weatherPreset.GetDBEntryInfo()).ToList();
                
                case DatabaseEntryType.Coalition:
                    return (from DBEntryCoalition coalition in Database.Instance.GetAllEntries<DBEntryCoalition>() select coalition.GetDBEntryInfo()).ToList();
            }

            return new List<DatabaseEntryInfo>{new DatabaseEntryInfo()};
        }

        public static List<string> GetDatabaseEntriesIDs(DatabaseEntryType entryType, string parameter = "")
        {
            return (from DatabaseEntryInfo entryInfo in GetDatabaseEntriesInfo(entryType, parameter) select entryInfo.ID).ToList();
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            //Generator.Dispose();
        }
    }
}
