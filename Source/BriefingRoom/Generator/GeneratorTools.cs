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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace BriefingRoom4DCS.Generator
{
    internal static class GeneratorTools
    {
        private static readonly int[] DAYS_PER_MONTH = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        internal static string[] GetEmbeddedAirDefenseUnits(MissionTemplateRecord template, Side side, Country? country = null)
        {
            DBCommonAirDefenseLevel airDefenseInfo = (side == Side.Ally) ?
                 Database.Instance.Common.AirDefense.AirDefenseLevels[(int)template.SituationFriendlyAirDefense.Get()] :
                  Database.Instance.Common.AirDefense.AirDefenseLevels[(int)template.SituationEnemyAirDefense.Get()];

            DBEntryCoalition unitsCoalitionDB = Database.Instance.GetEntry<DBEntryCoalition>(template.GetCoalitionID(side));
            if (unitsCoalitionDB == null) return new string[0];

            List<string> units = new List<string>();

            if (Toolbox.RandomDouble() >= airDefenseInfo.EmbeddedChance) return new string[0];

            int airDefenseUnitsCount = airDefenseInfo.EmbeddedUnitCount.GetValue();

            for (int i = 0; i < airDefenseUnitsCount; i++)
            {
                var families = new List<UnitFamily> { UnitFamily.VehicleAAA, UnitFamily.VehicleAAA, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShort };
                units.AddRange(unitsCoalitionDB.GetRandomUnits(families, template.ContextDecade, 1, template.Mods, template.OptionsMission.Contains("AllowLowPoly"), country).Item2);
            }

            return units.ToArray();
        }

        internal static Tuple<Country, List<string>> GetNeutralRandomUnits(List<UnitFamily> families, List<Country> IgnoreCountries, Decade decade, int count, List<string> unitMods, bool allowLowPolly, Country? requiredCountry = null, MinMaxI? countMinMax = null)
        {
            // Count is zero, return an empty array.
            if (count < 1) throw new BriefingRoomException("Asking for a zero unit list");
            if (families.Select(x => x.GetUnitCategory()).Any(x => x != families.First().GetUnitCategory())) throw new BriefingRoomException($"Cannot mix Categories in types {string.Join(", ", families)}");

            UnitCategory category = families.First().GetUnitCategory();
            bool allowDifferentUnitTypes = false;

            var validUnits = new Dictionary<Country, List<string>>();

            foreach (Country country in Enum.GetValues(typeof(Country)).Cast<Country>().Where(x => !IgnoreCountries.Contains(x)).ToList())
                validUnits[country] = (
                        from DBEntryUnit unit in Database.Instance.GetAllEntries<DBEntryUnit>()
                        where unit.Families.Intersect(families).ToList().Count > 0 && unit.Operators.ContainsKey(country) &&
                            (string.IsNullOrEmpty(unit.RequiredMod) || unitMods.Contains(unit.RequiredMod, StringComparer.InvariantCultureIgnoreCase)) &&
                            (unit.Operators[country][0] <= decade) && (unit.Operators[country][1] >= decade) &&
                            (!unit.Flags.HasFlag(DBEntryUnitFlags.LowPolly) || allowLowPolly)
                        select unit.ID
                    ).Distinct().ToList();

            // Ensures that only countries with units listed get returned
            validUnits = validUnits.Where(x => x.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);
            // At least one unit found, return it
            if (validUnits.Count == 0)
            {
                validUnits = new Dictionary<Country, List<string>> { { Country.ALL, Database.Instance.GetEntry<DBEntryDefaultUnitList>("FirstWorld").DefaultUnits[(int)families.First(), (int)decade].ToList() } };

            }

            switch (category)
            {
                // Units are planes or helicopters, make sure unit count does not exceed the maximum flight group size
                case UnitCategory.Helicopter:
                case UnitCategory.Plane:
                    count = Toolbox.Clamp(count, 1, Toolbox.MAXIMUM_FLIGHT_GROUP_SIZE);
                    break;
                // Units are ships or static buildings, only one unit per group (that's the law in DCS World, buddy)
                case UnitCategory.Ship:
                    count = 1;
                    break;
                // Units are ground vehicles, allow multiple unit types in the group
                case UnitCategory.Vehicle:
                    allowDifferentUnitTypes = true;
                    break;
            }
            var selectableUnits = new List<string>();
            var selectedCountry = Toolbox.RandomFrom(validUnits.Keys.ToList());
            if (requiredCountry.HasValue)
                if (validUnits.ContainsKey(requiredCountry.Value))
                    selectedCountry = requiredCountry.Value;
                else
                    BriefingRoom.PrintToLog($"Could not find suitable units for {requiredCountry.Value} using units from other coalition members.", LogMessageErrorLevel.Info);

            selectableUnits = validUnits[selectedCountry];


            // Different unit types allowed in the group, pick a random type for each unit.
            if (allowDifferentUnitTypes)
            {
                List<string> selectedUnits = new List<string>();
                for (int i = 0; i < count; i++)
                    selectedUnits.Add(Toolbox.RandomFrom(selectableUnits));

                return new(selectedCountry, selectedUnits.ToList());
            }

            // Different unit types NOT allowed in the group, pick a random type and fill the whole array with it.
            string unitString = Toolbox.RandomFrom(selectableUnits);
            return new(selectedCountry, Enumerable.Repeat(unitString, count).ToList());
        }

        internal static object LowercaseFirstCharacter(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length == 1) return str.ToLowerInvariant();
            return str.Substring(0, 1).ToLowerInvariant() + str.Substring(1);
        }

        internal static string MakeHTMLList(params string[] listEntries)
        {
            string list = "";
            foreach (string listEntry in listEntries)
                list += $"<li>{listEntry}</li>\n";
            return list;
        }

        internal static string GenerateMissionName(string desiredName)
        {
            // Try to get the provided custom mission name.
            string missionName = (desiredName ?? "").ReplaceAll("", "\r", "\n", "\t").Trim();

            // No custom name found, generate one.
            if (string.IsNullOrEmpty(missionName))
            {
                missionName = Database.Instance.Common.Names.MissionNameTemplate.Get();
                for (int i = 0; i < DBCommonNames.MISSION_NAMES_PART_COUNT; i++)
                    missionName = missionName.Replace($"$P{i + 1}$", Toolbox.RandomFrom(Database.Instance.Common.Names.MissionNameParts[i].Get().Split(",")));
            }

            return missionName;
        }


        internal static string MakeRawTextList(string newLine = "\n", params string[] listEntries)
        {
            string list = "";
            foreach (string listEntry in listEntries)
                list += $"- {listEntry}{newLine}";
            return list;
        }

        internal static string MakeHTMLTable(params string[] tableEntries)
        {
            string table = "";
            foreach (string tableRow in tableEntries)
            {
                string[] rowCells = tableRow.Split('\t');
                table += "<tr>";
                foreach (string rowCell in rowCells)
                    table += $"<td>{rowCell.ReplaceAll("\n", "<br/>")}</td>";
                table += "</tr>\n";

            }
            return table;
        }

        internal static string GetTemplateCoalition(MissionTemplateRecord template, Coalition coalition)
        {
            if (coalition == Coalition.Red) return template.ContextCoalitionRed;
            return template.ContextCoalitionBlue;
        }

        internal static Coalition? GetSpawnPointCoalition(MissionTemplateRecord template, Side side, bool forceSide = false)
        {
            // No countries spawning restriction
            if (template.SpawnAnywhere && !forceSide) return null;
            if (side == Side.Neutral) return Coalition.Neutral;

            Coalition coalition = side == Side.Ally ? template.ContextPlayerCoalition : template.ContextPlayerCoalition.GetEnemy();

            return coalition;
        }

        internal static int GetDaysPerMonth(Month month, int year)
        {
            // Not February, return value stored in DAYS_PER_MONTH array
            if (month != Month.February) return DAYS_PER_MONTH[(int)month];

            bool leapYear = false;
            if ((year % 400) == 0) leapYear = true;
            else if ((year % 100) == 0) leapYear = false;
            else if ((year % 4) == 0) leapYear = true;

            return leapYear ? 29 : 28;
        }


        internal static object GetTACANCallsign(UnitFamily unitFamily)
        {
            switch (unitFamily)
            {
                default:
                    return "TCN";
                case UnitFamily.ShipCarrierCATOBAR:
                case UnitFamily.ShipCarrierSTOBAR:
                case UnitFamily.ShipCarrierSTOVL:
                    return "CVN";
                case UnitFamily.PlaneTankerBasket:
                case UnitFamily.PlaneTankerBoom:
                    return "TKR";
            }
        }

        internal static DCSSkillLevel GetDefaultSkillLevel(MissionTemplateRecord template, Side side) => (Side.Ally == side ? template.SituationFriendlySkill : template.SituationEnemySkill) switch
        {
            AmountR.VeryLow => DCSSkillLevel.Average,
            AmountR.Low => Toolbox.RandomFrom(DCSSkillLevel.Average, DCSSkillLevel.Good),
            AmountR.Average => Toolbox.RandomFrom(DCSSkillLevel.Average, DCSSkillLevel.Good, DCSSkillLevel.High),
            AmountR.High => Toolbox.RandomFrom(DCSSkillLevel.Good, DCSSkillLevel.High),
            AmountR.VeryHigh => Toolbox.RandomFrom(DCSSkillLevel.High, DCSSkillLevel.Excellent),
            _ => DCSSkillLevel.Random,
        };

        internal static string FormatRadioFrequency(double radioFrequency)
        {
            return radioFrequency.ToString("F1", NumberFormatInfo.InvariantInfo);
        }

        internal static void ReplaceKey(ref string lua, string key, object value)
        {
            string valueStr = Toolbox.ValToString(value);
            if (value is bool) valueStr = valueStr.ToLowerInvariant();

            lua = lua.Replace($"${key.ToUpperInvariant()}$", valueStr);
        }

        internal static void ReplaceKey(ref string lua, string key, object arrayValue, int arrayIndex)
        {
            try
            {
                object value = ((IEnumerable)arrayValue).Cast<object>().Select(x => x).ToArray()[arrayIndex];
                ReplaceKey(ref lua, key, value);
            }
            catch (Exception)
            {
                ReplaceKey(ref lua, key, "");
            }
        }

        internal static bool GetHiddenStatus(FogOfWar fogOfWar, Side side, UnitMakerGroupFlags unitMakerGroupFlags)
        {
            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.AlwaysHidden)) return true;
            if (unitMakerGroupFlags.HasFlag(UnitMakerGroupFlags.NeverHidden)) return false;

            switch (fogOfWar)
            {
                default:
                case FogOfWar.All:
                    return false;
                case FogOfWar.AlliesOnly:
                case FogOfWar.KnownUnitsOnly:
                    return side == Side.Enemy;
                case FogOfWar.SelfOnly:
                case FogOfWar.None:
                    return true;
            }
        }

        internal static int GetTACANFrequency(int channel, char channelMode, bool AA)
        {
            int res;

            if (!AA && channelMode == 'X')
            {
                if (channel < 64)
                {
                    res = 962 + channel - 1;
                }
                else
                {
                    res = 1151 + channel - 64;
                }
            }
            else
            {
                if (channel < 64)
                {
                    res = 1088 + channel - 1;
                }
                else
                {
                    res = 1025 + channel - 64;
                }
            }

            return GetRadioFrequency(res);
        }

        internal static int GetRadioFrequency(double radioFrequency)
        {
            return (int)(radioFrequency * 1000000.0);
        }

        internal static void CheckDBForMissingEntry<T>(string id, bool allowEmpty = false) where T : DBEntry
        {
            if (string.IsNullOrEmpty(id) && allowEmpty) return;
            if (!Database.Instance.EntryExists<T>(id)) throw new BriefingRoomException($"Database entry {typeof(T).Name} with ID \"{id}\" not found.");
        }

        internal static string ParseRandomString(string randomString, DCSMission mission = null)
        {
            while (randomString.Contains("{") && randomString.Contains("{"))
            {
                int start = randomString.LastIndexOf("{");
                string stringLeft = randomString.Substring(start);
                if (!stringLeft.Contains("}")) break;
                int end = stringLeft.IndexOf("}") + 1;

                string segment = randomString.Substring(start, end);
                string parsedSegment = segment.Replace("{", "").Replace("}", "").Trim();
                string[] items = parsedSegment.Split('|');
                string selItem = Toolbox.RandomFrom(items);

                randomString = randomString.Replace(segment, selItem);
            }

            var responseString = randomString.Replace("{", "").Replace("}", "").Trim();
            if (mission != null)
                return mission.ReplaceValues(responseString);
            return responseString;
        }

        private static string SetSkyNetPrefix(string groupName, UnitFamily unitFamily, Side side)
        {
            var ewrTypes = new List<UnitFamily> {
                UnitFamily.VehicleEWR,
                UnitFamily.ShipCarrierCATOBAR,
                UnitFamily.ShipCarrierSTOBAR,
                UnitFamily.ShipCarrierSTOVL,
                UnitFamily.ShipCruiser,
                UnitFamily.ShipFrigate};
            var samTypes = new List<UnitFamily> {
                UnitFamily.VehicleSAMMedium,
                UnitFamily.VehicleSAMLong
                };
            var isEWR = ewrTypes.Contains(unitFamily);
            if (isEWR || samTypes.Contains(unitFamily))
            {
                var prefix = isEWR ? "EW" : "SAM";
                var newGroupName = $"{prefix}-{groupName}";
                if (side == Side.Ally)
                    newGroupName = $"BLUE-{newGroupName}";
                return newGroupName;
            }
            return groupName;
        }

        internal static string GetGroupName(int groupID, UnitFamily family, Side side, bool isUsingSkynet)
        {
            string name = ParseRandomString(Database.Instance.Common.Names.UnitGroups[(int)family].Get());

            int fakeGroupNumber = groupID * 10 + Toolbox.RandomInt(1, 10);
            name = name.Replace("$N$", fakeGroupNumber.ToString(NumberFormatInfo.InvariantInfo));
            name = name.Replace("$NTH$", Toolbox.GetOrdinalAdjective(fakeGroupNumber));
            if (isUsingSkynet)
                return SetSkyNetPrefix(name, family, side);
            if (string.IsNullOrEmpty(name))
                throw new BriefingRoomException($"{family} cannot have empty group name");
            return name;
        }


        internal static string GetPlayerStartingAction(PlayerStartLocation playerStartLocation)
        {
            switch (playerStartLocation)
            {
                default: // case PlayerStartLocation.ParkingCold
                    return "From Parking Area";
                case PlayerStartLocation.ParkingHot:
                    return "From Parking Area Hot";
                case PlayerStartLocation.Runway:
                    return "From Runway";
            }
        }

        internal static string GetPlayerStartingType(PlayerStartLocation playerStartLocation)
        {
            switch (playerStartLocation)
            {
                default: // case PlayerStartLocation.ParkingCold
                    return "TakeOffParking";
                case PlayerStartLocation.ParkingHot:
                    return "TakeOffParkingHot";
                case PlayerStartLocation.Runway:
                    return "TakeOff";
            }
        }
    }
}
