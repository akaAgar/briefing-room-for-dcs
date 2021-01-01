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
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BriefingRoom4DCSWorld.Generator
{
    /// <summary>
    /// A static library of tools to help mission generation.
    /// </summary>
    public static class GeneratorTools
    {
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
        /// <returns>Updated array of units with added embedded air defense units</returns>
        public static string[] AddEmbeddedAirDefense(string[] units, AmountN airDefenseLevel, DBEntryCoalition coalitionDB)
        {
            int airDefenseLevelInt = (int)airDefenseLevel.Get();
            // No luck this time, don't add anything
            if (Toolbox.RandomDouble() >= Database.Instance.Common.EnemyAirDefense[airDefenseLevelInt].EmbeddedChance)
                return units;

            // Convert the unit array to an open-ended list so that units can be added
            List<string> unitsList = new List<string>(units);

            // Add some air defense units
            int embeddedCount = Database.Instance.Common.EnemyAirDefense[airDefenseLevelInt].EmbeddedUnitCount.GetValue();
            for (int i = 0; i < embeddedCount; i++)
                unitsList.AddRange(coalitionDB.GetRandomUnits(Toolbox.RandomFrom(EMBEDDED_AIR_DEFENSE_FAMILIES), 1));

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

        /// <summary>
        /// Checks for a <see cref="DBEntry"/> in <see cref="Database"/> and throws an exception if it isn't found.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="DBEntry"/> to look for</typeparam>
        /// <param name="id">The id of the entry to look for</param>
        public static void CheckDBForMissingEntry<T>(string id) where T : DBEntry
        {
            if (!Database.Instance.EntryExists<T>(id))
                throw new Exception($"{typeof(T).Name} \"{id}\" not found.");
        }

        /// <summary>
        /// Converts a distance in meters to nautical miles or kilometer (according to the prefered unit system) and
        /// returns it as a string with the proper unit symbol.
        /// </summary>
        /// <param name="distanceInMeters">Distance in meters</param>
        /// <param name="unitSystem">Unit system to use</param>
        /// <returns>Distance in nm or Km, with the proper unit symbol</returns>
        public static string ConvertDistance(double distanceInMeters, UnitSystem unitSystem)
        {
            if (unitSystem == UnitSystem.Imperial)
                return $"{distanceInMeters * Toolbox.METERS_TO_NM:F0} nm";

            return $"{distanceInMeters / 1000.0:F0} Km";
        }

        /// <summary>
        /// Returns coordinates situated somewhere between the initial airbase and the center point of objectives.
        /// </summary>
        /// <param name="mission">The DCS mission</param>
        /// <param name="lerpValue">Progess on the airbase to objectives center distance (0.0=airbase, 1.0=objective center)</param>
        /// <returns>A set of coordinates</returns>
        public static Coordinates GetCoordinatesOnFlightPath(DCSMission mission, double lerpValue)
        {
            Coordinates coordinates = Coordinates.Lerp(mission.InitialPosition, mission.ObjectivesCenter, lerpValue);
            double distance = mission.InitialPosition.GetDistanceFrom(mission.ObjectivesCenter);

            // Create some random variation proportional to the total flight path distance
            distance = Math.Max(20.0, distance);
            return coordinates + Coordinates.CreateRandom(distance / 8, distance / 4);
        }

        public static string GetBriefingStringForUnitFamily(UnitFamily unitFamily, bool plural)
        {
            return Database.Instance.Common.UnitBriefingNames[(int)unitFamily][plural ? 1 : 0];
        }

        /// <summary>
        /// Returns the coalition to look for in spawn points according to the template setting regarding enemy units location.
        /// </summary>
        /// <param name="template">A mission template</param>
        /// <returns>A coalition, or null if any coalition can be used</returns>
        public static Coalition? GetEnemySpawnPointCoalition(MissionTemplate template)
        {
            // No preference for enemy units location
            if (template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Any) return null;

            // All countries belong to the same coalition anyway, so preference have no meaning.
            if ((template.TheaterRegionsCoalitions == CountryCoalition.AllBlue) ||
                (template.TheaterRegionsCoalitions == CountryCoalition.AllRed))
                return null;

            if (template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Blue)
                return (template.TheaterRegionsCoalitions == CountryCoalition.Inverted) ? Coalition.Red : Coalition.Blue;

            // Last possibility is "template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Red"
            return (template.TheaterRegionsCoalitions == CountryCoalition.Inverted) ? Coalition.Blue : Coalition.Red;
        }

        /// <summary>
        /// Returns the coalition to look for in spawn points according to the template setting regarding enemy units location.
        /// </summary>
        /// <param name="template">A mission template</param>
        /// <returns>A coalition, or null if any coalition can be used</returns>
        public static Coalition? GetAllySpawnPointCoalition(MissionTemplate template)
        {
            // No preference for enemy units location
            if (template.OppositionUnitsLocation != SpawnPointPreferredCoalition.Any) return null;

            // All countries belong to the same coalition anyway, so preference have no meaning.
            if ((template.TheaterRegionsCoalitions != CountryCoalition.AllBlue) &&
                (template.TheaterRegionsCoalitions != CountryCoalition.AllRed))
                return null;

            if (template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Blue)
                return (template.TheaterRegionsCoalitions == CountryCoalition.Inverted) ? Coalition.Blue : Coalition.Red;

            // Last possibility is "template.OppositionUnitsLocation == SpawnPointPreferredCoalition.Red"
            return (template.TheaterRegionsCoalitions == CountryCoalition.Inverted) ? Coalition.Red : Coalition.Blue;
        }

        /// <summary>
        /// Returns a human-readable string for the mission weather.
        /// </summary>
        /// <param name="weatherConditions">Weather conditions</param>
        /// <returns>Weather conditions, as a string</returns>
        public static string GetEnumString(Weather weatherConditions)
        {
            switch (weatherConditions)
            {
                default: return "Clear"; // Weather.Clear
                case Weather.LightClouds: return "Light clouds";
                case Weather.Overcast: return "Overcast";
                case Weather.Precipitation: return "Precipitation";
                case Weather.SomeClouds: return "Some clouds";
                case Weather.Storm: return "Storm";
            }
        }

        /// <summary>
        /// Returns a human-readable string for the mission wind.
        /// </summary>
        /// <param name="windConditions">Wind conditions</param>
        /// <returns>Wind conditions, as a string</returns>
        public static string GetEnumString(Wind windConditions)
        {
            switch (windConditions)
            {
                default: return "Calm"; // Wind.Calm
                case Wind.GentleBreeze: return "Gentle breeze";
                case Wind.HighWind: return "High wind";
                case Wind.LightAir: return "Light air";
                case Wind.StrongBreeze: return "Strong breeze";
                case Wind.StrongGale: return "Strong gale";
            }
        }

        public static string MakeBriefingStringReplacements(string briefingString, DCSMission mission, DBEntryCoalition[] coalitionsDB, int objectiveIndex = 0)
        {
            DBEntryTheater theaterDB = Database.Instance.GetEntry<DBEntryTheater>(mission.Theater);

            briefingString = briefingString.Replace("$ALLYADJ$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.Adjective]));
            briefingString = briefingString.Replace("$ENEMYADJ$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionEnemy].BriefingElements[(int)CoalitionBriefingElement.Adjective]));
            briefingString = briefingString.Replace("$RECON$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.Recon]));
            briefingString = briefingString.Replace("$STRCOMMAND$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.StrategicCommand]));
            briefingString = briefingString.Replace("$TACCOMMAND$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.TacticalCommand]));
            briefingString = briefingString.Replace("$THEATER$", Toolbox.RandomFrom(theaterDB.BriefingNames));
            briefingString = briefingString.Replace("$THEALLIES$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionPlayer].BriefingElements[(int)CoalitionBriefingElement.TheCoalition]));
            briefingString = briefingString.Replace("$THEENEMIES$", Toolbox.RandomFrom(coalitionsDB[(int)mission.CoalitionEnemy].BriefingElements[(int)CoalitionBriefingElement.TheCoalition]));

            briefingString = briefingString.Replace("$OBJECTIVE$", mission.Objectives[objectiveIndex].Name);

            briefingString = briefingString.Replace("$UNITFAMILIES$",
                mission.Objectives[objectiveIndex].TargetFamily.HasValue ?
                GetBriefingStringForUnitFamily(mission.Objectives[objectiveIndex].TargetFamily.Value, true) : "units");
            briefingString = briefingString.Replace("$UNITFAMILY$",
                mission.Objectives[objectiveIndex].TargetFamily.HasValue ?
                GetBriefingStringForUnitFamily(mission.Objectives[objectiveIndex].TargetFamily.Value, false) : "unit");

            return Toolbox.ICase(SanitizeString(briefingString));
        }

        public static string RemoveAfterComma(string str)
        {
            str = Regex.Replace(str, @",.*", "");

            //str = Regex.Replace(str, @"\(.*\)", "");

            return SanitizeString(str);
        }

        /// <summary>
        /// Randomizes parts of a string.
        /// </summary>
        /// <param name="randomString">The string to randomize</param>
        /// <returns>A randomized string.</returns>
        public static string ParseRandomString(string randomString)
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

        /// <summary>
        /// Remove all double-quotes and backslashes from a string to avoid broken strings and code injections.
        /// </summary>
        /// <param name="str">The string to sanitize</param>
        /// <returns>A string without double-quotes and backslashes</returns>
        public static string SanitizeString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";

            return str.Replace('\\', '/').Replace("\"", "''").Replace("\r\n", "\n").Trim(' ', '\n', '\t');
        }

        /// <summary>
        /// Should a unit group be hidden on the F10 map and other planning info?
        /// </summary>
        /// <param name="unitGroup">Database info about a unit group</param>
        /// <param name="oppositionKnown"><see cref="Template.MissionTemplate.OptionsShowEnemyUnits"/> setting</param>
        /// <returns>True if hidden, false if visible</returns>
        public static bool ShouldUnitBeHidden(DBUnitGroup unitGroup, bool oppositionKnown)
        {
            if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.NeverOnMap)) return true;
            if (unitGroup.Flags.HasFlag(DBUnitGroupFlags.AlwaysOnMap)) return false;
            if (!oppositionKnown && !unitGroup.Flags.HasFlag(DBUnitGroupFlags.Friendly)) return true;
            return false;
        }
    }
}
