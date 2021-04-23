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
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BriefingRoom
{
    /// <summary>
    /// Provides reading and writing method from/to a .ini file.
    /// </summary>
    public class INIFile : IDisposable
    {
        /// <summary>
        /// Dictionary of all sections in this INI file.
        /// </summary>
        private readonly Dictionary<string, INIFileSection> Sections = new Dictionary<string, INIFileSection>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Constructor. Creates an empty file with no sections or values.
        /// </summary>
        public INIFile() { Clear(); }

        /// <summary>
        /// Constructor. Load data from an INI file.
        /// </summary>
        /// <param name="filePath">Path to the INI file</param>
        /// <param name="encoding">Text encoding to use. Default is UTF-8</param>
        public INIFile(string filePath, Encoding encoding = null)
        {
            Clear();

            if (!File.Exists(filePath)) return;
            string iniString = File.ReadAllText(filePath, encoding ?? Encoding.UTF8);
            ParseINIString(iniString);
        }

        //public string GetSectionParentSection(string section)
        //{
        //    if (!Sections.ContainsKey(section)) return "";

        //    return Sections[section].ParentSection ?? "";
        //}

        /// <summary>
        /// Creates an instance of INIFile from a raw INI string
        /// </summary>
        /// <param name="iniString">String containing INI data</param>
        /// <returns>An INIFile</returns>
        public static INIFile CreateFromRawINIString(string iniString)
        {
            INIFile ini = new INIFile();
            ini.Clear();
            ini.ParseINIString(iniString);
            return ini;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            Clear();
        }

        /// <summary>
        /// Clears all sections and values.
        /// </summary>
        public void Clear()
        {
            foreach (string s in Sections.Keys)
                Sections[s].Clear();

            Sections.Clear();
        }

        /// <summary>
        /// Saves the current state of the INIFile to a file.
        /// </summary>
        /// <param name="filePath">Path to the file to write</param>
        /// <param name="encoding">Text encoding to use. Default is UTF-8</param>
        /// <returns>True if everything went right, false if an error happened</returns>
        public bool SaveToFile(string filePath, Encoding encoding = null)
        {
            try
            {
                string fileContent = "";

                foreach (string s in Sections.Keys)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    fileContent += $"[{s}]\r\n";

                    foreach (string v in Sections[s].Keys)
                    {
                        if (string.IsNullOrEmpty(v)) continue;
                        if (string.IsNullOrEmpty(Sections[s][v])) continue;

                        fileContent += $"{v}={Sections[s][v]}\r\n";
                    }

                    fileContent += "\r\n";
                }

                File.WriteAllText(filePath, fileContent, encoding ?? Encoding.UTF8);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Returns the names of all sections.
        /// </summary>
        /// <param name="showAbstractSections">If true, abstract sections will be displayed (default is false)</param>
        /// <returns>A string array</returns>
        public string[] GetSections(bool showAbstractSections = false)
        {
            if (showAbstractSections)
                return Sections.Keys.OrderBy(x => x).ToArray();

            return (from string s in Sections.Keys where !Sections[s].Abstract select s).OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Reads and returns a value from the ini file.
        /// </summary>
        /// <typeparam name="T">Type of the value to read</typeparam>
        /// <param name="section">Section in which to read the value</param>
        /// <param name="key">Key of the value</param>
        /// <param name="defaultValue">Default value to return if the values doesn't exist or is invalid</param>
        /// <returns>A value</returns>
        public T GetValue<T>(string section, string key, T defaultValue = default)
        {
            if (!ValueExists(section, key))
            {
                if ((defaultValue == null) && (typeof(T) == typeof(string))) return (T)Convert.ChangeType("", typeof(T));
                return defaultValue;
            }

            string val = ReadValue(section, key) ?? "";

            try
            {
                if (CanConvertStringTo<T>())
                    return ConvertStringTo(val, defaultValue);
                else
                    return default;
            }
            catch (Exception)
            {
                return default;
            }
        }

        /// <summary>
        /// Reads and returns an array of value from the ini file.
        /// </summary>
        /// <typeparam name="T">Type of the value to read</typeparam>
        /// <param name="section">Section in which to read the value</param>
        /// <param name="key">Key of the value</param>
        /// <param name="separator">Separator character to use between values in the array</param>
        /// <returns>An array of values</returns>
        public T[] GetValueArray<T>(string section, string key, char separator = ',')
        {
            if (string.IsNullOrEmpty(GetValue<string>(section, key))) return new T[0];

            object val = ReadValue(section, key) ?? "";

            try
            {
                if (typeof(T) == typeof(string)) val = val.ToString().Split(separator);
                else if (typeof(T) == typeof(bool)) val = ConvertArray<bool>(val.ToString().Split(separator));
                else if (typeof(T) == typeof(int)) val = ConvertArray<int>(val.ToString().Split(separator));
                else if (typeof(T) == typeof(float)) val = ConvertArray<float>(val.ToString().Split(separator));
                else if (typeof(T) == typeof(double)) val = ConvertArray<double>(val.ToString().Split(separator));
                else if (typeof(T).IsEnum) val = ConvertArray<T>(val.ToString().Split(separator));
                //else if (CanConvertStringFrom<T>()) val = ConvertArray<T>(val.ToString().Split(separator));

                return (T[])Convert.ChangeType(val, typeof(T[]));
            }
            catch (Exception)
            {
                return default;
            }
        }

        public T GetValueArrayAsEnumFlags<T>(string section, string key, char separator = ',') where T : Enum
        {
            T[] enumArray = GetValueArray<T>(section, key, separator).Distinct().ToArray();

            int enumVal = 0;
            foreach (T e in enumArray)
                enumVal |= Convert.ToInt32(e);

            return (T)(object)enumVal;
        }

        public T[] GetValueArrayAsMinMaxEnum<T>(string section, string key) where T : Enum
        {
            T[] minMaxEnum = GetValueArray<T>(section, key);
            if (minMaxEnum.Length < 2) return new T[] { Toolbox.GetEnumValues<T>().First(), Toolbox.GetEnumValues<T>().Last() };
            return new T[] { (T)(object)Math.Min((int)(object)minMaxEnum[0], (int)(object)minMaxEnum[1]), (T)(object)Math.Max((int)(object)minMaxEnum[0], (int)(object)minMaxEnum[1]) };
        }

        /// <summary>
        /// Sets a value in the INI file.
        /// </summary>
        /// <typeparam name="T">Type of the value to set</typeparam>
        /// <param name="section">Section in which to set the value</param>
        /// <param name="key">Key of the value to set</param>
        /// <param name="value">Value</param>
        public void SetValue<T>(string section, string key, T value)
        {
            if (CanConvertStringFrom<T>())
                WriteValue(section, key, ConvertStringFrom(value));
            else
                WriteValue(section, key, "");
        }

        /// <summary>
        /// Sets an value array in the INI file.
        /// </summary>
        /// <typeparam name="T">Type of the value array to set</typeparam>
        /// <param name="section">Section in which to set the value array</param>
        /// <param name="key">Key of the value array to set</param>
        /// <param name="value">Value array</param>
        /// <param name="separator">Separator character to use between values in the array</param>
        public void SetValueArray<T>(string section, string key, T[] value, char separator = ',')
        {
            value = value ?? new T[0];
            object oVal = value;

            if (typeof(T) == typeof(string))
                WriteValue(section, key, string.Join(separator.ToString(), (string[])oVal));
            else if (typeof(T).IsEnum)
                WriteValue(section, key, string.Join(separator.ToString(), from T e in (T[])oVal select e.ToString()));
            else
                WriteValue(section, key, value.ToString());
        }

        /// <summary>
        /// Gets or sets a value.
        /// </summary>
        /// <param name="section">Section in which to set the value</param>
        /// <param name="key">Key of the value to set</param>
        /// <returns>The value</returns>
        public string this[string section, string key]
        {
            get
            {
                return ReadValue(section, key);
            }
            set
            {
                WriteValue(section, key, value);
            }
        }

        /// <summary>
        /// Returns all keys in a section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string[] GetKeysInSection(string section)
        {
            if (!Sections.ContainsKey(section)) return new string[0];

            List<string> keys = new List<string>();
            keys.AddRange(Sections[section].Keys);

            List<string> checkedSections = new List<string>();

            while (
                !string.IsNullOrEmpty(Sections[section].ParentSection) &&
                !checkedSections.Contains(section.ToLowerInvariant()) &&
                Sections.ContainsKey(Sections[section].ParentSection))
            {
                checkedSections.Add(section.ToLowerInvariant()); // To avoid circular inheritances
                section = Sections[section].ParentSection;
                keys.AddRange(Sections[section].Keys);
            }

            return keys.Distinct().OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Returns all top-level keys in a section.
        /// Top-level keys are keys before the first dot in the name. For instance, if keys are "client1.name", "client1.adress", "client2.name" and "client2.adress", top-level keys are "client1" and "client2"
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string[] GetTopLevelKeysInSection(string section)
        {
            string[] keys = GetKeysInSection(section);

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Contains("."))
                    keys[i] = keys[i].Substring(0, keys[i].IndexOf("."));
            }

            return keys.Distinct().ToArray();
        }

        /// <summary>
        /// Does the value exist?
        /// </summary>
        /// <param name="section">Section in which the value is stored</param>
        /// <param name="key">Value key</param>
        /// <returns>True if the value exists, false if it doesn't</returns>
        public bool ValueExists(string section, string key)
        {
            return ReadValue(section, key) != null;
        }

        /// <summary>
        /// Returns a raw string value or null if it doesn't exist.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string ReadValue(string section, string key)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key)) return null;

            List<string> checkedSections = new List<string>();

            while (Sections.ContainsKey(section))
            {
                if (Sections[section].ContainsKey(key))
                    return Sections[section][key];

                if (string.IsNullOrEmpty(Sections[section].ParentSection))
                    return null;

                checkedSections.Add(section.ToLowerInvariant()); // To avoid circular inheritances
                if (checkedSections.Contains(Sections[section].ParentSection.ToLowerInvariant()))
                    return null;

                section = Sections[section].ParentSection;
            }

            return null;
        }

        /// <summary>
        /// Write a value into the INI file.
        /// </summary>
        /// <param name="section">Section to write to</param>
        /// <param name="key">Key to write</param>
        /// <param name="value">Value</param>
        /// <returns>True if written successfully, false otherwise</returns>
        private bool WriteValue(string section, string key, string value)
        {
            section = (section ?? "").ToLowerInvariant().Trim();
            key = (key ?? "").ToLowerInvariant().Trim();
            value = value ?? "";
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key)) return false;

            if (!Sections.ContainsKey(section))
                Sections.Add(section, new INIFileSection());

            Sections[section][key] = value;

            return true;
        }

        /// <summary>
        /// Parses a INI string to populate sections, keys and values.
        /// </summary>
        /// <param name="iniString">A string containing INI data</param>
        private void ParseINIString(string iniString)
        {
            string[] lines = (iniString + "\n").Replace("\r\n", "\n").Split('\n');

            Clear();

            string section = null;

            foreach (string li in lines)
            {
                string l = li.Trim(' ', '\t'); // Trim line
                if (l.StartsWith(";")) continue; // Line is a comment

                if (l.StartsWith("[")) // found a new section
                {
                    // try to get section name, make sure it's valid
                    section = l.Trim('[', ']', ' ', '\t', ':').ToLowerInvariant();
                    string parentSection = null;

                    if (section.Contains(':')) // Sections inherits another section, name declared in the [SECTION:PARENT_SECTION] format
                    {
                        try
                        {
                            string sectionWithParent = section;
                            section = sectionWithParent.Split(':')[0].Trim();
                            parentSection = sectionWithParent.Split(':')[1].Trim();
                        }
                        catch (Exception)
                        {
                            section = l.Trim('[', ']', ' ', '\t', ':').ToLowerInvariant();
                            parentSection = null;
                        }
                    }

                    bool abstractSection = section.StartsWith("_");

                    if (string.IsNullOrEmpty(section)) { section = null; continue; }

                    if (!Sections.ContainsKey(section))
                        Sections.Add(section, new INIFileSection(abstractSection, parentSection));

                    continue;
                }

                if (l.Contains('=')) // The line contains an "equals" sign, it means we found a value
                {
                    if (section == null) continue; // we're not in a section, ignore

                    string[] v = l.Split(new char[] { '=' }, 2); // Split the line at the first "equal" sign: key = value
                    if (v.Length < 2) continue;

                    string key = v[0].Trim().Trim('"').ToLowerInvariant();
                    string value = v[1].Trim().Trim('"');
                    WriteValue(section, key, value);
                }
            }
        }

        /// <summary>
        /// Can a string be converted to a value type for reading from the INI file?
        /// Can be overridden to add more supported types.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <returns>True or false</returns>
        private bool CanConvertStringTo<T>()
        {
            Type type = typeof(T);

            if (
                (type == typeof(bool)) ||
                (type == typeof(double)) ||
                (type == typeof(float)) ||
                (type == typeof(int)) ||
                (type == typeof(string)) ||
                (type == typeof(Coordinates)) ||
                (type == typeof(MinMaxD)) ||
                (type == typeof(MinMaxI)) ||
                type.IsEnum)
                return true;

            return false;
        }

        /// <summary>
        /// Can a value type be converted to a string for writing into the INI file?
        /// Can be overridden to add more supported types.
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <returns>True or false</returns>
        private bool CanConvertStringFrom<T>()
        {
            Type type = typeof(T);

            if (
                (type == typeof(bool)) ||
                (type == typeof(double)) ||
                (type == typeof(float)) ||
                (type == typeof(int)) ||
                (type == typeof(string)) ||
                type.IsEnum)
                return true;

            return false;
        }

        /// <summary>
        /// Converts a string to a value type for reading from the INI file.
        /// Can be overridden to add more supported types.
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="value">Value</param>
        /// <param name="defaultValue">Default value to return if something goes wrong</param>
        /// <returns>A value of type T</returns>
        private T ConvertStringTo<T>(string value, T defaultValue = default)
        {
            Type type = typeof(T);

            object outObject = defaultValue;

            if (type == typeof(bool))
                try { outObject = Convert.ToBoolean(value, NumberFormatInfo.InvariantInfo); } catch (Exception) { }
            else if (type == typeof(double))
                try { outObject = Convert.ToDouble(value, NumberFormatInfo.InvariantInfo); } catch (Exception) { }
            else if (type == typeof(float))
                try { outObject = Convert.ToSingle(value, NumberFormatInfo.InvariantInfo); } catch (Exception) { }
            else if (type == typeof(int))
                try { outObject = Convert.ToInt32(value, NumberFormatInfo.InvariantInfo); } catch (Exception) { }
            else if (type == typeof(string))
                outObject = value;
            else if (type.IsEnum)
                try { outObject = Enum.Parse(typeof(T), value, true); } catch (Exception) { }
            else if (type == typeof(Coordinates))
            {
                try
                {
                    //string[] minMaxStr = value.Split(',');
                    //object obj = new Coordinates(Convert.ToDouble(minMaxStr[0].Trim(), NumberFormatInfo.InvariantInfo), Convert.ToDouble(minMaxStr[1].Trim(), NumberFormatInfo.InvariantInfo));
                    object obj = new Coordinates(value);
                    return (T)obj;
                }
                catch (Exception) { return default; }
            }
            else if (type == typeof(MinMaxD))
            {
                try
                {
                    string[] minMaxStr = value.Split(',');
                    object obj = new MinMaxD(Convert.ToDouble(minMaxStr[0].Trim(), NumberFormatInfo.InvariantInfo), Convert.ToDouble(minMaxStr[1].Trim(), NumberFormatInfo.InvariantInfo));
                    return (T)obj;
                }
                catch (Exception) { return default; }
            }
            else if (type == typeof(MinMaxI))
            {
                try
                {
                    string[] minMaxStr = value.Split(',');
                    object obj = new MinMaxI(Convert.ToInt32(minMaxStr[0].Trim(), NumberFormatInfo.InvariantInfo), Convert.ToInt32(minMaxStr[1].Trim(), NumberFormatInfo.InvariantInfo));
                    return (T)obj;
                }
                catch (Exception) { return default; }
            }

            return (T)outObject;
        }

        /// <summary>
        /// Converts a string from a value type for writing into the INI file.
        /// Can be overridden to add more supported types.
        /// </summary>
        /// <typeparam name="T">Type to convert from</typeparam>
        /// <param name="value">Value</param>
        /// <returns>A string</returns>
        private string ConvertStringFrom<T>(T value)
        {
            Type type = typeof(T);

            object inObject = value;
            string outString = value.ToString();

            if (type == typeof(bool))
                outString = ((bool)inObject).ToString(NumberFormatInfo.InvariantInfo);
            else if (type == typeof(double))
                outString = ((double)inObject).ToString(NumberFormatInfo.InvariantInfo);
            else if (type == typeof(float))
                outString = ((float)inObject).ToString(NumberFormatInfo.InvariantInfo);
            else if (type == typeof(int))
                outString = ((int)inObject).ToString(NumberFormatInfo.InvariantInfo);

            return outString;
        }

        /// <summary>
        /// Calls ConvertStringTo<T> on all elements in a array to convert a string[] array to a T[] array.
        /// </summary>
        /// <typeparam name="T">Type of array to convert to</typeparam>
        /// <param name="sourceArray">Source string array</param>
        /// <returns>An array of type T</returns>
        private T[] ConvertArray<T>(string[] sourceArray)
        {
            try
            {
                T[] arr = new T[sourceArray.Length];

                for (int i = 0; i < sourceArray.Length; i++)
                {
                    if (CanConvertStringTo<T>())
                        arr[i] = ConvertStringTo<T>(sourceArray[i]);
                    else
                        arr[i] = default;
                }

                return arr;
            }
            catch (Exception)
            {
                return new T[0];
            }
        }
    }
}
