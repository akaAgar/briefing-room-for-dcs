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

using BriefingRoom4DCSWorld.Debug;
using BriefingRoom4DCSWorld.Generator;
using BriefingRoom4DCSWorld.Mission;
using BriefingRoom4DCSWorld.Miz;
using BriefingRoom4DCSWorld.Template;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.Forms
{
    public partial class MainForm : Form
    {
        private static readonly Image IMAGE_ERROR = GUITools.GetImageFromResource("Icons.Error.png");
        private static readonly Image IMAGE_INFO = GUITools.GetImageFromResource("Icons.Info.png");
        private static readonly Image IMAGE_WARNING = GUITools.GetImageFromResource("Icons.Warning.png");

        private readonly MissionGenerator Generator;
        private readonly MissionTemplate Template;
        private DCSMission Mission = null;
        
        public MainForm()
        {
            InitializeComponent();

            Generator = new MissionGenerator();
            Template = new MissionTemplate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"BriefingRoom {BriefingRoom.BRIEFINGROOM_VERSION} for DCS World {BriefingRoom.TARGETED_DCS_WORLD_VERSION}";

#if DEBUG
            Text += " [DEBUG BUILD]";
            M_Mission_DebugExport.Visible = true;
#endif

            Icon = GUITools.GetIconFromResource("Icon.ico");
            LoadIcons();
            TemplatePropertyGrid.SelectedObject = Template;
            GenerateMission();
        }

        private void LoadIcons()
        {
            M_File_New.Image = GUITools.GetImageFromResource("Icons.New.png");
            M_File_Open.Image = GUITools.GetImageFromResource("Icons.Open.png");
            M_File_SaveAs.Image = GUITools.GetImageFromResource("Icons.Save.png");
            M_File_Exit.Image = GUITools.GetImageFromResource("Icons.Exit.png");
            M_Mission_Generate.Image = GUITools.GetImageFromResource("Icons.Update.png");
            M_Mission_Export.Image = GUITools.GetImageFromResource("Icons.ExportToMiz.png");
            M_Mission_ExportBriefing.Image = GUITools.GetImageFromResource("Icons.ExportBriefing.png");
            M_Mission_ExportBriefingHTML.Image = GUITools.GetImageFromResource("Icons.FileHTML.png");
            M_Mission_DebugExport.Image = GUITools.GetImageFromResource("Icons.DebugExport.png");
            M_Tools_CampaignCreator.Image = GUITools.GetImageFromResource("Icons.Campaign.png");

            T_File_New.Image = M_File_New.Image;
            T_File_Open.Image = M_File_Open.Image;
            T_File_SaveAs.Image = M_File_SaveAs.Image;
            T_Mission_Generate.Image = M_Mission_Generate.Image;
            T_Mission_Export.Image = M_Mission_Export.Image;
            T_Mission_ExportBriefing.Image = M_Mission_ExportBriefing.Image;
            T_Mission_ExportBriefingHTML.Image = M_Mission_ExportBriefingHTML.Image;
        }

        private void GenerateMission()
        {
            ToggleMissionButtonsEnabled(false);

            Mission = Generator.Generate(Template);
            PrintStatusLabelOutput("Mission generation");

            if (Mission == null) // Something went wrong during the mission generation
            {
                UpdateHTMLBriefing($"<p><strong>Mission generation failed</strong></p><p>{StatusLabel.Text}</p>");
                return;
            }

            ToggleMissionButtonsEnabled(true);
            UpdateHTMLBriefing(Mission.BriefingHTML);
        }

        private void ToggleMissionButtonsEnabled(bool enabled)
        {
            M_Mission_Export.Enabled = enabled;
            M_Mission_ExportBriefing.Enabled = enabled;
            M_Mission_ExportBriefingHTML.Enabled = enabled;
            T_Mission_Export.Enabled = enabled;
            T_Mission_ExportBriefing.Enabled = enabled;
            M_Mission_DebugExport.Enabled = enabled;
        }

        private void UpdateHTMLBriefing(string html)
        {
            BriefingWebBrowser.Navigate("about:blank"); // The WebBrowser control must navigate to a new page or it won't update its content
            if (BriefingWebBrowser.Document != null) BriefingWebBrowser.Document.Write(string.Empty);
            BriefingWebBrowser.DocumentText = html;
        }

        private void MenuClick(object sender, EventArgs e)
        {
            string senderName = ((ToolStripItem)sender).Name;

            switch (senderName)
            {
                case "M_About":
                case "T_About":
                    using (AboutForm form = new AboutForm()) form.ShowDialog();
                    return;
                case "M_File_New":
                case "T_File_New":
                    Template.Clear();
                    TemplatePropertyGrid.Refresh();
                    GenerateMission();
                    return;
                case "M_File_Open":
                case "T_File_Open":
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "BriefingRoom templates (*.brt)|*.brt";
                        ofd.RestoreDirectory = true;
                        if ((ofd.ShowDialog() == DialogResult.OK) && File.Exists(ofd.FileName))
                        {
                            Template.LoadFromFile(ofd.FileName);
                            TemplatePropertyGrid.Refresh();
                            GenerateMission();
                        }
                    }
                    return;
                case "M_File_SaveAs":
                case "T_File_SaveAs":
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "BriefingRoom templates (*.brt)|*.brt";
                        sfd.RestoreDirectory = true;
                        if (sfd.ShowDialog() == DialogResult.OK)
                            Template.SaveToFile(sfd.FileName);
                    }
                    return;
                case "M_File_Exit":
                    Close();
                    return;
                case "M_Mission_Export":
                case "T_Mission_Export":
                    if (Mission == null) return;
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "DCS World missions (*.miz)|*.miz";
                        sfd.FileName = Mission.MissionName + ".miz";
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = Toolbox.GetDCSMissionPath();
                        if (sfd.ShowDialog() == DialogResult.OK)
                            ExportToMiz(sfd.FileName);
                    }
                    return;
                case "M_Mission_ExportBriefingHTML":
                case "T_Mission_ExportBriefingHTML":
                    if (Mission == null) return;
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "HTML pages (*.html)|*.html";
                        sfd.FileName = Mission.MissionName + ".html";
                        sfd.RestoreDirectory = true;
                        if (sfd.ShowDialog() == DialogResult.OK)
                            File.WriteAllText(sfd.FileName, Mission.BriefingHTML);
                    }
                    return;
                case "M_Mission_Generate":
                case "T_Mission_Generate":
                    GenerateMission();
                    return;
#if DEBUG
                case "M_Mission_DebugExport":
                    if (Mission == null) return; // No mission to export
                    Toolbox.CreateDirectoryIfMissing(BRPaths.DEBUGOUTPUT);
                    using (MizFile miz = Mission.ExportToMiz())
                        miz.SaveToDirectory(BRPaths.DEBUGOUTPUT);
                    return;
#endif
                case "M_Tools_CampaignCreator":
                    using (CampaignCreatorForm campaignForm = new CampaignCreatorForm())
                        campaignForm.ShowDialog();
                    return;
            }
        }

        private void ExportToMiz(string filePath)
        {
            if (Mission == null) return; // Mission not generated, nothing to export

            using (MizFile miz = Mission.ExportToMiz())
            {
                PrintStatusLabelOutput("Miz export");

                if (miz == null) // Something went wrong during the .miz export
                    return;

                miz.SaveToFile(filePath);
            }
        }

        private void PrintStatusLabelOutput(string operationDescription)
        {
            if (DebugLog.Instance.ErrorCount > 0)
            {
                StatusLabel.Text = $"{operationDescription} failed with {DebugLog.Instance.ErrorCount} error(s). Click this message for more information.";
                StatusLabel.Image = IMAGE_ERROR;
            }
            else if (DebugLog.Instance.WarningCount > 0)
            {
                StatusLabel.Text = $"{operationDescription} completed with {DebugLog.Instance.WarningCount} warning(s). Click this message for more information.";
                StatusLabel.Image = IMAGE_WARNING;
            }
            else
            {
                StatusLabel.Text = DebugLog.Instance.LastMessage;
                StatusLabel.Image = IMAGE_INFO;
            }
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
#if !DEBUG
            e.Cancel =
                MessageBox.Show(
                    "Close BriefingRoom? Unsaved changes will be lost.", "Close BriefingRoom?",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK;
#endif
        }

        private void TemplatePropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            GenerateMission();
        }

        private void StatusLabel_Click(object sender, EventArgs e)
        {
            string text, title;
            MessageBoxIcon icon;

            if (DebugLog.Instance.ErrorCount > 0)
            {
                text = string.Join("\r\n", (from s in DebugLog.Instance.GetErrors() select $"- {s}"));
                title = $"{DebugLog.Instance.ErrorCount} errors(s)";
                icon = MessageBoxIcon.Error;
            }
            else if (DebugLog.Instance.WarningCount > 0)
            {
                text = string.Join("\r\n", (from s in DebugLog.Instance.GetWarnings() select $"- {s}"));
                title = $"{DebugLog.Instance.WarningCount} warning(s)";
                icon = MessageBoxIcon.Warning;
            }
            else
            {
                text = DebugLog.Instance.LastMessage;
                title = "No problem found";
                icon = MessageBoxIcon.Information;
            }

            MessageBox.Show(text, title, MessageBoxButtons.OK, icon);
        }
    }
}
