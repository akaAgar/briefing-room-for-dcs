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
using System;
using System.Windows.Forms;

namespace BriefingRoom.GUI
{
    /// <summary>
    /// Main application class.
    /// </summary>
    public static class BriefingRoom
    {
        /// <summary>
        /// Main application entry point.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            Console.Title = "BriefingRoom output console";
            DebugLog.Instance.CreateLogFileWriter();

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
