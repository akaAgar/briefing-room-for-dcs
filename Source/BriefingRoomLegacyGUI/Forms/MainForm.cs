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
        private readonly BriefingRoom BriefingRoom;
        private MissionTemplate Template;

        public MainForm(BriefingRoom briefingRoom)
        {
            InitializeComponent();

            BriefingRoom = briefingRoom;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Template = new MissionTemplate();
            SetupTemplateControls();
            UpdateAllTemplateControls();
        }

        private void SetupTemplateControls()
        {
            LegacyGUITools.PopulateCheckedTreeViewFromEnum<MissionOption>(TreeViewOptions);
            LegacyGUITools.PopulateCheckedTreeViewFromEnum<RealismOption>(TreeViewRealism);

            TreeViewDCSMods.Nodes.Clear();
            foreach (DatabaseEntryInfo entryInfo in BriefingRoom.GetDatabaseEntriesInfo(DatabaseEntryType.DCSMod))
                TreeViewDCSMods.Nodes.Add(entryInfo.ID, entryInfo.Name);
            TreeViewDCSMods.Sort();
        }

        private void UpdateAllTemplateControls()
        {
            LegacyGUITools.UpdateCheckedTreeViewFromEnumList(TreeViewOptions, Template.OptionsMission);
            LegacyGUITools.UpdateCheckedTreeViewFromEnumList(TreeViewRealism, Template.OptionsRealism);

            FlightGroupsDataGridView.Rows.Clear();
            foreach (MissionTemplateFlightGroup flightGroup in Template.PlayerFlightGroups)
                FlightGroupsDataGridView.Rows.Add(flightGroup.Aircraft, flightGroup.Count, flightGroup.Payload, flightGroup.Carrier);

            //DGVC_FGAircraft
        }

        private void FlightGroupsDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            MainContextMenuStrip.Items.Clear();
            MainContextMenuStrip.Items.Add("Test");

            MainContextMenuStrip.Show(FlightGroupsDataGridView, FlightGroupsDataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location.Add(e.Location));
        }
    }
}
