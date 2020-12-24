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
        /// <param name="aiEscortTypeCAP">Type of aircraft selected for AI CAP escort (single-player only)</param>
        /// <param name="aiEscortTypeSEAD">Type of aircraft selected for AI SEAD escort (single-player only)</param>
        /// <returns>An array of <see cref="UnitFlightGroupBriefingDescription"/> describing the flight groups, to be used in the briefing</returns>
        public UnitFlightGroupBriefingDescription[] CreateUnitGroups(DCSMission mission, MissionTemplate template, DBEntryObjective objectiveDB, DBEntryCoalition playerCoalitionDB, out string aiEscortTypeCAP, out string aiEscortTypeSEAD)
        {
            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

            aiEscortTypeCAP = "";
            aiEscortTypeSEAD = "";

            if (template.GetMissionType() == MissionType.SinglePlayer)
            {
                UnitFlightGroupBriefingDescription? escortDescription;

                briefingFGList.Add(GenerateSinglePlayerFlightGroup(mission, template, objectiveDB));

                escortDescription = GenerateSinglePlayerAIEscort(mission, template, template.PlayerSPEscortCAP, MissionTemplateMPFlightGroupTask.SupportCAP, playerCoalitionDB);
                if (escortDescription.HasValue)
                {
                    briefingFGList.Add(escortDescription.Value);
                    aiEscortTypeCAP = escortDescription.Value.Type;
                }

                escortDescription = GenerateSinglePlayerAIEscort(mission, template, template.PlayerSPEscortSEAD, MissionTemplateMPFlightGroupTask.SupportSEAD, playerCoalitionDB);
                if (escortDescription.HasValue)
                {
                    briefingFGList.Add(escortDescription.Value);
                    aiEscortTypeSEAD = escortDescription.Value.Type;
                }
            }
            else
                briefingFGList.AddRange(GenerateMultiplayerFlightGroups(mission, template, objectiveDB));

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
            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                mission,
                Enumerable.Repeat(template.PlayerSPAircraft, template.PlayerSPWingmen + 1).ToArray(),
                Side.Ally, mission.InitialPosition,
                "GroupAircraftPlayer", "UnitAircraft",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerSPWingmenSkillLevel), DCSMissionUnitGroupFlags.FirstUnitIsPlayer,
                objectiveDB.Payload,
                null, mission.InitialAirbaseID, true);

            if (group == null)
                throw new Exception($"Failed to create group of player aircraft of type \"{template.PlayerSPAircraft}\".");

            return new UnitFlightGroupBriefingDescription(
                        group.Name, group.Units.Length, template.PlayerSPAircraft,
                        objectiveDB.BriefingTaskFlightGroup,
                        Database.Instance.GetEntry<DBEntryUnit>(template.PlayerSPAircraft).AircraftData.GetRadioAsString());
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
        private UnitFlightGroupBriefingDescription? GenerateSinglePlayerAIEscort(DCSMission mission, MissionTemplate template, int count, MissionTemplateMPFlightGroupTask task, DBEntryCoalition playerCoalitionDB)
        {
            if (count < 1) return null; // No aircraft, nothing to generate.

            // Select proper payload for the flight group according to its tasking
            UnitTaskPayload payload = GetPayloadByTask(task, null);

            string groupLua;
            string[] aircraft;

            switch (task)
            {
                default: return null; // Should never happen
                case MissionTemplateMPFlightGroupTask.SupportCAP:
                    groupLua = "GroupAircraftPlayerEscortCAP";
                    aircraft = playerCoalitionDB.GetRandomUnits(UnitFamily.PlaneFighter, count);
                    break;
                case MissionTemplateMPFlightGroupTask.SupportSEAD:
                    groupLua = "GroupAircraftPlayerEscortSEAD";
                    aircraft = playerCoalitionDB.GetRandomUnits(UnitFamily.PlaneSEAD, count);
                    break;
            }

            Coordinates position = mission.InitialPosition;

            // Player starts on runway, so escort starts in the air above the airfield (so player doesn't have to wait for them to take off)
            // Add a random distance so they don't crash into each other.
            if (template.PlayerStartLocation == PlayerStartLocation.Runway)
                position += Coordinates.CreateRandom(2, 4) * Toolbox.NM_TO_METERS; 

            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                mission,
                aircraft,
                Side.Ally, position,
                groupLua, "UnitAircraft",
                Toolbox.BRSkillLevelToDCSSkillLevel(template.PlayerSPWingmenSkillLevel), 0,
                payload,
                null, mission.InitialAirbaseID, true);

            if (group == null)
            {
                DebugLog.Instance.WriteLine($"Failed to create AI escort flight group tasked with {task} with aircraft of type \"{aircraft[0]}\".", 1, DebugLogMessageErrorLevel.Warning);
                return null;
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

            foreach (MissionTemplateMPFlightGroup fg in template.PlayerMPFlightGroups)
            {
                // Select proper payload for the flight group according to its tasking
                UnitTaskPayload payload = GetPayloadByTask(fg.Task, objectiveDB);

                DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                    mission,
                    Enumerable.Repeat(fg.AircraftType, fg.Count).ToArray(),
                    Side.Ally, mission.InitialPosition,
                    "GroupAircraftPlayer", "UnitAircraft",
                    DCSSkillLevel.Client, 0,
                    payload,
                    null, mission.InitialAirbaseID, true);

                if (group == null)
                {
                    DebugLog.Instance.WriteLine($"Failed to create group of player aircraft of type \"{fg.AircraftType}\".", 1, DebugLogMessageErrorLevel.Warning);
                    continue;
                }

                briefingFGList.Add(
                    new UnitFlightGroupBriefingDescription(
                        group.Name, group.Units.Length, fg.AircraftType,
                        GetTaskingDescription(fg.Task, objectiveDB),
                        Database.Instance.GetEntry<DBEntryUnit>(fg.AircraftType).AircraftData.GetRadioAsString()));

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
        private UnitTaskPayload GetPayloadByTask(MissionTemplateMPFlightGroupTask task, DBEntryObjective objectiveDB = null)
        {
            switch (task)
            {
                default: // case MissionTemplateMPFlightGroupTask.Objectives
                    if (objectiveDB == null) return UnitTaskPayload.Default;
                    return  objectiveDB.Payload;
                case MissionTemplateMPFlightGroupTask.SupportCAP:
                    return UnitTaskPayload.AirToAir;
                case MissionTemplateMPFlightGroupTask.SupportSEAD:
                    return UnitTaskPayload.SEAD;
            }
        }

        /// <summary>
        /// Return a string describing the task of a player flight group, to display in the flight group table part of the briefing.
        /// </summary>
        /// <param name="task">Task assigned to this flight group in the mission package</param>
        /// <param name="objectiveDB">(optional) Mission objective database entry</param>
        /// <returns>Name of task as it will appear in the flight group table</returns>
        private string GetTaskingDescription(MissionTemplateMPFlightGroupTask task, DBEntryObjective objectiveDB)
        {
            switch (task)
            {
                default: // case MissionTemplateMPFlightGroupTask.Objectives
                    if (objectiveDB == null) return "Mission objectives";
                    return objectiveDB.BriefingTaskFlightGroup;
                case MissionTemplateMPFlightGroupTask.SupportCAP:
                    return "CAP escort";
                case MissionTemplateMPFlightGroupTask.SupportSEAD:
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
