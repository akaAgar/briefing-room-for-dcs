using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BriefingRoom4DCS.Generator
{
    internal struct UnitMakerGroupInfo
    {
        internal Coordinates Coordinates { get; }
        internal int GroupID { get; }
        internal string Name { get; }
        internal int[] UnitsID { get; }

        internal double Frequency { get; }

        internal UnitMakerGroupInfo(int groupID, Coordinates coordinates, List<int> unitsID, string name, double frequency = 0.0)
        {
            GroupID = groupID;
            Coordinates = coordinates;
            Name = name;
            UnitsID = unitsID.ToArray();
            Frequency = frequency;
        }
    }

    internal class UnitMaker : IDisposable
    {
        private const double AIRCRAFT_UNIT_SPACING = 50.0;
        private const double SHIP_UNIT_SPACING = 100.0;
        private const double VEHICLE_UNIT_SPACING = 20.0;

        private readonly DCSMission Mission;
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
            DCSMission mission, MissionTemplate template,
            DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB,
            Coalition playerCoalition, Country[][] coalitionsCountries)
        {
            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB);

            Mission = mission;
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
            Coordinates coordinates, DCSSkillLevel? skill = null,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            AircraftPayload aircraftPayload = AircraftPayload.Default,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (unitCount <= 0) return null;
            DBEntryCoalition unitsCoalitionDB = CoalitionsDB[(int)((side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy())];
            
            string[] units = unitsCoalitionDB.GetRandomUnits(family, Template.ContextDecade, unitCount, Template.Mods, true);
            if (units.Length == 0) return null;

            return AddUnitGroup(units, side, family, groupLua, unitLua, coordinates, skill, unitMakerGroupFlags, aircraftPayload, extraSettings);
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            string[] units, Side side, UnitFamily unitFamily,
            string groupTypeLua, string unitTypeLua,
            Coordinates coordinates, DCSSkillLevel? skill = null,
            UnitMakerGroupFlags unitMakerGroupFlags = 0, 
            AircraftPayload aircraftPayload = AircraftPayload.Default,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (units.Length == 0) return null;

            Coalition coalition = (side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy();
            Country country = (coalition == Coalition.Blue) ? Country.CJTFBlue : Country.CJTFRed;

            if (!skill.HasValue)
                skill = GeneratorTools.GetDefaultSkillLevel(Template, unitFamily, side);

            string groupName;
            UnitCallsign? callsign = null;
            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                callsign = CallsignGenerator.GetCallsign(unitFamily, coalition);
                groupName = callsign.Value.GroupName;
            }
            else
                groupName = GeneratorTools.GetGroupName(GroupID, unitFamily);

            

            var groupLua = CreateGroup(
                groupTypeLua,
                coordinates,
                groupName,
                extraSettings
            );
            

            int firstUnitID = UnitID;
            var (unitsLuaTable, unitsIDList) = AddUnits(
                units,
                groupName,
                callsign,
                unitTypeLua,
                coordinates,
                aircraftPayload,
                unitMakerGroupFlags,
                extraSettings
            );


            if (unitsIDList.Count == 0) return null; // No valid units added to this group
            GeneratorTools.ReplaceKey(ref groupLua, "Units", unitsLuaTable);
                
            DBEntryUnit firstUnitDB = units.Select(x => Database.Instance.GetEntry<DBEntryUnit>(x)).First(x => x != null);
            var aircraftCategories = new UnitCategory[]{UnitCategory.Helicopter, UnitCategory.Plane};
            var isAircraft = firstUnitDB != null && aircraftCategories.Contains(firstUnitDB.Category);
    
            if(isAircraft)
            {
                groupLua = ApplyAircraftFields(groupLua, firstUnitDB);
                if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.ImmediateAircraftSpawn))
                    Mission.AppendValue("AircraftActivatorCurrentQueue", $"{GroupID},");
                else
                    Mission.AppendValue("AircraftActivatorReserveQueue", $"{GroupID},");
            }

            GeneratorTools.ReplaceKey(ref groupLua, "UnitID", firstUnitID); // Must be after units are added
            GeneratorTools.ReplaceKey(ref groupLua, "Skill", skill.Value); // Must be after units are added, because skill is set as a unit level
            GeneratorTools.ReplaceKey(ref groupLua, "Hidden", GeneratorTools.GetHiddenStatus(Template.OptionsFogOfWar, side, unitMakerGroupFlags)); // If "hidden" was not set through custom values

            AddUnitGroupToTable(country, unitFamily.GetUnitCategory(), groupLua);

            BriefingRoom.PrintToLog($"Added group of {units.Length} {coalition} {unitFamily} at {coordinates}");

            GroupID++;

            if (firstUnitDB == null)
                return new UnitMakerGroupInfo(GroupID - 1, coordinates, unitsIDList, groupName);
            return new UnitMakerGroupInfo(GroupID - 1, coordinates, unitsIDList, groupName, firstUnitDB.AircraftData.RadioFrequency);
        }


        private string CreateGroup(
            string groupTypeLua,
            Coordinates coordinates,
            string groupName,
            params KeyValuePair<string, object>[] extraSettings
        )
        {
            string lua = File.ReadAllText($"{BRPaths.INCLUDE_LUA_UNITS}{Toolbox.AddMissingFileExtension(groupTypeLua, ".lua")}");
            foreach (KeyValuePair<string, object> extraSetting in extraSettings) // Replace custom values first so they override other replacements
                if (!(extraSetting.Value is Array)) // Array extra settings are treated on a per-unit basis
                    GeneratorTools.ReplaceKey(ref lua, extraSetting.Key, extraSetting.Value);
            
            GeneratorTools.ReplaceKey(ref lua, "GroupID", GroupID);
            GeneratorTools.ReplaceKey(ref lua, "GroupX", coordinates.X);
            GeneratorTools.ReplaceKey(ref lua, "GroupY", coordinates.Y);
            GeneratorTools.ReplaceKey(ref lua, "GroupX2", coordinates.X); // GroupX2 and GroupY2 are replaced by the default coordinates only if they were not replaced earlier in extraSetting replacements. 
            GeneratorTools.ReplaceKey(ref lua, "GroupY2", coordinates.Y);
            GeneratorTools.ReplaceKey(ref lua, "Name", groupName);

            return lua;
        }

        private string ApplyAircraftFields(string groupLua, DBEntryUnit firstUnitDB){
            GeneratorTools.ReplaceKey(ref groupLua, "Altitude", firstUnitDB.AircraftData.CruiseAltitude);
            GeneratorTools.ReplaceKey(ref groupLua, "AltitudeHalf", firstUnitDB.AircraftData.CruiseAltitude / 2);
            GeneratorTools.ReplaceKey(ref groupLua, "EPLRS", firstUnitDB.Flags.HasFlag(DBEntryUnitFlags.EPLRS));
            GeneratorTools.ReplaceKey(ref groupLua, "ParkingID", 0);
            GeneratorTools.ReplaceKey(ref groupLua, "RadioBand", (int)firstUnitDB.AircraftData.RadioModulation);
            GeneratorTools.ReplaceKey(ref groupLua, "RadioFrequency", firstUnitDB.AircraftData.RadioFrequency);
            GeneratorTools.ReplaceKey(ref groupLua, "Speed", firstUnitDB.AircraftData.CruiseSpeed);
            return groupLua;
        }

        private (string unitsLua, List<int> unitsIDList) AddUnits(
            string[] unitSets,
            string groupName,
            UnitCallsign? callsign,
            string unitTypeLua,
            Coordinates coordinates,
            AircraftPayload aircraftPayload,
            UnitMakerGroupFlags unitMakerGroupFlags,
            params KeyValuePair<string, object>[] extraSettings
            )
        {
            string unitsLuaTable = "";
            int unitLuaIndex = 1;
            List<int> unitsIDList = new List<int>();
            foreach (var unitSet in unitSets)
            {
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(unitSet);
                if (unitDB == null)
                {
                    BriefingRoom.PrintToLog($"Unit \"{unitSet}\" not found.", LogMessageErrorLevel.Warning);
                    continue;
                }
                int unitSetIndex = 0;
                foreach (string DCSID in unitDB.DCSIDs)
                {
                    unitsLuaTable += AddUnit(
                        DCSID,
                        groupName,
                        callsign,
                        unitLuaIndex,
                        unitSetIndex,
                        unitDB,
                        unitTypeLua,
                        coordinates,
                        aircraftPayload,
                        unitMakerGroupFlags,
                        extraSettings
                        );

                    unitsIDList.Add(UnitID);
                    unitSetIndex++;
                    unitLuaIndex++;
                    UnitID++;   
                }
            }
            return (unitsLuaTable, unitsIDList);
        }


        private string AddUnit(
            string DCSID,
            string groupName,
            UnitCallsign? callsign,
            int unitLuaIndex,
            int unitSetIndex,
            DBEntryUnit unitDB,
            string unitTypeLua,
            Coordinates coordinates,
            AircraftPayload aircraftPayload,
            UnitMakerGroupFlags unitMakerGroupFlags,
            params KeyValuePair<string, object>[] extraSettings)
        {
            string unitLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_UNITS}{Toolbox.AddMissingFileExtension(unitTypeLua, ".lua")}");
            var groupHeading = GetGroupHeading(coordinates, extraSettings);
            SetUnitCoordinatesAndHeading(unitDB, unitSetIndex, coordinates,  groupHeading, out Coordinates unitCoordinates, out double unitHeading);
            // LEAVE UNTIL SURE ALL BUGS GONE
            if (unitLuaIndex == 1) {
                Console.WriteLine($"Group Heading for {DCSID}: {groupHeading}");
                Console.WriteLine("TEST BLOCK -> GetGroupHeading");
                Console.WriteLine($"{GetGroupHeading(new Coordinates(0,0),"GroupX2".ToKeyValuePair(0.0),"GroupY2".ToKeyValuePair(1.0))} = 1.5708");
                Console.WriteLine($"{GetGroupHeading(new Coordinates(0,0),"GroupX2".ToKeyValuePair(1.0),"GroupY2".ToKeyValuePair(1.0))} = 1.5708");
                Console.WriteLine($"{GetGroupHeading(new Coordinates(0,0),"GroupX2".ToKeyValuePair(1.0),"GroupY2".ToKeyValuePair(0.0))} = 0");
                Console.WriteLine($"{GetGroupHeading(new Coordinates(0,0),"GroupX2".ToKeyValuePair(-1.0),"GroupY2".ToKeyValuePair(0.0))} = 3.14159");
                Console.WriteLine("TEST BLOCK -> TransformFromOffset");
                Console.WriteLine($"{TransformFromOffset(1.5708, new Coordinates(0,0), new Coordinates(3,4))} = (X: 3.99999, Y: -3.00001)");
                Console.WriteLine($"{TransformFromOffset(3.14159, new Coordinates(0,0), new Coordinates(3,4))} = (X: -2.9999, Y: -4)");
                Console.WriteLine($"{TransformFromOffset(4.71239, new Coordinates(0,0), new Coordinates(3,4))} = (X: -4, Y: -3)");
                Console.WriteLine($"{TransformFromOffset(1.5708, new Coordinates(0,0), new Coordinates(15,3))} = (X: 2.99994, Y: -15)");
                Console.WriteLine($"{TransformFromOffset(3.14159, new Coordinates(0,0), new Coordinates(15,3))} = (X: -15, Y: 3)");
                Console.WriteLine("DONE");
            }

                    string singleUnitLuaTable = String.Copy(unitLuaTemplate);
                    foreach (KeyValuePair<string, object> extraSetting in extraSettings) // Replace custom values first so they override other replacements
                        if (extraSetting.Value is Array)
                            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, extraSetting.Key, extraSetting.Value, unitSetIndex);
                        else
                            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, extraSetting.Key, extraSetting.Value);

                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "ExtraLua", unitDB.ExtraLua);
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Heading", unitHeading);
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "DCSID", DCSID);
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitID", UnitID);
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitX", unitCoordinates.X);
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitY", unitCoordinates.Y);
                    if ((unitDB.Category == UnitCategory.Helicopter) || (unitDB.Category == UnitCategory.Plane))
                    {
                        if ((unitLuaIndex == 1) && unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.FirstUnitIsPlayer))
                            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Skill", "Client"); //Must Always be client to support SP and MP AI wingmen

                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Callsign", callsign.Value.GetLua(unitLuaIndex + 1));
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Name", callsign.Value.GetUnitName(unitLuaIndex + 1));
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "OnBoardNumber", Toolbox.RandomInt(1, 1000).ToString("000"));
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PropsLua", unitDB.AircraftData.PropsLua);
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "RadioPresetsLua", string.Join("", unitDB.AircraftData.RadioPresets.Select((x, index) => $"[{index + 1}] = {x.ToLuaString()}")));
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Speed", unitDB.AircraftData.CruiseSpeed);
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PayloadCommon", unitDB.AircraftData.PayloadCommon);
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PayloadPylons", unitDB.AircraftData.GetPayloadLua(aircraftPayload, Template.ContextDecade));
                    }
                    else
                        GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Name", $"{groupName} {unitLuaIndex + 1}");

                    var unitString = $"[{unitLuaIndex}] =\n";
                    unitString += "{\n";
                    unitString += $"{singleUnitLuaTable}\n";
                    unitString += $"}}, -- end of [{unitLuaIndex}]\n";

                    return unitString;
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
                    unitsLuaTable += "[\"group\"] =\n";
                    unitsLuaTable += "{\n";
                        for (int groupIndex = 0; groupIndex < UnitLuaTables[country][unitCategory].Count; groupIndex++)
                    {
                        unitsLuaTable += $"[{groupIndex + 1}] =\n";
                        unitsLuaTable += "{\n";
                        unitsLuaTable += $"{UnitLuaTables[country][unitCategory][groupIndex]}\n";
                        unitsLuaTable += $"}}, -- end of [{groupIndex + 1}]\n";
                    }

                    unitsLuaTable += $"}}, -- end of [\"group\"]\n";
                    unitsLuaTable += $"}}, -- end of [\"{unitCategory.ToString().ToLowerInvariant()}\"]\n";
                }

                unitsLuaTable += $"}}, -- end of [{countryIndex + 1}]\n";
            }

            return unitsLuaTable;
        }

        private double GetGroupHeading(Coordinates groupCoordinates, params KeyValuePair<string, object>[] extraSettings){
            if(!extraSettings.Any(x => x.Key == "GroupX2"))
                return 0.0;
            var waypointCoor = new Coordinates((double)extraSettings.First(x => x.Key == "GroupX2").Value, (double)extraSettings.First(x => x.Key == "GroupY2").Value);
            return Coordinates.ToAngleInRadians(groupCoordinates, waypointCoor);
        }

        private void SetUnitCoordinatesAndHeading(
            DBEntryUnit unitDB, int unitIndex, Coordinates groupCoordinates, double groupHeading,
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
                    Coordinates offsetCoordinates = unitDB.OffsetCoordinates[unitIndex];
                    unitCoordinates = TransformFromOffset(groupHeading, groupCoordinates, offsetCoordinates);
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

        private Coordinates TransformFromOffset(double groupHeading, Coordinates groupCoordinates, Coordinates offsetCoordinates)
        {
                    double sinTheta = Math.Sin(groupHeading);
                    double cosTheta = Math.Cos(groupHeading);
                    return groupCoordinates + new Coordinates(
                        (offsetCoordinates.X * cosTheta) + (offsetCoordinates.Y * sinTheta),
                        (-offsetCoordinates.X * sinTheta) + (offsetCoordinates.Y * cosTheta));
        }

        public void Dispose()
        {
            CallsignGenerator.Dispose();
            SpawnPointSelector.Dispose();
        }
    }
}