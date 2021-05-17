using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.LegacyGUI.Forms
{
    public partial class MainForm : Form
    {
        private BriefingRoom BriefingRoom;
        private MissionTemplate Template;

        public MainForm(BriefingRoom briefingRoom)
        {
            InitializeComponent();
            BriefingRoom = briefingRoom;

            Template = new MissionTemplate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            TemplateSettingsListView.Columns.Clear();
            TemplateSettingsListView.Columns.Add("");
            TemplateSettingsListView.Columns.Add("");

            PlayerFlightGroupsListView.Columns.Clear();
            PlayerFlightGroupsListView.Columns.Add("Aircraft");
            PlayerFlightGroupsListView.Columns.Add("Payload");

            MissionFeaturesListView.Items.Clear();
            foreach (DatabaseEntryInfo dbEntryInfo in BriefingRoom.GetDatabaseEntriesInfo(DatabaseEntryType.MissionFeature))
            {
                MissionFeaturesListView.Items.Add(dbEntryInfo.Name);
            }

            UpdateAllListViews();
        }

        private void UpdateAllListViews()
        {
            TemplateSettingsListView.Items.Clear();
            TemplateSettingsListView.Items.Add("Coalition, blue").SubItems.Add(Template.ContextCoalitionBlue);
            TemplateSettingsListView.Items.Add("Coalition, red").SubItems.Add(Template.ContextCoalitionRed);
            TemplateSettingsListView.Items.Add("Player coalition").SubItems.Add(Template.ContextPlayerCoalition.ToString());

            PlayerFlightGroupsListView.Items.Clear();
            foreach (MissionTemplateFlightGroup flightGroup in Template.PlayerFlightGroups)
                PlayerFlightGroupsListView.Items.Add(flightGroup.Aircraft);
        }

        private void TemplateSettingsListView_MouseClick(object sender, MouseEventArgs e)
        {
            MainContextMenuStrip.Items.Clear();
            MainContextMenuStrip.Items.Add("Hello!");

            MainContextMenuStrip.Show(TemplateSettingsListView, e.Location);
        }
    }
}
