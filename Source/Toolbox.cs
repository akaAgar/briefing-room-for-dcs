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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// A static "toolbox" class with various methods and constants used by BriefingRoom
    /// </summary>
    public static class Toolbox
    {
        /// <summary>
        /// The number of days in each month.
        /// </summary>
        private static readonly int[] DAYS_PER_MONTH = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        /// <summary>
        /// Returns the number of days in a month.
        /// </summary>
        /// <param name="month">The month of the year.</param>
        /// <param name="year">The year. Used to know if it's a leap year.</param>
        /// <returns>The number of days in the month.</returns>
        public static int GetDaysPerMonth(Month month, int year)
        {
            // Not february, return value stored in DAYS_PER_MONTH array
            if (month != Month.February) return DAYS_PER_MONTH[(int)month];

            bool leapYear = false;
            if ((year % 400) == 0) leapYear = true;
            else if ((year % 100) == 0) leapYear = false;
            else if ((year % 4) == 0) leapYear = true;

            return leapYear ? 29 : 28;
        }

        /// <summary>
        /// Returns a string with the first letter cast to upper case.
        /// </summary>
        /// <param name="value">A string</param>
        /// <param name="vcastRestOfStringToLowerCasealue">Should the rest of the string be cast to lowercase?</param>
        /// <returns>A string of null if the input string was null</returns>
        public static string ICase(string value, bool castRestOfStringToLowerCase = false)
        {
            if (value == null) return null;
            if (value.Length < 2) return value.ToUpperInvariant();
            return value[0].ToString().ToUpperInvariant() + (castRestOfStringToLowerCase ? value.Substring(1).ToLowerInvariant() : value.Substring(1));
        }

        /// <summary>
        /// Maximum number of aircraft per flightgroup.
        /// </summary>
        public const int MAXIMUM_FLIGHT_GROUP_SIZE = 4;

        public static Decade YearToTimePeriod(int year)
        {
            if (year >= 2010) return Decade.Decade2010;
            if (year >= 2000) return Decade.Decade2000;
            if (year >= 1990) return Decade.Decade1990;
            if (year >= 1980) return Decade.Decade1980;
            if (year >= 1970) return Decade.Decade1970;
            if (year >= 1960) return Decade.Decade1960;
            if (year >= 1950) return Decade.Decade1950;
            return Decade.Decade1940;
        }

        /// <summary>
        /// Path to the Windows user directory.
        /// </summary>
        public static string PATH_USER { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        /// <summary>
        /// Path to the Windows "My Documents" directory.
        /// </summary>
        public static string PATH_USER_DOCS { get; } = NormalizeDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        /// <summary>
        /// Flags required to center text properly.
        /// </summary>
        public const TextFormatFlags CENTER_TEXT_FLAGS = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

        /// <summary>
        /// Returns all values for a given enum.
        /// Basically a shortcut for (T[])Enum.GetValues(typeof(T)).
        /// </summary>
        /// <typeparam name="T">A type of enum</typeparam>
        /// <returns>All enum values</returns>
        public static T[] GetEnumValues<T>() where T : struct
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Degrees to radians multipier constant.
        /// </summary>
        public const double DEGREES_TO_RADIANS = 0.0174533;

        /// <summary>
        /// Radians to degrees multipier constant.
        /// </summary>
        public const double RADIANS_TO_DEGREES = 57.2958;

        /// <summary>
        /// Meters to nautical miles multipier constant.
        /// </summary>
        public const double METERS_TO_NM = 0.000539957;

        /// <summary>
        /// Nautical miles to meters multipier constant.
        /// </summary>
        public const double NM_TO_METERS = 1852.0;

        /// <summary>
        /// The total number of minutes in a day.
        /// </summary>
        public const int MINUTES_PER_DAY = 24 * 60;

        /// <summary>
        /// The total number of seconds in a day.
        /// </summary>
        public const int SECONDS_PER_DAY = MINUTES_PER_DAY * 60;

        /// <summary>
        /// Feet to meters multiplier.
        /// </summary>
        public const double FEET_TO_METERS = 0.3048;

        /// <summary>
        /// Knots to meters per second multiplier.
        /// </summary>
        public const double KNOTS_TO_METERS_PER_SECOND = 0.514444;

        /// <summary>
        /// The default language for missions and UI. BriefingRoom will not start if this language is not found in Library\Languages.
        /// </summary>
        public const string DEFAULT_LANGUAGE = "English";

        /// <summary>
        /// An instance of the Random class for all randomization methods.
        /// </summary>
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Two times Pi.
        /// </summary>
        public const double TWO_PI = Math.PI * 2;

        /// <summary>
        /// Parses a string to an array of enums.
        /// </summary>
        /// <typeparam name="T">The type of enum to parse to.</typeparam>
        /// <param name="enumString">The string.</param>
        /// <param name="separator">The character used to separate values. Default is comma (,).</param>
        /// <param name="prefix">A prefix to add at the beginning of each value in the string. Default is none.</param>
        /// <returns>An array of enums of type T.</returns>
        public static T[] ParseEnumString<T>(string enumString, char separator = ',', string prefix = "") where T : struct
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
        /// Returns the number of values in an enum. Basically a shortcut for "Enum.GetValues(typeof(T)).Length".
        /// </summary>
        /// <typeparam name="T">The type of enum.</typeparam>
        /// <returns>The number of values.</returns>
        public static int EnumCount<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        /// <summary>
        /// Do units belonging to this category are aircraft?
        /// </summary>
        /// <param name="category">An unit category</param>
        /// <returns>True if unit is an aircraft (helicopter or plane), false otherwise.</returns>
        public static bool IsAircraft(UnitCategory category)
        {
            return (category == UnitCategory.Helicopter) || (category == UnitCategory.Plane);
        }

        /// <summary>
        /// Returns the UnitCategory an UnitFamily belongs to.
        /// </summary>
        /// <param name="family">The unit family.</param>
        /// <returns>The unit category.</returns>
        public static UnitCategory GetUnitCategoryFromUnitFamily(UnitFamily family)
        {
            string roleStr = family.ToString().ToLowerInvariant();

            if (roleStr.StartsWith("helicopter")) return UnitCategory.Helicopter;
            if (roleStr.StartsWith("plane")) return UnitCategory.Plane;
            if (roleStr.StartsWith("ship")) return UnitCategory.Ship;
            if (roleStr.StartsWith("static")) return UnitCategory.Static;
            return UnitCategory.Vehicle;
        }

        /// <summary>
        /// Creates a directory if it doesn't exist already.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>True if successful or if directory was already present, false if creation was required and failed.</returns>
        public static bool CreateDirectoryIfMissing(string path)
        {
            if (Directory.Exists(path)) return true;

            try { Directory.CreateDirectory(path); }
            catch (Exception) { return false; }

            return true;
        }

        /// <summary>
        /// Converts a boolean to a string. Basically, a shortcut for ToString(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as a string.</returns>
        public static string ValToString(bool value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns a System.Drawing.Image as bytes for an image file of the specified format.
        /// </summary>
        /// <param name="image">An image</param>
        /// <param name="format">An image format</param>
        /// <returns>Bytes of an image file</returns>
        public static byte[] ImageToBytes(Image image, ImageFormat format)
        {
            byte[] imageBytes;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                imageBytes = ms.ToArray();
            }

            return imageBytes;
        }

        /// <summary>
        /// Converts an integer to a string. Basically, a shortcut for ToString(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as a string.</returns>
        public static string ValToString(int value, string stringFormat = null)
        {
            if (string.IsNullOrEmpty(stringFormat))
                return value.ToString(NumberFormatInfo.InvariantInfo);
            else
                return value.ToString(stringFormat, NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Converts a double to a string. Basically, a shortcut for ToString(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value, as a string.</returns>
        public static string ValToString(double value, string stringFormat = null)
        {
            if (string.IsNullOrEmpty(stringFormat))
                return value.ToString(NumberFormatInfo.InvariantInfo);
            else
                return value.ToString(stringFormat, NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Converts a string to a double. Basically, a shortcut for Convert.ToDouble(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The double.</returns>
        public static double StringToDouble(string stringValue, double defaultValue = 0.0)
        {
            try { return Convert.ToDouble(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Converts a string to a boolean. Basically, a shortcut for Convert.ToBoolean(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The bool.</returns>
        public static bool StringToBool(string stringValue, bool defaultValue = false)
        {
            try { return Convert.ToBoolean(stringValue, NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Converts a string to a float. Basically, a shortcut for Convert.ToSingle(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The float.</returns>
        public static float StringToFloat(string stringValue, float defaultValue = 0.0f)
        {
            try { return Convert.ToSingle(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Converts a string to an integer. Basically, a shortcut for Convert.ToInt32(NumberFormatInfo.InvariantInfo).
        /// </summary>
        /// <param name="stringValue">The string to convert.</param>
        /// <param name="defaultValue">The default value to return if the string parsing fails.</param>
        /// <returns>The integer.</returns>
        public static int StringToInt(string stringValue, int defaultValue = 0)
        {
            try { return Convert.ToInt32(stringValue.Trim(), NumberFormatInfo.InvariantInfo); }
            catch (Exception) { return defaultValue; }
        }

        /// <summary>
        /// Search for the DCS world custom mission path ([User]\Saved Games\DCS\Missions\)
        /// Looks first for DCS.earlyaccess, then DCS.openbeta, then DCS.
        /// </summary>
        /// <returns>The path, or the user My document folder if none is found.</returns>
        public static string GetDCSMissionPath()
        {
            string[] possibleDCSPaths = new string[] { "DCS.earlyaccess", "DCS.openbeta", "DCS" };

            for (int i = 0; i < possibleDCSPaths.Length; i++)
            {
                string dcsPath = PATH_USER + "Saved Games\\" + possibleDCSPaths[i] + "\\Missions\\";
                if (Directory.Exists(dcsPath)) return dcsPath;
            }

            return PATH_USER_DOCS;
        }

        /// <summary>
        /// Normalize a Windows directory path. Make sure all slashes are backslashes and that the directory ends with a backslash.
        /// </summary>
        /// <param name="path">The directory path to normalize.</param>
        /// <returns>The normalized directory path.</returns>
        public static string NormalizeDirectoryPath(string path)
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
        public static double Lerp(double value1, double value2, double linearInterpolation)
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
        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Clamps a value between min and max.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Clamps a value between min and max.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Returns a random value from an array of type T.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>A random value, or the default value of type T if the array was empty or null.</returns>
        public static T RandomFrom<T>(params T[] array)
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
        public static T RandomFrom<T>(List<T> list)
        {
            if ((list == null) || (list.Count == 0)) return default;
            return list[Rnd.Next(list.Count)];
        }

        /// <summary>
        /// Returns true one time out of oneOutOf. Return false the rest of the time.
        /// </summary>
        /// <returns>True or false.</returns>
        public static bool RandomChance(int oneOutOf)
        { return Rnd.Next(oneOutOf) == 0; }

        /// <summary>
        /// Returns a random integer between 0 and Int32.MaxValue.
        /// </summary>
        /// <returns>A random integer.</returns>
        public static int RandomInt()
        { return Rnd.Next(); }

        /// <summary>
        /// Returns a random integer between 0 and max (excluded).
        /// </summary>
        /// <param name="max">Maximum value (excluded).</param>
        /// <returns>A random integer.</returns>
        public static int RandomInt(int max)
        { return Rnd.Next(max); }

        /// <summary>
        /// Returns a random integer between min (included) and max (excluded).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (excluded).</param>
        /// <returns>A random integer.</returns>
        public static int RandomInt(int min, int max)
        { return Rnd.Next(min, max); }

        /// <summary>
        /// Returns a random integer between min (included) and max (included).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random integer.</returns>
        public static int RandomMinMax(int min, int max)
        { return Rnd.Next(min, max + 1); }

        /// <summary>
        /// Returns a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A random double.</returns>
        public static double RandomDouble()
        { return Rnd.NextDouble(); }

        /// <summary>
        /// Returns a random double between 0.0 and max (included).
        /// </summary>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random double.</returns>
        public static double RandomDouble(double max)
        { return Rnd.NextDouble() * max; }

        /// <summary>
        /// Returns a random double between min (included) and max (included).
        /// </summary>
        /// <param name="min">Minimum value (included).</param>
        /// <param name="max">Maximum value (included).</param>
        /// <returns>A random double.</returns>
        public static double RandomDouble(double min, double max)
        { return (Rnd.NextDouble() * (max - min)) + min; }

        /// <summary>
        /// Returns a string representing the ordinal adjective (1st, 2nd, 3rd, 4th...) for a given integer.
        /// </summary>
        /// <param name="number">The integer.</param>
        /// <returns>A string with the ordinal adjective.</returns>
        public static string GetOrdinalAdjective(int number)
        {
            string numberStr = ValToString(number);

            if (numberStr.EndsWith("11") || numberStr.EndsWith("12") || numberStr.EndsWith("13")) return $"{number}th";
            if (numberStr.EndsWith("3")) return $"{number}rd";
            if (numberStr.EndsWith("2")) return $"{number}nd";
            if (numberStr.EndsWith("1")) return $"{number}st";

            return $"{number}th";
        }

        /// <summary>
        /// Turns a camel cased enum name into a string with spaces between words.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns>A string</returns>
        public static string SplitCamelCase<T>(T enumValue) where T : struct
        {
            string input = enumValue.ToString();
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (i == 0)
                {
                    output += input[i].ToString().ToUpperInvariant();
                    continue;
                }
                else if (i == input.Length - 1)
                {
                    output += input[i];
                    break;
                }

                if (char.IsUpper(input[i]))
                {
                    if (!char.IsUpper(input[i - 1]))
                    {
                        output += " ";
                        output += input[i];
                        continue;
                    }

                    if (!char.IsUpper(input[i + 1]))
                    {
                        output += " ";
                        output += input[i].ToString().ToLowerInvariant();
                        continue;
                    }

                }

                output += input[i];
            }

            return output;

            //return Regex.Replace(Regex.Replace(enumValue.ToString(), @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        /// <summary>
        /// Join the words from a string split by GUITools.SplitEnumCamelCase and turn it back into an enum value
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="enumString">A string split by SplitEnumCamelCase</param>
        /// <returns>A value of enum T</returns>
        public static T JoinCamelCase<T>(string enumString) where T : struct
        {
            if (string.IsNullOrEmpty(enumString)) return default;
            enumString = enumString.Replace(" ", "");
            if (Enum.TryParse(enumString, true, out T parsedEnum)) return parsedEnum;
            return default;
        }

    }
}
