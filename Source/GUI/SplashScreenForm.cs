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

using BriefingRoom4DCSWorld.DB;
using BriefingRoom4DCSWorld.Debug;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.GUI
{
    /// <summary>
    /// Splash screen displayed while the database is loaded.
    /// </summary>
    public partial class SplashScreenForm : Form
    {
        /// <summary>
        /// Minimum display time of the splash screen even if the database is done loading, in seconds.
        /// </summary>
        private const int MINIMUM_DISPLAY_TIME = 3;

        /// <summary>
        /// Path to the image to display on the splash screen.
        /// </summary>
        private static readonly string SPLASHSCREEN_IMAGE = $"{BRPaths.MEDIA}SplashScreen.png";

        /// <summary>
        /// Has the database been loaded successfully?
        /// </summary>
        private bool DatabaseLoadComplete = false;

        /// <summary>
        /// Elapsed seconds since the splash screen has been loaded.
        /// </summary>
        private int ElapsedSeconds = 0;

        /// <summary>
        /// Should the program startup be aborted? Set to true if the database wasn't loaded successfully.
        /// </summary>
        public bool AbortStartup { get; private set; } = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreenForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the splash screen is loaded.
        /// </summary>
        /// <param name="sender">Form</param>
        /// <param name="e">Event arguments</param>
        private void SplashScreenForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(SPLASHSCREEN_IMAGE))
                PanelSplashImage.BackgroundImage = Image.FromFile(SPLASHSCREEN_IMAGE);

            LabelVersion.Text = $"Version {BriefingRoom.BRIEFINGROOM_VERSION}\r\nfor DCS World {BriefingRoom.TARGETED_DCS_WORLD_VERSION}";
#if DEBUG
            LabelVersion.Text = $"DEBUG BUILD\r\n" + LabelVersion.Text;
#endif

            CloseTimer.Start();
            LoadTimer.Start();
        }

        /// <summary>
        /// Timer ran ONCE once 500ms after the splash screen was loaded. Just to make sure the debug text box is properly displayed before starting to output log lines into it.
        /// </summary>
        /// <param name="sender">The sender control, LoadTimer</param>
        /// <param name="e">Event arguments</param>
        private void OnLoadTimerTick(object sender, EventArgs e)
        {
            LoadTimer.Stop(); // Stop the timer at once to make sure LoadTimer_Tick is called only once

            Database.Instance.Initialize();

            if (DebugLog.Instance.WarningCount > 0) // Warnings were raised while loading the database
                ShowProblemMessageBox("warning", DebugLog.Instance.GetWarnings(), MessageBoxIcon.Warning);

            if (DebugLog.Instance.ErrorCount > 0) // Errors were raised while loading the database, abort everything and return
            {
                AbortStartup = true;
                ShowProblemMessageBox("error", DebugLog.Instance.GetErrors(), MessageBoxIcon.Error);
                Close();
                return;
            }
            else
                DatabaseLoadComplete = true; // No errors so note that database was loaded successfully

            if (DebugLog.Instance.WarningCount > 0)
                Close(); // No need to wait for CloseTimer to make sure the splash screen was seen
        }

        /// <summary>
        /// Shows a messagebox with a list of problems that happened while loading the database
        /// </summary>
        /// <param name="problemName">The type of problem, as a string to be displayed. Can be "error", "warning"...</param>
        /// <param name="problems">An array of messages describing the problems</param>
        /// <param name="icon">Icon to show in the messagebox</param>
        private void ShowProblemMessageBox(string problemName, string[] problems, MessageBoxIcon icon)
        {
            // Make sure not too many lines are displayed in the messagebox, but store the actual number of problems
            int problemCount = problems.Length;
            if (problems.Length > 12) Array.Resize(ref problems, 12);

            string message = "- " + string.Join("\r\n- ", problems);

            MessageBox.Show(
                message, $"{problemCount} {problemName}(s) while loading database",
                MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Timer ran EVERY SECOND once the splash screen is loaded.
        /// </summary>
        /// <param name="sender">The sender control, CloseTimer</param>
        /// <param name="e">Event arguments</param>
        private void OnCloseTimerTick(object sender, EventArgs e)
        {
            ElapsedSeconds++;

            // If the database wasn't loaded or the minimum time hasn't been elapsed, keep waiting.
            if (!DatabaseLoadComplete || (ElapsedSeconds < MINIMUM_DISPLAY_TIME)) return;
            Close();
        }
    }
}
