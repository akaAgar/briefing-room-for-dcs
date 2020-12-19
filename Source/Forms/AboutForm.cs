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

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.Forms
{
    /// <summary>
    /// Small dialog box with additional info about the program.
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event raised when the form is loaded.
        /// </summary>
        /// <param name="sender">Sender control, this form</param>
        /// <param name="e">Event args</param>
        private void AboutForm_Load(object sender, EventArgs e)
        {
            string infoText = "";
            infoText += $"Current version: {BriefingRoom.BRIEFINGROOM_VERSION}\r\n";
            infoText += $"Targeted DCS World version: {BriefingRoom.TARGETED_DCS_WORLD_VERSION}\r\n";
            infoText += "\r\n";
            infoText += "Created by Ambroise Garel\r\n";
            infoText += "Uses the Silk icon set (http://famfamfam.com/lab/icons/silk/)\r\n";
            infoText += "\r\n";
            infoText += "Released under the GNU General Public License 3.0 \r\n";
            infoText += $"Project website: {BriefingRoom.WEBSITE_URL}\r\n";
            infoText += $"Source code repository: {BriefingRoom.REPO_URL}";
            InfoRichTextBox.Text = infoText;
        }

        /// <summary>
        /// Event raised when the "Close" button is clicked.
        /// </summary>
        /// <param name="sender">Sender control, the "close" button</param>
        /// <param name="e">Event args</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void IntoRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
            // {BriefingRoom.WEBSITE_URL}
        }
    }
}
