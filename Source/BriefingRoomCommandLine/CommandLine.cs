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
using System.IO;
using System.Windows.Forms;

namespace BriefingRoom4DCS.CommandLineTool
{
    /// <summary>
    /// Tool to generate mission(s) from command-line parameters.
    /// </summary>
    public class CommandLine : IDisposable
    {
        /// <summary>
        /// Maximum number of missions to generate.
        /// </summary>
        private const int MAX_MISSION_COUNT = 10;

        private static readonly string LOG_FILE = $"{Application.StartupPath}\\BriefingRoomCommandLineDebugLog.txt";

        private readonly StreamWriter LogWriter;

        [STAThread]
        private static void Main(string[] args)
        {
#if DEBUG
            args = new string[] { "Default.brt" };
#endif

            if (args.Length <= 0) // No arguments
            {
                return;
            }

            //            if (DebugLog.Instance.ErrorCount > 0) return; // Errors found, abort! abort!

#if !DEBUG
            try
            {
#endif
                using (CommandLine cl = new CommandLine())
                    cl.DoCommandLine(args);
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
            }
#endif

#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press any key to close this window");
            Console.ReadKey();
#endif
        }

        private readonly BriefingRoom4DCS.BriefingRoom BriefingRoomGenerator;

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
        /// <returns>True if everything when wrong, false otherwise</returns>
        public bool DoCommandLine(params string[] args)
        {
            ParseCommandLineArguments(args, out string[] templateFiles, out int missionCount);

            if (templateFiles.Length == 0)
            {
                WriteToDebugLog("No mission template files provided.", LogMessageErrorLevel.Warning);
                return false;
            }

            foreach (string t in templateFiles)
            {
                for (int i = 0; i < missionCount; i++)
                {
                    DCSMission mission = BriefingRoomGenerator.GenerateMission(t);
                    if (mission == null)
                    {
                        Console.WriteLine($"Failed to generate a mission from template {Path.GetFileName(t)}");
                        continue;
                    }

                    string mizFileName = Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(t) + ".miz");
                    mizFileName = GetUnusedFileName(mizFileName);

                    if (!mission.SaveToMizFile(mizFileName))
                    {
                        WriteToDebugLog($"Failed to export .miz file from template {Path.GetFileName(t)}", LogMessageErrorLevel.Warning);
                        continue;
                    }
                    else
                        WriteToDebugLog($"Mission {Path.GetFileName(mizFileName)} exported to .miz file from template {Path.GetFileName(t)}");
                }
            }

            return true;
        }

        /// <summary>
        /// Parse command line arguments to 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="templates"></param>
        /// <param name="missionCount"></param>
        private void ParseCommandLineArguments(string[] args, out string[] templates, out int missionCount)
        {
            missionCount = 0;
            List<string> templateFiles = new List<string>();

            // Parse all arguments to try to know
            foreach (string arg in args)
            {
                // Argument is an integer, use it as the number of missions to generate
                if (int.TryParse(arg, out int argAsInt))
                    missionCount = argAsInt;

                // Argument is a template file, add it to the list of template files to parse
                if (File.Exists(arg) && arg.ToLowerInvariant().EndsWith(".brt"))
                    templateFiles.Add(Path.GetFullPath(arg));
            }

            missionCount = Toolbox.Clamp(missionCount, 1, MAX_MISSION_COUNT);
            templates = templateFiles.ToArray();
        }

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
