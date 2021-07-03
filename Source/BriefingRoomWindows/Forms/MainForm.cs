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

namespace BriefingRoom4DCS.WindowsTool.Forms
{
    public partial class MainForm : Form
    {
        private readonly BriefingRoom BriefingRoomLibrary;
        private MissionTemplate Template; 

        public MainForm(BriefingRoom briefingRoomLibrary)
        {
            InitializeComponent();
            Icon = new Icon("Media\\Icon.ico");

            BriefingRoomLibrary = briefingRoomLibrary;
            Template = new MissionTemplate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupListViews();
            PopulateTreeView<MissionOption>(OptionsTreeView);
            //PopulateTreeView<RealismOption>(OptionsTreeView);
            PopulateTreeView(ModsTreeView, DatabaseEntryType.DCSMod);
            PopulateTreeView(MissionFeaturesTreeView, DatabaseEntryType.MissionFeature);
            UpdateListViews();
        }

        private void PopulateTreeView<T>(TreeView treeView) where T : Enum
        {
            treeView.Nodes.Clear();

            foreach (T enumValue in Enum.GetValues(typeof(T)))
                treeView.Nodes.Add(enumValue.ToString());

            treeView.Sort();
        }

        private void PopulateTreeView(TreeView treeView, DatabaseEntryType databaseEntryType)
        {
            treeView.Nodes.Clear();

            foreach (DatabaseEntryInfo dbi in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType))
            {
                TreeNode node = new TreeNode(dbi.Name)
                {
                    Name = dbi.ID,
                    ToolTipText = dbi.Description
                };
                treeView.Nodes.Add(node);
            }
        }

        private void UpdateListViews()
        {
        }

        private void SetupListViews()
        {
            ListViewGroup groupContext = MissionSettingsListView.Groups.Add("context", "Context");
            ListViewGroup groupEnvironment = MissionSettingsListView.Groups.Add("environment", "Environment");

            MissionSettingsListView.Items.Add(CreateListViewItem(groupContext, "redCoalition", "Coalition, red", "Russia"));
            MissionSettingsListView.Items.Add(CreateListViewItem(groupContext, "blueCoalition", "Coalition, blue", "USA"));
            MissionSettingsListView.Items.Add(CreateListViewItem(groupEnvironment, "season", "Season", "Random"));
        }

        private ListViewItem CreateListViewItem(ListViewGroup group, string name, params string[] itemValues)
        {
            ListViewItem listViewItem = new ListViewItem(itemValues);
            listViewItem.Name = name;
            listViewItem.Group = group;
            return listViewItem;
        }

        private void MissionSettingsListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender == null) return;
            ListView senderListView = (ListView)sender;
            if (senderListView.SelectedItems.Count == 0) return;

            switch (senderListView.SelectedItems[0].Name)
            {
                default: return;
                case "redCoalition":
                case "blueCoalition":
                    PopulateContextMenuStrip(DatabaseEntryType.Coalition);
                    break;
                case "season":
                    break;
            }

            ValuesContextMenuStrip.Show(senderListView, e.Location);
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
    }
}
