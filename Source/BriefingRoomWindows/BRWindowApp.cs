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

using BriefingRoom4DCS.WindowsTool.Forms;
using System;
using System.Windows.Forms;

namespace BriefingRoom4DCS.WindowsTool
{
    /// <summary>
    /// Main application class.
    /// </summary>
    public class BRWindowApp
    {
        /// <summary>
        /// Application static entry point.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BRWindowApp winApp = new BRWindowApp();
        }

        private readonly BriefingRoom BriefingRoomLibrary;
        private readonly ConsoleForm ConsoleWinForm;
        private readonly MainForm MainWinForm;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BRWindowApp()
        {
            // Setup console output form.
            ConsoleWinForm = new ConsoleForm();
#if !DEBUG
            ConsoleWinForm.Show();
#endif

            // Show splash screen while library is loading.
            using (SplashForm splashForm = new SplashForm())
            {
                splashForm.Show();
                Application.DoEvents();
                BriefingRoomLibrary = new BriefingRoom(WriteToDebugLog);
                splashForm.Close();
            }

#if DEBUG
            ConsoleWinForm.Show();
#endif

            MainWinForm = new MainForm(BriefingRoomLibrary, ConsoleWinForm);

            Application.Run(MainWinForm);

            ConsoleWinForm.EnableClosing = true;

            MainWinForm.Dispose();
            ConsoleWinForm.Dispose();
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

            //LogWriter.WriteLine(message);
            //Console.WriteLine(message);
            ConsoleWinForm.PrintToConsole(message);
        }
    }
}
