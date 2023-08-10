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

using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Data.JSON;
using BriefingRoom4DCS.Generator;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BriefingRoom4DCS
{
    public sealed class BriefingRoom
    {
        public static string TARGETED_DCS_WORLD_VERSION { get; private set; }
        public static Dictionary<string, string> AvailableLanguagesMap { get; private set; }
        public static bool RUNNING_IN_DOCKER = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        public static DatabaseLanguage LanguageDB { get; private set; }

        public const string REPO_URL = "https://github.com/akaAgar/briefing-room-for-dcs";

        public const string WEBSITE_URL = "https://akaagar.github.io/briefing-room-for-dcs/";

        public const string DISCORD_URL = "https://discord.gg/MvdFTYxkpx";

        public const string VERSION = "0.5.308.10";

        public const string BUILD_VERSION = "~BUILD_VERSION~";

        public const int MAXFILESIZE = 50000000;



        public delegate void LogHandler(string message, LogMessageErrorLevel errorLevel);

        private static event LogHandler OnMessageLogged;

        public BriefingRoom(LogHandler logHandler = null)
        {
            INIFile ini = new(Path.Combine(BRPaths.DATABASE, "Common.ini"));
            TARGETED_DCS_WORLD_VERSION = ini.GetValue("Versions", "DCSVersion", "2.7");

            AvailableLanguagesMap = new Dictionary<string, string> { { "EN", "English" } };
            foreach (var key in ini.GetKeysInSection("Languages"))
                AvailableLanguagesMap.AddIfKeyUnused(key, ini.GetValue<string>("Languages", key));


            OnMessageLogged += logHandler;
            Database.Instance.Initialize();
            LanguageDB = Database.Instance.Language;
        }

        public static DatabaseEntryInfo[] GetDatabaseEntriesInfo(DatabaseEntryType entryType, string parameter = "")
        {
            switch (entryType)
            {
                case DatabaseEntryType.Airbase:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return none
                        return Array.Empty<DatabaseEntryInfo>();
                    else // A parameter was provided, return all airbases from specified theater
                        return (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>() where airbase.Theater == parameter.ToLower() select airbase.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.Situation:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return none
                        return Array.Empty<DatabaseEntryInfo>();
                    else // A parameter was provided, return all airbases from specified theater
                        return (from DBEntrySituation situation in Database.Instance.GetAllEntries<DBEntrySituation>() where situation.Theater == parameter.ToLower() select situation.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
                case DatabaseEntryType.ObjectiveTarget:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return none
                        return (from DBEntryObjectiveTarget objectiveTarget in Database.Instance.GetAllEntries<DBEntryObjectiveTarget>() select objectiveTarget.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
                    else
                        return (from DBEntryObjectiveTarget objectiveTarget in Database.Instance.GetAllEntries<DBEntryObjectiveTarget>() where Database.Instance.GetEntry<DBEntryObjectiveTask>(parameter).ValidUnitCategories.Contains(objectiveTarget.UnitCategory) select objectiveTarget.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
                case DatabaseEntryType.ObjectiveTargetBehavior:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return none
                        return (from DBEntryObjectiveTargetBehavior objectiveTargetBehavior in Database.Instance.GetAllEntries<DBEntryObjectiveTargetBehavior>() select objectiveTargetBehavior.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
                    else
                        return (from DBEntryObjectiveTargetBehavior objectiveTargetBehavior in Database.Instance.GetAllEntries<DBEntryObjectiveTargetBehavior>() where objectiveTargetBehavior.ValidUnitCategories.Contains(Database.Instance.GetEntry<DBEntryObjectiveTarget>(parameter).UnitCategory) select objectiveTargetBehavior.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
                case DatabaseEntryType.Coalition:
                    return (from DBEntryCoalition coalition in Database.Instance.GetAllEntries<DBEntryCoalition>() select coalition.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.DCSMod:
                    return (from DBEntryDCSMod dcsMod in Database.Instance.GetAllEntries<DBEntryDCSMod>() select dcsMod.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.MissionFeature:
                    return (from DBEntryFeatureMission missionFeature in Database.Instance.GetAllEntries<DBEntryFeatureMission>() select missionFeature.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.OptionsMission:
                    return (from DBEntryOptionsMission missionFeature in Database.Instance.GetAllEntries<DBEntryOptionsMission>() select missionFeature.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.ObjectiveFeature:
                    return (from DBEntryFeatureObjective objectiveFeature in Database.Instance.GetAllEntries<DBEntryFeatureObjective>() select objectiveFeature.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.ObjectivePreset:
                    return (from DBEntryObjectivePreset objectivePreset in Database.Instance.GetAllEntries<DBEntryObjectivePreset>() select objectivePreset.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.ObjectiveTask:
                    return (from DBEntryObjectiveTask objectiveTask in Database.Instance.GetAllEntries<DBEntryObjectiveTask>() select objectiveTask.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.Theater:
                    return (from DBEntryTheater theater in Database.Instance.GetAllEntries<DBEntryTheater>() select theater.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.Unit:
                    return (from DBEntryJSONUnit unit in Database.Instance.GetAllEntries<DBEntryJSONUnit>() select unit.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.UnitCarrier:
                    return Database.Instance.GetAllEntries<DBEntryJSONUnit>().Where(unitCarrier => Toolbox.CARRIER_FAMILIES.Intersect(unitCarrier.Families).Any()).Select(unitCarrier => unitCarrier.GetDBEntryInfo())
                    .Concat(Database.Instance.GetAllEntries<DBEntryTemplate>().Where(template => template.Type == "FOB").Select(template => template.GetDBEntryInfo()))
                    .OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.UnitFlyableAircraft:
                    return (from DBEntryAircraft unitFlyable in Database.Instance.GetAllEntries<DBEntryJSONUnit, DBEntryAircraft>() where unitFlyable.PlayerControllable select unitFlyable.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();

                case DatabaseEntryType.WeatherPreset:
                    return (from DBEntryWeatherPreset weatherPreset in Database.Instance.GetAllEntries<DBEntryWeatherPreset>() select weatherPreset.GetDBEntryInfo()).OrderBy(x => x.Name.Get()).ToArray();
            }

            return null;
        }

        public static DatabaseEntryInfo? GetSingleDatabaseEntryInfo(DatabaseEntryType entryType, string id)
        {
            // Database entry ID doesn't exist
            if (!GetDatabaseEntriesIDs(entryType).Contains(id)) return null;

            DatabaseEntryInfo[] databaseEntryInfos = GetDatabaseEntriesInfo(entryType);
            return
                (from DatabaseEntryInfo databaseEntryInfo in databaseEntryInfos
                 where databaseEntryInfo.ID.ToLower() == id.ToLower()
                 select databaseEntryInfo).First();
        }

        public static List<string> GetAircraftLiveries(string aircraftID) =>
            Database.Instance.GetEntry<DBEntryJSONUnit, DBEntryAircraft>(aircraftID).Liveries
            .Select(x => x.Value)
            .Aggregate(new List<string>(), (acc, x) => { acc.AddRange(x); return acc; })
            .Distinct().Order().ToList();

        public static List<string> GetAircraftCallsigns(string aircraft) => new();


        public static List<string> GetAircraftPayloads(string aircraftID) =>
            Database.Instance.GetEntry<DBEntryJSONUnit, DBEntryAircraft>(aircraftID).Payloads.Select(x => x.name).Distinct().Order().ToList();

         public static List<SpawnPoint> GetTheaterSpawnPoints(string theaterID) =>
            Database.Instance.GetEntry<DBEntryTheater>(theaterID).SpawnPoints.Select(x => x.ToSpawnPoint()).ToList();


        public static string GetAlias(int index) => Toolbox.GetAlias(index);

        public static string[] GetDatabaseEntriesIDs(DatabaseEntryType entryType, string parameter = "")
        {
            return (from DatabaseEntryInfo entryInfo in GetDatabaseEntriesInfo(entryType, parameter) select entryInfo.ID).ToArray();
        }

        public static async Task<DCSMission> GenerateMissionAsync(string templateFilePath)
        {
            return await MissionGenerator.GenerateRetryableAsync(new MissionTemplate(templateFilePath));
        }

        public static async Task<DCSMission> GenerateMissionAsync(MissionTemplate template)
        {
            return await MissionGenerator.GenerateRetryableAsync(template);
        }

        public static async Task<DCSCampaign> GenerateCampaignAsync(string templateFilePath)
        {
            return await CampaignGenerator.GenerateAsync(new CampaignTemplate(templateFilePath));
        }

        public static async Task<DCSCampaign> GenerateCampaignAsync(CampaignTemplate template)
        {
            return await CampaignGenerator.GenerateAsync(template);
        }

        public static string GetBriefingRoomRootPath() { return BRPaths.ROOT; }

        public static string GetBriefingRoomMarkdownPath() { return BRPaths.INCLUDE_MARKDOWN; }

        public static string GetDCSMissionPath()
        {
            string[] possibleDCSPaths = new string[] { "DCS.earlyaccess", "DCS.openbeta", "DCS" };

            for (int i = 0; i < possibleDCSPaths.Length; i++)
            {
                string dcsPath = Path.Combine(Toolbox.PATH_USER, "Saved Games", possibleDCSPaths[i], "Missions");
                if (Directory.Exists(dcsPath)) return dcsPath;
            }

            return Toolbox.PATH_USER_DOCS;
        }

        public static string GetDCSCampaignPath()
        {
            string campaignPath = Path.Combine(GetDCSMissionPath(), "Campaigns", "multilang");

            if (Directory.Exists(campaignPath)) return campaignPath;

            return Toolbox.PATH_USER_DOCS;
        }

        public static string Translate(string key) => LanguageDB.Translate(key);

        internal static void PrintToLog(string message, LogMessageErrorLevel errorLevel = LogMessageErrorLevel.Info)
        {
            OnMessageLogged?.Invoke(message, errorLevel);
            if (errorLevel == LogMessageErrorLevel.Warning || errorLevel == LogMessageErrorLevel.Error)
                Console.WriteLine($"{errorLevel}: {message}");
        }

        public static void ReloadDatabase()
        {
            Database.Reset();
        }
    }
}
