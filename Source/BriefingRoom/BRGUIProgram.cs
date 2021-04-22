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

using BriefingRoom.DB;
using BriefingRoom.Debug;
using BriefingRoom.GUI;
using System;
using System.Windows.Forms;

namespace BriefingRoom
{
    /// <summary>
    /// Main application class.
    /// </summary>
    public static class BriefingRoom
    {
        /// <summary>
        /// Absolute URL to the project source code repository.
        /// </summary>
        public const string REPO_URL = "https://github.com/akaAgar/briefing-room-for-dcs";

        /// <summary>
        /// Targeted DCS world version (just for info, doesn't mean that the program will not work with another version)
        /// </summary>
        public static string TARGETED_DCS_WORLD_VERSION { get; private set; }

        /// <summary>
        /// Absolute URL to the project website.
        /// </summary>
        public const string WEBSITE_URL = "https://akaagar.itch.io/briefing-room-for-dcs";

        /// <summary>
        /// Main application entry point.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Console.Title = "BriefingRoom output console";
            DebugLog.Instance.CreateLogFileWriter();

            using (INIFile ini = new INIFile($"{BRPaths.DATABASE}Common.ini"))
                TARGETED_DCS_WORLD_VERSION = ini.GetValue("Versions", "DCSVersion", "2.5");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Display the splash screen while the database is being loaded
            using (SplashScreenForm splashScreen = new SplashScreenForm())
            {
                splashScreen.ShowDialog();
                if (splashScreen.AbortStartup) return;
            }

            Application.Run(new MainForm());

            DebugLog.Instance.CloseLogFileWriter();
        }
    }
}
