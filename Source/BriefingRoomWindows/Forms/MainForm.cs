using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.WindowsTool.Forms
{
    public partial class MainForm : Form
    {
        private readonly BriefingRoom BriefingRoomLibrary;
        private readonly ConsoleForm ConsoleWinForm;
        private readonly MissionTemplate Template;

        private readonly OptionsTabForm TabFormOptions;
        private readonly SettingsTabForm TabFormSettings;

        public MainForm(BriefingRoom briefingRoomLibrary, ConsoleForm consoleWinForm)
        {
            InitializeComponent();
            Icon = new Icon("Media\\Icon.ico");

            BriefingRoomLibrary = briefingRoomLibrary;
            ConsoleWinForm = consoleWinForm;
            Template = new MissionTemplate();

            // Load icons into the image list
            IconsImageList.Images.Clear();
            foreach (string imageFilePath in Directory.GetFiles("Media\\WinGUI", "*.png"))
                IconsImageList.Images.Add(Path.GetFileNameWithoutExtension(imageFilePath).ToLowerInvariant(), Image.FromFile(imageFilePath));

            TabFormOptions = new OptionsTabForm(Template, IconsImageList);
            TabFormSettings = new SettingsTabForm(Template, IconsImageList);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $"BriefingRoom {BriefingRoom.VERSION} for DCS World {BriefingRoom.TARGETED_DCS_WORLD_VERSION}";

            WindowsGUIToolbox.AssignFormToTabPage(TabFormSettings, SettingsTabPage);
            WindowsGUIToolbox.AssignFormToTabPage(TabFormOptions, OptionsTabPage);

            UpdateTemplate();
        }

        private void UpdateTemplate()
        {
            TabFormSettings.UpdateValues();
            TabFormOptions.UpdateValues();
        }

        private void PopulateContextMenuStrip(DatabaseEntryType databaseEntryType)
        {
            ValuesContextMenuStrip.Items.Clear();

            //foreach (DatabaseEntryInfo dbi in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType))
            //{
            //    if (string.IsNullOrEmpty(dbi.Category)) continue;

            //    ValuesContextMenuStrip.Items.Add(dbi.Category)
            //}


            foreach (DatabaseEntryInfo dbi in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(dbi.Name)
                {
                    Tag = dbi.ID,
                    ToolTipText = dbi.Description
                };

                ValuesContextMenuStrip.Items.Add(item);
            }

            // TODO: sort the context menu
        }

        private void OnMenuClick(object sender, EventArgs e)
        {
            if (sender == M_Tools_Console)
            {
                ConsoleWinForm.BringToFront();
                ConsoleWinForm.Focus();
            }
        }
    }
}
