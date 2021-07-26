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

using BriefingRoom4DCS.Campaign;
using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Generator;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.IO;
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
        public const string VERSION = "0.5.107.18";

        /// <summary>
        /// Log message handler delegate.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorLevel"></param>
        public delegate void LogHandler(string message, LogMessageErrorLevel errorLevel);

        /// <summary>
        /// Campaign generator.
        /// </summary>
        private readonly CampaignGenerator CampaignGen;

        /// <summary>
        /// Mission generator.
        /// </summary>
        private readonly MissionGenerator Generator;

        /// <summary>
        /// Event raised when a message is logged.
        /// </summary>
        private static event LogHandler OnMessageLogged;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logHandler">Method to call when a message is logged.</param>
        public BriefingRoom(LogHandler logHandler = null)
        {
            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
                TARGETED_DCS_WORLD_VERSION = ini.GetValue("Versions", "DCSVersion", "2.7");

            OnMessageLogged += logHandler;
            Database.Instance.Initialize();

            Generator = new MissionGenerator();
            CampaignGen = new CampaignGenerator(Generator);
        }

        /// <summary>
        /// Returns information about all database entries of a given type.
        /// </summary>
        /// <param name="entryType">The type of entry to look for.</param>
        /// <param name="parameter">A special parameter for certain entry types (e.g. theater an airbase must be located in)</param>
        /// <returns>An array of <see cref="DatabaseEntryInfo"/></returns>
        public static DatabaseEntryInfo[] GetDatabaseEntriesInfo(DatabaseEntryType entryType, string parameter = "")
        {
            switch (entryType)
            {
                case DatabaseEntryType.Airbase:
                    if (string.IsNullOrEmpty(parameter)) // No parameter, return all airbases
                        return (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>() select airbase.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();
                    else // A parameter was provided, return all airbases from specified theater
                        return (from DBEntryAirbase airbase in Database.Instance.GetAllEntries<DBEntryAirbase>() where airbase.Theater == parameter.ToLowerInvariant() select airbase.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.Coalition:
                    return (from DBEntryCoalition coalition in Database.Instance.GetAllEntries<DBEntryCoalition>() select coalition.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.DCSMod:
                    return (from DBEntryDCSMod dcsMod in Database.Instance.GetAllEntries<DBEntryDCSMod>() select dcsMod.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.MissionFeature:
                    return (from DBEntryFeatureMission missionFeature in Database.Instance.GetAllEntries<DBEntryFeatureMission>() select missionFeature.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.ObjectiveFeature:
                    return (from DBEntryFeatureObjective objectiveFeature in Database.Instance.GetAllEntries<DBEntryFeatureObjective>() select objectiveFeature.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.ObjectivePreset:
                    return (from DBEntryObjectivePreset objectivePreset in Database.Instance.GetAllEntries<DBEntryObjectivePreset>() select objectivePreset.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.ObjectiveTarget:
                    return (from DBEntryObjectiveTarget objectiveTarget in Database.Instance.GetAllEntries<DBEntryObjectiveTarget>() select objectiveTarget.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.ObjectiveTargetBehavior:
                    return (from DBEntryObjectiveTargetBehavior objectiveTargetBehavior in Database.Instance.GetAllEntries<DBEntryObjectiveTargetBehavior>() select objectiveTargetBehavior.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.ObjectiveTask:
                    return (from DBEntryObjectiveTask objectiveTask in Database.Instance.GetAllEntries<DBEntryObjectiveTask>() select objectiveTask.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.Theater:
                    return (from DBEntryTheater theater in Database.Instance.GetAllEntries<DBEntryTheater>() select theater.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.Unit:
                    return (from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>() select unit.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.UnitCarrier:
                    return (from DBEntryUnit unitCarrier in Database.Instance.GetAllEntries<DBEntryUnit>() where Toolbox.SHIP_CARRIER_FAMILIES.Intersect(unitCarrier.Families).Count() > 0 select unitCarrier.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.UnitFlyableAircraft:
                    return (from DBEntryUnit unitFlyable in Database.Instance.GetAllEntries<DBEntryUnit>() where unitFlyable.AircraftData.PlayerControllable select unitFlyable.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();

                case DatabaseEntryType.WeatherPreset:
                    return (from DBEntryWeatherPreset weatherPreset in Database.Instance.GetAllEntries<DBEntryWeatherPreset>() select weatherPreset.GetDBEntryInfo()).OrderBy(x => x.Name).ToArray();
            }

            return null;
        }

        /// <summary>
        /// Returns information about a single database entry.
        /// </summary>
        /// <param name="entryType">The type of entry to look for.</param>
        /// <param name="id">ID of the entry to look for.</param>
        /// <returns>A <see cref="DatabaseEntryInfo"/> or null if ID doesn't exist.</returns>
        public static DatabaseEntryInfo? GetSingleDatabaseEntryInfo(DatabaseEntryType entryType, string id)
        {
            // Database entry ID doesn't exist
            if (!GetDatabaseEntriesIDs(entryType).Contains(id)) return null;

            DatabaseEntryInfo[] databaseEntryInfos = GetDatabaseEntriesInfo(entryType);
            return
                (from DatabaseEntryInfo databaseEntryInfo in databaseEntryInfos
                 where databaseEntryInfo.ID.ToLowerInvariant() == id.ToLowerInvariant()
                 select databaseEntryInfo).First();
        }

        /// <summary>
        /// Returns the unique IDs of all database entries of a given type.
        /// </summary>
        /// <param name="entryType">The type of entry to look for.</param>
        /// <param name="parameter">A special parameter for certain entry types (e.g. theater an airbase must be located in)</param>
        /// <returns>An array of strings</returns>
        public static string[] GetDatabaseEntriesIDs(DatabaseEntryType entryType, string parameter = "")
        {
            return (from DatabaseEntryInfo entryInfo in GetDatabaseEntriesInfo(entryType, parameter) select entryInfo.ID).ToArray();
        }

        /// <summary>
        /// Generates a mission from a BriefingRoom template file.
        /// </summary>
        /// <param name="templateFilePath">Path to the BriefingRoom template (.brt) file to use.</param>
        /// <param name="useObjectivePresets">If true, <see cref="MissionTemplateObjective.Preset"/> will be used to generate the objective. Otherwise, specific objective parameters will be used.</param>
        /// <returns>A <see cref="DCSMission"/>, or null if mission generation failed.</returns>
        public DCSMission GenerateMission(string templateFilePath, bool useObjectivePresets = false)
        {
            return Generator.Generate(new MissionTemplate(templateFilePath), useObjectivePresets);
        }

        /// <summary>
        /// Generates a mission from a mission template.
        /// </summary>
        /// <param name="template">Mission template from which the mission should be generated.</param>
        /// <param name="useObjectivePresets">If true, <see cref="MissionTemplateObjective.Preset"/> will be used to generate the objective. Otherwise, specific objective parameters will be used.</param>
        /// <returns>A <see cref="DCSMission"/>, or null if mission generation failed.</returns>
        public DCSMission GenerateMission(MissionTemplate template, bool useObjectivePresets = false)
        {
            return Generator.Generate(template, useObjectivePresets);
        }

        /// <summary>
        /// Generates a campaign from a BriefingRoom campaign template file.
        /// </summary>
        /// <param name="templateFilePath">Path to the BriefingRoom campaign template (.cbrt) file to use.</param>
        /// <returns>A <see cref="DCSCampaign"/>, or null if mission generation failed.</returns>
        public DCSCampaign GenerateCampaign(string templateFilePath, bool useObjectivePresets = false)
        {
            return CampaignGen.Generate(new CampaignTemplate(templateFilePath));
        }

        /// <summary>
        /// Generates a campaign from a campaign template.
        /// </summary>
        /// <param name="template">Campaign template from which the campaign should be generated.</param>
        /// <returns>A <see cref="DCSCampaign"/>, or null if campaign generation failed.</returns>
        public DCSCampaign GenerateCampaign(CampaignTemplate template)
        {
            return CampaignGen.Generate(template);
        }

        /// <summary>
        /// Returns the path of the directory where BriefingRoom is installed.
        /// </summary>
        /// <returns>Path to the directory, as a string.</returns>
        public static string GetBriefingRoomRootPath() { return BRPaths.ROOT; }

        /// <summary>
        /// Returns the DCS World custom mission path ([User]\Saved Games\DCS\Missions\).
        /// Looks first for DCS.earlyaccess, then DCS.openbeta, then DCS.
        /// </summary>
        /// <returns>The path, or the user's My document folder if none is found.</returns>
        public static string GetDCSMissionPath()
        {
            string[] possibleDCSPaths = new string[] { "DCS.earlyaccess", "DCS.openbeta", "DCS" };

            for (int i = 0; i < possibleDCSPaths.Length; i++)
            {
                string dcsPath = Toolbox.PATH_USER + "Saved Games\\" + possibleDCSPaths[i] + "\\Missions\\";
                if (Directory.Exists(dcsPath)) return dcsPath;
            }

            return Toolbox.PATH_USER_DOCS;
        }

        /// <summary>
        /// Returns the DCS World custom campaign path ([User]\Saved Games\DCS\Missions\Campaigns\multilang\).
        /// Looks first for DCS.earlyaccess, then DCS.openbeta, then DCS.
        /// </summary>
        /// <returns>The path, or the user's My document folder if none is found.</returns>
        public static string GetDCSCampaignPath()
        {
            string campaignPath = $"{GetDCSMissionPath()}Campaigns\\multilang\\";

            if (Directory.Exists(campaignPath)) return campaignPath;

            return Toolbox.PATH_USER_DOCS;
        }

        /// <summary>
        /// Prints a message to the log.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorLevel"></param>
        internal static void PrintToLog(string message, LogMessageErrorLevel errorLevel = LogMessageErrorLevel.Info)
        {
            OnMessageLogged?.Invoke(message, errorLevel);

            // Throw an exception if there was an error.
            if (errorLevel == LogMessageErrorLevel.Error)
                throw new BriefingRoomException(message);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            Generator.Dispose();
        }
    }
}
