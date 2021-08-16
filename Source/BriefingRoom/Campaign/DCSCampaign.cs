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

using BriefingRoom4DCS.Mission;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace BriefingRoom4DCS.Campaign
{
    /// <summary>
    /// Stores a DCS World campaign, with all its unit groups, Lua scripts and parameters.
    /// Can be exported to a directory through use of the <see cref="ExportToMiz"/> method.
    /// </summary>
    public sealed class DCSCampaign : IDisposable
    {
        /// <summary>
        /// Contents of the campaign's CMP file.
        /// </summary>
        internal string CMPFile { get; set; }

        /// <summary>
        /// The media files (pictures, etc) to add to the campaign directory.
        /// </summary>
        private readonly Dictionary<string, byte[]> MediaFiles;

        /// <summary>
        /// The total number of missions in the campaign.
        /// </summary>
        public int MissionCount { get { return Missions.Count; } }

        /// <summary>
        /// The missions in this campaign.
        /// </summary>
        public readonly List<DCSMission> Missions;

        /// <summary>
        /// The name of this campaign.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public DCSCampaign()
        {
            CMPFile = "";
            MediaFiles = new Dictionary<string, byte[]>(StringComparer.InvariantCultureIgnoreCase);
            Missions = new List<DCSMission>();
            Name = "";
        }

        /// <summary>
        /// Exports the campaign to a directory. Preferably <see cref="BriefingRoom.GetDCSCampaignPath()"/>.
        /// </summary>
        /// <param name="exportPath">The path to export to.</param>
        /// <returns>True if the export was completed successfully, false otherwise.</returns>
        public bool ExportToDirectory(string exportPath)
        {
            // Try to create the directory if it doesn't exist.
            if (!Toolbox.CreateMissingDirectory(exportPath))
            {
                BriefingRoom.PrintToLog("Failed to create campaign directory.", LogMessageErrorLevel.Error);
                return false;
            }


            // Write CMP file
            File.WriteAllText($"{exportPath}Campaign.cmp", CMPFile); // TODO: check .cmp file name

            // Write media files
            foreach (string f in MediaFiles.Keys)
                File.WriteAllBytes($"{exportPath}{f}", MediaFiles[f]);

            // Write missions
            for (int i = 0; i < Missions.Count; i++)
                Missions[i].SaveToMizFile($"{exportPath}{Name}{i + 1:00}.miz");

            return false;
        }

        /// <summary>
        /// Exports the campaign as a zip.
        /// </summary>
        /// <returns>ByteArray of data</returns>
        public byte[] ExportToCompressedByteArray()
        {
            Dictionary<string, byte[]> FileEntries = new Dictionary<string, byte[]>();

            FileEntries.Add("Campaign.cmp", Encoding.UTF8.GetBytes(CMPFile));

            foreach (string key in MediaFiles.Keys)
            {
                FileEntries.Add(key, MediaFiles[key]);
            }

            for (int i = 0; i < Missions.Count; i++)
                    FileEntries.Add($"{Name}{i + 1:00}.miz", Missions[i].SaveToMizBytes());

            return Toolbox.ZipData(FileEntries);
        }

        /// <summary>
        /// Exports the campaign as a zip.
        /// </summary>
        /// <returns>ByteArray of data</returns>
        public byte[] ExportBriefingsToCompressedByteArray()
        {
            Dictionary<string, byte[]> FileEntries = new Dictionary<string, byte[]>();

            for (int i = 0; i < Missions.Count; i++)
                    FileEntries.Add($"{Name}{i + 1:00}.html", Encoding.UTF8.GetBytes(Missions[i].Briefing.GetBriefingAsHTML()));
                
            return Toolbox.ZipData(FileEntries);
        }

        /// <summary>
        /// Adds a mission to the campaign.
        /// </summary>
        /// <param name="mission">The mission to add.</param>
        internal void AddMission(DCSMission mission)
        {
            if (mission == null) return;
            Missions.Add(mission);
        }

        /// <summary>
        /// Adds a media file to the campaign.
        /// </summary>
        /// <param name="fileName">Name of the media file.</param>
        /// <param name="mediaFileBytes">Bytes of the file.</param>
        internal void AddMediaFile(string fileName, byte[] mediaFileBytes)
        {
            if (MediaFiles.ContainsKey(fileName))
                MediaFiles[fileName] = mediaFileBytes;
            else
                MediaFiles.Add(fileName, mediaFileBytes);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {

        }
    }
}

