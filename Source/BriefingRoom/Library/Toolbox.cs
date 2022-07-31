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

using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO.Compression;
using BriefingRoom4DCS.Data;
using System.Text.RegularExpressions;

namespace BriefingRoom4DCS
{
    internal static class Toolbox
    {
        internal const int MAXIMUM_FLIGHT_GROUP_SIZE = 4;

        internal const double DEGREES_TO_RADIANS = 0.0174533;

        internal const double RADIANS_TO_DEGREES = 57.2958;

        internal const double METERS_TO_NM = 0.000539957;

        internal const double NM_TO_METERS = 1852.0;

        internal const int MINUTES_PER_DAY = 24 * 60;

        internal const int SECONDS_PER_DAY = MINUTES_PER_DAY * 60;

        internal const double FEET_TO_METERS = 0.3048;

        internal const double KNOTS_TO_METERS_PER_SECOND = 0.514444;

        internal static List<string> ALIASES = new List<string>{
                "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliet", "Kilo",
                "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor",
                "Whiskey", "X-Ray", "Yankee", "Zulu"};

        internal static string FormatPayload(string key)
        {
            if (key == "default")
            {
                return "General";
            }
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(key.Replace("-", " "));
        }
        internal static bool CreateMissingDirectory(string path)
        {
            if (Directory.Exists(path)) return true;

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception) { return false; }

            return true;
        }

        internal static string GetAlias(int index) =>
            ALIASES[index % ALIASES.Count];


        internal const double METERS_PER_SECOND_TO_KNOTS = 1.94384;

        internal const double TWO_PI = Math.PI * 2;

        internal static readonly UnitFamily[] CARRIER_FAMILIES = new UnitFamily[] { UnitFamily.ShipCarrierCATOBAR, UnitFamily.ShipCarrierSTOBAR, UnitFamily.ShipCarrierSTOVL, UnitFamily.FOB };

        private static readonly Random Rnd = new Random();

        internal static bool StringICompare(string string1, string string2)
        {
            if ((string1 == null) || (string2 == null)) return string1 == string2;

            return string1.ToLowerInvariant() == string2.ToLowerInvariant();
        }

        internal static string ToInvariantString(this double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        internal static Dictionary<string, object> ToDictionaryObject(string dataStr)
        {
            var dict = new Dictionary<string, object>();
            var regex = new Regex("\\[\\\"(.*?)\\\"\\] ?= ?(.*)");
            if (string.IsNullOrEmpty(dataStr))
                return dict;
            foreach (string str in dataStr.Split(","))
            {
                if (!regex.IsMatch(str))
                    throw new Exception($"Invalid Lua {dataStr}");
                var match = regex.Match(str);
                dict.Add(match.Groups[1].Value, DeterminType(match.Groups[2].Value));
            }
            return dict;
        }

        internal static object DeterminType(object value)
        {
            if (!(value is string))
                return value;
            var strVal = value as string;
            if (bool.TryParse(strVal, out bool boolVal))
                return boolVal;
            if (int.TryParse(strVal, out int intVal))
                return intVal;
            if (float.TryParse(strVal, out float floatVal))
                return floatVal;
            return strVal;
        }

        internal static string ToLuaName(this UnitCategory unitCategory)
        {
            switch (unitCategory)
            {
                case UnitCategory.Helicopter: return "HELICOPTER";
                case UnitCategory.Plane: return "AIRPLANE";
                case UnitCategory.Ship: return "SHIP";
                case UnitCategory.Cargo:
                case UnitCategory.Static: return "STRUCTURE";
                default: return "GROUND_UNIT"; // case UnitCategory.Vehicle
            }
        }

        internal static string RemoveInvalidPathCharacters(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "_";
            return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
        }


        internal static double RandomAngle()
        {
            return RandomDouble(TWO_PI);
        }

        internal static bool IsFilePathValid(string path, bool allowRelativePaths = false)
        {
            bool isValid;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path);
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '/' })) == false;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }
        internal static T ElementAtOrDefault<T>(this IList<T> list, int index, T @default)
        {
            return index >= 0 && index < list.Count ? list[index] : @default;
        }

        internal static T[] ShuffleArray<T>(T[] array)
        {
            return array.OrderBy(x => Rnd.Next()).ToArray();
        }

        internal static KeyValuePair<string, object> ToKeyValuePair(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        internal static void AddIfKeyUnused<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary.ContainsKey(key)) return;
            dictionary.Add(key, value);
        }

        internal static string ReplaceAll(this string str, string replaceTo, params string[] replaceFrom)
        {
            StringBuilder sb = new StringBuilder(str);

            foreach (string r in replaceFrom)
                sb.Replace(r, replaceTo);

            return sb.ToString();
        }

        internal static Coalition GetEnemy(this Coalition coalition)
        {
            return coalition == Coalition.Blue ? Coalition.Red : Coalition.Blue;
        }

        internal static string ReadAllTextIfFileExists(string filePath)
        {
            if (!File.Exists(filePath)) return "";
            return File.ReadAllText(filePath);
        }

        internal static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        internal static string AddMissingFileExtension(string filePath, string extension)
        {
            if (filePath == null) return null;
            if (!filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                return $"{filePath}{extension}";

            return filePath;
        }

        internal static string[] AddMissingFileExtensions(string[] filePaths, string extension)
        {
            if (filePaths == null) return null;

            return (from string filePath in filePaths select AddMissingFileExtension(filePath, extension)).ToArray();
        }

        internal static UnitFamily[] SetSingleCategoryFamilies(UnitFamily[] unitFamilies)
        {
            if ((unitFamilies == null) || (unitFamilies.Length == 0)) return new UnitFamily[0];

            return (from UnitFamily unitFamily in unitFamilies
                    where unitFamily.GetUnitCategory() == unitFamilies[0].GetUnitCategory()
                    select unitFamily).Distinct().ToArray();
        }

        internal static int GetRandomYearFromDecade(Decade decade)
        {
            switch (decade)
            {
                case Decade.Decade1940: return RandomInt(1942, 1945); // WW2 only
                case Decade.Decade1950: return RandomInt(1950, 1960);
                case Decade.Decade1960: return RandomInt(1960, 1970);
                case Decade.Decade1970: return RandomInt(1970, 1980);
                case Decade.Decade1980: return RandomInt(1980, 1990);
                case Decade.Decade1990: return RandomInt(1990, 2000);
                default: return RandomInt(2000, 2010); // case Decade.Decade2000
                case Decade.Decade2010: return RandomInt(2010, 2020);
                case Decade.Decade2020: return RandomInt(2020, 2030);
            }
        }


        internal static string PATH_USER { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        internal static string PATH_USER_DOCS { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));


        internal const TextFormatFlags CENTER_TEXT_FLAGS = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

        internal static T[] GetEnumValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        internal static AmountNR Get(this AmountNR amountNR)
        {
            return AmountNR.Random == amountNR ? (AmountNR)(RandomInt((int)AmountNR.VeryHigh) + 1) : amountNR;
        }

        internal static bool RollChance(this AmountNR amountN)
        {
            int chance;
            switch (amountN.Get())
            {
                case AmountNR.None:
                    return false;
                case AmountNR.VeryLow:
                    chance = 90;
                    break;
                case AmountNR.Low:
                    chance = 75;
                    break;
                case AmountNR.High:
                    chance = 25;
                    break;
                case AmountNR.VeryHigh:
                    chance = 10;
                    break;
                default:
                    chance = 50;
                    break;
            }
            return (RandomMinMax(1, 100) > chance);
        }

        internal static T[] ParseEnumString<T>(string enumString, char separator = ',', string prefix = "") where T : struct
        {
            if ((enumString == null) || (enumString.Length == 0)) return new T[0];

            string[] strParts = enumString.Split(separator);

            List<T> enumValues = new List<T>();
            foreach (string s in strParts)
            {
                if (Enum.TryParse(prefix + s, true, out T e))
                    enumValues.Add(e);
            }

            return enumValues.ToArray();
        }

        internal static double ClampAngle(double angle)
        {
            int angleDeg = (int)(angle * RADIANS_TO_DEGREES);
            while (angleDeg < 0) { angleDeg += 360; }
            angleDeg %= 360;
            return angleDeg * DEGREES_TO_RADIANS;
        }

        internal static int EnumCount<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        internal static UnitCategory GetUnitCategory(this UnitFamily unitFamily)
        {
            switch (unitFamily)
            {
                case UnitFamily.HelicopterAttack:
                case UnitFamily.HelicopterTransport:
                case UnitFamily.HelicopterUtility:
                    return UnitCategory.Helicopter;
                case UnitFamily.PlaneAttack:
                case UnitFamily.PlaneAWACS:
                case UnitFamily.PlaneBomber:
                case UnitFamily.PlaneDrone:
                case UnitFamily.PlaneFighter:
                case UnitFamily.PlaneInterceptor:
                case UnitFamily.PlaneSEAD:
                case UnitFamily.PlaneStrike:
                case UnitFamily.PlaneTankerBasket:
                case UnitFamily.PlaneTankerBoom:
                case UnitFamily.PlaneTransport:
                case UnitFamily.PlaneCATOBAR:
                case UnitFamily.PlaneSTOBAR:
                case UnitFamily.PlaneSTOVL:
                    return UnitCategory.Plane;
                case UnitFamily.ShipCarrierCATOBAR:
                case UnitFamily.ShipCarrierSTOBAR:
                case UnitFamily.ShipCarrierSTOVL:
                case UnitFamily.ShipCruiser:
                case UnitFamily.ShipFrigate:
                case UnitFamily.ShipSpeedboat:
                case UnitFamily.ShipSubmarine:
                case UnitFamily.ShipTransport:
                    return UnitCategory.Ship;
                case UnitFamily.StaticStructureMilitary:
                case UnitFamily.StaticStructureProduction:
                case UnitFamily.FOB:
                case UnitFamily.StaticStructureOffshore:
                    return UnitCategory.Static;
                case UnitFamily.Cargo:
                    return UnitCategory.Cargo;
                default:
                    return UnitCategory.Vehicle;
            }
        }

        internal static bool IsAircraft(this UnitCategory unitCategory)
        {
            return (unitCategory == UnitCategory.Helicopter) || (unitCategory == UnitCategory.Plane);
        }

        internal static bool IsAirDefense(this UnitFamily unitFamily)
        {
            switch (unitFamily)
            {
                case UnitFamily.VehicleAAA:
                case UnitFamily.VehicleAAAStatic:
                case UnitFamily.VehicleInfantryMANPADS:
                case UnitFamily.VehicleSAMLong:
                case UnitFamily.VehicleSAMMedium:
                case UnitFamily.VehicleSAMShort:
                case UnitFamily.VehicleSAMShortIR:
                    return true;
            }

            return false;
        }

        internal static bool IsCarrier(this UnitFamily unitFamily)
        {
            switch (unitFamily)
            {
                case UnitFamily.ShipCarrierCATOBAR:
                case UnitFamily.ShipCarrierSTOBAR:
                case UnitFamily.ShipCarrierSTOVL:
                    return true;
            }

            return false;
        }

        internal static string ValToString(object value, string stringFormat = "")
        {
            if (value == null) return "";
            if (value is string) return (string)value;
            if (value is bool) return ((bool)value).ToString(NumberFormatInfo.InvariantInfo);
            if (value is int) return ((int)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            if (value is float) return ((float)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            if (value is double) return ((double)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            if (value is LanguageString) return ((LanguageString)value).Get();
            return value.ToString();
        }

        internal static double StringToDouble(string stringValue, double defaultValue = 0.0)
        {
            try { return Convert.ToDouble(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        internal static int StringToInt(string stringValue, int defaultValue = 0)
        {
            try { return Convert.ToInt32(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        internal static string NormalizeDirectoryPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            return path.Replace('/', '\\').TrimEnd('\\') + "\\";
        }

        internal static double Lerp(double value1, double value2, double linearInterpolation)
        {
            return value1 * (1 - linearInterpolation) + value2 * linearInterpolation;
        }

        internal static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        internal static T RandomFrom<T>(params T[] array)
        {
            if ((array == null) || (array.Length == 0)) return default;
            return array[Rnd.Next(array.Length)];
        }

        internal static T RandomFrom<T>(List<T> list)
        {
            if ((list == null) || (list.Count == 0)) return default;
            return list[Rnd.Next(list.Count)];
        }

        internal static bool RandomChance(int oneOutOf)
        { return Rnd.Next(oneOutOf) == 0; }

        internal static int RandomInt()
        { return Rnd.Next(); }

        internal static int RandomInt(int max)
        { return Rnd.Next(max); }

        internal static int RandomInt(int min, int max)
        { return Rnd.Next(min, max); }

        internal static int RandomMinMax(int min, int max)
        { return Rnd.Next(min, max + 1); }

        internal static double RandomDouble()
        { return Rnd.NextDouble(); }

        internal static double RandomDouble(double max)
        { return Rnd.NextDouble() * max; }

        internal static double RandomDouble(double min, double max)
        { return (Rnd.NextDouble() * (max - min)) + min; }

        internal static string GetOrdinalAdjective(int number)
        {
            string numberStr = ValToString(number);

            if (numberStr.EndsWith("11") || numberStr.EndsWith("12") || numberStr.EndsWith("13")) return $"{number}th";
            if (numberStr.EndsWith("3")) return $"{number}rd";
            if (numberStr.EndsWith("2")) return $"{number}nd";
            if (numberStr.EndsWith("1")) return $"{number}st";

            return $"{number}th";
        }

        internal static byte[] ZipData(Dictionary<string, byte[]> FileEntries)
        {
            byte[] mizBytes;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Update))
                    {
                        foreach (string entryKey in FileEntries.Keys)
                        {
                            ZipArchiveEntry entry = zip.CreateEntry(entryKey, CompressionLevel.Optimal);
                            using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                                writer.Write(FileEntries[entryKey]);
                        }
                    }

                    mizBytes = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new BriefingRoomException(ex.Message);
            }

            return mizBytes;
        }
    }
}
