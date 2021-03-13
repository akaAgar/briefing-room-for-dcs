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

using BriefingRoom4DCSWorld.Debug;
using System;
using System.Linq;
using System.IO;

namespace BriefingRoom4DCSWorld.DB
{
    /// <summary>
    /// Provides access to common localized strings.
    /// </summary>
    public class DatabaseStrings : IDisposable
    {
        /// <summary>
        /// Default language to use.
        /// </summary>
        private const string DEFAULT_LANGUAGE = "English";

        /// <summary>
        /// INI file where language strings are stored.
        /// </summary>
        private readonly INIFile StringsINI;

        /// <summary>
        /// List of available languages (sections in the <see cref="StringsINI"/> .ini file)
        /// </summary>
        private string[] AvailableLanguages { get { return StringsINI.GetSections(); } }

        /// <summary>
        /// Currently selected langauge;
        /// </summary>
        public string SelectedLanguage { get { return SelectedLanguage_; } set { if (AvailableLanguages.Contains(value)) SelectedLanguage_ = value; } }
        private string SelectedLanguage_ = DEFAULT_LANGUAGE;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseStrings()
        {
            StringsINI = new INIFile($"{BRPaths.DATABASE}Strings.ini");
        }

        /// <summary>
        /// Return the localized string with this key.
        /// </summary>
        /// <param name="key">Key to the localized string.</param>
        /// <returns>The localized string, or the string in the default language if the string wasn't found, or the key if the string wasn't found in the default language either.</returns>
        public string GetString(string key)
        {
            string locStr = StringsINI.GetValue<string>(SelectedLanguage, key);
            if (string.IsNullOrEmpty(locStr)) locStr = StringsINI.GetValue<string>(DEFAULT_LANGUAGE, key);
            if (string.IsNullOrEmpty(locStr)) locStr = key;
            return locStr;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            StringsINI.Dispose();
        }
    }
}