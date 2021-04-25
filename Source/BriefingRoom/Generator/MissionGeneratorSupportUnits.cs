///*
//==========================================================================
//This file is part of Briefing Room for DCS World, a mission
//generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

//Briefing Room for DCS World is free software: you can redistribute it
//and/or modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation, either version 3 of
//the License, or (at your option) any later version.

//Briefing Room for DCS World is distributed in the hope that it will
//be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
//==========================================================================
//*/

//using BriefingRoom4DCS.Data;
//using BriefingRoom4DCS.Mission;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace BriefingRoom4DCS.Generator
//{
//    /// <summary>
//    /// Generates friendly support units (AWACS, tankers...)
//    /// </summary>
//    internal class MissionGeneratorSupportUnits : IDisposable
//    {
//        /// <summary>
//        /// Unit maker class to use to generate units.
//        /// </summary>
//        private readonly UnitMaker UnitMaker;

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="unitMaker">Unit maker class to use to generate units</param>
//        internal MissionGeneratorSupportUnits(UnitMaker unitMaker)
//        {
//            UnitMaker = unitMaker;
//        }

//        /// <summary>
//        /// Main unit generation method.
//        /// </summary>
//        /// <param name="mission">Mission to which generated units should be added</param>
//        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
//        /// <param name="unitMods">Unit mods selected units can belong to</param>
//        internal UnitFlightGroupBriefingDescription[] CreateUnitGroups(DCSMission mission, DBEntryCoalition allyCoalitionDB, string[] unitMods)
//        {
//            List<UnitFlightGroupBriefingDescription> briefingFGList = new List<UnitFlightGroupBriefingDescription>
//            {
//                AddSupportUnit(mission, allyCoalitionDB, UnitFamily.PlaneTankerBasket, unitMods, new Tacan(29, "TKR", AA: true)),
//                AddSupportUnit(mission, allyCoalitionDB, UnitFamily.PlaneTankerBoom, unitMods, new Tacan(92, "TKR", AA: true)),
//                AddSupportUnit(mission, allyCoalitionDB, UnitFamily.PlaneAWACS, unitMods) // AWACS must be added last, so it its inserted first into the spawning queue
//            };

//            return (from UnitFlightGroupBriefingDescription fg in briefingFGList where !string.IsNullOrEmpty(fg.Type) select fg).ToArray();
//        }

//        /// <summary>
//        /// Spawn a group of support units.
//        /// </summary>
//        /// <param name="mission">Mission to which generated units should be added</param>
//        /// <param name="allyCoalitionDB">Ally coalition database entry</param>
//        /// <param name="unitFamily">Family of support unit to spawn</param>
//        /// <param name="unitMods">Unit mods selected units can belong to</param>
//        /// <param name="TACAN">TACAN info for the unit, if any</param>
//        private UnitFlightGroupBriefingDescription AddSupportUnit(DCSMission mission, DBEntryCoalition allyCoalitionDB, UnitFamily unitFamily, string[] unitMods, Tacan TACAN = null)
//        {
//            BriefingRoom.PrintToLog($"Adding {unitFamily} support unit...");

//            string[] validUnitTypes = allyCoalitionDB.GetRandomUnits(unitFamily, mission.DateTime.Decade, 1, unitMods, false);

//            if (validUnitTypes.Length == 0)
//            {
//                BriefingRoom.PrintToLog($"No support unit found for this role in coalition \"{allyCoalitionDB.ID}\"");
//                return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded
//            }

//            string groupLua;

//            switch (unitFamily)
//            {
//                case UnitFamily.PlaneAWACS:
//                    groupLua = "GroupAircraftAWACS";
//                    break;
//                case UnitFamily.PlaneTankerBasket:
//                case UnitFamily.PlaneTankerBoom:
//                    groupLua = "GroupAircraftTanker";
//                    break;
//                default: // Should never happen
//                    return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded
//            }

//            Coordinates location = GeneratorTools.GetCoordinatesOnFlightPath(mission, .5) + Coordinates.CreateRandom(8, 12) * Toolbox.NM_TO_METERS;
//            Coordinates location2 = location + Coordinates.CreateRandom(12, 20) * Toolbox.NM_TO_METERS;

//            string unitType = Toolbox.RandomFrom(validUnitTypes);

//            DCSMissionUnitGroup group = UnitMaker.AddUnitGroup(
//                mission, new string[] { unitType },
//                Side.Ally, location,
//                groupLua, "UnitAircraft",
//                DCSSkillLevel.Excellent, 0,
//                UnitTaskPayload.Default,
//                location2);
//            if (group == null)
//                return new UnitFlightGroupBriefingDescription(); // Empty FG info will automatically be discarded

//            group.TACAN = TACAN;
//            mission.AircraftSpawnQueue.Insert(0, new DCSMissionAircraftSpawnQueueItem(group.GroupID, true)); // Support aircraft must be activated first

//            return new UnitFlightGroupBriefingDescription(
//                group.Name, group.Units.Length, unitType,
//                (unitFamily == UnitFamily.PlaneAWACS) ? "Early warning" : "Refueling",
//                Database.GetEntry<DBEntryUnit>(unitType).AircraftData.GetRadioAsString(), TACAN != null? $"TACAN: {TACAN.ToString()}":"");
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose()
//        {

//        }
//    }
//}
