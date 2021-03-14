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

using System.Collections.Generic;

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// Stores a string and its translation in a variety of languages.
    /// </summary>
    public class LocalizedString
    {
        /// <summary>
        /// A dictionary storing the translations.
        /// </summary>
        private readonly Dictionary<string, string> Strings;

        /// <summary>
        /// Creates a (non-localized) string from a unique, default string.
        /// </summary>
        /// <param name="defaultString">The string to use in all languages.</param>
        public LocalizedString(string defaultString)
        {
            Strings = new Dictionary<string, string> { { Toolbox.DEFAULT_LANGUAGE, defaultString } };
        }

        /// <summary>
        /// Creates a localized string from a .INI file.
        /// </summary>
        /// <param name="ini">The .ini file to load from.</param>
        /// <param name="section">The section in which the string is stored.</param>
        /// <param name="key">The key to load from. Languages others than <see cref="Toolbox.DEFAULT_LANGUAGE"/> are loaded from keys with the name of the language appended (e.g. "mystring.lituanian")</param>
        public LocalizedString(INIFile ini, string section, string key)
        {
            Strings = new Dictionary<string, string> { { Toolbox.DEFAULT_LANGUAGE, ini.GetValue<string>(section, key) } };

            foreach (string k in ini.GetKeysInSection(section))
            {
                if (!k.StartsWith(key + ".")) continue;
                string language = k.Substring(k.Length + 1).ToLowerInvariant();
                if (Strings.ContainsKey(language)) continue;
                Strings.Add(language, ini.GetValue<string>(section, k));
            }
        }

        /// <summary>
        /// Returns the translation of the string in the desired language, or the string in <see cref="Toolbox.DEFAULT_LANGUAGE"/> if no translation was found.
        /// </summary>
        /// <param name="language">The language in which the string should be translated</param>
        /// <returns>The string in the desired language, or the string in <see cref="Toolbox.DEFAULT_LANGUAGE"/> if no translation was found.</returns>
        public string Get(string language = Toolbox.DEFAULT_LANGUAGE)
        {
            language = language.ToLowerInvariant();
            if (Strings.ContainsKey(language)) return Strings[language];
            return Strings[Toolbox.DEFAULT_LANGUAGE]; // No translation was found, return the string in the default language.
        }

        /// <summary>
        /// ToString() override. Returns the string translated in <see cref="Toolbox.DEFAULT_LANGUAGE"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Get(); }
    }
}
