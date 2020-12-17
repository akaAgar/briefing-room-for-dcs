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
using System.IO;

namespace BriefingRoom4DCSWorld.Miz
{
    /// <summary>
    /// Handles the inclusion of all .ogg files into the .miz file.
    /// </summary>
    public class MizMakerMediaAudio : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MizMakerMediaAudio() { }

        public void AddMediaFiles(MizFile miz, DCSMission mission, out string resourceOggString)
        {
            resourceOggString = "";

            foreach (string ogg in mission.OggFiles)
                AddOggFile(miz, ogg, ref resourceOggString);
        }

        /// <summary>
        /// Adds an .ogg file (read from the Include\Ogg\ directory) to the .miz file.
        /// </summary>
        /// <param name="miz">The .miz file</param>
        /// <param name="file">Name of the ogg file, without .ogg extension.</param>
        /// <param name="resourceOggString"></param>
        private void AddOggFile(MizFile miz, string file, ref string resourceOggString)
        {
            string oggFilePath = $"{BRPaths.INCLUDE_OGG}{file}.ogg";
            if (!File.Exists(oggFilePath))
            {
                DebugLog.Instance.WriteLine($"  WARNING: Failed to load .ogg file Include\\Ogg\\{file}.ogg");
                return; // File doesn't exist
            }

            DebugLog.Instance.WriteLine($"  Added file Include\\Ogg\\{file}.ogg");
            miz.AddEntry($"l10n/DEFAULT/{file}.ogg", File.ReadAllBytes(oggFilePath));

            resourceOggString += $" [\"ResKey_Snd_{Path.GetFileNameWithoutExtension(file)}\"] = \"{file}.ogg\",\r\n";
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation
        /// </summary>
        public void Dispose() { }
    }
}
