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
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BriefingRoom4DCS.CommandLineTool
{
    /// <summary>
    /// Tool to generate mission(s) from command-line parameters.
    /// </summary>
    public class CommandLine : IDisposable
    {
        /// <summary>
        /// Path to the log file where BriefingRoom output must be written.
        /// </summary>
        private static readonly string LOG_FILE = $"{Application.StartupPath}\\BriefingRoomCommandLineDebugLog.txt";

        /// <summary>
        /// StreamWriter used to write the log to the disk.
        /// </summary>
        private readonly StreamWriter LogWriter;

        /// <summary>
        /// BriefingRoom library instance.
        /// </summary>
        private readonly BriefingRoom BriefingRoomGenerator;

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command-line parameters.</param>
        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0) args = new string[] { "Default.brt" };
#endif

            try
            {
                using (CommandLine cl = new CommandLine())
                    cl.DoCommandLine(args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"CRITICAL ERROR: {e.Message}");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandLine()
        {
            if (File.Exists(LOG_FILE)) File.Delete(LOG_FILE);
            LogWriter = File.AppendText(LOG_FILE);
            LogWriter.Flush();
            BriefingRoomGenerator = new BriefingRoom(WriteToDebugLog);
        }

        /// <summary>
        /// Debug log writer method.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="errorLevel">Error level (info, warning, error).</param>
        private void WriteToDebugLog(string message, LogMessageErrorLevel errorLevel = LogMessageErrorLevel.Info)
        {
            switch (errorLevel)
            {
                case LogMessageErrorLevel.Error: message = $"ERROR: {message}"; break;
                case LogMessageErrorLevel.Warning: message = $"WARNING: {message}"; break;
            }

            LogWriter.WriteLine(message);
            Console.WriteLine(message);
        }

        /// <summary>
        /// Generates mission(s) from command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>True if everything when right, false otherwise.</returns>
        public bool DoCommandLine(string[] args)
        {
            string[] templateFiles = (from string arg in args where File.Exists(arg) select arg).ToArray();
            string[] invalidTemplateFiles = (from string arg in args where !File.Exists(arg) select arg).ToArray();

            foreach (string filePath in invalidTemplateFiles)
                WriteToDebugLog($"Template file {filePath} doesn't exist.", LogMessageErrorLevel.Warning);

            if (templateFiles.Length == 0)
            {
                WriteToDebugLog("No valid mission template files given as parameters.", LogMessageErrorLevel.Error);
                WriteToDebugLog("");
                WriteToDebugLog("Command-line format is BriefingRoomCommandLine.exe <MissionTemplate.brt> [<MissionTemplate2.brt> <MissionTemplate3.brt>...]");
                return false;
            }

            foreach (string t in templateFiles)
            {
                DCSMission mission = BriefingRoomGenerator.GenerateMission(t);
                if (mission == null)
                {
                    Console.WriteLine($"Failed to generate a mission from template {Path.GetFileName(t)}");
                    continue;
                }

                string mizFileName;
                if (templateFiles.Length == 1) // Single template file provided, use the mission name as file name.
                    mizFileName = Path.Combine(Application.StartupPath, RemoveInvalidPathCharacters(mission.Briefing.Name) + ".miz");
                else // Multiple template files provided, use the template name as file name so we know from which template mission was generated.
                    mizFileName = Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(t) + ".miz");
                mizFileName = GetUnusedFileName(mizFileName);

                if (!mission.SaveToMizFile(mizFileName))
                {
                    WriteToDebugLog($"Failed to export .miz file from template {Path.GetFileName(t)}", LogMessageErrorLevel.Warning);
                    continue;
                }
                else
                    WriteToDebugLog($"Mission {Path.GetFileName(mizFileName)} exported to .miz file from template {Path.GetFileName(t)}");
            }

            return true;
        }

        /// <summary>
        /// Removed invalid path characters from a filename and replaces them with underscores.
        /// </summary>
        /// <param name="fileName">A filename.</param>
        /// <returns>The filename, without invalid characters.</returns>
        private string RemoveInvalidPathCharacters(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "_";
            return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// Gets an unused filename. If the provided filepath is not used, returns it.
        /// Else, tries to append some extra number (1, 2, 3) until it found an unused filename.
        /// </summary>
        /// <param name="filePath">Desired filepath.</param>
        /// <returns>Path to unused file.</returns>
        private string GetUnusedFileName(string filePath)
        {
            if (!File.Exists(filePath)) return filePath; // File doesn't exist, use the desired name

            string newName;
            int extraNameCount = 2;

            do
            {
                newName = Path.Combine(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)} ({extraNameCount}){Path.GetExtension(filePath)}");
                extraNameCount++;
            } while (File.Exists(newName));

            return newName;
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            BriefingRoomGenerator.Dispose();
            LogWriter.Dispose();
        }
    }
}
