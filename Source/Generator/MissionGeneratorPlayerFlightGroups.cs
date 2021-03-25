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
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates player-controlled unit groups (and their AI CAP/SEAD escort for single-player missions)
    /// </summary>
    public class MissionGeneratorPlayerFlightGroups : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorPlayerFlightGroups(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <param name="playerCoalitionDB">Player coalition database entry</param>
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for AI CAP escort</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for AI SEAD escort</param>
        /// <returns>An array of <see cref="UnitFlightGroupBriefingDescription"/> describing the flight groups, to be used in the briefing</returns>
        public UnitFlightGroupBriefingDescription[] CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryCoalition playerCoalitionDB, out string aiEscortTypeCAP, out string aiEscortTypeSEAD)
        {
            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

            if (template.MissionType == MissionType.SinglePlayer)
                briefingFGList.Add(GenerateSinglePlayerFlightGroup(mission, template, objectiveDB));
            else
                briefingFGList.AddRange(GenerateMultiplayerFlightGroups(mission, template, objectiveDB));

            aiEscortTypeCAP = "";
            aiEscortTypeSEAD = "";
            UnitFlightGroupBriefingDescription? escortDescription;

            escortDescription = GenerateAIEscort(mission, template, template.SituationFriendlyEscortCAP, MissionTemplateFlightGroupTask.SupportCAP, playerCoalitionDB);
            if (escortDescription.HasValue)
            {
                briefingFGList.Add(escortDescription.Value);
                aiEscortTypeCAP = escortDescription.Value.Type;
            }

            escortDescription = GenerateAIEscort(mission, template, template.SituationFriendlyEscortSEAD, MissionTemplateFlightGroupTask.SupportSEAD, playerCoalitionDB);
            if (escortDescription.HasValue)
            {
                briefingFGList.Add(escortDescription.Value);
                aiEscortTypeSEAD = escortDescription.Value.Type;
            }

            return briefingFGList.ToArray();
        }

        /// <summary>
        /// Creates the flight group player will lead in a single-player mission.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <returns>A <see cref="UnitFlightGroupBriefingDescription"/> describing the flight group, to be used in the briefing</returns>
        private UnitFlightGroupBriefingDescription GenerateSinglePlayerFlightGroup(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB)
        {
            var playerFlightGroup = template.PlayerFlightGroups[0];
            bool isCarrier = !string.IsNullOrEmpty(playerFlightGroup.Carrier);
            DebugLog.Instance.WriteLine($"{playerFlightGroup.Carrier} -> {string.Join(",",mission.Carriers.Select(x => x.Name).ToArray())}");
            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                mission,
                Enumerable.Repeat(playerFlightGroup.Aircraft, playerFlightGroup.Count).ToArray(),
                Side.Ally, isCarrier? mission.Carriers.First(x => x.Units[0].Name == playerFlightGroup.Carrier).Coordinates : mission.InitialPosition,
                isCarrier? "GroupAircraftPlayerCarrier" : "GroupAircraftPlayer", "UnitAircraft",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationFriendlyAISkillLevel), DCSMissionUnitGroupFlags.FirstUnitIsPlayer,
                objectiveDB.Payload,
                null, isCarrier? -99 : mission.InitialAirbaseID, true,  country: playerFlightGroup.Country,
                startLocation: playerFlightGroup.StartLocation
                );

            if (group == null)
                throw new Exception($"Failed to create group of player aircraft of type \"{playerFlightGroup.Aircraft}\".");
            
            if(isCarrier)
                group.CarrierId = mission.Carriers.First(x => x.Units[0].Name == playerFlightGroup.Carrier).Units[0].ID;

            return new UnitFlightGroupBriefingDescription(
                        group.Name, group.Units.Length, playerFlightGroup.Aircraft,
                        objectiveDB.BriefingTaskFlightGroup,
                        Database.Instance.GetEntry<DBEntryUnit>(playerFlightGroup.Aircraft).AircraftData.GetRadioAsString());
        }

        /// <summary>
        /// Creates an AI escort flight group in a single-player mission.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="count">Number of aircraft in the flight group</param>
        /// <param name="task">Escort task the flight group will be assigned with</param>
        /// <param name="playerCoalitionDB">Player coalition database entry</param>
        /// <returns>A <see cref="UnitFlightGroupBriefingDescription"/> describing the flight group, to be used in the briefing</returns>
        private UnitFlightGroupBriefingDescription? GenerateAIEscort(DCSMission mission, MissionTemplate template, int count, MissionTemplateFlightGroupTask task, DBEntryCoalition playerCoalitionDB)
        {
            if (count < 1) return null; // No aircraft, nothing to generate.

            // Select proper payload for the flight group according to its tasking
            UnitTaskPayload payload = GetPayloadByTask(task, null);

            string groupLua;
            string[] aircraft;

            switch (task)
            {
                default: return null; // Should never happen
                case MissionTemplateFlightGroupTask.SupportCAP:
                    //groupLua = (template.GetMissionType() == MissionType.SinglePlayer) ? "GroupAircraftPlayerEscortCAP" : "GroupAircraftCAP";
                    groupLua = "GroupAircraftCAP";
                    aircraft = playerCoalitionDB.GetRandomUnits(UnitFamily.PlaneFighter, mission.DateTime.Decade, count, template.UnitMods);
                    break;
                case MissionTemplateFlightGroupTask.SupportSEAD:
                    //groupLua = (template.GetMissionType() == MissionType.SinglePlayer) ? "GroupAircraftPlayerEscortSEAD" : "GroupAircraftSEAD";
                    groupLua = "GroupAircraftSEAD";
                    aircraft = playerCoalitionDB.GetRandomUnits(UnitFamily.PlaneSEAD, mission.DateTime.Decade, count, template.UnitMods);
                    break;
            }

            Coordinates position = mission.InitialPosition;

            // Player starts on runway, so escort starts in the air above the airfield (so player doesn't have to wait for them to take off)
            // OR mission is MP, so escorts start in air (but won't be spawned until at least one player takes off)
            // Add a random distance so they don't crash into each other.
            if ((template.PlayerFlightGroups[0].StartLocation == PlayerStartLocation.Runway) ||
                (template.MissionType != MissionType.SinglePlayer))
                position += Coordinates.CreateRandom(2, 4) * Toolbox.NM_TO_METERS;

            DCSMissionUnitGroup group;
            //if (template.GetMissionType() == MissionType.SinglePlayer)
            //    group = UnitMaker.AddUnitGroup(
            //        mission, aircraft,
            //        Side.Ally, position,
            //        groupLua, "UnitAircraft",
            //        Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerAISkillLevel), 0,
            //        payload, null, mission.InitialAirbaseID, true);
            //else
                group = UnitMaker.AddUnitGroup(
                    mission, aircraft,
                    Side.Ally, position,
                    groupLua, "UnitAircraft",
                    Toolbox.BRSkillLevelToDCSSkillLevel(template.SituationFriendlyAISkillLevel), 0,
                    payload, mission.ObjectivesCenter);

            if (group == null)
            {
                DebugLog.Instance.WriteLine($"Failed to create AI escort flight group tasked with {task} with aircraft of type \"{aircraft[0]}\".", 1, DebugLogMessageErrorLevel.Warning);
                return null;
            }

            switch (task)
            {
                default: return null; // Should never happen
                case MissionTemplateFlightGroupTask.SupportCAP:
                    mission.EscortCAPGroupId = group.GroupID;
                    break;
                case MissionTemplateFlightGroupTask.SupportSEAD:
                    mission.EscortSEADGroupId = group.GroupID;
                    break;
            }

            return
                new UnitFlightGroupBriefingDescription(
                    group.Name, group.Units.Length, aircraft[0],
                    GetTaskingDescription(task, null),
                    Database.Instance.GetEntry<DBEntryUnit>(aircraft[0]).AircraftData.GetRadioAsString());
        }


        /// <summary>
        /// Creates multiplayer client flight groups.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="template">Mission template to use</param>
        /// <param name="objectiveDB">Mission objective database entry</param>
        /// <returns>An array of <see cref="UnitFlightGroupBriefingDescription"/> describing the flight groups, to be used in the briefing</returns>
        private UnitFlightGroupBriefingDescription[] GenerateMultiplayerFlightGroups(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB)
        {
            int totalGroupsCreated = 0;

            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

            foreach (MissionTemplateFlightGroup fg in template.PlayerFlightGroups)
            {
                // Select proper payload for the flight group according to its tasking
                UnitTaskPayload payload = GetPayloadByTask(fg.Tasking, objectiveDB);
                bool hasCarrier = !string.IsNullOrEmpty(fg.Carrier);
                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission,
                    Enumerable.Repeat(fg.Aircraft, fg.Count).ToArray(),
                    Side.Ally, hasCarrier ? mission.Carriers.First(x => x.Units[0].Name == fg.Carrier).Coordinates : mission.InitialPosition,
                    hasCarrier ? "GroupAircraftPlayerCarrier" : "GroupAircraftPlayer", "UnitAircraft",
                    DCSSkillLevel.Client, 0,
                    payload,
                    null, hasCarrier ? -99 : mission.InitialAirbaseID, true, country: fg.Country,
                    startLocation: fg.StartLocation == PlayerStartLocation.Runway ? PlayerStartLocation.ParkingHot : fg.StartLocation);
                if (group == null)
                {
                    DebugLog.Instance.WriteLine($"Failed to create group of player aircraft of type \"{fg.Aircraft}\".", 1, DebugLogMessageErrorLevel.Warning);
                    continue;
                }
                if(hasCarrier)
                    group.CarrierId = mission.Carriers.First(x => x.Units[0].Name == fg.Carrier).Units[0].ID;

                briefingFGList.Add(
                    new UnitFlightGroupBriefingDescription(
                        group.Name, group.Units.Length, fg.Aircraft,
                        GetTaskingDescription(fg.Tasking, objectiveDB),
                        Database.Instance.GetEntry<DBEntryUnit>(fg.Aircraft).AircraftData.GetRadioAsString()));

                totalGroupsCreated++;
            }

            // Not a single player flight group was created succesfully, abort mission generation
            if (totalGroupsCreated == 0)
                throw new Exception("No player flight groups could be created, mission generation failed.");

            return briefingFGList.ToArray();
        }

        /// <summary>
        /// Returns the proper payload type for a given task.
        /// </summary>
        /// <param name="task">Task assigned to this flight group in the mission package</param>
        /// <param name="objectiveDB">(optional) Mission objective database entry</param>
        /// <returns>A payload</returns>
        private UnitTaskPayload GetPayloadByTask(MissionTemplateFlightGroupTask task, DBEntryObjective objectiveDB = null)
        {
            switch (task)
            {
                default: // case MissionTemplateMPFlightGroupTask.Objectives
                    if (objectiveDB == null) return UnitTaskPayload.Default;
                    return objectiveDB.Payload;
                case MissionTemplateFlightGroupTask.SupportCAP:
                    return UnitTaskPayload.AirToAir;
                case MissionTemplateFlightGroupTask.SupportSEAD:
                    return UnitTaskPayload.SEAD;
            }
        }

        /// <summary>
        /// Return a string describing the task of a player flight group, to display in the flight group table part of the briefing.
        /// </summary>
        /// <param name="task">Task assigned to this flight group in the mission package</param>
        /// <param name="objectiveDB">(optional) Mission objective database entry</param>
        /// <returns>Name of task as it will appear in the flight group table</returns>
        private string GetTaskingDescription(MissionTemplateFlightGroupTask task, DBEntryObjective objectiveDB)
        {
            switch (task)
            {
                default: // case MissionTemplateMPFlightGroupTask.Objectives
                    if (objectiveDB == null) return "Mission objectives";
                    return objectiveDB.BriefingTaskFlightGroup;
                case MissionTemplateFlightGroupTask.SupportCAP:
                    return "CAP escort";
                case MissionTemplateFlightGroupTask.SupportSEAD:
                    return "SEAD escort";
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
