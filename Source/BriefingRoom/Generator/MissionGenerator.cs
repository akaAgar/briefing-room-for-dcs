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
using System.IO;
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
        internal MissionGenerator() { }

        /// <summary>
        /// Generates a DCS World mission from a mission template.
        /// </summary>
        /// <param name="template">The mission template to use.</param>
        /// <param name="useObjectivePresets">If true, <see cref="MissionTemplateObjective.Preset"/> will be used to generate the objective. Otherwise, specific objective parameters will be used.</param>
        /// <returns>A DCS World mission, or null if something went wrong.</returns>
        internal DCSMission Generate(MissionTemplate template, bool useObjectivePresets)
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
            mission.SetValue("BriefingAllyCoalition", coalitionsDB[(int)template.ContextPlayerCoalition].UIDisplayName);
            mission.SetValue("BriefingEnemyCoalition", coalitionsDB[(int)template.ContextPlayerCoalition.GetEnemy()].UIDisplayName);
            mission.SetValue("EnableAudioRadioMessages", !template.OptionsMission.Contains(MissionOption.RadioMessagesTextOnly));
            mission.SetValue("LuaPlayerCoalition", $"coalition.side.{template.ContextPlayerCoalition.ToString().ToUpperInvariant()}");
            mission.SetValue("LuaEnemyCoalition", $"coalition.side.{template.ContextPlayerCoalition.GetEnemy().ToString().ToUpperInvariant()}");
            mission.SetValue("TheaterID", theaterDB.DCSID);
            mission.SetValue("AircraftActivatorCurrentQueue", ""); // Just to make sure aircraft groups spawning queues are empty
            mission.SetValue("AircraftActivatorReserveQueue", "");
            mission.SetValue("MissionPlayerSlots", template.GetPlayerSlotsCount() == 1 ? "Single-player mission" : $"{template.GetPlayerSlotsCount()}-players mission");

            // Add common media files
            foreach (string oggFile in Database.Instance.Common.CommonOGG)
                mission.AddMediaFile($"l10n/DEFAULT/{Toolbox.AddMissingFileExtension(oggFile, ".ogg")}", $"{BRPaths.INCLUDE_OGG}{Toolbox.AddMissingFileExtension(oggFile, ".ogg")}");

            Country[][] coalitionsCountries;
            // Generate list of countries for each coalition
            using (MissionGeneratorCountries countriesGenerator = new MissionGeneratorCountries())
                coalitionsCountries = countriesGenerator.GenerateCountries(mission, template);

            // Create unit maker
            UnitMaker unitMaker = new UnitMaker(mission, template, coalitionsDB, theaterDB, template.ContextPlayerCoalition, coalitionsCountries, template.GetPlayerSlotsCount() == 1);

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
            List<UnitFamily> objectiveTargetUnitFamilies = new List<UnitFamily>();
            Coordinates lastObjectiveCoordinates = playerAirbase.Coordinates;
            using (MissionGeneratorObjectives objectivesGenerator = new MissionGeneratorObjectives(unitMaker))
                for (i = 0; i < template.Objectives.Count; i++)
                {
                    lastObjectiveCoordinates = objectivesGenerator.GenerateObjective(mission, template, theaterDB, i, lastObjectiveCoordinates, playerAirbase, useObjectivePresets, out string objectiveName, out UnitFamily objectiveTargetUnitFamily);
                    objectiveCoordinates.Add(lastObjectiveCoordinates);
                    waypoints.Add(objectivesGenerator.GenerateObjectiveWaypoint(template.Objectives[i], lastObjectiveCoordinates, objectiveName));
                    objectiveTargetUnitFamilies.Add(objectiveTargetUnitFamily);
                }
            Coordinates objectivesCenter = (objectiveCoordinates.Count == 0) ? playerAirbase.Coordinates : Coordinates.Sum(objectiveCoordinates) / objectiveCoordinates.Count;

            // Generate carrier groups
            BriefingRoom.PrintToLog("Generating carrier groups...");
            Dictionary<string, UnitMakerGroupInfo> carrierDictionary;
            using (MissionGeneratorCarrierGroup carrierGroupGenerator = new MissionGeneratorCarrierGroup(unitMaker))
                carrierDictionary = carrierGroupGenerator.GenerateCarrierGroup(mission, template, objectivesCenter, windSpeedAtSeaLevel, windDirectionAtSeaLevel);
            Coordinates averageInitialPosition = playerAirbase.Coordinates;
            if (carrierDictionary.Count > 0) averageInitialPosition = (averageInitialPosition + carrierDictionary.First().Value.Coordinates) / 2.0;

            // Generate extra flight plan info
            using (MissionGeneratorFlightPlan flightPlanGenerator = new MissionGeneratorFlightPlan())
            {
                flightPlanGenerator.GenerateBullseyes(mission, objectivesCenter);
                flightPlanGenerator.GenerateObjectiveWPCoordinatesLua(template, mission, waypoints);
                flightPlanGenerator.GenerateIngressAndEgressWaypoints(template, waypoints, averageInitialPosition, objectivesCenter);
                flightPlanGenerator.SaveWaypointsToBriefing(mission, playerAirbase.Coordinates, waypoints, template.OptionsMission.Contains(MissionOption.ImperialUnitsForBriefing));
            }

            // Generate surface-to-air defenses
            using (MissionGeneratorAirDefense airDefenseGenerator = new MissionGeneratorAirDefense(unitMaker))
                airDefenseGenerator.GenerateAirDefense(template, averageInitialPosition, objectivesCenter);

            // Generate combat air patrols
            using (MissionGeneratorCombatAirPatrols capGenerator = new MissionGeneratorCombatAirPatrols(unitMaker))
            {
                int[] capGroupsID = capGenerator.GenerateCAP(template, averageInitialPosition, objectivesCenter);
                foreach (int capGroupID in capGroupsID) // Add 50% of CAP groups to the list of A/C activated on takeoff, the other 50% to the list of A/C activated later.
                    if (Toolbox.RandomChance(2))
                        immediateActivationAircraftGroupsIDs.Add(capGroupID);
                    else
                        lateActivationAircraftGroupsIDs.Add(capGroupID);
            }

            // Generate player flight groups
            BriefingRoom.PrintToLog("Generating player flight groups...");
            using (MissionGeneratorPlayerFlightGroups playerFlightGroupsGenerator = new MissionGeneratorPlayerFlightGroups(unitMaker))
                for (i = 0; i < template.PlayerFlightGroups.Count; i++)
                    playerFlightGroupsGenerator.GeneratePlayerFlightGroup(mission, template.PlayerFlightGroups[i], playerAirbase, waypoints, carrierDictionary);

            // Generate mission features
            BriefingRoom.PrintToLog("Generating mission features...");
            using (MissionGeneratorFeaturesMission missionFeaturesGenerator = new MissionGeneratorFeaturesMission(unitMaker))
                for (i = 0; i < template.MissionFeatures.Count; i++)
                    missionFeaturesGenerator.GenerateMissionFeature(mission, template.MissionFeatures[i], i, playerAirbase.Coordinates, objectivesCenter);

            // Add ogg files to the media files dictionary
            foreach (string mediaFile in mission.GetMediaFileNames())
            {
                if (!mediaFile.ToLowerInvariant().EndsWith(".ogg")) continue; // Not an .ogg file
                mission.AppendValue("MapResourcesFiles", $"[\"ResKey_Snd_{Path.GetFileNameWithoutExtension(mediaFile)}\"] = \"{Path.GetFileName(mediaFile)}\",\n");
            }

            // Get unit tables from the unit maker (MUST BE DONE AFTER ALL UNITS ARE GENERATED)
            mission.SetValue("CountriesBlue", unitMaker.GetUnitsLuaTable(Coalition.Blue));
            mission.SetValue("CountriesRed", unitMaker.GetUnitsLuaTable(Coalition.Red));

            // Generate briefing and additional mission info
            BriefingRoom.PrintToLog("Generating briefing...");
            using (MissionGeneratorBriefing briefingGenerator = new MissionGeneratorBriefing())
            {
                briefingGenerator.GenerateMissionName(mission, template);
                briefingGenerator.GenerateMissionBriefingDescription(mission, template, objectiveTargetUnitFamilies);
                mission.SetValue("DescriptionText", mission.Briefing.GetBriefingAsRawText("\\\n"));
            }

            // Generate mission options
            BriefingRoom.PrintToLog("Generating options...");
            using (MissionGeneratorOptions optionsGenerator = new MissionGeneratorOptions())
                optionsGenerator.GenerateForcedOptions(mission, template);

            // Generate warehouses
            BriefingRoom.PrintToLog("Generating warehouses...");
            using (MissionGeneratorWarehouses warehousesGenerator = new MissionGeneratorWarehouses())
                warehousesGenerator.GenerateWarehouses(mission);

            // Generate image files
            BriefingRoom.PrintToLog("Generating images...");
            using (MissionGeneratorImages imagesGenerator = new MissionGeneratorImages()) {
                imagesGenerator.GenerateTitle(mission, template);
                imagesGenerator.GenerateKneeboardImage(mission);
            }

            return mission;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
