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

using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Mission;
using System;
using System.Globalization;

namespace BriefingRoom4DCSWorld.Miz
{
    /// <summary>
    /// Exports HQ4DCS missions to DCS World .miz files.
    /// </summary>
    public class MizMaker : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MizMaker() { }

        /// <summary>
        /// Exports a BriefingRoom mission to a DCS .miz file.
        /// </summary>
        /// <param name="mission">The mission to export</param>
        /// <returns>A MizFile if everything went well, null if an error happened.</returns>
        public MizFile ExportToMizFile(DCSMission mission)
        {
            string resourceOggString;
        
            if (mission == null) return null;

            DebugLog.Instance.Clear();
            DebugLog.Instance.WriteLine($"Started MIZ file export...");

            DateTime exportStartTime = DateTime.Now;
            MizFile miz = new MizFile();

            miz.AddEntry("Credits.txt", "Generated with BriefingRoom for DCS World (https://github.com/akaAgar/briefing-room-for-dcs]");

            DebugLog.Instance.WriteLine(" Adding \"briefing.html\" entry...", 1);
            miz.AddEntry("briefing.html", mission.BriefingHTML);

            DebugLog.Instance.WriteLine(" Adding \"mission\" entry...", 1);
            using (MizMakerLuaMission luaMission = new MizMakerLuaMission())
                miz.AddEntry("mission", luaMission.MakeLua(mission));

            DebugLog.Instance.WriteLine(" Adding \"options\" entry...", 1);
            miz.AddEntry("options", LuaTools.ReadIncludeLuaFile("Options.lua"));

            DebugLog.Instance.WriteLine(" Adding \"theater\" entry...", 1);
            miz.AddEntry("theatre", mission.Theater);

            DebugLog.Instance.WriteLine(" Adding \"warehouses\" entry...", 1);
            using (MizMakerLuaWarehouse luaWarehouses = new MizMakerLuaWarehouse())
                miz.AddEntry("warehouses", luaWarehouses.MakeLua(mission));

            DebugLog.Instance.WriteLine(" Adding \"l10n/DEFAULT/dictionary\" entry...", 1);
            miz.AddEntry("l10n/DEFAULT/dictionary", LuaTools.ReadIncludeLuaFile("Dictionary.lua"));

            DebugLog.Instance.WriteLine(" Adding \"l10n/DEFAULT/script.lua\" entry...", 1);
            using (MizMakerLuaScript luaScript = new MizMakerLuaScript())
                miz.AddEntry("l10n/DEFAULT/script.lua", luaScript.MakeLua(mission), false);

            DebugLog.Instance.WriteLine(" Adding .ogg audio media files...", 1);
            using (MizMakerMediaAudio oggMedia = new MizMakerMediaAudio())
                oggMedia.AddMediaFiles(miz, mission, out resourceOggString);

            DebugLog.Instance.WriteLine(" Adding \"l10n/DEFAULT/mapResource\" entry...", 1);
            using (MizMakerLuaMapResource luaMapResource = new MizMakerLuaMapResource())
                miz.AddEntry("l10n/DEFAULT/mapResource", luaMapResource.MakeLua(resourceOggString));

            DebugLog.Instance.WriteLine(" Adding .jpg image media files...", 1);
            using (MizMakerMediaImages jpgMedia = new MizMakerMediaImages())
                jpgMedia.AddMediaFiles(miz, mission);

            DebugLog.Instance.WriteLine("MIZ file created!");
            DebugLog.Instance.WriteLine($"Completed miz file export in {(DateTime.Now - exportStartTime).TotalSeconds.ToString(NumberFormatInfo.InvariantInfo)} seconds.");

            return miz;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose() { }
    }
}
