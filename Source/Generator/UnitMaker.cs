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
using System.Globalization;
using System.Linq;

namespace BriefingRoom4DCSWorld.Generator
{
    public class UnitMaker : IDisposable
    {
        private const double AIRCRAFT_UNIT_SPACING = 50.0;

        private const double SHIP_UNIT_SPACING = 100.0;

        private const double VEHICLE_UNIT_SPACING = 20.0;

        private int NextGroupID;
        private int NextUnitID;

        public UnitMakerSpawnPointSelector SpawnPointSelector { get; }
        public UnitMakerCallsignGenerator CallsignGenerator { get; }

        public UnitMaker(DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB)
        {
            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB);

            NextGroupID = 1;
            NextUnitID = 1;
        }

        public DCSMissionUnitGroup AddUnitGroup(
            DCSMission mission, string[] units, Side side,
            Coordinates coordinates, string groupLua, string unitLua,
            DCSSkillLevel skill, DCSMissionUnitGroupFlags flags = 0, UnitTaskPayload payload = UnitTaskPayload.Default,
            Coordinates? coordinates2 = null, int airbaseID = 0, bool requiresParkingSpots = false)
        {
            if (units.Length == 0) return null; // No units database entries ID provided, cancel group creation

            // TODO: check for missing units
            DBEntryUnit[] unitsBP = (from string u in units where Database.Instance.EntryExists<DBEntryUnit>(u) select Database.Instance.GetEntry<DBEntryUnit>(u)).ToArray();
            unitsBP = (from DBEntryUnit u in unitsBP where u != null select u).ToArray();
            if (unitsBP.Length == 0) return null; // All database entries were null, cancel group creation

            Coalition coalition = (side == Side.Ally) ? mission.CoalitionPlayer : mission.CoalitionEnemy; // Pick group coalition

            double groupHeading = unitsBP[0].IsAircraft ? 0 : Toolbox.RandomDouble(Toolbox.TWO_PI); // Generate global group heading

            // Generate units in the group
            int unitIndex = 0;
            List<DCSMissionUnitGroupUnit> groupUnits = new List<DCSMissionUnitGroupUnit>();
            foreach (DBEntryUnit unitBP in unitsBP)
            {
                if (unitBP == null) continue;

                for (int i = 0; i < unitBP.DCSIDs.Length; i++)
                {
                    // Set unit coordinates and heading
                    Coordinates unitCoordinates = coordinates;
                    double unitHeading = groupHeading;
                    SetUnitCoordinatesAndHeading(ref unitCoordinates, ref unitHeading, unitBP, unitIndex);

                    // Get parking spot for the unit, if unit is parked at an airdrome
                    int parkingSpot = 0;
                    if (airbaseID > 0)
                    {
                        if (requiresParkingSpots)
                        {
                            parkingSpot = SpawnPointSelector.GetFreeParkingSpot(airbaseID, out Coordinates parkingCoordinates);
                            //if (parkingSpot >= 0)
                            //    unitCoordinates = parkingCoordinates;
                            //else
                            //    parkingSpot = 0;
                        }
                    }

                    // Add unit to the list of units
                    DCSMissionUnitGroupUnit unit = new DCSMissionUnitGroupUnit
                    {
                        Coordinates = unitCoordinates,
                        Heading = unitHeading,
                        ID = NextUnitID,
                        Type = unitBP.DCSIDs[i],
                        ParkingSpot = parkingSpot,
                    };
                    groupUnits.Add(unit);
                    unitIndex++; NextUnitID++;
                }
            }

            // Generate group name
            string groupName;
            UnitCallsign callsign = new UnitCallsign();
            if (unitsBP[0].IsAircraft) // Aircraft group, name is a callsign
            {
                callsign = CallsignGenerator.GetCallsign(unitsBP[0].DefaultFamily, coalition);
                groupName = callsign.GroupName;
            }
            else // Vehicle/ship/static group, name is a random group name
                groupName = GetGroupName(unitsBP[0].DefaultFamily);

            // Add group to the mission
            DCSMissionUnitGroup group = new DCSMissionUnitGroup
            {
                AirbaseID = airbaseID,
                CallsignLua = callsign.Lua,
                Category = unitsBP[0].Category,
                Coalition = coalition,
                Coordinates = coordinates,
                Coordinates2 = coordinates2 ?? coordinates,
                Flags = flags,
                GroupID = NextGroupID,
                LuaGroup = groupLua,
                Name = groupName,
                Skill = skill,
                Payload = payload,
                UnitID = units[0],
                LuaUnit = unitLua,
                Units = groupUnits.ToArray(),
            };
            mission.UnitGroups.Add(group);

            NextGroupID++;

            DebugLog.Instance.WriteLine($"Added \"{group.Units[0].Type}\" unit group \"{group.Name}\" for coalition {group.Coalition.ToString().ToUpperInvariant()}", 2);

            return group;
        }

        private string GetGroupName(UnitFamily family)
        {
            string name = GeneratorTools.ParseRandomString(Database.Instance.Common.UnitGroupNames[(int)family]);

            int fakeGroupNumber = NextGroupID * 10 + Toolbox.RandomInt(1, 10);
            name = name.Replace("$N$", fakeGroupNumber.ToString(NumberFormatInfo.InvariantInfo));
            name = name.Replace("$NTH$", Toolbox.GetOrdinalAdjective(fakeGroupNumber));
            return name;
        }

        private void SetUnitCoordinatesAndHeading(ref Coordinates unitCoordinates, ref double unitHeading, DBEntryUnit unitBP, int unitIndex)
        {
            if (unitBP.IsAircraft)
                unitCoordinates += new Coordinates(AIRCRAFT_UNIT_SPACING, AIRCRAFT_UNIT_SPACING) * unitIndex;
            else
            {
                if (unitBP.OffsetCoordinates.Length > unitIndex) // Unit has a fixed set of coordinates (for SAM sites, etc.)
                {
                    // TODO: proper rotation
                    //Coordinates offsetCoordinates =
                    //    new Coordinates(
                    //        unitBP.OffsetCoordinates[unitIndex].X * Math.Cos(unitHeading) + unitBP.OffsetCoordinates[unitIndex].Y * Math.Sin(unitHeading),
                    //        -unitBP.OffsetCoordinates[unitIndex].X * Math.Sin(unitHeading) + unitBP.OffsetCoordinates[unitIndex].Y * Math.Cos(unitHeading));
                    //unitCoordinates += offsetCoordinates;
                    unitCoordinates += unitBP.OffsetCoordinates[unitIndex];
                }
                else // No fixed coordinates, generate random coordinates
                {
                    switch (unitBP.Category)
                    {
                        case UnitCategory.Ship:
                            unitCoordinates += new Coordinates(SHIP_UNIT_SPACING * unitIndex);
                            break;
                        default:
                            unitCoordinates += new Coordinates(VEHICLE_UNIT_SPACING * unitIndex);
                            break;
                    }
                }

                if (unitBP.OffsetHeading.Length > unitIndex) // Unit has a fixed heading (for SAM sites, etc.)
                    // TODO: proper rotation
                    //unitHeading = (int)((unitHeading + unitBP.OffsetHeading[unitIndex]) * Toolbox.RADIANS_TO_DEGREES) % 360 * Toolbox.DEGREES_TO_RADIANS;
                    unitHeading = unitBP.OffsetHeading[unitIndex];
                else
                    unitHeading = Toolbox.RandomDouble(Toolbox.TWO_PI);
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
