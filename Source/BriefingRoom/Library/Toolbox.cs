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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace BriefingRoom4DCS
{
    /// <summary>
    /// A static "toolbox" class with various methods and constants used by BriefingRoom
    /// </summary>
    public static class Toolbox
    {
        public static readonly UnitFamily[] SHIP_CARRIER_FAMILIES = new UnitFamily[] { UnitFamily.ShipCarrierCATOBAR, UnitFamily.ShipCarrierSTOBAR, UnitFamily.ShipCarrierSTOVL };

        public static readonly int DECADES_COUNT = EnumCount<Decade>();
        public static readonly int UNITFAMILIES_COUNT = EnumCount<UnitFamily>();

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

        internal static string ToInvariantString(this double value)
        {
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }

        public static string ToLuaName(this UnitCategory unitCategory)
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
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Returns a string with the first letter cast to upper case.
        /// </summary>
        /// <param name="value">A string</param>
        /// <param name="vcastRestOfStringToLowerCasealue">Should the rest of the string be cast to lowercase?</param>
        /// <returns>A string of null if the input string was null</returns>
        internal static string ICase(string value, bool castRestOfStringToLowerCase = false)
        {
            if (value == null) return null;
            if (value.Length < 2) return value.ToUpperInvariant();
            return value[0].ToString().ToUpperInvariant() + (castRestOfStringToLowerCase ? value.Substring(1).ToLowerInvariant() : value.Substring(1));
        }

        internal static KeyValuePair<string, object> ToKeyValuePair(this string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
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

        public static string ReadAllTextIfFileExists(string filePath)
        {
            if (!File.Exists(filePath)) return "";
            return File.ReadAllText(filePath);
        }

        internal static Point Add(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }

        public static bool IsUnitFamilyAircraft(UnitFamily value)
        {
            return
                (value.GetUnitCategory() == UnitCategory.Helicopter) ||
                (value.GetUnitCategory() == UnitCategory.Plane);
        }

        public static object LowerCaseFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length == 1) return str.ToLowerInvariant();
            return str.Substring(0, 1).ToLowerInvariant() + str.Substring(1);
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
        /// Maximum number of aircraft per flightgroup.
        /// </summary>
        public const int MAXIMUM_FLIGHT_GROUP_SIZE = 4;

        /// <summary>
        /// Returns a random year from the provided decade.
        /// </summary>
        /// <param name="decade">A decade between the 1940s and the 2010s</param>
        /// <returns>A year</returns>
        public static int GetRandomYearFromDecade(Decade decade)
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
        public static T[] GetEnumValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Return a random value value if <see cref="AmountN"/> is set to random, else returns the selected value.
        /// </summary>
        public static AmountNR Get(this AmountNR amountNR)
        {
            return AmountNR.Random == amountNR ? (AmountNR)(RandomInt((int)AmountNR.VeryHigh) + 1) : amountNR;
        }

        //public static CloudPreset Get(this CloudPreset cloudPreset)
        //{
        //    return CloudPreset.Random == cloudPreset ? (CloudPreset)(RandomInt((int)CloudPreset.RainyPreset3) + 1) : cloudPreset;
        //}


        /// <summary>
        /// Rolls for boolean value.
        /// </summary>
        public static bool RollChance(this AmountNR amountN)
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
        /// meters per second to Knots multiplier.
        /// </summary>
        public const double METERS_PER_SECOND_TO_KNOTS = 1.94384;


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
        /// Makes sure an angle is between 0 and 2*Pi.
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>Angle of same value, clamped between 0 and 2*Pi</returns>
        public static double ClampAngle(double angle)
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
        public static int EnumCount<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Length;
        }

        /// <summary>
        /// Returns the UnitCategory an UnitFamily belongs to.
        /// </summary>
        /// <param name="unitFamily">The unit family.</param>
        /// <returns>The unit category.</returns>
        public static UnitCategory GetUnitCategory(this UnitFamily unitFamily)
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
                    return UnitCategory.Static;
                default:
                    return UnitCategory.Vehicle;
            }
        }

        /// <summary>
        /// Do units belonging to this category are aircraft?
        /// </summary>
        /// <param name="category">An unit category</param>
        /// <returns>True if unit is an aircraft (helicopter or plane), false otherwise.</returns>
        public static bool IsAircraft(this UnitCategory unitCategory)
        {
            return (unitCategory == UnitCategory.Helicopter) || (unitCategory == UnitCategory.Plane);
        }

        /// <summary>
        /// Is this unit family an air defense unit family (SAM, MANPADS, AAA...)?
        /// </summary>
        /// <param name="unitFamily">An unit family.</param>
        /// <returns>True if the unit family is an air defense family, false otherwise.</returns>
        public static bool IsAirDefense(this UnitFamily unitFamily)
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
        public static bool IsCarrier(this UnitFamily unitFamily)
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
        /// Converts an object to a string with proper formatting for use in Lua files, etc.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stringFormat">String format, if any.</param>
        /// <returns>The value as a string.</returns>
        public static string ValToString(object value, string stringFormat = "")
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

        //TODO Extract this into a larger ToLua Class for all this kinda stuff we may need
        public static string ListToLuaString(IEnumerable<int> list)
        {
            return $"{{{string.Join(",", list.Select((x,i) => $"[{i+1}] = {x}"))}}}";
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
        /// Search for the DCS world custom campaigns path ([User]\Saved Games\DCS\Missions\Campaigns\MultiLang\)
        /// Looks first for DCS.earlyaccess, then DCS.openbeta, then DCS.
        /// If Saved Games\DCS\Missions is found, but not the child directories, tries to create them.
        /// </summary>
        /// <returns>The path, or the user My document folder if none is found.</returns>
        public static string GetDCSCampaignPath()
        {
            string[] possibleDCSPaths = new string[] { "DCS.earlyaccess", "DCS.openbeta", "DCS" };

            for (int i = 0; i < possibleDCSPaths.Length; i++)
            {
                string dcsPath = PATH_USER + "Saved Games\\" + possibleDCSPaths[i] + "\\Missions\\";
                if (!Directory.Exists(dcsPath)) continue;
                if (!CreateDirectoryIfMissing(dcsPath + "Campaigns\\")) continue;
                if (!CreateDirectoryIfMissing(dcsPath + "Campaigns\\MultiLang\\")) continue;

                return dcsPath + "Campaigns\\MultiLang\\";
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
