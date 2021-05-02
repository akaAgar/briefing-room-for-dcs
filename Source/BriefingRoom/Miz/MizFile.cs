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

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Text;

//namespace BriefingRoom4DCS.Miz
//{
//    /// <summary>
//    /// A DCS World .miz file.
//    /// </summary>
//    public class MizFile : IDisposable
//    {
//        /// <summary>
//        /// Dictionary of files to save into .miz file.
//        /// </summary>
//        private readonly Dictionary<string, byte[]> Entries;

//        /// <summary>
//        /// Returns all entries.
//        /// </summary>
//        public string[] EntryNames { get { return Entries.Keys.ToArray(); } }

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public MizFile()
//        {
//            Entries = new Dictionary<string, byte[]>();
//        }

//        /// <summary>
//        /// Adds a text entry to the .miz file.
//        /// </summary>
//        /// <param name="entryPath">Path to the entry in the .miz file</param>
//        /// <param name="text">The (UTF-8) text of the entry</param>
//        /// <param name="removeEmptyLines">Should empty lines be removed?</param>
//        /// <returns>True if everything went well, false if an error happened</returns>
//        public bool AddEntry(string entryPath, string text, bool removeEmptyLines = true)
//        {
//            if (string.IsNullOrEmpty(text)) text = "";
            
//            text = text.Replace("\r\n", "\n"); // Convert CRLF end of lines to LF, CRLF can cause problems
//            if (removeEmptyLines)
//                while (text.Contains("\n\n")) { text = text.Replace("\n\n", "\n"); }

//            return AddEntry(entryPath, Encoding.UTF8.GetBytes(text));
//        }

//        /// <summary>
//        /// Adds a binary entry to the .miz file.
//        /// </summary>
//        /// <param name="entryPath">Path to the entry in the .miz file</param>
//        /// <param name="bytes">Bytes of the entry</param>
//        /// <returns>True if everything went well, false if an error happened</returns>
//        public bool AddEntry(string entryPath, byte[] bytes)
//        {
//            BriefingRoom.PrintToLog($"Adding file {entryPath} to the .miz file");

//            entryPath = NormalizeEntryPath(entryPath);

//            if (Entries.ContainsKey(entryPath)) return false;
//            Entries.Add(entryPath, bytes);
//            return true;
//        }

//        /// <summary>
//        /// Remove an entry from the miz file
//        /// </summary>
//        /// <param name="entryPath">Path to the entry in the .miz file</param>
//        /// <returns>True if entry existed and was removed successfully, false otherwise</returns>
//        public bool RemoveEntry(string entryPath)
//        {
//            entryPath = NormalizeEntryPath(entryPath);
//            if (Entries.ContainsKey(entryPath)) return false;
//            Entries.Remove(entryPath);
//            return true;
//        }

//#if DEBUG
//        /// <summary>
//        /// Saves all entries in the .miz file to a directory. Monstly for debugging purposes.
//        /// </summary>
//        /// <param name="directoryPath">Path to an EXISTING directory</param>
//        /// <returns>True if everything went well, false if an error happened</returns>
//        public bool SaveToDirectory(string directoryPath)
//        {
//            if (!Directory.Exists(directoryPath)) return false;

//            foreach (string entryName in Entries.Keys)
//            {
//                string fileName = entryName.Replace('/', '_');
//                if (string.IsNullOrEmpty(Path.GetExtension(fileName))) fileName += ".lua";
//                //if (Path.GetExtension(fileName).ToLowerInvariant() != ".lua") continue; // No need to export media files

//                File.WriteAllBytes(Path.Combine(directoryPath, fileName), Entries[entryName]);
//            }

//            return true;
//        }
//#endif

//        /// <summary>
//        /// Saves the .miz file to the hard drive.
//        /// </summary>
//        /// <param name="filePath">The path to the .miz file to save to.</param>
//        /// <returns>True if everything went well, false if an error happened</returns>
//        public bool SaveToFile(string filePath)
//        {
//            try
//            {
//                if (File.Exists(filePath))
//                    File.Delete(filePath);

//                using (ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Create))
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
//            }
//            catch (Exception ex)
//            {
//                BriefingRoom.PrintToLog(ex.Message, LogMessageErrorLevel.Error);
//                return false;
//            }

//            return true;
//        }

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

//        /// <summary>
//        /// Normalize an entry path to the proper DCS World miz format.
//        /// </summary>
//        /// <param name="entryPath">Path to the entry in the .miz file</param>
//        /// <returns>Normalized path</returns>
//        private string NormalizeEntryPath(string entryPath)
//        {
//            return entryPath.Replace('\\', '/');
//        }

//        /// <summary>
//        /// <see cref="IDisposable"/> implementation.
//        /// </summary>
//        public void Dispose()
//        {
//            Entries.Clear();
//        }
//    }
//}
