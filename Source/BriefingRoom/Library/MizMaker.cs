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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace BriefingRoom4DCS
{
    /// <summary>
    /// Exports HQ4DCS missions to DCS World .miz files.
    /// </summary>
    internal class MizMaker : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal MizMaker() { }

        /// <summary>
        /// Exports a BriefingRoom mission to a zip file.
        /// </summary>
        /// <param name="mission">The mission to export.</param>
        /// <returns>The zip file, as a byte array.</returns>
        internal byte[] ExportToMizBytes(DCSMission mission)
        {
            Dictionary<string, byte[]> MizFileEntries = new Dictionary<string, byte[]>();

            AddStringValueToEntries(MizFileEntries, "briefing.html", mission.GetHTMLBriefing(true));
            AddStringValueToEntries(MizFileEntries, "credits.txt", "Generated with BriefingRoom for DCS World (https://akaagar.itch.io/briefing-room-for-dcs)");
            AddLuaFileToEntries(MizFileEntries, "mission", "Mission.lua", mission);
            AddLuaFileToEntries(MizFileEntries, "options", "Options.lua", null);
            AddStringValueToEntries(MizFileEntries, "theatre", mission.GetValue("THEATER_ID"));
            AddLuaFileToEntries(MizFileEntries, "warehouses", "Warehouses.lua", mission);

            AddLuaFileToEntries(MizFileEntries, "l10n/DEFAULT/dictionary", "Dictionary.lua", null);
            AddLuaFileToEntries(MizFileEntries, "l10n/DEFAULT/mapResource", "MapResource.lua", mission);
            AddStringValueToEntries(MizFileEntries, "l10n/DEFAULT/script.lua", " ");

            byte[] mizBytes;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Update))
                    {
                        foreach (string entryKey in MizFileEntries.Keys)
                        {
                            ZipArchiveEntry entry = zip.CreateEntry(entryKey, CompressionLevel.Optimal);
                            using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                                writer.Write(MizFileEntries[entryKey]);
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

        //        public byte[] ZipFiles()
        //        {
        //            using (MemoryStream ms = new MemoryStream())
        //            {
        //                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Update))
        //                {
        //                    foreach (string entryName in Entries.Keys)
        //                    {
        //                        ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
        //                        using (BinaryWriter writer = new BinaryWriter(entry.Open()))
        //                        {
        //                            writer.Write(Entries[entryName]);
        //                        }
        //                    }
        //                }
        //                return ms.ToArray();
        //            }

        //        }


        /*
        string resourceOggString;

        if (mission == null) return null;

        BriefingRoom.PrintToLog($"Started MIZ file export...");

        DateTime exportStartTime = DateTime.Now;
        MizFile miz = new MizFile();

        miz.AddEntry("Credits.txt", $"Generated with BriefingRoom for DCS World ({BriefingRoom.WEBSITE_URL})");

        BriefingRoom.PrintToLog(" Adding \"briefing.html\" entry...");
        miz.AddEntry("briefing.html", mission.BriefingHTML);

        BriefingRoom.PrintToLog(" Adding \"mission\" entry...");
        using (MizMakerLuaMission luaMission = new MizMakerLuaMission())
            miz.AddEntry("mission", luaMission.MakeLua(mission));

        BriefingRoom.PrintToLog(" Adding \"options\" entry...");
        miz.AddEntry("options", LuaTools.ReadIncludeLuaFile("Options.lua"));

        BriefingRoom.PrintToLog(" Adding \"theater\" entry...");
        miz.AddEntry("theatre", Database.Instance.GetEntry<DBEntryTheater>(mission.Theater).DCSID);

        BriefingRoom.PrintToLog(" Adding \"warehouses\" entry...");
        using (MizMakerLuaWarehouse luaWarehouses = new MizMakerLuaWarehouse())
            miz.AddEntry("warehouses", luaWarehouses.MakeLua(mission));

        BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/dictionary\" entry...");
        miz.AddEntry("l10n/DEFAULT/dictionary", LuaTools.ReadIncludeLuaFile("Dictionary.lua"));

        BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/script.lua\" entry...");
        using (MizMakerLuaScript luaScript = new MizMakerLuaScript())
            miz.AddEntry("l10n/DEFAULT/script.lua", luaScript.MakeLua(mission), false);

        BriefingRoom.PrintToLog(" Adding .ogg audio media files...");
        using (MizMakerMediaAudio oggMedia = new MizMakerMediaAudio())
            oggMedia.AddMediaFiles(miz, mission, out resourceOggString);

        BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/mapResource\" entry...");
        using (MizMakerLuaMapResource luaMapResource = new MizMakerLuaMapResource())
            miz.AddEntry("l10n/DEFAULT/mapResource", luaMapResource.MakeLua(mission, resourceOggString));

        BriefingRoom.PrintToLog(" Adding .jpg image media files...");
        using (MizMakerMediaImages jpgMedia = new MizMakerMediaImages())
            jpgMedia.AddMediaFiles(miz, mission);

        BriefingRoom.PrintToLog($"Export to .miz file completed in {(DateTime.Now - exportStartTime).TotalSeconds.ToString("F3", NumberFormatInfo.InvariantInfo)} second(s).");

        return miz;
    }
        */

        private bool AddLuaFileToEntries(Dictionary<string, byte[]> mizFileEntries, string mizEntryKey, string sourceFile, DCSMission mission = null)
        {
            if (string.IsNullOrEmpty(mizEntryKey) || mizFileEntries.ContainsKey(mizEntryKey) || string.IsNullOrEmpty(sourceFile)) return false;
            sourceFile = $"{BRPaths.INCLUDE_LUA}{sourceFile}";
            if (!File.Exists(sourceFile)) return false;

            string luaContent = File.ReadAllText(sourceFile);
            if (mission != null) // A mission was provided, do the required replacements in the file.
                luaContent = mission.ReplaceValues(luaContent);

            mizFileEntries.Add(mizEntryKey, Encoding.UTF8.GetBytes(luaContent));
            return true;
        }

        private bool AddStringValueToEntries(Dictionary<string, byte[]> mizFileEntries, string mizEntryKey, string stringValue)
        {
            if (string.IsNullOrEmpty(mizEntryKey) || mizFileEntries.ContainsKey(mizEntryKey)) return false;
            mizFileEntries.Add(mizEntryKey, Encoding.UTF8.GetBytes(stringValue));
            return true;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}