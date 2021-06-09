///*
//==========================================================================
//This file is part of Briefing Room for DCS World, a mission
//generator for DCS World, by @akaAgar
//(https://github.com/akaAgar/briefing-room-for-dcs)

//Briefing Room for DCS World is free software: you can redistribute it
//and/or modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation, either version 3 of
//the License, or (at your option) any later version.

//Briefing Room for DCS World is distributed in the hope that it will
//be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Briefing Room for DCS World.
//If not, see https://www.gnu.org/licenses/
//==========================================================================
//*/

//using BriefingRoom4DCS.Data;
//using BriefingRoom4DCS.Mission;
//using System;
//using System.Globalization;

//namespace BriefingRoom4DCS.Miz
//{
//    /// <summary>
//    /// Exports HQ4DCS missions to DCS World .miz files.
//    /// </summary>
//    internal class MizMaker : IDisposable
//    {
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        internal MizMaker()
//        {

//        }

//        /// <summary>
//        /// Exports a BriefingRoom mission to a DCS .miz file.
//        /// </summary>
//        /// <param name="mission">The mission to export</param>
//        /// <returns>A MizFile if everything went well, null if an error happened.</returns>
//        internal MizFile ExportToMizFile(DCSMission mission)
//        {
//            string resourceOggString;
        
//            if (mission == null) return null;

//            BriefingRoom.PrintToLog($"Started MIZ file export...");

//            DateTime exportStartTime = DateTime.Now;
//            MizFile miz = new MizFile();

//            miz.AddEntry("Credits.txt", $"Generated with BriefingRoom for DCS World ({BriefingRoom.WEBSITE_URL})");

//            BriefingRoom.PrintToLog(" Adding \"briefing.html\" entry...");
//            miz.AddEntry("briefing.html", mission.BriefingHTML);

//            BriefingRoom.PrintToLog(" Adding \"mission\" entry...");
//            using (MizMakerLuaMission luaMission = new MizMakerLuaMission())
//                miz.AddEntry("mission", luaMission.MakeLua(mission));

//            BriefingRoom.PrintToLog(" Adding \"options\" entry...");
//            miz.AddEntry("options", LuaTools.ReadIncludeLuaFile("Options.lua"));

//            BriefingRoom.PrintToLog(" Adding \"theater\" entry...");
//            miz.AddEntry("theatre", Database.Instance.GetEntry<DBEntryTheater>(mission.Theater).DCSID);

//            BriefingRoom.PrintToLog(" Adding \"warehouses\" entry...");
//            using (MizMakerLuaWarehouse luaWarehouses = new MizMakerLuaWarehouse())
//                miz.AddEntry("warehouses", luaWarehouses.MakeLua(mission));

//            BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/dictionary\" entry...");
//            miz.AddEntry("l10n/DEFAULT/dictionary", LuaTools.ReadIncludeLuaFile("Dictionary.lua"));

//            BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/script.lua\" entry...");
//            using (MizMakerLuaScript luaScript = new MizMakerLuaScript())
//                miz.AddEntry("l10n/DEFAULT/script.lua", luaScript.MakeLua(mission), false);

//            BriefingRoom.PrintToLog(" Adding .ogg audio media files...");
//            using (MizMakerMediaAudio oggMedia = new MizMakerMediaAudio())
//                oggMedia.AddMediaFiles(miz, mission, out resourceOggString);

//            BriefingRoom.PrintToLog(" Adding \"l10n/DEFAULT/mapResource\" entry...");
//            using (MizMakerLuaMapResource luaMapResource = new MizMakerLuaMapResource())
//                miz.AddEntry("l10n/DEFAULT/mapResource", luaMapResource.MakeLua(mission, resourceOggString));

//            BriefingRoom.PrintToLog(" Adding .jpg image media files...");
//            using (MizMakerMediaImages jpgMedia = new MizMakerMediaImages())
//                jpgMedia.AddMediaFiles(miz, mission);

//            BriefingRoom.PrintToLog($"Export to .miz file completed in {(DateTime.Now - exportStartTime).TotalSeconds.ToString("F3", NumberFormatInfo.InvariantInfo)} second(s).");

//            return miz;
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose() { }
//    }
//}
