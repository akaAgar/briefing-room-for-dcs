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

namespace BriefingRoom4DCS
{
    /// <summary>
    /// A static "toolbox" class with various methods and constants used by BriefingRoom
    /// </summary>
    internal static class Toolbox
    {
        /// <summary>
        /// Maximum number of aircraft per flightgroup.
        /// </summary>
        internal const int MAXIMUM_FLIGHT_GROUP_SIZE = 4;

        /// <summary>
        /// Degrees to radians multipier constant.
        /// </summary>
        internal const double DEGREES_TO_RADIANS = 0.0174533;

        /// <summary>
        /// Radians to degrees multipier constant.
        /// </summary>
        internal const double RADIANS_TO_DEGREES = 57.2958;

        /// <summary>
        /// Meters to nautical miles multipier constant.
        /// </summary>
        internal const double METERS_TO_NM = 0.000539957;

        /// <summary>
        /// Nautical miles to meters multipier constant.
        /// </summary>
        internal const double NM_TO_METERS = 1852.0;

        /// <summary>
        /// The total number of minutes in a day.
        /// </summary>
        internal const int MINUTES_PER_DAY = 24 * 60;

        /// <summary>
        /// The total number of seconds in a day.
        /// </summary>
        internal const int SECONDS_PER_DAY = MINUTES_PER_DAY * 60;

        /// <summary>
        /// Feet to meters multiplier.
        /// </summary>
        internal const double FEET_TO_METERS = 0.3048;

        /// <summary>
        /// Knots to meters per second multiplier.
        /// </summary>
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
            return textInfo.ToTitleCase(key.Replace("-"," "));
        }
        /// <summary>
        /// Tries to create a directory if it doesn't exist already.
        /// </summary>
        /// <param name="path">The directory to create.</param>
        /// <returns>True if the directory already exists or was created successfully, false otherwise.</returns>
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


        /// <summary>
        /// meters per second to Knots multiplier.
        /// </summary>
        internal const double METERS_PER_SECOND_TO_KNOTS = 1.94384;

        /// <summary>
        /// Two times Pi.
        /// </summary>
        internal const double TWO_PI = Math.PI * 2;

        /// <summary>
        /// Unit families which can be used as carriers.
        /// </summary>
        internal static readonly UnitFamily[] CARRIER_FAMILIES = new UnitFamily[] { UnitFamily.ShipCarrierCATOBAR, UnitFamily.ShipCarrierSTOBAR, UnitFamily.FOB };

        /// <summary>
        /// An instance of the Random class for all randomization methods.
        /// </summary>
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Case insensitive string comparison.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="string2">Another string.</param>
        /// <returns>True if both strings are the same, false otherwise</returns>
        internal static bool StringICompare(string string1, string string2)
        {
            if ((string1 == null) || (string2 == null)) return string1 == string2;

            return string1.ToLowerInvariant() == string2.ToLowerInvariant();
        }

        /// <summary>
        /// Converts a number to an invariant culture string. Just to make sure decimal separator is always a dot, not a comma like in some languages (which can cause problems).
        /// </summary>
        /// <param name="value">The number to convert</param>
        /// <returns>A string</returns>
        internal static string ToInvariantString(this double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the DCS World unit category Lua game for an unit category (see https://wiki.hoggitworld.com/view/DCS_Class_Unit)
        /// </summary>
        /// <param name="unitCategory">The unit category</param>
        /// <returns>A string</returns>
        internal static string ToLuaName(this UnitCategory unitCategory)
        {
            switch (unitCategory)
            {
                case UnitCategory.Helicopter: return "HELICOPTER";
                case UnitCategory.Plane: return "AIRPLANE";
                case UnitCategory.Ship: return "SHIP";
                case UnitCategory.Static: return "STRUCTURE";
                default: return "GROUND_UNIT"; // case UnitCategory.Vehicle
            }
        }

        /// <summary>
        /// Removes invalid path characters from a filename and replaces them with underscores.
        /// </summary>
        /// <param name="fileName">A filename.</param>
        /// <returns>The filename, without invalid characters.</returns>
        internal static string RemoveInvalidPathCharacters(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "_";
            return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
        }


        /// <summary>
        /// Returns a random angle in radians.
        /// </summary>
        /// <returns>Random angle, in radians.</returns>
        internal static double RandomAngle()
        {
            return RandomDouble(TWO_PI);
        }

        /// <summary>
        /// Check if a filepath is valid.
        /// Code by https://stackoverflow.com/questions/6198392/check-whether-a-path-is-valid
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <param name="allowRelativePaths">Should relative paths be allowed?</param>
        /// <returns>True if the path is valid, false otherwise</returns>
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

        /// <summary>
        /// Randomize the items in an array.
        /// </summary>
        /// <typeparam name="T">Type of the array elements.</typeparam>
        /// <param name="array">An array</param>
        /// <returns>A shuffled array.</returns>
        internal static T[] ShuffleArray<T>(T[] array)
        {
            return array.OrderBy(x => Rnd.Next()).ToArray();
        }

        /// <summary>
        /// Create a string/object from a string, which is used a the key.
        /// </summary>
        /// <param name="key">The string, to use as a key.</param>
        /// <param name="value">The pairs's value.</param>
        /// <returns>A string/object key value pair.</returns>
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
            return (Coalition)(1 - (int)coalition);
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

        /// <summary>
        /// Makes sure a file name/path ends with the proper extension.
        /// If the extension is not present, append it to the file path.
        /// </summary>
        /// <param name="filePath">Absolute path to a file, or filename.</param>
        /// <param name="extension">File extension, WITH THE LEADING DOT (e.g. ".lua")</param>
        /// <returns>The file name/path, with the added extension if it was missing.</returns>
        internal static string AddMissingFileExtension(string filePath, string extension)
        {
            if (filePath == null) return null;
            if (!filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                return $"{filePath}{extension}";

            return filePath;
        }

        /// <summary>
        /// Makes sure each file name/path in an array ends with the proper extension.
        /// If the extension is not present, append it to the file path.
        /// </summary>
        /// <param name="filePath">Array of absolute paths to a file, or filenames.</param>
        /// <param name="extension">File extension, WITH THE LEADING DOT (e.g. ".lua")</param>
        /// <returns>An array of file names/paths, with the added extension if it was missing.</returns>
        internal static string[] AddMissingFileExtensions(string[] filePaths, string extension)
        {
            if (filePaths == null) return null;

            return (from string filePath in filePaths select AddMissingFileExtension(filePath, extension)).ToArray();
        }

        /// <summary>
        /// Makes sure all unit families in an array belong to the same unit category.
        /// (e.g. <see cref="UnitFamily.HelicopterAttack"/> and <see cref="UnitFamily.HelicopterUtility"/> can be mixed, but not <see cref="UnitFamily.HelicopterAttack"/> and <see cref="UnitFamily.VehicleAAA"/>.
        /// If that's not the case, removes all families which do not belong to the same category as unitFamily[0].
        /// </summary>
        /// <param name="unitFamilies">An array of <see cref="UnitFamily"/>.</param>
        /// <returns>An array of <see cref="UnitFamily"/>.</returns>
        internal static UnitFamily[] SetSingleCategoryFamilies(UnitFamily[] unitFamilies)
        {
            if ((unitFamilies == null) || (unitFamilies.Length == 0)) return new UnitFamily[0];

            return (from UnitFamily unitFamily in unitFamilies
                    where unitFamily.GetUnitCategory() == unitFamilies[0].GetUnitCategory()
                    select unitFamily).Distinct().ToArray();
        }

        /// <summary>
        /// Returns a random year from the provided decade.
        /// </summary>
        /// <param name="decade">A decade between the 1940s and the 2010s</param>
        /// <returns>A year</returns>
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


        /// <summary>
        /// Path to the Windows user directory.
        /// </summary>
        internal static string PATH_USER { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        /// <summary>
        /// Path to the Windows "My Documents" directory.
        /// </summary>
        internal static string PATH_USER_DOCS { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));


        /// <summary>
        /// Flags required to center text properly.
        /// </summary>
        internal const TextFormatFlags CENTER_TEXT_FLAGS = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

        /// <summary>
        /// Returns all values for a given enum.
        /// Basically a shortcut for (T[])Enum.GetValues(typeof(T)).
        /// </summary>
        /// <typeparam name="T">A type of enum</typeparam>
        /// <returns>All enum values</returns>
        internal static T[] GetEnumValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Return a random value value if <see cref="AmountNR"/> is set to random, else returns the selected value.
        /// </summary>
        internal static AmountNR Get(this AmountNR amountNR)
        {
            return AmountNR.Random == amountNR ? (AmountNR)(RandomInt((int)AmountNR.VeryHigh) + 1) : amountNR;
        }

        /// <summary>
        /// Rolls for boolean value.
        /// </summary>
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

        /// <summary>
        /// Parses a string to an array of enums.
        /// </summary>
        /// <typeparam name="T">The type of enum to parse to.</typeparam>
        /// <param name="enumString">The string.</param>
        /// <param name="separator">The character used to separate values. Default is comma (,).</param>
        /// <param name="prefix">A prefix to add at the beginning of each value in the string. Default is none.</param>
        /// <returns>An array of enums of type T.</returns>
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

        /// <summary>
        /// Makes sure an angle is between 0 and 2*Pi.
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>Angle of same value, clamped between 0 and 2*Pi</returns>
        internal static double ClampAngle(double angle)
        {
            int angleDeg = (int)(angle * RADIANS_TO_DEGREES);
            while (angleDeg < 0) { angleDeg += 360; }
            angleDeg %= 360;
            return angleDeg * DEGREES_TO_RADIANS;
        }

        /// <summary>
        /// Returns the number of values in an enum. Basically a shortcut for "Enum.GetValues(typeof(T)).Length".
        /// </summary>
        /// <typeparam name="T">The type of enum.</typeparam>
        /// <returns>The number of values.</returns>
        internal static int EnumCount<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        /// <summary>
        /// Returns the UnitCategory an UnitFamily belongs to.
        /// </summary>
        /// <param name="unitFamily">The unit family.</param>
        /// <returns>The unit category.</returns>
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
                    return UnitCategory.Static;
                case UnitFamily.StaticGroup:
                    return UnitCategory.StaticGroup;
                default:
                    return UnitCategory.Vehicle;
            }
        }

        /// <summary>
        /// Do units belonging to this category are aircraft?
        /// </summary>
        /// <param name="category">An unit category</param>
        /// <returns>True if unit is an aircraft (helicopter or plane), false otherwise.</returns>
        internal static bool IsAircraft(this UnitCategory unitCategory)
        {
            return (unitCategory == UnitCategory.Helicopter) || (unitCategory == UnitCategory.Plane);
        }

        /// <summary>
        /// Is this unit family an air defense unit family (SAM, MANPADS, AAA...)?
        /// </summary>
        /// <param name="unitFamily">An unit family.</param>
        /// <returns>True if the unit family is an air defense family, false otherwise.</returns>
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

        /// <summary>
        /// Is this unit family an carrier ship family?
        /// </summary>
        /// <param name="unitFamily">An unit family.</param>
        /// <returns>True if the unit family is an carrier ship family, false otherwise.</returns>
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

        /// <summary>
        /// Converts an object to a string with proper formatting for use in Lua files, etc.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stringFormat">String format, if any.</param>
        /// <returns>The value as a string.</returns>
        internal static string ValToString(object value, string stringFormat = "")
        {
            if (value == null) return "";
            if (value is string) return (string)value;
            if (value is bool) return ((bool)value).ToString(NumberFormatInfo.InvariantInfo);
            if (value is int) return ((int)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            if (value is float) return ((float)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            if (value is double) return ((double)value).ToString(stringFormat, NumberFormatInfo.InvariantInfo);
            return value.ToString();
        }

        /// <summary>
        /// Converts a string to a double. Basically, a shortcut for Convert.ToDouble(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The double.</returns>
        internal static double StringToDouble(string stringValue, double defaultValue = 0.0)
        {
            try { return Convert.ToDouble(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Converts a string to an integer. Basically, a shortcut for Convert.ToInt32(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The integer.</returns>
        internal static int StringToInt(string stringValue, int defaultValue = 0)
        {
            try { return Convert.ToInt32(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Normalize a Windows directory path. Make sure all slashes are backslashes and that the directory ends with a backslash.
        /// </summary>
        /// <param name="path">The directory path to normalize.</param>
        /// <returns>The normalized directory path.</returns>
        internal static string NormalizeDirectoryPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            return path.Replace('/', '\\').TrimEnd('\\') + "\\";
        }

        /// <summary>
        /// Returns a linear interpolated value between value1 and value 2.
        /// </summary>
        /// <param name="value1">The first double.</param>
        /// <param name="value2">The second double.</param>
        /// <param name="linearInterpolation">Lerp parameter.</param>
        /// <returns>The value</returns>
        internal static double Lerp(double value1, double value2, double linearInterpolation)
        {
            return value1 * (1 - linearInterpolation) + value2 * linearInterpolation;
        }

        /// <summary>
        /// Clamps a value between min and max.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        internal static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Returns a random value from an array of type T.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>A random value, or the default value of type T if the array was empty or null.</returns>
        internal static T RandomFrom<T>(params T[] array)
        {
            if ((array == null) || (array.Length == 0)) return default;
            return array[Rnd.Next(array.Length)];
        }

        /// <summary>
        /// Returns a random value from a list of type T.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A random value, or the default value of type T if the list was empty or null.</returns>
        internal static T RandomFrom<T>(List<T> list)
        {
            if ((list == null) || (list.Count == 0)) return default;
            return list[Rnd.Next(list.Count)];
        }

        /// <summary>
        /// Returns true one time out of oneOutOf. Return false the rest of the time.
        /// </summary>
        /// <returns>True or false.</returns>
        internal static bool RandomChance(int oneOutOf)
        { return Rnd.Next(oneOutOf) == 0; }

        /// <summary>
        /// Returns a random integer between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>A random integer.</returns>
        internal static int RandomInt()
        { return Rnd.Next(); }

        /// <summary>
        /// Returns a random integer between 0 and max (excluded).
        /// </summary>
        /// <param name="max">Maximum value (excluded).</param>
        /// <returns>A random integer.</returns>
        internal static int RandomInt(int max)
        { return Rnd.Next(max); }

        /// <summary>
        /// Returns a random integer between min (included) and max (excluded).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (excluded).</param>
        /// <returns>A random integer.</returns>
        internal static int RandomInt(int min, int max)
        { return Rnd.Next(min, max); }

        /// <summary>
        /// Returns a random integer between min (included) and max (included).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random integer.</returns>
        internal static int RandomMinMax(int min, int max)
        { return Rnd.Next(min, max + 1); }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double.</returns>
        internal static double RandomDouble()
        { return Rnd.NextDouble(); }

        /// <summary>
        /// Returns a random double between 0.0 and max (included).
        /// </summary>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random double.</returns>
        internal static double RandomDouble(double max)
        { return Rnd.NextDouble() * max; }

        /// <summary>
        /// Returns a random double between min (included) and max (included).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random double.</returns>
        internal static double RandomDouble(double min, double max)
        { return (Rnd.NextDouble() * (max - min)) + min; }

        /// <summary>
        /// Returns a string representing the ordinal adjective (1st, 2nd, 3rd, 4th...) for a given integer.
        /// </summary>
        /// <param name="number">The integer.</param>
        /// <returns>A string with the ordinal adjective.</returns>
        internal static string GetOrdinalAdjective(int number)
        {
            string numberStr = ValToString(number);

            if (numberStr.EndsWith("11") || numberStr.EndsWith("12") || numberStr.EndsWith("13")) return $"{number}th";
            if (numberStr.EndsWith("3")) return $"{number}rd";
            if (numberStr.EndsWith("2")) return $"{number}nd";
            if (numberStr.EndsWith("1")) return $"{number}st";

            return $"{number}th";
        }

        /// <summary>
        /// Returns a ByteArray of a Zipped folder
        /// </summary>
        /// <param name="FileEntries">Files to be Zipped.</param>
        /// <returns>Returns a ByteArray of a Zipped folder</returns>
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
                BriefingRoom.PrintToLog(ex.Message, LogMessageErrorLevel.Error);
                return null;
            }

            return mizBytes;
        }
    }
}
