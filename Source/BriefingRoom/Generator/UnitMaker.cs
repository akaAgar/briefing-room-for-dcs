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

        internal DBEntryUnit UnitDB { get; }

        internal UnitMakerGroupInfo(int groupID, Coordinates coordinates, List<int> unitsID, string name, double frequency = 0.0, DBEntryUnit unitDB = null)
        {
            GroupID = groupID;
            Coordinates = coordinates;
            Name = name;
            UnitsID = unitsID.ToArray();
            Frequency = frequency;
            UnitDB = unitDB;
        }
    }

    internal class UnitMaker
    {
        private const double AIRCRAFT_UNIT_SPACING = 50.0;
        private const double SHIP_UNIT_SPACING = 100.0;
        private const double VEHICLE_UNIT_SPACING = 20.0;

        private readonly DCSMission Mission;
        private readonly MissionTemplateRecord Template;
        private readonly DBEntryCoalition[] CoalitionsDB;
        private readonly Coalition PlayerCoalition;
        private readonly Country[][] CoalitionsCountries;

        private readonly Dictionary<Country, Dictionary<UnitCategory, List<string>>> UnitLuaTables = new Dictionary<Country, Dictionary<UnitCategory, List<string>>>();

        private int GroupID;
        private int UnitID;
        private bool SinglePlayerMission;

        internal UnitMakerSpawnPointSelector SpawnPointSelector { get; }

        internal UnitMakerCallsignGenerator CallsignGenerator { get; }

        internal UnitMaker(
            DCSMission mission, MissionTemplateRecord template,
            DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB, DBEntrySituation situationDB,
            Coalition playerCoalition, Country[][] coalitionsCountries,
            bool singlePlayerMission)
        {
            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB, situationDB, template.OptionsMission.Contains("InvertCountriesCoalitions"));

            Mission = mission;
            Template = template;

            CoalitionsDB = coalitionsDB;
            PlayerCoalition = playerCoalition;
            CoalitionsCountries = coalitionsCountries;
            SinglePlayerMission = singlePlayerMission;

            GroupID = 1;
            UnitID = 1;
        }

        // Removing this overload breaks backwards compatability with MissionGeneratorCarrierGroup and MissionGeneratorCombatAirPatrol
        internal UnitMakerGroupInfo? AddUnitGroup(
            UnitFamily family, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            params KeyValuePair<string, object>[] extraSettings) => AddUnitGroup(
                new List<UnitFamily> { family }, unitCount, side,
                groupLua, unitLua, coordinates,
                null, unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            List<UnitFamily> families, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            params KeyValuePair<string, object>[] extraSettings) => AddUnitGroup(
                families, unitCount, side,
                groupLua, unitLua, coordinates,
                null, unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            UnitFamily family, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            MinMaxI? unitCountMinMax,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            params KeyValuePair<string, object>[] extraSettings) => AddUnitGroup(
                new List<UnitFamily> { family }, unitCount, side,
                groupLua, unitLua, coordinates,
                unitCountMinMax,
                unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            List<UnitFamily> families, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            MinMaxI? unitCountMinMax,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (unitCount <= 0) throw new BriefingRoomException("Asking for a zero units");
            if (families.Count <= 0) throw new BriefingRoomException("No Unit Families Provided");
            DBEntryCoalition unitsCoalitionDB = CoalitionsDB[(int)((side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy())];

            var (country, units) = unitsCoalitionDB.GetRandomUnits(families, Template.ContextDecade, unitCount, Template.Mods, countMinMax: unitCountMinMax);
            if (units.Count == 0) throw new BriefingRoomException($"Found no units for {string.Join(", ", families)} {country}");
            if (country != Country.ALL)
                extraSettings = extraSettings.Append("Country".ToKeyValuePair(country)).ToArray();


            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.EmbeddedAirDefense) && (families.First().GetUnitCategory() == UnitCategory.Vehicle))
            {
                string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(Template, side, country != Country.ALL ? country : null);
                units.AddRange(airDefenseUnits);
            }

            return AddUnitGroup(Toolbox.ShuffleArray(units.ToArray()), side, families.First(), groupLua, unitLua, coordinates, unitMakerGroupFlags, extraSettings);
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            string[] units, Side side, UnitFamily unitFamily,
            string groupTypeLua, string unitTypeLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags = 0,
            params KeyValuePair<string, object>[] extraSettings)
        {
            if (units.Length == 0) return null;

            Coalition coalition = (side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy();
            Country country = (coalition == Coalition.Blue) ? Country.CJTFBlue : Country.CJTFRed;

            if (extraSettings.Any(x => x.Key == "Country"))
                country = (Country)extraSettings.First(x => x.Key == "Country").Value;

            var skill = GeneratorTools.GetDefaultSkillLevel(Template, side);

            if (extraSettings.Any(x => x.Key == "Skill"))
                skill = (DCSSkillLevel)extraSettings.First(x => x.Key == "Skill").Value;


            var isUsingSkynet = Template.MissionFeatures.Contains("SkynetIADS");
            string groupName;
            UnitCallsign? callsign = null;
            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                callsign = CallsignGenerator.GetCallsign(unitFamily, coalition, side, isUsingSkynet);
                groupName = callsign.Value.GroupName;
                if (extraSettings.Any(x => x.Key == "PlayerStartingType") && extraSettings.First(x => x.Key == "PlayerStartingType").Value.ToString() == "TakeOffParking")
                    groupName += "(C)";
            }
            else
                groupName = GeneratorTools.GetGroupName(GroupID, unitFamily, side, isUsingSkynet);

            if (unitFamily.GetUnitCategory() == UnitCategory.Static || unitFamily.GetUnitCategory() == UnitCategory.Cargo && unitFamily != UnitFamily.FOB)
                return AddStaticGroup(
                    country,
                    coalition,
                    skill,
                    unitFamily,
                    side,
                    units,
                    groupName,
                    callsign,
                    groupTypeLua,
                    coordinates,
                    unitMakerGroupFlags,
                    extraSettings
                );

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
                unitMakerGroupFlags,
                extraSettings
            );


            if (unitsIDList.Count == 0) return null; // No valid units added to this group
            GeneratorTools.ReplaceKey(ref groupLua, "Units", unitsLuaTable);

            DBEntryUnit firstUnitDB = units.Select(x => Database.Instance.GetEntry<DBEntryUnit>(x)).First(x => x != null);
            var aircraftCategories = new UnitCategory[] { UnitCategory.Helicopter, UnitCategory.Plane };
            var isAircraft = firstUnitDB != null && aircraftCategories.Contains(firstUnitDB.Category);

            if (isAircraft)
            {
                groupLua = ApplyAircraftFields(groupLua, firstUnitDB, extraSettings);
                if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.ImmediateAircraftSpawn))
                    Mission.AppendValue("AircraftActivatorCurrentQueue", $"{GroupID},");
                else if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.RadioAircraftSpawn))
                    Mission.AppendValue("AircraftRadioActivator", $"{{{GroupID}, \"{groupName}\"}},");
                else if (groupTypeLua != "GroupAircraftParkedUncontrolled")
                    Mission.AppendValue("AircraftActivatorReserveQueue", $"{GroupID},");
            }

            GeneratorTools.ReplaceKey(ref groupLua, "UnitID", firstUnitID); // Must be after units are added
            GeneratorTools.ReplaceKey(ref groupLua, "Skill", skill); // Must be after units are added, because skill is set as a unit level
            GeneratorTools.ReplaceKey(ref groupLua, "Hidden", GeneratorTools.GetHiddenStatus(Template.OptionsFogOfWar, side, unitMakerGroupFlags)); // If "hidden" was not set through custom values

            AddUnitGroupToTable(country, unitFamily.GetUnitCategory(), groupLua);

            BriefingRoom.PrintToLog($"Added group of {units.Length} {coalition} {unitFamily} at {coordinates}");

            GroupID++;

            if (firstUnitDB == null)
                return new UnitMakerGroupInfo(GroupID - 1, coordinates, unitsIDList, groupName);
            return new UnitMakerGroupInfo(GroupID - 1, coordinates, unitsIDList, groupName, firstUnitDB.AircraftData.RadioFrequency, firstUnitDB);
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

        private string ApplyAircraftFields(string groupLua, DBEntryUnit firstUnitDB, params KeyValuePair<string, object>[] extraSettings)
        {
            GeneratorTools.ReplaceKey(ref groupLua, "Altitude", firstUnitDB.AircraftData.CruiseAltitude);
            GeneratorTools.ReplaceKey(ref groupLua, "AltitudeHalf", firstUnitDB.AircraftData.CruiseAltitude / 2);
            GeneratorTools.ReplaceKey(ref groupLua, "EPLRS", firstUnitDB.Flags.HasFlag(DBEntryUnitFlags.EPLRS));
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

        private UnitMakerGroupInfo? AddStaticGroup(
            Country country,
            Coalition coalition,
            DCSSkillLevel? skill,
            UnitFamily unitFamily,
            Side side,
            string[] unitSets,
            string groupName,
            UnitCallsign? callsign,
            string groupTypeLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            params KeyValuePair<string, object>[] extraSettings
        )
        {
            List<int> unitsIDList = new List<int>();
            var initalGroupId = GroupID;
            foreach (var unitSet in unitSets)
            {
                DBEntryUnit unitDB = Database.Instance.GetEntry<DBEntryUnit>(unitSet);
                if (unitDB == null)
                {
                    BriefingRoom.PrintToLog($"Unit \"{unitSet}\" not found.", LogMessageErrorLevel.Warning);
                    continue;
                }
                int unitSetIndex = 0;
                foreach (var DCSID in unitDB.DCSIDs)
                {
                    var groupHeading = GetGroupHeading(coordinates, extraSettings);
                    SetUnitCoordinatesAndHeading(unitDB, unitSetIndex, coordinates, groupHeading, out Coordinates unitCoordinates, out double unitHeading);
                    var firstUnitID = UnitID;
                    var groupLua = CreateGroup(
                        groupTypeLua,
                        unitCoordinates,
                        groupName,
                        extraSettings
                    );
                    var unitLua = DCSID == "FARP" ? "UnitStaticFOB" : (unitDB.Category == UnitCategory.Cargo ? "UnitCargo" : "UnitStatic");
                    var unitsLuaTable = AddUnit(
                        DCSID,
                        groupName,
                        callsign,
                        1,
                        unitSetIndex,
                        unitDB,
                        unitLua,
                        coordinates,
                        unitMakerGroupFlags,
                        extraSettings
                        );

                    unitsIDList.Add(UnitID);
                    unitSetIndex++;
                    UnitID++;

                    GeneratorTools.ReplaceKey(ref groupLua, "Units", unitsLuaTable);


                    GeneratorTools.ReplaceKey(ref groupLua, "UnitID", firstUnitID); // Must be after units are added
                    GeneratorTools.ReplaceKey(ref groupLua, "Skill", skill); // Must be after units are added, because skill is set as a unit level
                    GeneratorTools.ReplaceKey(ref groupLua, "Hidden", GeneratorTools.GetHiddenStatus(Template.OptionsFogOfWar, side, unitMakerGroupFlags)); // If "hidden" was not set through custom values

                    AddUnitGroupToTable(country, UnitCategory.Static, groupLua);

                    BriefingRoom.PrintToLog($"Added group of {DCSID} {coalition} {unitFamily} at {coordinates}");

                    GroupID++;
                }
            }

            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.EmbeddedAirDefense) && unitFamily != UnitFamily.StaticStructureOffshore)
            {
                var firstUnitID = UnitID;
                string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(Template, side);
                var groupLua = CreateGroup(
                        "GroupVehicle",
                        coordinates,
                        groupName,
                        extraSettings
                    );
                var (unitsLuaTable, embeddedunitsIDList) = AddUnits(
                    airDefenseUnits,
                    groupName,
                    callsign,
                    "UnitVehicle",
                    coordinates,
                    unitMakerGroupFlags,
                    extraSettings
                );
                GeneratorTools.ReplaceKey(ref groupLua, "Units", unitsLuaTable);
                GeneratorTools.ReplaceKey(ref groupLua, "UnitID", firstUnitID); // Must be after units are added
                GeneratorTools.ReplaceKey(ref groupLua, "Skill", skill); // Must be after units are added, because skill is set as a unit level
                GeneratorTools.ReplaceKey(ref groupLua, "Hidden", GeneratorTools.GetHiddenStatus(Template.OptionsFogOfWar, side, unitMakerGroupFlags)); // If "hidden" was not set through custom values
                GroupID++;
                unitsIDList.AddRange(embeddedunitsIDList);
                AddUnitGroupToTable(country, UnitCategory.Vehicle, groupLua);
                BriefingRoom.PrintToLog($"Added group of Embedded Air Defense for Static {coalition} {unitFamily} at {coordinates}");
            }

            DBEntryUnit firstUnitDB = Database.Instance.GetEntry<DBEntryUnit>(unitSets.First());
            if (firstUnitDB == null)
                return new UnitMakerGroupInfo(initalGroupId, coordinates, unitsIDList, groupName);
            return new UnitMakerGroupInfo(initalGroupId, coordinates, unitsIDList, groupName, firstUnitDB.AircraftData.RadioFrequency, firstUnitDB);
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
            UnitMakerGroupFlags unitMakerGroupFlags,
            params KeyValuePair<string, object>[] extraSettings)
        {
            string unitLuaTemplate = File.ReadAllText($"{BRPaths.INCLUDE_LUA_UNITS}{Toolbox.AddMissingFileExtension(unitTypeLua, ".lua")}");
            var groupHeading = GetGroupHeading(coordinates, extraSettings);
            SetUnitCoordinatesAndHeading(unitDB, unitSetIndex, coordinates, groupHeading, out Coordinates unitCoordinates, out double unitHeading);

            string singleUnitLuaTable = new string(unitLuaTemplate);
            if (Toolbox.IsAircraft(unitDB.Category) && (unitLuaIndex == 1) && unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.FirstUnitIsClient))
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Skill", SinglePlayerMission ? "Player" : "Client");

            foreach (KeyValuePair<string, object> extraSetting in extraSettings) // Replace custom values first so they override other replacements
                if (extraSetting.Value is Array)
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, extraSetting.Key, extraSetting.Value, unitLuaIndex - 1);
                else
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, extraSetting.Key, extraSetting.Value);

            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "ExtraLua", unitDB.ExtraLua);
            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Heading", unitHeading);
            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "DCSID", DCSID);
            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitID", UnitID);
            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitX", unitCoordinates.X);
            GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "UnitY", unitCoordinates.Y);
            if (Toolbox.IsAircraft(unitDB.Category))
            {
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Callsign", callsign.Value.GetLua(unitLuaIndex));
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Name", callsign.Value.GetUnitName(unitLuaIndex));
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "OnBoardNumber", Toolbox.RandomInt(1, 1000).ToString("000"));
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PropsLua", unitDB.AircraftData.PropsLua);
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "RadioPresetsLua", string.Join("", unitDB.AircraftData.RadioPresets.Select((x, index) => $"[{index + 1}] = {x.ToLuaString()}")));
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Speed", unitDB.AircraftData.CruiseSpeed);
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PayloadCommon", unitDB.AircraftData.PayloadCommon);
                var payload = unitDB.AircraftData.GetPayloadLua(extraSettings.Any(x => x.Key == "Payload") ? extraSettings.First(x => x.Key == "Payload").Value.ToString() : "default");
                if (extraSettings.Any(x => x.Key == "Payload" && x.Value.ToString() == "EMPTY"))
                    payload = "";
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "PayloadPylons", payload);
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Livery", extraSettings.Any(x => x.Key == "Livery") ? extraSettings.First(x => x.Key == "Livery").Value : "default");
            }
            else if (unitDB.Category == UnitCategory.Static || unitDB.Category == UnitCategory.Cargo)
            {
                if (unitDB.Shape.Length - 1 > unitSetIndex)
                    GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Shape", unitDB.Shape[unitSetIndex]);
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Name", $"{groupName} {unitLuaIndex}");
            }
            else
                GeneratorTools.ReplaceKey(ref singleUnitLuaTable, "Name", $"{groupName} {unitLuaIndex}");

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


                unitsLuaTable += $"[{countryIndex + 1}] =\n";
                unitsLuaTable += "{\n";
                unitsLuaTable += $"[\"id\"] = {(int)country},\n";
                unitsLuaTable += $"[\"name\"] = \"{country}\",\n";


                if (UnitLuaTables.ContainsKey(country))
                {
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
                }
                unitsLuaTable += $"}}, -- end of [{countryIndex + 1}]\n";
            }

            return unitsLuaTable;
        }

        private double GetGroupHeading(Coordinates groupCoordinates, params KeyValuePair<string, object>[] extraSettings)
        {
            if (!extraSettings.Any(x => x.Key == "GroupX2"))
                return 0.0;
            var waypointCoor = new Coordinates((double)extraSettings.First(x => x.Key == "GroupX2").Value, (double)extraSettings.First(x => x.Key == "GroupY2").Value);
            return Coordinates.ToAngleInRadians(groupCoordinates, waypointCoor);
        }

        private void SetUnitCoordinatesAndHeading(
            DBEntryUnit unitDB, int unitIndex, Coordinates groupCoordinates, double groupHeading,
            out Coordinates unitCoordinates, out double unitHeading)
        {
            unitCoordinates = groupCoordinates;
            unitHeading = groupHeading;

            if (unitDB.IsAircraft)
                unitCoordinates = groupCoordinates + new Coordinates(AIRCRAFT_UNIT_SPACING, AIRCRAFT_UNIT_SPACING) * unitIndex;
            else
            {
                if (unitDB.OffsetCoordinates.Length > unitIndex) // Unit has a fixed set of coordinates (for SAM sites, etc.)
                {
                    Coordinates offsetCoordinates = unitDB.OffsetCoordinates[unitIndex];
                    unitCoordinates = TransformFromOffset(unitHeading, groupCoordinates, offsetCoordinates);
                }
                else // No fixed coordinates, generate random coordinates
                {
                    switch (unitDB.Category)
                    {
                        case UnitCategory.Ship:
                            if (unitIndex > 0)
                                unitCoordinates = groupCoordinates.CreateNearRandom(SHIP_UNIT_SPACING, SHIP_UNIT_SPACING * 10);
                            break;
                        case UnitCategory.Cargo:
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
            // it seems that for some reason X&Y are reversed when it comes to this stuff and needs to rotated backawards from heading.
            // Why don't know Maybe ED will announce its a bug and poof or soviet russia x is y and y is x
            // Its late my head hurts, be ware all who venture here.
            double sinTheta = Math.Sin(Toolbox.TWO_PI - groupHeading);
            double cosTheta = Math.Cos(Toolbox.TWO_PI - groupHeading);
            return groupCoordinates + new Coordinates(
                (offsetCoordinates.X * cosTheta) + (offsetCoordinates.Y * sinTheta),
                (-offsetCoordinates.X * sinTheta) + (offsetCoordinates.Y * cosTheta));
        }
    }
}