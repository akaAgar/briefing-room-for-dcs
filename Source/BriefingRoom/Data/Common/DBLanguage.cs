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
using System.IO;

namespace BriefingRoom4DCS.Data
{
    public class DatabaseLanguage
    {
        public Dictionary<string,LanguageString> LangMap { get; private set; }
    
        public DatabaseLanguage() { }

        public void Load()
        {
            LangMap = new Dictionary<string, LanguageString>();
            BriefingRoom.PrintToLog("Loading language global settings...");
            string directory = $"{BRPaths.DATABASE}Language";
            if (!Directory.Exists(directory))
                throw new Exception($"Directory {directory} not found.");
            
            foreach (string filePath in Directory.EnumerateFiles(directory, "*.ini", SearchOption.AllDirectories))
            {
                string id = Path.GetFileNameWithoutExtension(filePath).Replace(",", "").Trim(); // No commas in file names, so we don't break comma-separated arrays

                
                LoadValues(filePath);
                BriefingRoom.PrintToLog($"Loaded {filePath} \"{id}\"");
            }
        }

        private void LoadValues(string filePath)
        {
            var ini = new INIFile(filePath);
            foreach (var section in ini.GetSections())
            {
                foreach (var key in ini.GetKeysInSection(section))
                {
                    var upperKey = key.ToUpper();
                    if(LangMap.ContainsKey(upperKey))
                    {
                        BriefingRoom.PrintToLog($"Duplicate Lang Key {key}", LogMessageErrorLevel.Warning);
                        continue;
                    }
                    LangMap.Add(upperKey, ini.GetLangStrings(section,key));
                }
            }
        }

        public string ReplaceValues(string rawText, string lang)
        {
            if (rawText == null) return null;

            foreach (KeyValuePair<string, LanguageString> keyPair in LangMap)
                rawText = rawText.Replace($"$LANG{keyPair.Key.ToUpperInvariant()}$", keyPair.Value.Get(lang));

            return rawText;
        }


    }
}