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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// Generates friendly support units (AWACS, tankers...)
    /// </summary>
    public class MissionGeneratorSupportUnits : IDisposable
    {
        /// <summary>
        /// Unit maker class to use to generate units.
        /// </summary>
        private readonly UnitMaker UnitMaker;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="unitMaker">Unit maker class to use to generate units</param>
        public MissionGeneratorSupportUnits(UnitMaker unitMaker)
        {
            UnitMaker = unitMaker;
        }

        /// <summary>
        /// Main unit generation method.
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
        public UnitFlightGroupBriefingDescription[] CreateUnitGroups(DCSMission mission, DBEntryCoalition allyCoalitionDB)
        {
            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>();

            briefingFGList.Add(AddSupportUnit(mission, allyCoalitionDB, SupportUnitRoles.TankerBasket, new Tacan(47,"TKR", 1134000000))); //TACAN choise due to https://forums.eagle.ru/topic/165047-hornet-mini-updates/page/6/?tab=comments#comment-3803291
            briefingFGList.Add(AddSupportUnit(mission, allyCoalitionDB, SupportUnitRoles.TankerBoom, new Tacan(48, "TKR", 1135000000)));
            briefingFGList.Add(AddSupportUnit(mission, allyCoalitionDB, SupportUnitRoles.AWACS)); // AWACS must be added last, so it its inserted first into the spawning queue

            return (from UnitFlightGroupBriefingDescription fg in briefingFGList where !string.IsNullOrEmpty(fg.Type) select fg).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mission">Mission to which generated units should be added</param>
        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
        /// <param name="supportRole"></param>
        private UnitFlightGroupBriefingDescription AddSupportUnit(DCSMission mission, DBEntryCoalition allyCoalitionDB, SupportUnitRoles supportRole, Tacan TACAN = null)
        {
            DebugLog.Instance.WriteLine($"Adding {Toolbox.SplitCamelCase(supportRole)} support units...", 1);

            if (allyCoalitionDB.SupportUnits[(int)supportRole].Length == 0)
            {
                DebugLog.Instance.WriteLine($"No support unit found for this role in coalition \"{allyCoalitionDB.ID}\"", 2);
                return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded
            }

            string groupLua;

            switch (supportRole)
            {
                case SupportUnitRoles.AWACS:
                    groupLua = "GroupAircraftAWACS";
                    break;
                case SupportUnitRoles.TankerBasket:
                case SupportUnitRoles.TankerBoom:
                    groupLua = "GroupAircraftTanker";
                    break;
                default: // Should never happen
                    return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded
            }

            string unitType = Toolbox.RandomFrom(allyCoalitionDB.SupportUnits[(int)supportRole]);

            Coordinates location = GeneratorTools.GetCoordinatesOnFlightPath(mission, .5) + Coordinates.CreateRandom(8, 12) * Toolbox.NM_TO_METERS;
            Coordinates location2 = location + Coordinates.CreateRandom(12, 20) * Toolbox.NM_TO_METERS;

            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
                mission, new string[] { unitType },
                Side.Ally, location,
                groupLua, "UnitAircraft",
                DCSSkillLevel.Excellent, 0,
                UnitTaskPayload.Default,
                location2);
            if (group == null)
                return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded

            group.TACAN = TACAN;
            mission.AircraftSpawnQueue.Insert(0, new DCSMissionAircraftSpawnQueueItem(group.GroupID, true)); // Support aircraft must be activated first

            return new UnitFlightGroupBriefingDescription(
                group.Name, group.Units.Length, unitType,
                (supportRole == SupportUnitRoles.AWACS) ? "Early warning" : "Refueling",
                Database.Instance.GetEntry<DBEntryUnit>(unitType).AircraftData.GetRadioAsString(), TACAN != null? $"TACAN: {TACAN.ToString()}":"");
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
