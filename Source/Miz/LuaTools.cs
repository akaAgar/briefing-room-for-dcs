/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;
using System.IO;

namespace BriefingRoom4DCSWorld.Miz
{
    /// <summary>
    /// Static class provided various tool methods to handle included Lua files.
    /// </summary>
    public static class LuaTools
    {
        /// <summary>
        /// Reads the content of an embedded Lua file in BriefingRoom4DCSWorld.Resources.Lua namespace.
        /// </summary>
        /// <param name="filePath">Path to resource file from BriefingRoom4DCSWorld.Resources.Lua</param>
        /// <returns>The content of the Lua file, or an empty string if the resource was not found.</returns>
        public static string ReadIncludeLuaFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return "";
            if (!filePath.ToLowerInvariant().EndsWith(".lua")) filePath += ".lua";
            filePath = $"{BRPaths.INCLUDE_LUA}{filePath}";

            if (!File.Exists(filePath)) return "";
            return File.ReadAllText(filePath) + "\n";
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        public static void ReplaceKey(ref string lua, string key, bool value) { ReplaceKey(ref lua, key, Toolbox.ValToString(value).ToLowerInvariant()); }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        /// <param name="stringFormat">The string format string to use when converting the value to a string.</param>
        public static void ReplaceKey(ref string lua, string key, int value, string stringFormat = null)
        {
            ReplaceKey(ref lua, key, Toolbox.ValToString(value, stringFormat));
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        /// <param name="stringFormat">The string format string to use when converting the value to a string.</param>
        public static void ReplaceKey(ref string lua, string key, float value, string stringFormat = null)
        {
            ReplaceKey(ref lua, key, Toolbox.ValToString(value, stringFormat));
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        /// <param name="stringFormat">The string format string to use when converting the value to a string.</param>
        public static void ReplaceKey(ref string lua, string key, double value, string stringFormat = null)
        {
            ReplaceKey(ref lua, key, Toolbox.ValToString(value, stringFormat));
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        /// <param name="allowNewLines">Should line breaks (\n) be allowed?</param>
        public static void ReplaceKey(ref string lua, string key, string value, bool allowNewLines = false)
        {
            value = value ?? "";
            if (allowNewLines) value = value.Replace("\n", "\\\n");
            else value = value.Replace("\n", " ");

            lua = lua.Replace($"${key.ToUpperInvariant()}$", value);
        }

        /// <summary>
        /// Replaces all instance of "$KEY$" in a Lua script by value.
        /// </summary>
        /// <param name="lua">The Lua script.</param>
        /// <param name="key">The key to replace, without the dollar signs.</param>
        /// <param name="value">The value to replace the key with.</param>
        /// <param name="bool">Should the enum value be cast to upper case?</param>
        public static void ReplaceKey<T>(ref string lua, string key, T value, bool upperCase) where T: Enum
        {
            ReplaceKey(ref lua, key, upperCase ? value.ToString().ToUpperInvariant() : value.ToString());
        }
    }
}
