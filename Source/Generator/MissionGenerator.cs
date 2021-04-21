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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// The main mission generator class. Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>.
    /// </summary>
    public class MissionGenerator : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MissionGenerator() { }

        /// <summary>
        /// Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>.
        /// Basically just calls <see cref="DoMissionGeneration(MissionTemplate, bool)"/> but encapsulates it in a try/catch loop
        /// when not running a debug build.
        /// </summary>
        /// <param name="template">The <see cref="MissionTemplate"/> to use</param>
        /// <returns>A <see cref="DCSMission"/>, or nuLL if something when wrong</returns>
        public DCSMission Generate(MissionTemplate template)
        {
            DCSMission mission;

#if !DEBUG
            try
            {
#endif
                mission = DoMissionGeneration(template);
#if !DEBUG
            }
            catch (Exception ex)
            {
                DebugLog.Instance.WriteLine(ex.Message, DebugLogMessageErrorLevel.Error);
                return null;
            }
#endif

            return mission;
        }

        /// <summary>
        /// Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>
        /// </summary>
        /// <param name="template">The <see cref="MissionTemplate"/> to use</param>
        /// <returns>A <see cref="DCSMission"/>, or nuLL if something when wrong</returns>
        private DCSMission DoMissionGeneration(MissionTemplate template)
        {
            DateTime generationStartTime = DateTime.Now;
            DebugLog.Instance.Clear();
            DebugLog.Instance.WriteLine($"Starting mission generation...");

            // Check for missing entries in the database
            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionBlue);
            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionRed);
            GeneratorTools.CheckDBForMissingEntry<DBEntryObjective>(template.ObjectiveType, true);
            GeneratorTools.CheckDBForMissingEntry<DBEntryTheater>(template.ContextTheater);

            // Create the mission and copy some values (theater database entry ID, etc.) from the template
            DCSMission mission = new DCSMission();
            CopyTemplateValues(mission, template);

            // Get some DB entries from the database for easier reference
            DBEntryCoalition[] coalitionsDB = new DBEntryCoalition[2];
            coalitionsDB[(int)Coalition.Blue] = Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionBlue);
            coalitionsDB[(int)Coalition.Red] = Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionRed);
            DBEntryObjective objectiveDB;
            if (string.IsNullOrEmpty(template.ObjectiveType)) // Random objective
                objectiveDB = Toolbox.RandomFrom(Database.Instance.GetAllEntries<DBEntryObjective>());
            else 
                objectiveDB = Database.Instance.GetEntry<DBEntryObjective>(template.ObjectiveType);
            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);

            // Create the unit maker, which will be used to generate unit groups and their properties
            UnitMaker unitMaker = new UnitMaker(coalitionsDB, theaterDB);

            // Create a list of flight group descriptions which will be used in the briefing
            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

            // Setup airbases
            DBEntryTheaterAirbase airbaseDB;
            using (MissionGeneratorAirbases airbaseGen = new MissionGeneratorAirbases())
            {
                airbaseDB = airbaseGen.SelectStartingAirbase(mission, template, theaterDB, objectiveDB);

                mission.InitialAirbaseID = airbaseDB.DCSID;
                mission.InitialPosition = airbaseDB.Coordinates;

                airbaseGen.SetupAirbasesCoalitions(mission, template.OptionsTheaterCountriesCoalitions, theaterDB);
            }

            // Generate mission objectives
            DebugLog.Instance.WriteLine("Generating mission objectives...");
            using (MissionGeneratorObjectives objectives = new MissionGeneratorObjectives(unitMaker.SpawnPointSelector))
                objectives.CreateObjectives(mission, template, objectiveDB, theaterDB);

            // Generate mission date and time
            DebugLog.Instance.WriteLine("Generating mission date and time...");
            using (MissionGeneratorDateTime dateTime = new MissionGeneratorDateTime())
            {
                dateTime.GenerateMissionDate(mission, template, coalitionsDB);
                dateTime.GenerateMissionTime(mission, template, theaterDB);
            }

            // Generate mission weather
            DebugLog.Instance.WriteLine("Generating mission weather...");
            using (MissionGeneratorWeather weather = new MissionGeneratorWeather())
            {
                weather.GenerateWeather(mission, template.EnvironmentWeather, theaterDB, Database.Instance.Common);
                weather.GenerateWind(mission, template.EnvironmentWind, Database.Instance.Common);
            }

            // Generate Carrier
            using(MissionGeneratorCarrier unitGroupGen = new MissionGeneratorCarrier(unitMaker))
                unitGroupGen.GenerateCarriers(mission, template, coalitionsDB[(int)mission.CoalitionPlayer]);
            

            // Generate player unit groups
            DebugLog.Instance.WriteLine("Generating player unit groups and mission package...");
            string aiEscortTypeCAP, aiEscortTypeSEAD;
            using (MissionGeneratorPlayerFlightGroups unitGroupGen = new MissionGeneratorPlayerFlightGroups(unitMaker))
                briefingFGList.AddRange(
                    unitGroupGen.CreateUnitGroups(
                        mission, template, objectiveDB, coalitionsDB[(int)mission.CoalitionPlayer],
                        out aiEscortTypeCAP, out aiEscortTypeSEAD));

            // Generate objective unit groups
            DebugLog.Instance.WriteLine("Generating objectives unit groups...");
            using (MissionGeneratorObjectivesUnitGroups unitGroupGen = new MissionGeneratorObjectivesUnitGroups(unitMaker))
                unitGroupGen.CreateUnitGroups(mission, template, objectiveDB, coalitionsDB);

            // Generate friendly support units
            DebugLog.Instance.WriteLine("Generating friendly support units...");
            using (MissionGeneratorSupportUnits unitGroupGen = new MissionGeneratorSupportUnits(unitMaker))
                briefingFGList.AddRange(unitGroupGen.CreateUnitGroups(mission, coalitionsDB[(int)mission.CoalitionPlayer], template.UnitMods));

            // Generate friendly support units
            DebugLog.Instance.WriteLine("Generating enemy support units...");
            using (MissionGeneratorEnemySupportUnits unitGroupGen = new MissionGeneratorEnemySupportUnits(unitMaker))
                unitGroupGen.CreateUnitGroups(mission, template, coalitionsDB[(int)mission.CoalitionEnemy], template.UnitMods);

            // Generate enemy air defense unit groups
            DebugLog.Instance.WriteLine("Generating enemy air defense unit groups...");
            using (MissionGeneratorAirDefense unitGroupGen = new MissionGeneratorAirDefense(unitMaker, false, template, mission))
                unitGroupGen.CreateUnitGroups(mission, objectiveDB, coalitionsDB[(int)mission.CoalitionEnemy], GeneratorTools.GetEnemySpawnPointCoalition(template), template.UnitMods);

            // Generate ally air defense unit groups
            DebugLog.Instance.WriteLine("Generating friendly air defense unit groups...");
            using (MissionGeneratorAirDefense unitGroupGen = new MissionGeneratorAirDefense(unitMaker, true, template, mission))
                unitGroupGen.CreateUnitGroups(mission, objectiveDB, coalitionsDB[(int)mission.CoalitionPlayer], GeneratorTools.GetAllySpawnPointCoalition(template), template.UnitMods);

            //// Generate enemy fighter patrols
            DebugLog.Instance.WriteLine("Generating enemy fighter patrol unit groups...");
            using (MissionGeneratorEnemyFighterPatrols unitGroupGen = new MissionGeneratorEnemyFighterPatrols(unitMaker))
                unitGroupGen.CreateUnitGroups(mission, template, objectiveDB, coalitionsDB[(int)mission.CoalitionEnemy], aiEscortTypeCAP, aiEscortTypeSEAD);

            //// Generate mission features
            DebugLog.Instance.WriteLine("Generating mission features unit groups...");
            using (MissionGeneratorExtensionsAndFeatures featuresGen = new MissionGeneratorExtensionsAndFeatures(unitMaker))
                featuresGen.GenerateExtensionsAndFeatures(mission, template, objectiveDB, coalitionsDB);

            // Generates the mission flight plan
            DebugLog.Instance.WriteLine("Generating mission flight plan...");
            using (MissionGeneratorFlightPlan flightPlan = new MissionGeneratorFlightPlan())
            {
                flightPlan.SetBullseye(mission);
                flightPlan.AddObjectiveWaypoints(mission, objectiveDB);
                flightPlan.AddExtraWaypoints(mission, template);
            }

            // Generate briefing. Must be last because it uses information from other generators
            DebugLog.Instance.WriteLine("Generating mission briefing...");
            using (MissionGeneratorBriefing briefing = new MissionGeneratorBriefing())
            {
                briefing.GenerateMissionName(mission, template);
                briefing.GenerateMissionBriefing(mission, template, objectiveDB, airbaseDB, briefingFGList, coalitionsDB);
            }

            // Set if radio sounds are enabled
            mission.RadioSounds = (template.OptionsRadioSounds == YesNo.Yes);

            // Add common .ogg vorbis files and make sure each only appears only once.
            mission.OggFiles.AddRange(Database.Instance.Common.CommonOGG);
            mission.OggFiles.AddRange(Database.Instance.Common.CommonOGGForGameMode[(int)template.MissionType]);
            mission.OggFiles =
                (from string f in mission.OggFiles
                 where !string.IsNullOrEmpty(f.Trim()) select f.Trim())
                 .Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

            // If radio sounds are disabled, do not include radio .ogg files to save on file size
            if (!mission.RadioSounds)
                mission.OggFiles =
                    (from string f in mission.OggFiles
                     where (f.ToLowerInvariant() == "radio0") || (!f.ToLowerInvariant().StartsWith("radio")) select f).ToList();

            // Make sure included Lua scripts appear only once
            mission.IncludedLuaScripts = mission.IncludedLuaScripts.Distinct().OrderBy(x => x).ToList();

            // Create aircraft queues and finalize the core script
            CreateAircraftActivationQueues(mission);
            switch (template.MissionType)
            {
                case MissionType.SinglePlayer:
                    mission.CoreLuaScript += "briefingRoom.mission.missionType = brMissionType.SINGLE_PLAYER\r\n"; break;
                case MissionType.Multiplayer:
                    mission.CoreLuaScript += "briefingRoom.mission.missionType = brMissionType.COOPERATIVE\r\n"; break;
                //case MissionType.Versus:
                //    mission.CoreLuaScript += "briefingRoom.mission.missionType = brMissionType.VERSUS\r\n"; break;
            }

            DebugLog.Instance.WriteLine($"Mission generation completed successfully in {(DateTime.Now - generationStartTime).TotalSeconds.ToString("F3", NumberFormatInfo.InvariantInfo)} second(s).");

            unitMaker.Dispose();

            return mission;
        }

        private void CreateAircraftActivationQueues(DCSMission mission)
        {
            string[] initialQueue = (from DCSMissionAircraftSpawnQueueItem queueItem in mission.AircraftSpawnQueue
                                  where queueItem.SpawnOnStart select queueItem.GroupID.ToString()).ToArray();
            mission.CoreLuaScript += $"briefingRoom.aircraftActivator.currentQueue = {{ {string.Join(",", initialQueue)} }}\r\n";

            List<string> extraQueues = (from DCSMissionAircraftSpawnQueueItem queueItem in mission.AircraftSpawnQueue
                                     where !queueItem.SpawnOnStart select queueItem.GroupID.ToString()).ToList();
            int totalExtraQueues = extraQueues.Count;

            mission.CoreLuaScript += "briefingRoom.aircraftActivator.extraQueues = { ";
            for (int i = 0; i < mission.Objectives.Length; i++)
            {
                int length = (i == mission.Objectives.Length - 1) ? extraQueues.Count : totalExtraQueues / mission.Objectives.Length;
                mission.CoreLuaScript += $" {{ {string.Join(",", extraQueues.Take(length))} }}";
                if (i < mission.Objectives.Length - 1) mission.CoreLuaScript += ", ";
                extraQueues.RemoveRange(0, length);
            }
            mission.CoreLuaScript += "}\r\n";

            mission.CoreLuaScript += $"briefingRoom.aircraftActivator.escortCAP = {mission.EscortCAPGroupId}\r\n";
            mission.CoreLuaScript += $"briefingRoom.aircraftActivator.escortSEAD  = {mission.EscortSEADGroupId}\r\n";
        }

        /// <summary>
        /// Directly copies some simple values (theater database entry ID, etc.) from the template.
        /// </summary>
        /// <param name="mission">The mission</param>
        /// <param name="template">Mission template to use</param>
        private void CopyTemplateValues(DCSMission mission, MissionTemplate template)
        {
            mission.Coalitions[(int)Coalition.Blue] = template.GetCoalition(Coalition.Blue);
            mission.Coalitions[(int)Coalition.Red] = template.GetCoalition(Coalition.Red);
            mission.CivilianTraffic = template.OptionsCivilianTraffic;
            mission.CoalitionPlayer = template.ContextCoalitionPlayer;
            mission.Weather.CloudsPreset = template.EnvironmentCloudPreset.Get();
            mission.RadioAssists = !template.Realism.Contains(RealismOption.DisableDCSRadioAssists);
            mission.Theater = template.ContextTheater;
            mission.CountryBlues =new List<Country>{Country.CJTFBlue};
            mission.CountryReds = new List<Country>{Country.CJTFRed};
            var countries = template.PlayerFlightGroups.Select(x => x.Country).Distinct().ToList();
            if (template.ContextCoalitionPlayer == Coalition.Blue)
                mission.CountryBlues.AddRange(countries);
            else 
                mission.CountryReds.AddRange(countries);
            mission.CountryBlues = mission.CountryBlues.Distinct().ToList();
            mission.CountryReds = mission.CountryReds.Distinct().ToList();
            mission.EndMode = template.OptionsEndMode;
            mission.RealismOptions = template.Realism;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
