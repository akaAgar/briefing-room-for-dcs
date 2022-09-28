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
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BriefingRoom4DCS.Generator
{
    internal class MissionGenerator
    {

        internal static async Task<DCSMission> GenerateAsync(MissionTemplateRecord template, bool useObjectivePresets)
        {
            // Check for missing entries in the database
            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionBlue);
            GeneratorTools.CheckDBForMissingEntry<DBEntryCoalition>(template.ContextCoalitionRed);
            GeneratorTools.CheckDBForMissingEntry<DBEntryWeatherPreset>(template.EnvironmentWeatherPreset, true);
            GeneratorTools.CheckDBForMissingEntry<DBEntryTheater>(template.ContextTheater);
            if (!template.PlayerFlightGroups.Any(x => !x.Hostile))
                throw new BriefingRoomException("Cannot have all players on hostile side.");

            var mission = new DCSMission();

            var waypoints = new List<Waypoint>();

            var theaterDB = Database.Instance.GetEntry<DBEntryTheater>(template.ContextTheater);
            var situationDB = Toolbox.RandomFrom(
                Database.Instance.GetAllEntries<DBEntrySituation>()
                    .Where(x => x.Theater == template.ContextTheater.ToLower())
                    .ToArray()
                );
            if (template.ContextSituation.StartsWith(template.ContextTheater))
                situationDB = Database.Instance.GetEntry<DBEntrySituation>(template.ContextSituation);
            else if (template.ContextSituation == "None")
                template.OptionsMission.Add("SpawnAnywhere");


            var coalitionsDB = new DBEntryCoalition[]
            {
                Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionBlue),
                Database.Instance.GetEntry<DBEntryCoalition>(template.ContextCoalitionRed)
            };

            // Copy values from the template
            mission.SetValue("BriefingTheater", theaterDB.UIDisplayName.Get());
            mission.SetValue("BriefingSituation", template.OptionsMission.Contains("SpawnAnywhere") ? "None" : situationDB.UIDisplayName.Get());
            mission.SetValue("BriefingAllyCoalition", coalitionsDB[(int)template.ContextPlayerCoalition].UIDisplayName.Get());
            mission.SetValue("BriefingEnemyCoalition", coalitionsDB[(int)template.ContextPlayerCoalition.GetEnemy()].UIDisplayName.Get());
            mission.SetValue("EnableAudioRadioMessages", !template.OptionsMission.Contains("RadioMessagesTextOnly"));
            mission.SetValue("BDASetting", template.OptionsMission.Contains("NoBDA") ? "NONE" : (template.OptionsMission.Contains("TargetOnlyBDA") ? "TARGETONLY" : "ALL"));
            mission.SetValue("LuaPlayerCoalition", $"coalition.side.{template.ContextPlayerCoalition.ToString().ToUpperInvariant()}");
            mission.SetValue("LuaEnemyCoalition", $"coalition.side.{template.ContextPlayerCoalition.GetEnemy().ToString().ToUpperInvariant()}");
            mission.SetValue("TheaterID", theaterDB.DCSID);
            mission.SetValue("AircraftActivatorCurrentQueue", ""); // Just to make sure aircraft groups spawning queues are empty
            mission.SetValue("AircraftActivatorReserveQueue", "");
            mission.SetValue("AircraftActivatorIsResponsive", template.MissionFeatures.Contains("ImprovementsResponsiveAircraftActivator"));
            mission.SetValue("MissionPlayerSlots", template.GetPlayerSlotsCount() == 1 ? BriefingRoom.Translate("SinglePlayerMission") : $"{template.GetPlayerSlotsCount()}{BriefingRoom.Translate("XPlayerMission")}");
            mission.SetValue("CaCmdBlu", template.CombinedArmsCommanderBlue);
            mission.SetValue("CaCmdRed", template.CombinedArmsCommanderRed);
            mission.SetValue("CaJTACBlu", template.CombinedArmsJTACBlue);
            mission.SetValue("CaJTACRed", template.CombinedArmsJTACRed);
            mission.SetValue("CaJTACRed", template.CombinedArmsJTACRed);
            mission.SetValue("CaPilotControl", template.OptionsMission.Contains("CombinedArmsPilotControl"));



            foreach (string oggFile in Database.Instance.Common.CommonOGG)
                mission.AddMediaFile($"l10n/DEFAULT/{Toolbox.AddMissingFileExtension(oggFile, ".ogg")}", $"{BRPaths.INCLUDE_OGG}{Toolbox.AddMissingFileExtension(oggFile, ".ogg")}");


            var coalitionsCountries = MissionGeneratorCountries.GenerateCountries(mission, template);


            var unitMaker = new UnitMaker(mission, template, coalitionsDB, theaterDB, situationDB, template.ContextPlayerCoalition, coalitionsCountries, template.GetPlayerSlotsCount() == 1);

            var drawingMaker = new DrawingMaker(mission, template, theaterDB, situationDB);
            var zoneMaker = new ZoneMaker(unitMaker);


            BriefingRoom.PrintToLog("Generating mission date and time...");
            var month = MissionGeneratorDateTime.GenerateMissionDate(mission, template);
            MissionGeneratorDateTime.GenerateMissionTime(mission, template, theaterDB, month);


            BriefingRoom.PrintToLog("Setting up airbases...");
            var airbasesGenerator = new MissionGeneratorAirbases(template, situationDB);
            var requiredRunway = template.PlayerFlightGroups.Select(x => Database.Instance.GetEntry<DBEntryUnit>(x.Aircraft).AircraftData.MinimumRunwayLengthFt).Max();
            var playerAirbase = airbasesGenerator.SelectStartingAirbase(mission, template.FlightPlanTheaterStartingAirbase, requiredRunway: requiredRunway);
            mission.PopulatedAirbaseIds[template.ContextPlayerCoalition].Add(playerAirbase.DCSID);
            if (playerAirbase.DCSID > 0)
            {
                mission.MapData.Add($"AIRBASE_HOME", new List<double[]> {  theaterDB.GetRealWorldCoordinates(playerAirbase.Coordinates) });
                mission.Briefing.AddItem(DCSMissionBriefingItemType.Airbase, $"{playerAirbase.Name}\t{playerAirbase.Runways}\t{playerAirbase.ATC}\t{playerAirbase.ILS}\t{playerAirbase.TACAN}");
            }
            airbasesGenerator.SelectStartingAirbaseForPackages(mission, playerAirbase, theaterDB);
            airbasesGenerator.SetupAirbasesCoalitions(mission, playerAirbase);
            zoneMaker.AddAirbaseZones(template.MissionFeatures, playerAirbase, mission.MissionPackages);
            mission.SetValue("PlayerAirbaseName", playerAirbase.Name);
            mission.SetValue("MissionAirbaseX", playerAirbase.Coordinates.X);
            mission.SetValue("MissionAirbaseY", playerAirbase.Coordinates.Y);


            BriefingRoom.PrintToLog("Generating mission weather...");
            var turbulenceFromWeather = MissionGeneratorWeather.GenerateWeather(mission, template, theaterDB, month, playerAirbase);
            var (windSpeedAtSeaLevel, windDirectionAtSeaLevel) = MissionGeneratorWeather.GenerateWind(mission, template, turbulenceFromWeather);

            // Generate objectives
            BriefingRoom.PrintToLog("Generating objectives...");
            var objectiveCoordinates = new List<Coordinates>();
            var objectiveTargetUnitFamilies = new List<UnitFamily>();
            var lastObjectiveCoordinates = playerAirbase.Coordinates;
            var objectivesGenerator = new MissionGeneratorObjectives(unitMaker, drawingMaker, template);
            var objectiveGroupedWaypoints = new List<List<Waypoint>>();
            var i = 0;
            foreach (var objectiveTemplate in template.Objectives)
            {
                var (objectiveCoords, waypointGroup) = objectivesGenerator.GenerateObjective(
                    mission, template, situationDB,
                    objectiveTemplate, lastObjectiveCoordinates, playerAirbase, useObjectivePresets,
                    ref i, ref objectiveCoordinates, ref waypoints, ref objectiveTargetUnitFamilies);
                lastObjectiveCoordinates = objectiveCoords;
                mission.MapData.Add($"OBJECTIVE_AREA{i}", new List<double[]> { theaterDB.GetRealWorldCoordinates(objectiveCoords) });
                objectiveGroupedWaypoints.Add(waypointGroup);
                i++;
            }
            var objectivesCenter = (objectiveCoordinates.Count == 0) ? playerAirbase.Coordinates : Coordinates.Sum(objectiveCoordinates) / objectiveCoordinates.Count;
            mission.SetValue("MissionCenterX", objectivesCenter.X);
            mission.SetValue("MissionCenterY", objectivesCenter.Y);

            // Generate carrier groups
            BriefingRoom.PrintToLog("Generating carrier groups...");
            MissionGeneratorCarrierGroup.GenerateCarrierGroup(
                unitMaker, zoneMaker, mission, template,
                playerAirbase.Coordinates, objectivesCenter,
                windSpeedAtSeaLevel, windDirectionAtSeaLevel);
            var averageInitialPosition = playerAirbase.Coordinates;
            if (unitMaker.carrierDictionary.Count > 0) averageInitialPosition = (averageInitialPosition + unitMaker.carrierDictionary.First().Value.UnitMakerGroupInfo.Coordinates) / 2.0;

            // Generate extra flight plan info
            MissionGeneratorFlightPlan.GenerateBullseyes(mission, objectivesCenter);
            MissionGeneratorFlightPlan.GenerateObjectiveWPCoordinatesLua(template, mission, waypoints, drawingMaker);
            MissionGeneratorFlightPlan.GenerateAircraftPackageWaypoints(template, mission, objectiveGroupedWaypoints, averageInitialPosition, objectivesCenter);
            MissionGeneratorFlightPlan.GenerateIngressAndEgressWaypoints(template, waypoints, averageInitialPosition, objectivesCenter);

            // Generate surface-to-air defenses
            MissionGeneratorAirDefense.GenerateAirDefense(template, unitMaker, averageInitialPosition, objectivesCenter);

            // Generate combat air patrols
            MissionGeneratorCombatAirPatrols.GenerateCAP(unitMaker, template, mission, averageInitialPosition, objectivesCenter);

            // Generate player flight groups
            BriefingRoom.PrintToLog("Generating player flight groups...");
            foreach (var templateFlightGroup in template.PlayerFlightGroups)
                MissionGeneratorPlayerFlightGroups.GeneratePlayerFlightGroup(unitMaker, mission, template, templateFlightGroup, playerAirbase, waypoints, averageInitialPosition, objectivesCenter, theaterDB);

            
            // Generate mission features
            BriefingRoom.PrintToLog("Generating mission features...");
            mission.AppendValue("ScriptMissionFeatures", ""); // Just in case there's no features
            var missionFeaturesGenerator = new MissionGeneratorFeaturesMission(unitMaker, template);
            foreach (var templateFeature in template.MissionFeatures)
                missionFeaturesGenerator.GenerateMissionFeature(mission, templateFeature, playerAirbase.Coordinates, objectivesCenter);


            // Add ogg files to the media files dictionary
            foreach (string mediaFile in mission.GetMediaFileNames())
            {
                if (!mediaFile.ToLowerInvariant().EndsWith(".ogg")) continue; // Not an .ogg file
                mission.AppendValue("MapResourcesFiles", $"[\"ResKey_Snd_{Path.GetFileNameWithoutExtension(mediaFile)}\"] = \"{Path.GetFileName(mediaFile)}\",\n");
            }

            // Get unit tables from the unit maker (MUST BE DONE AFTER ALL UNITS ARE GENERATED)
            mission.SetValue("CountriesBlue", unitMaker.GetUnitsLuaTable(Coalition.Blue));
            mission.SetValue("CountriesRed", unitMaker.GetUnitsLuaTable(Coalition.Red));
            mission.SetValue("CountriesNeutral", unitMaker.GetUnitsLuaTable(Coalition.Neutral));
            mission.SetValue("RequiredModules", unitMaker.GetRequiredModules());
            mission.SetValue("RequiredModulesBriefing", unitMaker.GetRequiredModulesBriefing());
            mission.SetValue("Drawings", drawingMaker.GetLuaDrawings());
            mission.SetValue("Zones", zoneMaker.GetLuaZones());

            // Generate briefing and additional mission info
            BriefingRoom.PrintToLog("Generating briefing...");
            var missionName = GeneratorTools.GenerateMissionName(template.BriefingMissionName);
            mission.Briefing.Name = missionName;
            mission.SetValue("MISSIONNAME", missionName);

            MissionGeneratorBriefing.GenerateMissionBriefingDescription(mission, template, objectiveTargetUnitFamilies, situationDB);
            mission.SetValue("DescriptionText", mission.Briefing.GetBriefingAsRawText("\\\n"));
            mission.SetValue("EditorNotes", mission.Briefing.GetEditorNotes("\\\n"));

            // Generate mission options
            BriefingRoom.PrintToLog("Generating options...");
            MissionGeneratorOptions.GenerateForcedOptions(mission, template);

            // Generate warehouses
            BriefingRoom.PrintToLog("Generating warehouses...");
            MissionGeneratorWarehouses.GenerateWarehouses(mission);

            // Generate image files
            BriefingRoom.PrintToLog("Generating images...");
            MissionGeneratorImages.GenerateTitle(mission, template);
            if (!template.OptionsMission.Contains("DisableKneeboardImages"))
                await MissionGeneratorImages.GenerateKneeboardImagesAsync(mission);

            return mission;
        }

        internal static async Task<DCSMission> GenerateRetryableAsync(MissionTemplate template, bool useObjectivePresets)
        {
            var templateRecord = new MissionTemplateRecord(template);
            var mission = await Policy
                .HandleResult<DCSMission>(x => x.IsExtremeDistance(template, out double distance))
                .Or<BriefingRoomException>(x =>
                {
                    BriefingRoom.PrintToLog($"Recoverable Error thrown, {x.Message}", LogMessageErrorLevel.Warning);
                    return true;
                })
                .RetryAsync(10)
                .ExecuteAsync(() => GenerateAsync(templateRecord, useObjectivePresets));

            if (mission.IsExtremeDistance(template, out double distance))
                BriefingRoom.PrintToLog($"Distance to objectives exceeds 1.7x of requested distance. ({Math.Round(distance, 2)}NM)", LogMessageErrorLevel.Warning);

            return mission;
        }
    }
}
