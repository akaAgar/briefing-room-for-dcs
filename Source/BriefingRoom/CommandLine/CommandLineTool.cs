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

using BriefingRoom.Debug;
using BriefingRoom.Generator;
using BriefingRoom.Mission;
using BriefingRoom.Miz;
using BriefingRoom.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BriefingRoom.CommandLine
{
    /// <summary>
    /// Tool to generate mission(s) from command-line parameters.
    /// </summary>
    public class CommandLineTool : IDisposable
    {
        /// <summary>
        /// Maximum number of missions to generate.
        /// </summary>
        private const int MAX_MISSION_COUNT = 10;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CommandLineTool() { }

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
                DebugLog.Instance.WriteLine("No mission template files provided", DebugLogMessageErrorLevel.Error);
                return false;
            }

            using (MissionGenerator generator = new MissionGenerator())
            {
                foreach (string t in templateFiles)
                {
                    using (MissionTemplate template = new MissionTemplate(t))
                    {
                        for (int i = 0; i < missionCount;i++)
                        {
                            DCSMission mission = generator.Generate(template);
                            if (mission == null)
                            {
                                DebugLog.Instance.WriteLine($"Failed to generate a mission from template {Path.GetFileName(t)}", DebugLogMessageErrorLevel.Warning);
                                continue;
                            }

                            string mizFileName = Path.Combine(Application.StartupPath, Path.GetFileNameWithoutExtension(t) + ".miz");
                            mizFileName = GetUnusedFileName(mizFileName);

                            MizFile miz = mission.ExportToMiz();
                            if ((miz == null) || !miz.SaveToFile(mizFileName))
                            {
                                DebugLog.Instance.WriteLine($"Failed to export .miz file from template {Path.GetFileName(t)}", DebugLogMessageErrorLevel.Warning);
                                continue;
                            }
                            else
                                DebugLog.Instance.WriteLine($"Mission {Path.GetFileName(mizFileName)} exported to .miz file from template {Path.GetFileName(t)}");
                        }
                    }
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
        public void Dispose() { }
    }
}
