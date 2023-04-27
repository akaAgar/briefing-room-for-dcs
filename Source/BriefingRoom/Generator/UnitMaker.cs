using BriefingRoom4DCS.Data;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Mission.DCSLuaObjects;
using BriefingRoom4DCS.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BriefingRoom4DCS.Generator
{
    internal struct UnitMakerGroupInfo
    {
        internal Coordinates Coordinates { get { return new Coordinates(DCSGroup.X, DCSGroup.Y); } }
        internal int GroupID { get { return DCSGroup.GroupId; } }
        internal string Name { get { return DCSGroup.Name; } }
        internal string[] UnitNames { get { return DCSGroup.Units.Select(x => x.Name).ToArray(); } }

        internal double Frequency { get { return DCSGroup.Frequency; } }

        internal DBEntryJSONUnit UnitDB { get; }

        internal DCSGroup DCSGroup { get {return DCSGroups.First();} }
        internal List<DCSGroup> DCSGroups { get; }

        internal UnitMakerGroupInfo(ref DCSGroup dCSGroup, DBEntryJSONUnit unitDB = null)
        {
            UnitDB = unitDB;
            DCSGroups = new List<DCSGroup> { dCSGroup };
        }

        internal UnitMakerGroupInfo(ref List<DCSGroup> dCSGroups, DBEntryJSONUnit unitDB = null)
        {
            UnitDB = unitDB;
            DCSGroups = dCSGroups;
        }
    }

    internal class CarrierUnitMakerGroupInfo
    {
        internal UnitMakerGroupInfo UnitMakerGroupInfo { get; }
        internal int RemainingSpotCount { get; set; }
        internal int TotalSpotCount { get; init; }

        internal Coalition Coalition { get; init; }

        internal CarrierUnitMakerGroupInfo(UnitMakerGroupInfo unitMakerGroupInfo, int remainingSpotCount, Coalition coalition)
        {
            UnitMakerGroupInfo = unitMakerGroupInfo;
            RemainingSpotCount = remainingSpotCount;
            TotalSpotCount = remainingSpotCount;
            Coalition = coalition;
        }
    }

    internal class UnitMaker
    {
        private const double AIRCRAFT_UNIT_SPACING = 50.0;
        private const double SHIP_UNIT_SPACING = 100.0;
        private const double VEHICLE_UNIT_SPACING = 10.0;

        private readonly DCSMission Mission;
        private readonly MissionTemplateRecord Template;
        private readonly DBEntryCoalition[] CoalitionsDB;
        private readonly Coalition PlayerCoalition;
        private readonly Country[][] CoalitionsCountries;

        private readonly List<string> ModUnits = new List<string>();
        private readonly Dictionary<Country, Dictionary<DCSUnitCategory, List<DCSGroup>>> UnitLuaTables = new Dictionary<Country, Dictionary<DCSUnitCategory, List<DCSGroup>>>();

        private int GroupID;
        private int UnitID;
        private bool SinglePlayerMission;

        internal UnitMakerSpawnPointSelector SpawnPointSelector { get; }

        internal UnitMakerCallsignGenerator CallsignGenerator { get; }

        internal Dictionary<string, CarrierUnitMakerGroupInfo> carrierDictionary { get; } = new Dictionary<string, CarrierUnitMakerGroupInfo> { };
        private readonly List<string> IGNORE_PROPS = new List<string> { "Skill" };

        internal UnitMaker(
            DCSMission mission, MissionTemplateRecord template,
            DBEntryCoalition[] coalitionsDB, DBEntryTheater theaterDB, DBEntrySituation situationDB,
            Coalition playerCoalition, Country[][] coalitionsCountries,
            bool singlePlayerMission)
        {
            CallsignGenerator = new UnitMakerCallsignGenerator(coalitionsDB);
            SpawnPointSelector = new UnitMakerSpawnPointSelector(theaterDB, situationDB, template.OptionsMission.Contains("InvertCountriesCoalitions"), template.BorderLimit);

            Mission = mission;
            Template = template;

            CoalitionsDB = coalitionsDB;
            PlayerCoalition = playerCoalition;
            CoalitionsCountries = coalitionsCountries;
            SinglePlayerMission = singlePlayerMission;

            GroupID = 1;
            UnitID = 1;
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            UnitFamily family, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings) => AddUnitGroup(
                new List<UnitFamily> { family }, unitCount, side,
                groupLua, unitLua, coordinates,
                null, unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            List<UnitFamily> families, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings) => AddUnitGroup(
                families, unitCount, side,
                groupLua, unitLua, coordinates,
                null, unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            UnitFamily family, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            MinMaxI? unitCountMinMax,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings) => AddUnitGroup(
                new List<UnitFamily> { family }, unitCount, side,
                groupLua, unitLua, coordinates,
                unitCountMinMax,
                unitMakerGroupFlags, extraSettings);

        internal UnitMakerGroupInfo? AddUnitGroup(
            List<UnitFamily> families, int unitCount, Side side,
            string groupLua, string unitLua,
            Coordinates coordinates,
            MinMaxI? unitCountMinMax,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings)
        {
            if (unitCount <= 0) throw new BriefingRoomException("Asking for a zero units");
            if (families.Count <= 0) throw new BriefingRoomException("No Unit Families Provided");
            DBEntryCoalition unitsCoalitionDB = CoalitionsDB[(int)((side == Side.Ally) ? PlayerCoalition : PlayerCoalition.GetEnemy())];
            var (country, units) = unitsCoalitionDB.GetRandomUnits(families, Template.ContextDecade, unitCount, Template.Mods, Template.OptionsMission.Contains("AllowLowPoly"), countMinMax: unitCountMinMax,  lowUnitVariation: unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.LowUnitVariation));
            if (side == Side.Neutral)
            {
                (country, units) = GeneratorTools.GetNeutralRandomUnits(families, CoalitionsDB.SelectMany(x => x.Countries).ToList(), Template.ContextDecade, unitCount, Template.Mods, Template.OptionsMission.Contains("AllowLowPoly"), countMinMax: unitCountMinMax);
            }
            if (units.Where(x => x != null).Count() == 0) throw new BriefingRoomException($"Found no units for {string.Join(", ", families)} {country}");
            if (country != Country.ALL)
                extraSettings["Country"] = country;

            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.EmbeddedAirDefense))
            {
                string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(Template, side, families.First().GetUnitCategory(), country != Country.ALL ? country : null);
                units.AddRange(airDefenseUnits);
            }

            return AddUnitGroup(Toolbox.ShuffleArray(units.ToArray()), side, families.First(), groupLua, unitLua, coordinates, unitMakerGroupFlags, extraSettings);
        }

        internal UnitMakerGroupInfo? AddUnitGroup(
            string[] units, Side side, UnitFamily unitFamily,
            string groupTypeLua, string unitTypeLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings)
        {
            if (units.Length == 0) return null;

            var coalition = (side == Side.Ally) ? PlayerCoalition : (side == Side.Neutral ? Coalition.Neutral : PlayerCoalition.GetEnemy());
            var country = (Country)extraSettings.GetValueOrDefault("Country", (coalition == Coalition.Blue) ? Country.CombinedJointTaskForcesBlue : Country.CombinedJointTaskForcesRed);
            var isUsingSkynet = Template.MissionFeatures.Contains("SkynetIADS");
            var groupName = GeneratorTools.GetGroupName(GroupID, unitFamily, side, isUsingSkynet);
            UnitCallsign? callsign = null;

            if (unitFamily.GetUnitCategory() == UnitCategory.Static || unitFamily.GetUnitCategory() == UnitCategory.Cargo && unitFamily != UnitFamily.FOB)
                return AddStaticGroup(
                    country,
                    coalition,
                    unitFamily,
                    side,
                    units,
                    groupName,
                    callsign,
                    groupTypeLua,
                    unitTypeLua,
                    coordinates,
                    unitMakerGroupFlags,
                    extraSettings
                );

            var firstUnitID = UnitID;
            var firstUnitDB = units.Select(x => Database.Instance.GetEntry<DBEntryJSONUnit>(x)).First(x => x != null);
            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                callsign = CallsignGenerator.GetCallsign((DBEntryAircraft)firstUnitDB, country, side, isUsingSkynet, extraSettings.GetValueOrDefault("OverrideCallsignName", "").ToString(), (int)extraSettings.GetValueOrDefault("OverrideCallsignNumber", 1));
                groupName = callsign.Value.GroupName;
                if (extraSettings.ContainsKey("PlayerStartingType") && extraSettings.GetValueOrDefault("PlayerStartingType").ToString() == "TakeOffParking")
                    groupName += "(C)";
            }
            var dCSGroup = CreateGroup(
                groupTypeLua,
                coordinates,
                groupName,
                unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.AlwaysHidden),
                unitFamily,
                firstUnitDB,
                extraSettings
            );


            var (dcsUnits, unitsIDList) = AddUnits(
                units,
                groupName,
                callsign,
                unitTypeLua,
                coordinates,
                unitMakerGroupFlags,
                country,
                side,
                extraSettings
            );


            if (unitsIDList.Count == 0) return null; // No valid units added to this group
            dCSGroup.Units = dcsUnits;


            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.ImmediateAircraftSpawn))
                {
                    dCSGroup.Name += "-IQ-";
                    if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.ScrambleStart))
                        dCSGroup.LateActivation = false;
                }
                else if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.RadioAircraftSpawn))
                    Mission.AppendValue("AircraftRadioActivator", $"{{{GroupID}, \"{groupName}\"}},");
                else if (groupTypeLua != "AircraftUncontrolled" && !groupTypeLua.Contains("Player"))
                    dCSGroup.Name += "-RQ-";
                else if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.StaticAircraft))
                    dCSGroup.Name += "-STATIC-";
            }

            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.Immortal))
                dCSGroup.Waypoints[0].Tasks.Add(new DCSWrappedWaypointTask("SetImmortal", new Dictionary<string, object> { { "value", true } }, _auto: false, _priority: 1));

            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.Inert))
                dCSGroup.Waypoints[0].Tasks.Add(new DCSWrappedWaypointTask("Option", new Dictionary<string, object> { { "value", 4 }, { "name", 0 } }));

            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.Invisible))
                dCSGroup.Waypoints[0].Tasks.Add(new DCSWrappedWaypointTask("SetInvisible", new Dictionary<string, object> { { "value", true } }, _auto: false, _priority: 1));

            dCSGroup.Waypoints[0].X = dCSGroup.Units[0].Coordinates.X;
            dCSGroup.Waypoints[0].Y = dCSGroup.Units[0].Coordinates.Y;

            AddUnitGroupToTable(country, unitFamily.GetDCSUnitCategory(), dCSGroup);

            BriefingRoom.PrintToLog($"Added group of {units.Length} {coalition} {unitFamily} at {dCSGroup.Units[0].Coordinates}");
            GroupID++;

            return new UnitMakerGroupInfo(ref dCSGroup, firstUnitDB);
        }


        private DCSGroup CreateGroup(
            string groupTypeLua,
            Coordinates coordinates,
            string groupName,
            bool hidden,
            UnitFamily unitFamily,
            DBEntryJSONUnit firstUnitDB,
            Dictionary<string, object> extraSettings
            )
        {
            string groupYml = File.ReadAllText(Path.Combine(BRPaths.INCLUDE_YAML_GROUP, Toolbox.AddMissingFileExtension(groupTypeLua, ".yml")));
            foreach (KeyValuePair<string, object> extraSetting in extraSettings) // Replace custom values first so they override other replacements
                if (!(extraSetting.Value is Array)) // Array extra settings are treated on a per-unit basis
                    GeneratorTools.ReplaceKey(ref groupYml, extraSetting.Key, extraSetting.Value);

            GeneratorTools.ReplaceKey(ref groupYml, "GroupID", GroupID);
            GeneratorTools.ReplaceKey(ref groupYml, "GroupX", coordinates.X);
            GeneratorTools.ReplaceKey(ref groupYml, "GroupY", coordinates.Y);
            GeneratorTools.ReplaceKey(ref groupYml, "GroupX2", coordinates.X); // GroupX2 and GroupY2 are replaced by the default coordinates only if they were not replaced earlier in extraSetting replacements. 
            GeneratorTools.ReplaceKey(ref groupYml, "GroupY2", coordinates.Y);
            GeneratorTools.ReplaceKey(ref groupYml, "Name", groupName);
            GeneratorTools.ReplaceKey(ref groupYml, "NoCM", false);
            GeneratorTools.ReplaceKey(ref groupYml, "Hidden", hidden);
            GeneratorTools.ReplaceKey(ref groupYml, "UnitID", UnitID); // Must be after units are added

            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                var aircraftDB = (DBEntryAircraft)firstUnitDB;
                GeneratorTools.ReplaceKey(ref groupYml, "Altitude", (int)Math.Floor(aircraftDB.MaxAlt * 0.6));
                GeneratorTools.ReplaceKey(ref groupYml, "EPLRS", aircraftDB.EPLRS);
                GeneratorTools.ReplaceKey(ref groupYml, "RadioBand", (int)aircraftDB.Radio.Modulation);
                GeneratorTools.ReplaceKey(ref groupYml, "RadioFrequency", aircraftDB.Radio.Frequency);
                GeneratorTools.ReplaceKey(ref groupYml, "Speed", aircraftDB.CruiseSpeed);
            }

            var dCSGroup = DCSGroup.YamlToGroup(groupYml);

            if (unitFamily.GetUnitCategory().IsAircraft() && extraSettings.ContainsKey("GroupAirbaseID") && dCSGroup.Waypoints[0].AirdromeId == default)
            {
                dCSGroup.Waypoints[0].AirdromeId = (int)extraSettings.GetValueOrDefault("GroupAirbaseID", 0);
                var isHotStart = new List<UnitFamily> { UnitFamily.PlaneAWACS, UnitFamily.PlaneTankerBasket, UnitFamily.PlaneTankerBoom, UnitFamily.PlaneSEAD, UnitFamily.PlaneDrone }.Contains(unitFamily);
                dCSGroup.Waypoints[0].Type = isHotStart ? "TakeOffParkingHot" : "TakeOffParking";
                dCSGroup.Waypoints[0].Action = isHotStart ? "From Parking Area Hot" : "From Parking Area";
                dCSGroup.LateActivation = false;
                if (!isHotStart)
                    dCSGroup.Uncontrolled = true;
            }

            return dCSGroup;

        }


        private (List<DCSUnit> dCSUnits, List<int> unitsIDList) AddUnits(
            string[] unitSets,
            string groupName,
            UnitCallsign? callsign,
            string unitTypeLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Country country,
            Side side,
            Dictionary<string, object> extraSettings
            )
        {
            int unitLuaIndex = 1;
            var unitsIDList = new List<int>();
            var dCSUnits = new List<DCSUnit>();
            foreach (var unitSet in unitSets)
            {
                DBEntryJSONUnit unitDB = Database.Instance.GetEntry<DBEntryJSONUnit>(unitSet);
                if (unitDB == null)
                {
                    BriefingRoom.PrintToLog($"Unit \"{unitSet}\" not found.", LogMessageErrorLevel.Warning);
                    continue;
                }
                int unitSetIndex = 0;
                // foreach (string DCSID in unitDB.DCSIDs)
                // {
                    dCSUnits.Add(AddUnit(
                       unitDB.DCSID,
                       groupName,
                       callsign,
                       unitLuaIndex,
                       unitSetIndex,
                       unitDB,
                       unitTypeLua,
                       coordinates,
                       unitMakerGroupFlags,
                       country,
                       side,
                       extraSettings,
                       unitSets.Count() == 1
                       ));

                    unitsIDList.Add(UnitID);
                    unitSetIndex++;
                    unitLuaIndex++;
                    UnitID++;
                // }
            }
            return (dCSUnits, unitsIDList);
        }

        private UnitMakerGroupInfo? AddStaticGroup(
            Country country,
            Coalition coalition,
            UnitFamily unitFamily,
            Side side,
            string[] unitSets,
            string groupName,
            UnitCallsign? callsign,
            string groupTypeLua,
            string unitTypeLua,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Dictionary<string, object> extraSettings
        )
        {
            throw new NotImplementedException();
            // List<int> unitsIDList = new List<int>();
            // var initalGroupId = GroupID;
            // var DCSGroups = new List<DCSGroup>();
            // foreach (var unitSet in unitSets)
            // {
            //     var unitDB = Database.Instance.GetEntry<DBEntryJSONUnit>(unitSet);
            //     if (unitDB == null)
            //     {
            //         BriefingRoom.PrintToLog($"Unit \"{unitSet}\" not found.", LogMessageErrorLevel.Warning);
            //         continue;
            //     }
            //     int unitSetIndex = 0;
            //     foreach (var DCSID in unitDB.DCSIDs)
            //     {
            //         var groupHeading = GetGroupHeading(coordinates, extraSettings);
            //         var (unitCoordinates, unitHeading) = SetUnitCoordinatesAndHeading(unitDB, unitSetIndex, coordinates, groupHeading);
            //         var firstUnitID = UnitID;
            //         var dCSGroup = CreateGroup(
            //             groupTypeLua,
            //             unitCoordinates,
            //             groupName,
            //             unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.AlwaysHidden),
            //             UnitFamily.StaticStructureMilitary,
            //             unitDB,
            //             extraSettings
            //         );
            //         var dCSUnit = AddUnit(
            //             DCSID,
            //             groupName,
            //             callsign,
            //             1,
            //             unitSetIndex,
            //             unitDB,
            //             unitTypeLua,
            //             coordinates,
            //             unitMakerGroupFlags,
            //             country,
            //             side,
            //             extraSettings
            //             );

            //         unitsIDList.Add(UnitID);
            //         unitSetIndex++;
            //         UnitID++;

            //         dCSGroup.Units = new List<DCSUnit> { dCSUnit };

            //         DCSGroups.Add(dCSGroup);
            //         AddUnitGroupToTable(country, DCSUnitCategory.Static, dCSGroup);

            //         BriefingRoom.PrintToLog($"Added group of {DCSID} {coalition} {unitFamily} at {coordinates}");

            //         GroupID++;
            //     }
            // }

            // if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.EmbeddedAirDefense) && unitFamily != UnitFamily.StaticStructureOffshore)
            // {
            //     var firstUnitID = UnitID;
            //     string[] airDefenseUnits = GeneratorTools.GetEmbeddedAirDefenseUnits(Template, side, unitFamily.GetUnitCategory());
            //     var dCSGroup = CreateGroup(
            //             "Vehicle",
            //             coordinates,
            //             groupName,
            //             unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.AlwaysHidden),
            //             UnitFamily.VehicleAAA,
            //             null,
            //             extraSettings
            //         );
            //     var (unitsLuaTable, embeddedunitsIDList) = AddUnits(
            //         airDefenseUnits,
            //         groupName,
            //         callsign,
            //         "Vehicle",
            //         coordinates,
            //         unitMakerGroupFlags,
            //         country,
            //         side,
            //         extraSettings
            //     );
            //     dCSGroup.Units = unitsLuaTable;
            //     GroupID++;
            //     unitsIDList.AddRange(embeddedunitsIDList);
            //     AddUnitGroupToTable(country, DCSUnitCategory.Vehicle, dCSGroup);
            //     BriefingRoom.PrintToLog($"Added group of Embedded Air Defense for Static {coalition} {unitFamily} at {coordinates}");
            // }

            // var firstUnitDB = Database.Instance.GetEntry<DBEntryUnit>(unitSets.First());
            // return new UnitMakerGroupInfo(ref DCSGroups, firstUnitDB);
        }


        private DCSUnit AddUnit(
            string DCSID,
            string groupName,
            UnitCallsign? callsign,
            int unitLuaIndex,
            int unitSetIndex,
            DBEntryJSONUnit unitDB,
            string unitType,
            Coordinates coordinates,
            UnitMakerGroupFlags unitMakerGroupFlags,
            Country country,
            Side side,
            Dictionary<string, object> extraSettings,
            bool singleUnit = false)
        {
            if (!string.IsNullOrEmpty(unitDB.Module) && DBEntryDCSMod.coreMods.Contains(unitDB.Module, StringComparer.InvariantCultureIgnoreCase))
            {
                DBEntryDCSMod mod = Database.Instance.GetEntry<DBEntryDCSMod>(unitDB.Module);
                if (mod != null && !string.IsNullOrEmpty(mod.RequiredID))
                    ModUnits.Add(mod.RequiredID);
            }
            var unit = new DCSUnit(unitType);

            var groupHeading = GetGroupHeading(coordinates, extraSettings);
            var (unitCoordinates, unitHeading) = SetUnitCoordinatesAndHeading(unitDB, unitSetIndex, coordinates, groupHeading, singleUnit);

            foreach (KeyValuePair<string, object> extraSetting in extraSettings.Where(x => !IGNORE_PROPS.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value))
            {
                var prop = unit.GetType().GetProperty(extraSetting.Key, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (prop != null)
                    prop.SetValue(unit, extraSetting.Value);
            }
            unit.Heading = unitHeading;
            unit.DCSID = DCSID;
            unit.UnitId = UnitID;
            unit.Coordinates = ((List<Coordinates>)extraSettings.GetValueOrDefault("UnitCoords", new List<Coordinates>())).ElementAtOrDefault(unitLuaIndex - 1, unitCoordinates);
            unit.PlayerCanDrive = true;

            if (Toolbox.IsAircraft(unitDB.Category) && (unitLuaIndex == 1) && unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.FirstUnitIsClient))
                unit.Skill = SinglePlayerMission ? "Player" : "Client";
            else
                unit.Skill = extraSettings.GetValueOrDefault("Skill", GeneratorTools.GetDefaultSkillLevel(Template, side)).ToString(); ;

            GetLivery(ref unit, unitDB, country, extraSettings);

            if (Toolbox.IsAircraft(unitDB.Category))
            {
                var aircraftUnitDB = (DBEntryAircraft)unitDB;
                unit.Callsign = callsign.Value.GetLua(unitLuaIndex);
                unit.Name = callsign.Value.GetUnitName(unitLuaIndex);
                unit.OnboardNum = Toolbox.RandomInt(1, 1000).ToString("000");
                unit.PropsLua = aircraftUnitDB.ExtraProps;
                unit.RadioPresets = aircraftUnitDB.PanelRadios.Select(x => x.SetOverrides(
                    (double)extraSettings.GetValueOrDefault("RadioFrequency", aircraftUnitDB.Radio.Frequency),
                    (int)extraSettings.GetValueOrDefault("RadioBand", (int)aircraftUnitDB.Radio.Modulation),
                    (double?)extraSettings.GetValueOrDefault("AirbaseRadioFrequency", null),
                    (int?)extraSettings.GetValueOrDefault("AirbaseRadioModulation", null)
                    )).ToList();
                unit.PayloadCommon = aircraftUnitDB.PayloadCommon;
                unit.Pylons = extraSettings.ContainsKey("Payload")? aircraftUnitDB.GetPylonsObject(extraSettings.GetValueOrDefault("Payload", "").ToString()) : aircraftUnitDB.GetPylonsObject((DCSTask)extraSettings.GetValueOrDefault("DCSTask", DCSTask.Nothing));
                unit.Parking = ((List<int>)extraSettings.GetValueOrDefault("ParkingID", new List<int>())).ElementAtOrDefault(unitLuaIndex - 1);
            }
            else if (unitDB.Category == UnitCategory.Static || unitDB.Category == UnitCategory.Cargo)
            {
                // if (unitDB.Shape.Length - 1 > unitSetIndex)
                //     unit.ShapeName = unitDB.Shape[unitSetIndex];
                unit.Name = $"{groupName} {unitLuaIndex}";
            }
            else
                unit.Name = $"{groupName} {unitLuaIndex}";


            return unit;
        }

        private void GetLivery(ref DCSUnit unit, DBEntryJSONUnit unitDB, Country country, Dictionary<string, object> extraSettings)
        {
            var LiveryId = extraSettings.GetValueOrDefault("Livery", "default").ToString();
            if (LiveryId == "default")
                LiveryId = Toolbox.RandomFrom(unitDB.Liveries.GetValueOrDefault(country, new List<string>{"default"}));
            unit.LiveryId = LiveryId;
        }

        private void AddUnitGroupToTable(Country country, DCSUnitCategory category, DCSGroup dCSGroup)
        {
            if (!UnitLuaTables.ContainsKey(country)) UnitLuaTables.Add(country, new Dictionary<DCSUnitCategory, List<DCSGroup>>());
            if (!UnitLuaTables[country].ContainsKey(category)) UnitLuaTables[country].Add(category, new List<DCSGroup>());
            UnitLuaTables[country][category].Add(dCSGroup);
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
                    foreach (DCSUnitCategory unitCategory in Toolbox.GetEnumValues<DCSUnitCategory>()) // Check all coalitions
                    {
                        if (!UnitLuaTables[country].ContainsKey(unitCategory)) continue; // No unit for this unit category

                        unitsLuaTable += $"[\"{unitCategory.ToString().ToLower()}\"] =\n";
                        unitsLuaTable += "{\n";
                        unitsLuaTable += "[\"group\"] =\n";
                        unitsLuaTable += "{\n";
                        for (int groupIndex = 0; groupIndex < UnitLuaTables[country][unitCategory].Count; groupIndex++)
                        {
                            unitsLuaTable += $"[{groupIndex + 1}] = {UnitLuaTables[country][unitCategory][groupIndex].ToLuaString(0)},";
                        }

                        unitsLuaTable += $"}}, -- end of [\"group\"]\n";
                        unitsLuaTable += $"}}, -- end of [\"{unitCategory.ToString().ToLower()}\"]\n";
                    }
                }
                unitsLuaTable += $"}}, -- end of [{countryIndex + 1}]\n";
            }

            return unitsLuaTable;
        }

        internal string GetRequiredModules()
        {
            var str = "";
            foreach (var unitId in ModUnits.Distinct().ToList())
            {
                str += $"[\"{unitId}\"] = \"{unitId}\",";
            }
            return str;
        }

        internal string GetRequiredModulesBriefing()
        {
            if (ModUnits.Count == 0)
                return "";
            return "<p><strong>Required Mods:</strong><br/>" + string.Join("<br/>", ModUnits.Distinct().ToList()) + "</p>";
        }

        private double GetGroupHeading(Coordinates groupCoordinates, Dictionary<string, object> extraSettings)
        {
            if (!extraSettings.ContainsKey("GroupX2"))
                return 0.0;
            var waypointCoor = new Coordinates((double)extraSettings["GroupX2"], (double)extraSettings["GroupY2"]);
            return Coordinates.ToAngleInRadians(groupCoordinates, waypointCoor);
        }

        private (Coordinates unitCoordinates, double unitHeading) SetUnitCoordinatesAndHeading(
            DBEntryJSONUnit unitDB, int unitIndex, Coordinates groupCoordinates, double groupHeading, bool singleUnit = false)
        {
            var unitCoordinates = groupCoordinates;
            var unitHeading = groupHeading;

            if (unitDB.IsAircraft)
                unitCoordinates = groupCoordinates + new Coordinates(AIRCRAFT_UNIT_SPACING, AIRCRAFT_UNIT_SPACING) * unitIndex;
            // else
            // {
            //     if (unitDB.OffsetCoordinates.Length > unitIndex) // Unit has a fixed set of coordinates (for SAM sites, etc.)
            //     {
            //         Coordinates offsetCoordinates = unitDB.OffsetCoordinates[unitIndex];
            //         unitCoordinates = TransformFromOffset(unitHeading, groupCoordinates, offsetCoordinates);
            //     }
            //     else if (!singleUnit || unitDB.DCSIDs.Count() != 1) // No fixed coordinates, generate random coordinates
            //     {
            //         switch (unitDB.Category)
            //         {
            //             case UnitCategory.Ship:
            //                 if (unitIndex > 0)
            //                     unitCoordinates = groupCoordinates.CreateNearRandom(SHIP_UNIT_SPACING, SHIP_UNIT_SPACING * 10);
            //                 break;
            //             case UnitCategory.Cargo:
            //             case UnitCategory.Static:
            //                 // Static units are spawned exactly on the group location (and there's only a single unit per group)
            //                 break;
            //             default:
            //                 unitCoordinates = groupCoordinates.CreateNearRandom(VEHICLE_UNIT_SPACING, VEHICLE_UNIT_SPACING * 10);
            //                 break;
            //         }
            //     }

            //     if (unitDB.OffsetHeading.Length > unitIndex) // Unit has a fixed heading (for SAM sites, etc.)
            //         unitHeading = Toolbox.ClampAngle(unitHeading + unitDB.OffsetHeading[unitIndex]);
            //     else if (unitDB.Category != UnitCategory.Ship)
            //         unitHeading = Toolbox.RandomDouble(Toolbox.TWO_PI);
            // }
            return (unitCoordinates, unitHeading);
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