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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BriefingRoom4DCS.Generator
{
    /// <summary>
    /// A static library of tools to help mission generation.
    /// </summary>
    internal static class GeneratorTools
    {
        /// <summary>
        /// The number of days in each month.
        /// </summary>
        private static readonly int[] DAYS_PER_MONTH = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// List of possible unit families to choose from for "embedded" air defense.
        /// Some families are more frequent than other.
        /// </summary>
        private static readonly UnitFamily[] EMBEDDED_AIR_DEFENSE_FAMILIES = new UnitFamily[]
        {
            UnitFamily.VehicleAAA, UnitFamily.VehicleAAA, UnitFamily.VehicleAAA,
            UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR,
            UnitFamily.VehicleSAMShort
        };

        /// <summary>
        /// Adds "embedded" short-range air-defense units to an unit group.
        /// </summary>
        /// <param name="units">Array of units in the group</param>
        /// <param name="airDefenseLevel">Air defense level setting to use, from the mission template</param>
        /// <param name="coalitionDB">Database entry for the coalition to use for air-defense units</param>
        /// <param name="decade">Decade during which the units must be operated</param>
        /// <param name="unitMods">Unit mods the units can belong to</param>
        /// <returns>Updated array of units with added embedded air defense units</returns>
        internal static string[] AddEmbeddedAirDefense(string[] units, AmountNR airDefenseLevel, DBEntryCoalition coalitionDB, Decade decade, List<string> unitMods)
        {
            int airDefenseLevelInt = (int)airDefenseLevel.Get();
            // No luck this time, don't add anything
            if (Toolbox.RandomDouble() >= Database.Instance.Common.AirDefense.AirDefenseLevels[airDefenseLevelInt].EmbeddedChance)
                return units;

            // Convert the unit array to an open-ended list so that units can be added
            List<string> unitsList = new List<string>(units);

            // Add some air defense units
            int embeddedCount = Database.Instance.Common.AirDefense.AirDefenseLevels[airDefenseLevelInt].EmbeddedUnitCount.GetValue();
            for (int i = 0; i < embeddedCount; i++)
                unitsList.AddRange(
                    coalitionDB.GetRandomUnits(Toolbox.RandomFrom(EMBEDDED_AIR_DEFENSE_FAMILIES), decade, 1, unitMods));

            if (unitsList.Count == 0) return new string[0];
            // Randomize unit order so embbedded air defense units are not always at the end of the group
            // but keep unit #0 at its place, because the first unit of the group is used to determine the group type, and we don't want
            // a artillery platoon to be named "air defense bataillon" because the first unit is a AAA.
            string unit0 = unitsList[0];
            unitsList.RemoveAt(0);
            unitsList = unitsList.OrderBy(x => Toolbox.RandomInt()).ToList();
            unitsList.Insert(0, unit0);

            return unitsList.ToArray();
        }

        internal static string[] GetEmbeddedAirDefenseUnits(MissionTemplate template, Side side)
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
                UnitFamily airDefenseFamily = Toolbox.RandomFrom(UnitFamily.VehicleAAA, UnitFamily.VehicleAAA, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShortIR, UnitFamily.VehicleSAMShort);
                units.AddRange(unitsCoalitionDB.GetRandomUnits(airDefenseFamily, template.ContextDecade, 1, template.Mods, true));
            }

            return units.ToArray();
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

        /// <summary>
        /// Generate a random mission name if none is provided in the template, or returns the provided name if there is one.
        /// </summary>
        /// <param name="mission">A mission</param>
        /// <param name="template">Mission template to use</param>
        internal static string GenerateMissionName(string desiredName)
        {
            // Try to get the provided custom mission name.
            string missionName = (desiredName ?? "").ReplaceAll("", "\r", "\n", "\t").Trim();

            // No custom name found, generate one.
            if (string.IsNullOrEmpty(missionName))
            {
                missionName = Database.Instance.Common.Names.MissionNameTemplate;
                for (int i = 0; i < DBCommonNames.MISSION_NAMES_PART_COUNT; i++)
                    missionName = missionName.Replace($"$P{i + 1}$", Toolbox.RandomFrom(Database.Instance.Common.Names.MissionNameParts[i]));
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
                    table += $"<td>{rowCell}</td>";
                table += "</tr>\n";

            }
            return table;
        }

        internal static string GetTemplateCoalition(MissionTemplate template, Coalition coalition)
        {
            if (coalition == Coalition.Red) return template.ContextCoalitionRed;
            return template.ContextCoalitionBlue;
        }

        internal static Coalition? GetSpawnPointCoalition(MissionTemplate template, Side side)
        {
            // No countries spawning restriction
            if (!template.OptionsMission.Contains(MissionOption.OnlySpawnInFriendlyCountries)) return null;

            Coalition coalition = side == Side.Ally ? template.ContextPlayerCoalition : template.ContextPlayerCoalition.GetEnemy();

            if (template.OptionsMission.Contains(MissionOption.InvertCountriesCoalitions))
                coalition = coalition.GetEnemy();

            return coalition;
        }

        /// <summary>
        /// Returns the number of days in a month.
        /// </summary>
        /// <param name="month">The month of the year.</param>
        /// <param name="year">The year. Used to know if it's a leap year.</param>
        /// <returns>The number of days in the month.</returns>
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

        internal static DCSSkillLevel GetDefaultSkillLevel(MissionTemplate template, UnitFamily unitFamily, Side side)
        {
            // Unit is an aircraft, air force quality decides skill level
            if (unitFamily.GetUnitCategory().IsAircraft())
            {
                AmountNR capLevel = (side == Side.Ally ? template.SituationFriendlyAirForce : template.SituationEnemyAirForce).Get();
                return Toolbox.RandomFrom(Database.Instance.Common.CAP.CAPLevels[(int)capLevel].SkillLevel);
            }

            // Unit is a surface-to-air unit, air defense quality decides skill level
            if (unitFamily.IsAirDefense())
            {
                AmountNR airDefenseIntensity;
                if (side == Side.Ally) airDefenseIntensity = template.SituationFriendlyAirDefense.Get();
                else airDefenseIntensity = template.SituationEnemyAirDefense.Get();

                return Toolbox.RandomFrom(Database.Instance.Common.AirDefense.AirDefenseLevels[(int)airDefenseIntensity].SkillLevel);
            }

            // Unit is nothing of that, return some random skill
            return Toolbox.RandomFrom(DCSSkillLevel.Average, DCSSkillLevel.Good, DCSSkillLevel.High);
        }

        internal static object FormatRadioFrequency(double radioFrequency)
        {
            return radioFrequency.ToString("F1", NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        internal static void ReplaceKey(ref string lua, string key, object value)
        {
            string valueStr = Toolbox.ValToString(value);
            if (value is bool) valueStr = valueStr.ToLowerInvariant();

            lua = lua.Replace($"${key.ToUpperInvariant()}$", valueStr);
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by a value from a given index arrayValue.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="arrayValue">An array from which to pick the value to replace the key with.</param>
        /// <param name="arrayIndex">Index of the array from which to pick the value.</param>
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

            return GetRadioFrenquency(res);
        }

        /// <summary>
        /// Converts a radio freqency in Mhz to a radio frequency in Hz.
        /// </summary>
        /// <param name="radioFrequency">Radio frequency in Mhz</param>
        /// <returns>Radio frequency in Hz.</returns>
        internal static int GetRadioFrenquency(double radioFrequency)
        {
            return (int)(radioFrequency * 1000000.0);
        }

        /// <summary>
        /// Checks for a <see cref="DBEntry"/> in <see cref="Database"/> and throws an exception if it isn't found.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DBEntry"/> to look for</typeparam>
        /// <param name="id">The id of the entry to look for</param>
        /// <param name="allowEmpty">If true, null or empty strings will be allowed</param>
        /// <returns></returns>
        internal static bool CheckDBForMissingEntry<T>(string id, bool allowEmpty = false) where T : DBEntry
        {
            if (string.IsNullOrEmpty(id) && allowEmpty) return true;

            if (!Database.Instance.EntryExists<T>(id))
            {
                BriefingRoom.PrintToLog($"Database entry {typeof(T).Name} with ID \"{id}\" not found.", LogMessageErrorLevel.Error);
                return false;
            }

            return true;
        }

        internal static string GetGroupName(int groupID, UnitFamily family)
        {
            string name = ParseRandomString(Database.Instance.Common.Names.UnitGroups[(int)family]);

            int fakeGroupNumber = groupID * 10 + Toolbox.RandomInt(1, 10);
            name = name.Replace("$N$", fakeGroupNumber.ToString(NumberFormatInfo.InvariantInfo));
            name = name.Replace("$NTH$", Toolbox.GetOrdinalAdjective(fakeGroupNumber));
            return name;
        }

        /// <summary>
        /// Returns coordinates situated somewhere between the initial airbase and the center point of objectives.
        /// </summary>
        /// <param name="mission">The DCS mission</param>
        /// <param name="lerpValue">Progess on the airbase to objectives center distance (0.0=airbase, 1.0=objective center)</param>
        /// <returns>A set of coordinates</returns>
        //internal static Coordinates GetCoordinatesOnFlightPath(DCSMission mission, double lerpValue)
        //{
        //    Coordinates coordinates = Coordinates.Lerp(mission.InitialPosition, mission.ObjectivesCenter, lerpValue);
        //    double distance = mission.InitialPosition.GetDistanceFrom(mission.ObjectivesCenter);

        //    // Create some random variation proportional to the total flight path distance
        //    distance = Math.Max(20.0, distance);
        //    return coordinates + Coordinates.CreateRandom(distance / 8, distance / 4);
        //}

        //internal static string GetBriefingStringForUnitFamily(UnitFamily unitFamily, bool plural)
        //{
        //    return Database.Instance.Common.Names.UnitBriefingNames[(int)unitFamily][plural ? 1 : 0];
        //}

        ///// <summary>
        ///// Returns the coalition to look for in spawn points according to the template setting regarding enemy units location.
        ///// </summary>
        ///// <param name="template">A mission template</param>
        ///// <returns>A coalition, or null if any coalition can be used</returns>
        //internal static Coalition? GetEnemySpawnPointCoalition(MissionTemplate template)
        //{
        //    // No preference for enemy units location
        //    if (template.OptionsEnemyUnitsLocation == SpawnPointPreferredCoalition.Any) return null;

        //    // All countries belong to the same coalition anyway, so preference have no meaning.
        //    if ((template.OptionsTheaterCountriesCoalitions == CountryCoalition.AllBlue) ||
        //        (template.OptionsTheaterCountriesCoalitions == CountryCoalition.AllRed))
        //        return null;

        //    if (template.OptionsEnemyUnitsLocation == SpawnPointPreferredCoalition.Blue)
        //        return (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Red : Coalition.Blue;

        //    // Last possibility is "template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Red"
        //    return (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Blue : Coalition.Red;
        //}

        ///// <summary>
        ///// Returns the coalition to look for in spawn points according to the template setting regarding enemy units location.
        ///// </summary>
        ///// <param name="template">A mission template</param>
        ///// <returns>A coalition, or null if any coalition can be used</returns>
        //internal static Coalition? GetAllySpawnPointCoalition(MissionTemplate template)
        //{
        //    // No preference for enemy units location
        //    if (template.OptionsEnemyUnitsLocation != SpawnPointPreferredCoalition.Any) return null;

        //    // All countries belong to the same coalition anyway, so preference have no meaning.
        //    if ((template.OptionsTheaterCountriesCoalitions != CountryCoalition.AllBlue) &&
        //        (template.OptionsTheaterCountriesCoalitions != CountryCoalition.AllRed))
        //        return null;

        //    if (template.OptionsEnemyUnitsLocation == SpawnPointPreferredCoalition.Blue)
        //        return (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Blue : Coalition.Red;

        //    // Last possibility is "template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Red"
        //    return (template.OptionsTheaterCountriesCoalitions == CountryCoalition.Inverted) ? Coalition.Red : Coalition.Blue;
        //}

        ///// <summary>
        ///// Returns a human-readable string for the mission weather.
        ///// </summary>
        ///// <param name="weatherConditions">Weather conditions</param>
        ///// <returns>Weather conditions, as a string</returns>
        //internal static string GetEnumString(Weather weatherConditions)
        //{
        //    switch (weatherConditions)
        //    {
        //        default: return "Clear"; // Weather.Clear
        //        case Weather.LightClouds: return "Light clouds";
        //        case Weather.Overcast: return "Overcast";
        //        case Weather.Precipitation: return "Precipitation";
        //        case Weather.SomeClouds: return "Some clouds";
        //        case Weather.Storm: return "Storm";
        //    }
        //}

        ///// <summary>
        ///// Returns a human-readable string for the mission wind.
        ///// </summary>
        ///// <param name="windConditions">Wind conditions</param>
        ///// <returns>Wind conditions, as a string</returns>
        //internal static string GetEnumString(Wind windConditions)
        //{
        //    switch (windConditions)
        //    {
        //        default: return "Calm"; // Wind.Calm
        //        case Wind.LightBreeze: return "Light Breeze";
        //        case Wind.ModerateBreeze: return "Moderate breeze";
        //        case Wind.StrongBreeze: return "Strong breeze";
        //        case Wind.Gale: return "Gale wind";
        //        case Wind.Storm: return "Storm";
        //    }
        //}

        //internal static string MakeBriefingStringReplacements(string briefingString, DCSMission mission, DBEntryCoalition[] coalitionsDB, int objectiveIndex = 0)
        //{
        //    DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(mission.Theater);

        //    briefingString = briefingString.Replace("$ALLYADJ$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.Adjective]));
        //    briefingString = briefingString.Replace("$ENEMYADJ$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionEnemy].BriefingElements[(int)CoalitionBriefingElement.Adjective]));
        //    briefingString = briefingString.Replace("$RECON$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.Recon]));
        //    briefingString = briefingString.Replace("$STRCOMMAND$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.StrategicCommand]));
        //    briefingString = briefingString.Replace("$TACCOMMAND$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.TacticalCommand]));
        //    briefingString = briefingString.Replace("$THEATER$", Toolbox.RandomFrom(theaterDB.BriefingNames));
        //    briefingString = briefingString.Replace("$THEALLIES$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.TheCoalition]));
        //    briefingString = briefingString.Replace("$THEENEMIES$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionEnemy].BriefingElements[(int)CoalitionBriefingElement.TheCoalition]));

        //    briefingString = briefingString.Replace("$OBJECTIVE$", mission.Objectives[objectiveIndex].Name);

        //    briefingString = briefingString.Replace("$UNITFAMILIES$",
        //        mission.Objectives[objectiveIndex].TargetFamily.HasValue ?
        //        GetBriefingStringForUnitFamily(mission.Objectives[objectiveIndex].TargetFamily.Value, true) : "units");
        //    briefingString = briefingString.Replace("$UNITFAMILY$",
        //        mission.Objectives[objectiveIndex].TargetFamily.HasValue ?
        //        GetBriefingStringForUnitFamily(mission.Objectives[objectiveIndex].TargetFamily.Value, false) : "unit");

        //    return Toolbox.ICase(SanitizeString(briefingString));
        //}

        //internal static string RemoveAfterComma(string str)
        //{
        //    str = Regex.Replace(str, @",.*", "");

        //    //str = Regex.Replace(str, @"\(.*\)", "");

        //    return SanitizeString(str);
        //}

        /// <summary>
        /// Randomizes parts of a string.
        /// </summary>
        /// <param name="randomString">The string to randomize</param>
        /// <returns>A randomized string.</returns>
        internal static string ParseRandomString(string randomString)
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

            return randomString.Replace("{", "").Replace("}", "").Trim();
        }

        ///// <summary>
        ///// Remove all double-quotes and backslashes from a string to avoid broken strings and code injections.
        ///// </summary>
        ///// <param name="str">The string to sanitize</param>
        ///// <returns>A string without double-quotes and backslashes</returns>
        //internal static string SanitizeString(string str)
        //{
        //    if (string.IsNullOrEmpty(str)) return "";

        //    return str.Replace('\\', '/').Replace("\"", "''").Replace("\r\n", "\n").Trim(' ', '\n', '\t');
        //}

        /////// <summary>
        /////// Should a unit group be hidden on the F10 map and other planning info?
        /////// </summary>
        /////// <param name="unitGroup">Database info about a unit group</param>
        /////// <param name="oppositionKnown"><see cref="Template.MissionTemplate.OptionsShowEnemyUnits"/> setting</param>
        /////// <returns>True if hidden, false if visible</returns>
        ////internal static bool ShouldUnitBeHidden(DBUnitGroup unitGroup, bool oppositionKnown)
        ////{
        ////    if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.NeverOnMap)) return true;
        ////    if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.AlwaysOnMap)) return false;
        ////    if (!oppositionKnown && !unitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly)) return true;
        ////    return false;
        ////}
        ///

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
