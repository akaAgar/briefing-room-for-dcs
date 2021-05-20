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
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// The main mission generator class. Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>.
    /// </summary>
    internal class MissionGenerator : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MissionGenerator()
        {

        }

        internal DCSMission Generate(MissionTemplate template)
        {
            int i;

            // Check for missing entries in the database
            if (!GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionBlue) ||
                !GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionRed) ||
                !GeneratorTools.CheckDBForMissingEntry<DBEntryWeatherPreset>(template.EnvironmentWeatherPreset, true) ||
                !GeneratorTools.CheckDBForMissingEntry<DBEntryTheater>(template.ContextTheater))
                return null;

            // Create mission class and other fields
            DCSMission mission = new DCSMission();
            
            List<Waypoint> waypoints = new List<Waypoint>();
            List<int> immediateActivationAircraftGroupsIDs = new List<int>();
            List<int> lateActivationAircraftGroupsIDs = new List<int>();

            // Get required database entries here, so we don't have to look for them each time they're needed.
            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            DBEntryCoalition[] coalitionsDB = new DBEntryCoalition[]
                {
                    Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionBlue),
                    Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionRed)
                };

            // Copy values from the template
            mission.SetValue("TheaterID", theaterDB.DCSID);
            mission.SetValue("DebugMode", template.OptionsMission.Contains(MissionOption.DebugMode));

            // Add common media files
            mission.AddOggFiles(Database.Instance.Common.CommonOGG);
            mission.AddOggFiles(Database.Instance.Common.CommonOGGForGameMode[(int)template.MissionType]);

            Country[][] coalitionsCountries;
            // Generate list of countries for each coalition
            using (MissionGeneratorCountries countriesGenerator = new MissionGeneratorCountries())
                coalitionsCountries = countriesGenerator.GenerateCountries(mission, template);

            // Create unit maker
            UnitMaker unitMaker = new UnitMaker(template, coalitionsDB, theaterDB, template.ContextPlayerCoalition, coalitionsCountries);

            // Generate mission date and time
            Month month;
            BriefingRoom.PrintToLog("Generating mission date and time...");
            using (MissionGeneratorDateTime dateTimeGenerator = new MissionGeneratorDateTime())
            {
                dateTimeGenerator.GenerateMissionDate(mission, template, out month);
                dateTimeGenerator.GenerateMissionTime(mission, template, theaterDB, month);
            }

            // Generate weather and wind
            BriefingRoom.PrintToLog("Generating mission weather...");
            double windSpeedAtSeaLevel, windDirectionAtSeaLevel;
            using (MissionGeneratorWeather weatherGenerator = new MissionGeneratorWeather())
            {
                weatherGenerator.GenerateWeather(mission, template, theaterDB, month, out int turbulenceFromWeather);
                weatherGenerator.GenerateWind(mission, template, turbulenceFromWeather, out windSpeedAtSeaLevel, out windDirectionAtSeaLevel);
            }

            // Setup airbases
            DBEntryAirbase playerAirbase;
            BriefingRoom.PrintToLog("Setting up airbases...");
            using (MissionGeneratorAirbases airbasesGenerator = new MissionGeneratorAirbases())
            {
                playerAirbase = airbasesGenerator.SelectStartingAirbase(mission, template);
                if (playerAirbase == null) throw new BriefingRoomException("No valid airbase was found for the player(s).");
                airbasesGenerator.SetupAirbasesCoalitions(mission, template, playerAirbase);
                mission.SetValue("PlayerAirbaseName", playerAirbase.Name);
                mission.SetValue("MissionAirbaseX", playerAirbase.Coordinates.X);
                mission.SetValue("MissionAirbaseY", playerAirbase.Coordinates.Y);
                mission.Briefing.AddItem(DCSMissionBriefingItemType.Airbase, $"{playerAirbase.Name}\t{playerAirbase.Runways}\t{playerAirbase.ATC}\t{playerAirbase.ILS}\t{playerAirbase.TACAN}");
            }

            // Generate objectives
            BriefingRoom.PrintToLog("Generating objectives...");
            List<Coordinates> objectiveCoordinates = new List<Coordinates>();
            Coordinates lastObjectiveCoordinates = playerAirbase.Coordinates;
            using (MissionGeneratorObjectives objectivesGenerator = new MissionGeneratorObjectives(unitMaker))
                for (i = 0; i < template.Objectives.Count; i++)
                {
                    lastObjectiveCoordinates = objectivesGenerator.GenerateObjective(mission, template, i, lastObjectiveCoordinates, out string objectiveName);
                    objectiveCoordinates.Add(lastObjectiveCoordinates);
                    waypoints.Add(objectivesGenerator.GenerateObjectiveWaypoint(template.Objectives[i], lastObjectiveCoordinates, objectiveName));
                }
            Coordinates objectivesCenter = (objectiveCoordinates.Count == 0) ? playerAirbase.Coordinates : Coordinates.Sum(objectiveCoordinates) / objectiveCoordinates.Count;

            // Generate extra flight plan info
            using (MissionGeneratorFlightPlan flightPlanGenerator = new MissionGeneratorFlightPlan())
            {
                flightPlanGenerator.GenerateBullseyes(mission, objectivesCenter);
                flightPlanGenerator.GenerateExtraWaypoints(template, playerAirbase.Coordinates, waypoints, false); // Ingress WPs
                flightPlanGenerator.GenerateExtraWaypoints(template, playerAirbase.Coordinates, waypoints, true); // Egress WPs
                flightPlanGenerator.SaveWaypointsToBriefing(mission, playerAirbase.Coordinates, waypoints, template.OptionsMission.Contains(MissionOption.ImperialUnitsForBriefing));
            }

            // Generate surface-to-air defenses
            using (MissionGeneratorAirDefense airDefenseGenerator = new MissionGeneratorAirDefense(unitMaker))
                airDefenseGenerator.GenerateAirDefense(template, playerAirbase.Coordinates, objectivesCenter);

            // Generate combat air patrols
            using (MissionGeneratorCombatAirPatrols capGenerator = new MissionGeneratorCombatAirPatrols(unitMaker))
            {
                int[] capGroupsID = capGenerator.GenerateCAP(template, playerAirbase.Coordinates, objectivesCenter);
                foreach (int capGroupID in capGroupsID) // Add 50% of CAP groups to the list of A/C activated on takeoff, the other 50% to the list of A/C activated later.
                    if (Toolbox.RandomChance(2))
                        immediateActivationAircraftGroupsIDs.Add(capGroupID);
                    else
                        lateActivationAircraftGroupsIDs.Add(capGroupID);
            }

            // Generate carrier groups
            BriefingRoom.PrintToLog("Generating carrier groups...");
            Dictionary<string, UnitMakerGroupInfo> carrierDictionary;
            using (MissionGeneratorCarrierGroup carrierGroupGenerator = new MissionGeneratorCarrierGroup(unitMaker))
                carrierDictionary = carrierGroupGenerator.GenerateCarrierGroup(mission, template, objectivesCenter, windSpeedAtSeaLevel, windDirectionAtSeaLevel);

            // Generate player flight groups
            BriefingRoom.PrintToLog("Generating player flight groups...");
            using (MissionGeneratorPlayerFlightGroups playerFlightGroupsGenerator = new MissionGeneratorPlayerFlightGroups(unitMaker))
                for (i = 0; i < template.PlayerFlightGroups.Count; i++)
                    playerFlightGroupsGenerator.GeneratePlayerFlightGroup(mission, template.PlayerFlightGroups[i], playerAirbase, waypoints, carrierDictionary);

            // Generate mission features
            BriefingRoom.PrintToLog("Generating mission features...");
            using (MissionGeneratorFeaturesMission missionFeaturesGenerator = new MissionGeneratorFeaturesMission(unitMaker))
                for (i = 0; i  < template.MissionFeatures.Count; i++)
                    missionFeaturesGenerator.GenerateMissionFeature(mission, template.MissionFeatures[i], i, playerAirbase.Coordinates, objectivesCenter);

            mission.SetValue("ResourcesOGGFiles", ""); // TODO

            // Get unit tables from the unit maker (must be done after all units are generated)
            mission.SetValue("CountriesBlue", unitMaker.GetUnitsLuaTable(Coalition.Blue));
            mission.SetValue("CountriesRed", unitMaker.GetUnitsLuaTable(Coalition.Red));

            // Generate briefing and additional mission info
            BriefingRoom.PrintToLog("Generating briefing...");
            using (MissionGeneratorBriefing briefingGenerator = new MissionGeneratorBriefing())
            {
                briefingGenerator.GenerateMissionName(mission, template);
                briefingGenerator.GenerateMissionBriefingDescription(mission, template);
            }

            // Generate mission options
            BriefingRoom.PrintToLog("Generating options...");
            using (MissionGeneratorOptions optionsGenerator = new MissionGeneratorOptions())
                optionsGenerator.GenerateForcedOptions(mission, template);

            // Generate warehouses
            BriefingRoom.PrintToLog("Generating warehouses...");
            using (MissionGeneratorWarehouses warehousesGenerator = new MissionGeneratorWarehouses())
                warehousesGenerator.GenerateWarehouses(mission);

            return mission;
        }

//        /// <summary>
//        /// Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>.
//        /// Basically just calls <see cref="DoMissionGeneration(MissionTemplate, bool)"/> but encapsulates it in a try/catch loop
//        /// when not running a debug build.
//        /// </summary>
//        /// <param name="template">The <see cref="MissionTemplate"/> to use</param>
//        /// <returns>A <see cref="DCSMission"/>, or nuLL if something when wrong</returns>
//        internal DCSMission Generate(MissionTemplate template)
//        {
//            DCSMission mission;

//            // When debugging, let the IDE/compile catch the errors.
//            // In the release build, print the errors to the log.
//#if !DEBUG
//            try
//            {
//#endif
//            mission = DoMissionGeneration(template);
//#if !DEBUG
//            }
//            catch (Exception ex)
//            {
//                BriefingRoom.PrintToLog(ex.Message, DebugLogMessageErrorLevel.Error);
//                return null;
//            }
//#endif

//            return mission;
//        }

//        /// <summary>
//        /// Generates a <see cref="DCSMission"/> from a <see cref="MissionTemplate"/>
//        /// </summary>
//        /// <param name="template">The <see cref="MissionTemplate"/> to use</param>
//        /// <returns>A <see cref="DCSMission"/>, or nuLL if something when wrong</returns>
//        private DCSMission DoMissionGeneration(MissionTemplate template)
//        {
//            DateTime generationStartTime = DateTime.Now;
//            BriefingRoom.PrintToLog($"Starting mission generation...");

//            // Check for missing entries in the database
//            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionBlue);
//            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionRed);
//            //GeneratorTools.CheckDBForMissingEntry<DBEntryObjective>(Database, template.ObjectiveType, true);
//            GeneratorTools.CheckDBForMissingEntry<DBEntryTheater>(template.ContextTheater);

//            // Create the mission and copy some values (theater database entry ID, etc.) from the template
//            DCSMission mission = new DCSMission();
//            CopyTemplateValues(mission, template);

//            // Get some DB entries from the database for easier reference
//            DBEntryCoalition[] coalitionsDB = new DBEntryCoalition[2];
//            coalitionsDB[(int)Coalition.Blue] = Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionBlue);
//            coalitionsDB[(int)Coalition.Red] = Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionRed);
//            //DBEntryObjective objectiveDB;
//            //if (string.IsNullOrEmpty(template.ObjectiveType)) // Random objective
//            //    objectiveDB = Toolbox.RandomFrom(Database.GetAllEntries<DBEntryObjective>());
//            //else 
//            //    objectiveDB = Database.GetEntry<DBEntryObjective>(template.ObjectiveType);
//            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);

//            //// Create the unit maker, which will be used to generate unit groups and their properties
//            //UnitMaker unitMaker = new UnitMaker(coalitionsDB, theaterDB);

//            //// Create a list of flight group descriptions which will be used in the briefing
//            //List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

//            //// Setup airbases
//            //DBEntryTheaterAirbase airbaseDB;
//            //using (MissionGeneratorAirbases airbaseGen = new MissionGeneratorAirbases())
//            //{
//            //    airbaseDB = airbaseGen.SelectStartingAirbase(mission, template, theaterDB);

//            //    mission.InitialAirbaseID = airbaseDB.DCSID;
//            //    mission.InitialPosition = airbaseDB.Coordinates;

//            //    airbaseGen.SetupAirbasesCoalitions(mission, template.OptionsTheaterCountriesCoalitions, theaterDB);
//            //}

//            //// Generate mission objectives
//            //BriefingRoom.PrintToLog("Generating mission objectives...");
//            //using (MissionGeneratorObjectives objectives = new MissionGeneratorObjectives(unitMaker.SpawnPointSelector))
//            //    objectives.CreateObjectives(mission, template, theaterDB);

//            //// Generate mission date and time
//            //BriefingRoom.PrintToLog("Generating mission date and time...");
//            //using (MissionGeneratorDateTime dateTime = new MissionGeneratorDateTime())
//            //{
//            //    dateTime.GenerateMissionDate(mission, template, coalitionsDB);
//            //    dateTime.GenerateMissionTime(mission, template, theaterDB);
//            //}

//            //// Generate mission weather
//            //BriefingRoom.PrintToLog("Generating mission weather...");
//            //using (MissionGeneratorWeather weather = new MissionGeneratorWeather())
//            //{
//            //    weather.GenerateWeather(mission, template.EnvironmentWeather, theaterDB, Database.Common);
//            //    weather.GenerateWind(mission, template.EnvironmentWind, Database.Common);
//            //}

//            //// Generate Carrier
//            //using (MissionGeneratorCarrier unitGroupGen = new MissionGeneratorCarrier(unitMaker))
//            //    unitGroupGen.GenerateCarriers(mission, template, coalitionsDB[(int)mission.CoalitionPlayer]);


//            //// Generate player unit groups
//            //BriefingRoom.PrintToLog("Generating player unit groups and mission package...");
//            //string aiEscortTypeCAP, aiEscortTypeSEAD;
//            //using (MissionGeneratorPlayerFlightGroups unitGroupGen = new MissionGeneratorPlayerFlightGroups(unitMaker))
//            //    briefingFGList.AddRange(
//            //        unitGroupGen.CreateUnitGroups(
//            //            mission, template, coalitionsDB[(int)mission.CoalitionPlayer],
//            //            out aiEscortTypeCAP, out aiEscortTypeSEAD));

//            //// Generate objective unit groups
//            //BriefingRoom.PrintToLog("Generating objectives unit groups...");
//            //using (MissionGeneratorObjectivesUnitGroups unitGroupGen = new MissionGeneratorObjectivesUnitGroups(unitMaker))
//            //    unitGroupGen.CreateUnitGroups(mission, template, coalitionsDB);

//            //// Generate friendly support units
//            //BriefingRoom.PrintToLog("Generating friendly support units...");
//            //using (MissionGeneratorSupportUnits unitGroupGen = new MissionGeneratorSupportUnits(unitMaker))
//            //    briefingFGList.AddRange(unitGroupGen.CreateUnitGroups(mission, coalitionsDB[(int)mission.CoalitionPlayer], template.UnitMods));

//            //// Generate friendly support units
//            //BriefingRoom.PrintToLog("Generating enemy support units...");
//            //using (MissionGeneratorEnemySupportUnits unitGroupGen = new MissionGeneratorEnemySupportUnits(unitMaker))
//            //    unitGroupGen.CreateUnitGroups(mission, template, coalitionsDB[(int)mission.CoalitionEnemy], template.UnitMods);

//            //// Generate enemy air defense unit groups
//            //BriefingRoom.PrintToLog("Generating enemy air defense unit groups...");
//            //using (MissionGeneratorAirDefense unitGroupGen = new MissionGeneratorAirDefense(unitMaker, false, template, mission))
//            //    unitGroupGen.CreateUnitGroups(mission, coalitionsDB[(int)mission.CoalitionEnemy], GeneratorTools.GetEnemySpawnPointCoalition(template), template.UnitMods);

//            //// Generate ally air defense unit groups
//            //BriefingRoom.PrintToLog("Generating friendly air defense unit groups...");
//            //using (MissionGeneratorAirDefense unitGroupGen = new MissionGeneratorAirDefense(unitMaker, true, template, mission))
//            //    unitGroupGen.CreateUnitGroups(mission, coalitionsDB[(int)mission.CoalitionPlayer], GeneratorTools.GetAllySpawnPointCoalition(template), template.UnitMods);

//            ////// Generate enemy fighter patrols
//            //BriefingRoom.PrintToLog("Generating enemy fighter patrol unit groups...");
//            //using (MissionGeneratorEnemyFighterPatrols unitGroupGen = new MissionGeneratorEnemyFighterPatrols(unitMaker))
//            //    unitGroupGen.CreateUnitGroups(mission, template, coalitionsDB[(int)mission.CoalitionEnemy], aiEscortTypeCAP, aiEscortTypeSEAD);

//            ////// Generate mission features
//            //BriefingRoom.PrintToLog("Generating mission features unit groups...");
//            //using (MissionGeneratorExtensionsAndFeatures featuresGen = new MissionGeneratorExtensionsAndFeatures(unitMaker))
//            //    featuresGen.GenerateExtensionsAndFeatures(mission, template, coalitionsDB);

//            //// Generates the mission flight plan
//            //BriefingRoom.PrintToLog("Generating mission flight plan...");
//            //using (MissionGeneratorFlightPlan flightPlan = new MissionGeneratorFlightPlan())
//            //{
//            //    flightPlan.SetBullseye(mission);
//            //    flightPlan.AddObjectiveWaypoints(mission);
//            //    flightPlan.AddExtraWaypoints(mission, template);
//            //}

//            //// Generate briefing. Must be last because it uses information from other generators
//            //BriefingRoom.PrintToLog("Generating mission briefing...");
//            //using (MissionGeneratorBriefing briefing = new MissionGeneratorBriefing())
//            //{
//            //    briefing.GenerateMissionName(mission, template);
//            //    briefing.GenerateMissionBriefing(mission, template, airbaseDB, briefingFGList, coalitionsDB);
//            //}

//            //// Set if radio sounds are enabled
//            //mission.RadioSounds = (template.OptionsRadioSounds == YesNo.Yes);

//            //// Add common .ogg vorbis files and make sure each only appears only once.
//            //mission.OggFiles.AddRange(Database.Common.CommonOGG);
//            //mission.OggFiles.AddRange(Database.Common.CommonOGGForGameMode[(int)template.MissionType]);
//            //mission.OggFiles =
//            //    (from string f in mission.OggFiles
//            //     where !string.IsNullOrEmpty(f.Trim())
//            //     select f.Trim())
//            //     .Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

//            // If radio sounds are disabled, do not include radio .ogg files to save on file size
//            if (!mission.OptionsMission.Contains(MissionOption.RadioMessagesTextOnly))
//                mission.OggFiles =
//                    (from string f in mission.OggFiles
//                     where (f.ToLowerInvariant() == "radio0") || (!f.ToLowerInvariant().StartsWith("radio"))
//                     select f).ToList();

//            // Make sure included Lua scripts appear only once
//            mission.IncludedLuaScripts = mission.IncludedLuaScripts.Distinct().OrderBy(x => x).ToList();

//            // Create aircraft queues and finalize the core script
//            CreateAircraftActivationQueues(mission);
//            switch (template.MissionType)
//            {
//                case MissionType.SinglePlayer:
//                    mission.LuaScriptObjectives += "briefingRoom.mission.missionType = brMissionType.SINGLE_PLAYER\r\n"; break;
//                case MissionType.Multiplayer:
//                    mission.LuaScriptObjectives += "briefingRoom.mission.missionType = brMissionType.COOPERATIVE\r\n"; break;
//                    //case MissionType.Versus:
//                    //    mission.CoreLuaScript += "briefingRoom.mission.missionType = brMissionType.VERSUS\r\n"; break;
//            }

//            BriefingRoom.PrintToLog($"Mission generation completed successfully in {(DateTime.Now - generationStartTime).TotalSeconds.ToString("F3", NumberFormatInfo.InvariantInfo)} second(s).");

//            //unitMaker.Dispose();

//            return mission;
//        }

//        private void CreateAircraftActivationQueues(DCSMission mission)
//        {
//            string[] initialQueue = (from DCSMissionAircraftSpawnQueueItem queueItem in mission.AircraftSpawnQueue
//                                     where queueItem.SpawnOnStart
//                                     select queueItem.GroupID.ToString()).ToArray();
//            mission.LuaScriptObjectives += $"briefingRoom.aircraftActivator.currentQueue = {{ {string.Join(",", initialQueue)} }}\r\n";

//            List<string> extraQueues = (from DCSMissionAircraftSpawnQueueItem queueItem in mission.AircraftSpawnQueue
//                                        where !queueItem.SpawnOnStart
//                                        select queueItem.GroupID.ToString()).ToList();
//            int totalExtraQueues = extraQueues.Count;

//            mission.LuaScriptObjectives += "briefingRoom.aircraftActivator.extraQueues = { ";
//            for (int i = 0; i < mission.Objectives.Length; i++)
//            {
//                int length = (i == mission.Objectives.Length - 1) ? extraQueues.Count : totalExtraQueues / mission.Objectives.Length;
//                mission.LuaScriptObjectives += $" {{ {string.Join(",", extraQueues.Take(length))} }}";
//                if (i < mission.Objectives.Length - 1) mission.LuaScriptObjectives += ", ";
//                extraQueues.RemoveRange(0, length);
//            }
//            mission.LuaScriptObjectives += "}\r\n";

//            //mission.CoreLuaScript += $"briefingRoom.aircraftActivator.escortCAP = {mission.EscortCAPGroupId}\r\n";
//            //mission.CoreLuaScript += $"briefingRoom.aircraftActivator.escortSEAD  = {mission.EscortSEADGroupId}\r\n";
//        }

//        /// <summary>
//        /// Directly copies some simple values (theater database entry ID, etc.) from the template.
//        /// </summary>
//        /// <param name="mission">The mission</param>
//        /// <param name="template">Mission template to use</param>
//        private void CopyTemplateValues(DCSMission mission, MissionTemplate template)
//        {
//            mission.Coalitions[(int)Coalition.Blue] = GeneratorTools.GetTemplateCoalition(template, Coalition.Blue);
//            mission.Coalitions[(int)Coalition.Red] = GeneratorTools.GetTemplateCoalition(template, Coalition.Red);
//            mission.CoalitionPlayer = template.ContextPlayerCoalition;
//            //mission.Weather.CloudsPreset = template.EnvironmentCloudPreset;
//            mission.Theater = template.ContextTheater;
//            //mission.CoalitionCountries[(int)Coalition.Blue] = new List<Country> { Country.CJTFBlue };
//            //mission.CoalitionCountries[(int)Coalition.Red] = new List<Country> { Country.CJTFRed };
//            var countries = template.PlayerFlightGroups.Select(x => x.Country).Distinct().ToList();
//            if (template.ContextPlayerCoalition == Coalition.Blue)
//                mission.CoalitionCountries[(int)Coalition.Blue].AddRange(countries);
//            else
//                mission.CoalitionCountries[(int)Coalition.Red].AddRange(countries);
//            mission.CoalitionCountries[(int)Coalition.Blue] = mission.CoalitionCountries[(int)Coalition.Blue].Distinct().ToList();
//            mission.CoalitionCountries[(int)Coalition.Red] = mission.CoalitionCountries[(int)Coalition.Red].Distinct().ToList();
//            //mission.EndMode = template.OptionsEndMode;
//            mission.OptionsRealism = template.OptionsRealism.ToArray();
//        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
