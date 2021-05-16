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
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    //internal enum UnitMakerGroupSetting
    //{
    //    AirbaseID,
    //    AircraftPayload,
    //    CoordinatesDestination,
    //    Country,
    //    FirstUnitIsPlayer,
    //    Hidden,
    //    PlayerStartLocation,
    //    RequiresOpenAirParking,
    //    RequiresParkingSpots,
    //}


    //FirstUnitIsPlayer = 1,
    //    /// <summary>
    //    /// Unit group will be hidden in the planning, on MFD SA pages and on the F10 map.
    //    /// </summary>
    //    Hidden = 2,

    //     = 4,
    //     = 8

    internal struct UnitMakerGroupInfo
    {
        internal int GroupID { get; }
        internal int[] UnitsID { get; }

        internal UnitMakerGroupInfo(int groupID, List<int> unitsID)
        {
            GroupID = groupID;
            UnitsID = unitsID.ToArray();
        }
    }

    internal class UnitMaker : IDisposable
    {
        private const double AIRCRAFT_UNIT_SPACING = 50.0;
        private const double SHIP_UNIT_SPACING = 100.0;
        private const double VEHICLE_UNIT_SPACING = 20.0;

        private readonly MissionTemplate Template;
        private readonly DBEntryCoalition[] CoalitionsDB;
        private readonly Coalition PlayerCoalition;
        private readonly Country[][] CoalitionsCountries;

        private readonly Dictionary<Country, Dictionary<UnitCategory, List<string>>> UnitLuaTables = new Dictionary<Country, Dictionary<UnitCategory, List<string>>>();

        private int GroupID;
        private int UnitID;

        internal UnitMakerSpawnPointSelector SpawnPointSelector { get; }

        internal UnitMakerCallsignGenerator CallsignGenerator { get; }

        internal UnitMaker(
            MissionTemplate template,
            DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB,
            Coalition playerCoalition, Country[][] coalitionsCountries)
        {
            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB);

            Template = template;

            CoalitionsDB = coalitionsDB;
            PlayerCoalition = playerCoalition;
            CoalitionsCountries = coalitionsCountries;

            Clear();
        }

        internal void Clear()
        {
            CallsignGenerator.Clear();
            SpawnPointSelector.Clear();

            GroupID = 1;
            UnitID = 1;
            UnitLuaTables.Clear();
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            UnitFamily family, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates, DCSSkillLevel skill, AircraftPayload aircraftPayload = AircraftPayload.Default,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (unitCount <= 0) return null;
            DBEntryCoalition unitsCoalitionDB = CoalitionsDB[(int)((side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy())];
            
            string[] units = unitsCoalitionDB.GetRandomUnits(family, Template.ContextDecade, unitCount, Template.Mods, true);
            if (units.Length == 0) return null;

            return AddUnitGroup(units, side, family, groupLua, unitLua, coordinates, skill, aircraftPayload, extraSettings);
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            string[] units, Side side, UnitFamily unitFamily,
            string groupLua, string unitLua,
            Coordinates coordinates, DCSSkillLevel skill,
            AircraftPayload aircraftPayload = AircraftPayload.Default,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (units.Length == 0) return null;

            Coalition coalition = (side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy();
            Country country = (coalition == Coalition.Blue) ? Country.CJTFBlue : Country.CJTFRed;

            string lua = File.ReadAllText($"{BRPaths.INCLUDE_LUA_UNITS}{Toolbox.AddMissingFileExtension(groupLua, ".lua")}");
            foreach (KeyValuePair<string, object> extraSetting in extraSettings)
                LuaTools.ReplaceKey(ref lua, extraSetting.Key, extraSetting.Value);

            string groupName;
            UnitCallsign? callsign = null;
            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                callsign = CallsignGenerator.GetCallsign(unitFamily, coalition);
                groupName = callsign.Value.GroupName;
            }
            else
                groupName = GeneratorTools.GetGroupName(GroupID, unitFamily);
            
            LuaTools.ReplaceKey(ref lua, "ID", GroupID);
            LuaTools.ReplaceKey(ref lua, "X", coordinates.X);
            LuaTools.ReplaceKey(ref lua, "Y", coordinates.Y);
            LuaTools.ReplaceKey(ref lua, "Name", groupName);

            string unitLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_UNITS}{Toolbox.AddMissingFileExtension(unitLua, ".lua")}");
            string unitsLuaTable = "";
            List<int> unitsIDList = new List<int>();
            for (int unitIndex = 0; unitIndex < units.Length; unitIndex++)
            {
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(units[unitIndex]);
                if (unitDB == null) continue;

                SetUnitCoordinatesAndHeading(unitDB, unitIndex, coordinates, out Coordinates unitCoordinates, out double unitHeading);

                string singleUnitLuaTable = unitLuaTemplate;
                foreach (KeyValuePair<string, object> extraSetting in extraSettings)
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, extraSetting.Key, extraSetting.Value);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "ID", UnitID);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "Type", units[unitIndex]);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "ExtraLua", unitDB.ExtraLua);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "Heading", unitHeading);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "X", unitCoordinates.X);
                LuaTools.ReplaceKey(ref singleUnitLuaTable, "Y", unitCoordinates.Y);
                if ((unitDB.Category == UnitCategory.Helicopter) || (unitDB.Category == UnitCategory.Plane))
                {
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "Altitude", unitDB.AircraftData.CruiseAltitude);
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "PropsLua", unitDB.AircraftData.PropsLua);
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "EPLRS", unitDB.Flags.HasFlag(DBEntryUnitFlags.EPLRS));
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "Speed", unitDB.AircraftData.CruiseSpeed);
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "Callsign", callsign.Value.GetLua(unitIndex + 1));
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "OnBoardNumber", Toolbox.RandomInt(1, 1000).ToString("000"));
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "Name", callsign.Value.GetUnitName(unitIndex + 1));
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "RadioPresetsLua", string.Join("", unitDB.AircraftData.RadioPresets.Select((x, index) => $"[{index + 1}] = {x.ToLuaString()}")));
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "PayloadCommon", unitDB.AircraftData.PayloadCommon);
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "PayloadPylons", unitDB.AircraftData.GetPayloadLua(aircraftPayload, Template.ContextDecade));

                    if (unitIndex == 0) // Replace these values only once, as they affect the whole group
                    {
                        LuaTools.ReplaceKey(ref lua, "RadioBand", (int)unitDB.AircraftData.RadioModulation);
                        LuaTools.ReplaceKey(ref lua, "RadioFrequency", unitDB.AircraftData.RadioFrequency);
                        LuaTools.ReplaceKey(ref lua, "Speed", unitDB.AircraftData.CruiseSpeed);
                    }
                }
                else
                    LuaTools.ReplaceKey(ref singleUnitLuaTable, "Name", $"{groupName} {unitIndex + 1}");

                unitsLuaTable += $"[{unitIndex + 1}] =\n";
                unitsLuaTable += "{\n";
                unitsLuaTable += $"{singleUnitLuaTable}\n";
                unitsLuaTable += $"}}, -- end of [{unitIndex + 1}]\n";

                unitsIDList.Add(UnitID);
                UnitID++;
            }

            if (unitsIDList.Count == 0) return null; // No valid units added to this group
            LuaTools.ReplaceKey(ref lua, "Units", unitsLuaTable);

            LuaTools.ReplaceKey(ref lua, "Skill", skill); // Must be after units are added, because skill is set as a unit level
            LuaTools.ReplaceKey(ref lua, "Hidden", GeneratorTools.GetHiddenStatus(Template.OptionsFogOfWar, side)); // If "hidden" was not set through custom values

            AddUnitGroupToTable(country, unitFamily.GetUnitCategory(), lua);

            BriefingRoom.PrintToLog($"Added group of {units.Length} {coalition} {unitFamily} at {coordinates}");

            GroupID++;
            return new UnitMakerGroupInfo(GroupID - 1, unitsIDList);
        }

        private void AddUnitGroupToTable(Country country, UnitCategory category, string unitGroupLua)
        {
            if (!UnitLuaTables.ContainsKey(country)) UnitLuaTables.Add(country, new Dictionary<UnitCategory, List<string>>());
            if (!UnitLuaTables[country].ContainsKey(category)) UnitLuaTables[country].Add(category, new List<string>());
            UnitLuaTables[country][category].Add(unitGroupLua);
        }

        internal string GetUnitsLuaTable(Coalition coalition)
        {
            string unitsLuaTable = "";

            for (int countryIndex = 0; countryIndex < CoalitionsCountries[(int)coalition].Length; countryIndex++) // Check all countries in this coalition
            {
                Country country = CoalitionsCountries[(int)coalition][countryIndex];

                if (!UnitLuaTables.ContainsKey(country)) continue; // No units for this country

                unitsLuaTable += $"[{countryIndex + 1}] =\n";
                unitsLuaTable += "{\n";
                unitsLuaTable += $"[\"id\"] = {(int)country},\n";

                foreach (UnitCategory unitCategory in Toolbox.GetEnumValues<UnitCategory>()) // Check all coalitions
                {
                    if (!UnitLuaTables[country].ContainsKey(unitCategory)) continue; // No unit for this unit category

                    unitsLuaTable += $"[\"{unitCategory.ToString().ToLowerInvariant()}\"] =\n";
                    unitsLuaTable += "{\n";

                    for (int groupIndex = 0; groupIndex < UnitLuaTables[country][unitCategory].Count; groupIndex++)
                    {
                        unitsLuaTable += $"[{groupIndex + 1}] =\n";
                        unitsLuaTable += "{\n";
                        unitsLuaTable += $"{UnitLuaTables[country][unitCategory][groupIndex]}\n";
                        unitsLuaTable += $"}}, -- end of [{groupIndex + 1}]\n";
                    }

                    unitsLuaTable += $"}}, -- end of [\"{unitCategory.ToString().ToLowerInvariant()}\"]\n";
                }

                unitsLuaTable += $"}}, -- end of [{countryIndex + 1}]\n";
            }

            return unitsLuaTable;
        }

        //    internal int AddUnitGroup(
        //string[] units, Side side,
        //string groupLua, string unitLua,
        //Coordinates coordinates, Coordinates? coordinates2,
        //DCSSkillLevel skill, DCSMissionUnitGroupFlags flags = 0, AircraftPayload payload = AircraftPayload.Default,
        //int airbaseID = 0, Country? country = null, PlayerStartLocation startLocation = PlayerStartLocation.Runway)
        //    {


        //        GroupID++;
        //        return GroupID - 1;
        //    }

        //    internal int AddUnitGroup(
        //string[] units, Side side,
        //string groupLua, string unitLua,
        //Coordinates coordinates, Coordinates? coordinates2,
        //DCSSkillLevel skill, DCSMissionUnitGroupFlags flags = 0, AircraftPayload payload = AircraftPayload.Default,
        //int airbaseID = 0, Country? country = null, PlayerStartLocation startLocation = PlayerStartLocation.Runway)
        //    {


        //        GroupID++;
        //        return GroupID - 1;
        //    }

        /*
         * public void MyFunction(params KeyValuePair<string, object>[] pairs)
{
    // ...
}*/

        private void SetUnitCoordinatesAndHeading(
            DBEntryUnit unitDB, int unitIndex, Coordinates groupCoordinates,
            out Coordinates unitCoordinates, out double unitHeading)
        {
            unitCoordinates = groupCoordinates;
            unitHeading = 0;

            if (unitDB.IsAircraft)
                unitCoordinates = groupCoordinates + new Coordinates(AIRCRAFT_UNIT_SPACING, AIRCRAFT_UNIT_SPACING) * unitIndex;
            else
            {
                if (unitDB.OffsetCoordinates.Length > unitIndex) // Unit has a fixed set of coordinates (for SAM sites, etc.)
                {
                    double s = Math.Sin(unitHeading);
                    double c = Math.Cos(unitHeading);
                    Coordinates offsetCoordinates = unitDB.OffsetCoordinates[unitIndex];
                    unitCoordinates = groupCoordinates + new Coordinates(offsetCoordinates.X * c - offsetCoordinates.Y * s, offsetCoordinates.X * s + offsetCoordinates.Y * c);
                }
                else // No fixed coordinates, generate random coordinates
                {
                    switch (unitDB.Category)
                    {
                        case UnitCategory.Ship:
                            unitCoordinates = groupCoordinates.CreateNearRandom(SHIP_UNIT_SPACING, SHIP_UNIT_SPACING * 10);
                            break;
                        case UnitCategory.Static:
                            // Static units are spawned exactly on the group location (and there's only a single unit per group)
                            break;
                        default:
                            unitCoordinates = groupCoordinates.CreateNearRandom(VEHICLE_UNIT_SPACING, VEHICLE_UNIT_SPACING * 10);
                            break;
                    }
                }

                if (unitDB.OffsetHeading.Length > unitIndex) // Unit has a fixed heading (for SAM sites, etc.)
                    unitHeading = Toolbox.ClampAngle(unitHeading + unitDB.OffsetHeading[unitIndex]);
                else if (unitDB.Category != UnitCategory.Ship)
                    unitHeading = Toolbox.RandomDouble(Toolbox.TWO_PI);
            }
        }

        public void Dispose()
        {
            CallsignGenerator.Dispose();
            SpawnPointSelector.Dispose();
        }
    }
}

//namespace BriefingRoom4DCS.Generator
//{
//    internal class UnitMaker : IDisposable
//    {
//        private int NextGroupID;
//        private int NextUnitID;

//        internal UnitMakerSpawnPointSelector SpawnPointSelector { get; }
//        internal UnitMakerCallsignGenerator CallsignGenerator { get; }

//        internal UnitMaker(DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB)
//        {
//            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
//            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB);

//            NextGroupID = 1;
//            NextUnitID = 1;
//        }

//        internal DCSMissionUnitGroup AddUnitGroup(
//            DCSMission mission, string[] units, Side side,
//            Coordinates coordinates, string groupLua, string unitLua,
//            DCSSkillLevel skill, DCSMissionUnitGroupFlags flags = 0, AircraftPayload payload = AircraftPayload.Default,
//            Coordinates? coordinates2 = null, int airbaseID = 0, bool requiresParkingSpots = false, bool requiresOpenAirParking = false, Country? country = null, PlayerStartLocation startLocation = PlayerStartLocation.Runway)
//        {
//            if (units.Length == 0) return null; // No units database entries ID provided, cancel group creation

//            // TODO: check for missing units
//            DBEntryUnit[] unitsBP = (from string u in units where Database.Instance.EntryExists<DBEntryUnit>(u) select Database.Instance.GetEntry<DBEntryUnit>(u)).ToArray();
//            unitsBP = (from DBEntryUnit u in unitsBP where u != null select u).ToArray();
//            if (unitsBP.Length == 0) return null; // All database entries were null, cancel group creation

//            Coalition coalition = (side == Side.Ally) ? mission.CoalitionPlayer : mission.CoalitionEnemy; // Pick group coalition
//            if(!country.HasValue)
//                country = coalition == Coalition.Blue? Country.CJTFBlue : Country.CJTFRed;

//            double groupHeading = unitsBP[0].IsAircraft ? 0 : Toolbox.RandomDouble(Toolbox.TWO_PI); // Generate global group heading

//            // Generate units in the group
//            int unitIndex = 0;
//            Coordinates? lastSpot = null;
//            List<DCSMissionUnitGroupUnit> groupUnits = new List<DCSMissionUnitGroupUnit>();
//            foreach (DBEntryUnit unitBP in unitsBP)
//            {
//                if (unitBP == null) continue;

//                for (int i = 0; i < unitBP.DCSIDs.Length; i++)
//                {
//                    // Set unit coordinates and heading
//                    Coordinates unitCoordinates = coordinates;
//                    double unitHeading = groupHeading;

//                    SetUnitCoordinatesAndHeading(ref unitCoordinates, ref unitHeading, unitBP, unitIndex);

//                    // Get parking spot for the unit, if unit is parked at an airdrome
//                    int parkingSpot = 0;
//                    if (airbaseID > 0)
//                    {
//                        if (requiresParkingSpots)
//                        {
//                            parkingSpot = SpawnPointSelector.GetFreeParkingSpot(airbaseID, lastSpot, out Coordinates parkingCoordinates, requiresOpenAirParking);
//                            if (parkingSpot >= 0)
//                               unitCoordinates = parkingCoordinates;
//                            else
//                               parkingSpot = 0;
//                            lastSpot = unitCoordinates;
//                        }
//                    } else if(airbaseID == -99) //carrier code always parks 1 maybe will need more
//                        parkingSpot = 1;
//                    // Add unit to the list of units
//                    DCSMissionUnitGroupUnit unit = new DCSMissionUnitGroupUnit
//                    {
//                        Coordinates = unitCoordinates,
//                        Heading = unitHeading,
//                        ID = NextUnitID,
//                        Type = unitBP.DCSIDs[i],
//                        ParkingSpot = parkingSpot,
//                        Name = unitBP.ID
//                    };
//                    groupUnits.Add(unit);
//                    unitIndex++; NextUnitID++;
//                }
//            }

//            // Generate group name
//            string groupName;
//            UnitCallsign callsign = new UnitCallsign();
//            if (unitsBP[0].IsAircraft) // Aircraft group, name is a callsign
//            {
//                callsign = CallsignGenerator.GetCallsign(unitsBP[0].Families[0], coalition);
//                groupName = callsign.GroupName;
//            }
//            else // Vehicle/ship/static group, name is a random group name
//                groupName = GetGroupName(unitsBP[0].Families[0]);

//            // Add group to the mission
//            DCSMissionUnitGroup group = new DCSMissionUnitGroup
//            {
//                AirbaseID = airbaseID,
//                CallsignLua = callsign.Lua,
//                Category = unitsBP[0].Category,
//                Coalition = coalition,
//                Country = country.Value,
//                Coordinates = airbaseID != 0?  groupUnits[0].Coordinates : coordinates,
//                Coordinates2 = coordinates2 ?? coordinates + Coordinates.CreateRandom(1, 2) * Toolbox.NM_TO_METERS,
//                Flags = flags,
//                GroupID = NextGroupID,
//                LuaGroup = groupLua,
//                Name = groupName,
//                Skill = skill,
//                Payload = payload,
//                UnitID = units[0],
//                LuaUnit = unitLua,
//                Units = groupUnits.ToArray(),
//                StartLocation = startLocation
//            };
//            mission.UnitGroups.Add(group);

//            NextGroupID++;

//            BriefingRoom.PrintToLog($"Added \"{group.Units[0].Type}\" unit group \"{group.Name}\" for coalition {group.Coalition.ToString().ToUpperInvariant()}");

//            return group;
//        }

//        private void SetUnitCoordinatesAndHeading(ref Coordinates unitCoordinates, ref double unitHeading, DBEntryUnit unitBP, int unitIndex)
//        {
//            if (unitBP.IsAircraft)
//                unitCoordinates += new Coordinates(AIRCRAFT_UNIT_SPACING, AIRCRAFT_UNIT_SPACING) * unitIndex;
//            else
//            {
//                if (unitBP.OffsetCoordinates.Length > unitIndex) // Unit has a fixed set of coordinates (for SAM sites, etc.)
//                {
//                    double s = Math.Sin(unitHeading);
//                    double c = Math.Cos(unitHeading);
//                    Coordinates offsetCoordinates= unitBP.OffsetCoordinates[unitIndex];
//                    unitCoordinates += new Coordinates( offsetCoordinates.X * c - offsetCoordinates.Y * s, offsetCoordinates.X * s + offsetCoordinates.Y * c);
//                }
//                else // No fixed coordinates, generate random coordinates
//                {
//                    switch (unitBP.Category)
//                    {
//                        case UnitCategory.Ship:
//                            unitCoordinates = unitCoordinates.CreateNearRandom(SHIP_UNIT_SPACING, SHIP_UNIT_SPACING * 10);
//                            break;
//                        case UnitCategory.Static: // Static units are spawned exactly on the group location (and there's only a single unit per group)
//                            break;
//                        default:
//                            unitCoordinates = unitCoordinates.CreateNearRandom(VEHICLE_UNIT_SPACING, VEHICLE_UNIT_SPACING * 10);
//                            break;
//                    }
//                }

//                if (unitBP.OffsetHeading.Length > unitIndex) // Unit has a fixed heading (for SAM sites, etc.)
//                    unitHeading = Toolbox.ClampAngle(unitHeading + unitBP.OffsetHeading[unitIndex]); // editor looks odd but works fine if negative or over 2Pi
//                else if(unitBP.Category != UnitCategory.Ship)
//                    unitHeading = Toolbox.RandomDouble(Toolbox.TWO_PI);
//            }
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }
//    }
//}
