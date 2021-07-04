using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BriefingRoom4DCS.WindowsTool.Forms
{
    public partial class OptionsTabForm : Form
    {
        private bool DisableCheckEvent = false;
        private readonly MissionTemplate Template;

        public OptionsTabForm(MissionTemplate template, ImageList iconsImageList)
        {
            InitializeComponent();

            Template = template;
        }

        private void OptionsTabForm_Load(object sender, EventArgs e)
        {
            PopulateTreeView<MissionOption>(MissionOptionsTreeView);
            PopulateTreeView<RealismOption>(RealismOptionsTreeView);
            PopulateTreeView(ModsTreeView, DatabaseEntryType.DCSMod);
        }

        private void PopulateTreeView<T>(TreeView treeView) where T : Enum
        {
            treeView.Nodes.Clear();

            foreach (T enumValue in Enum.GetValues(typeof(T)))
                treeView.Nodes.Add(enumValue.ToString(), WindowsGUIToolbox.BeautifyEnumValue(enumValue)).Tag = enumValue;

            treeView.Sort();
        }

        private void PopulateTreeView(TreeView treeView, DatabaseEntryType databaseEntryType)
        {
            treeView.Nodes.Clear();

            foreach (DatabaseEntryInfo dbi in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType))
            {
                TreeNode node = new TreeNode(dbi.Name) { Name = dbi.ID, ToolTipText = dbi.Description };
                treeView.Nodes.Add(node);
            }

            treeView.Sort();
        }

        public void UpdateValues()
        {
            DisableCheckEvent = true;

            foreach (TreeNode treeNode in MissionOptionsTreeView.Nodes)
                treeNode.Checked = Template.OptionsMission.Contains((MissionOption)treeNode.Tag);

            foreach (TreeNode treeNode in RealismOptionsTreeView.Nodes)
                treeNode.Checked = Template.OptionsRealism.Contains((RealismOption)treeNode.Tag);

            DisableCheckEvent = false;
        }

        private void OnTreeNodeChecked(object sender, TreeViewEventArgs e)
        {
            if (DisableCheckEvent) return;

            Template.OptionsMission = (from TreeNode treeNode in MissionOptionsTreeView.Nodes where treeNode.Checked select (MissionOption)treeNode.Tag).ToList();
            Template.OptionsRealism = (from TreeNode treeNode in RealismOptionsTreeView.Nodes where treeNode.Checked select (RealismOption)treeNode.Tag).ToList();

            UpdateValues();
        }
    }
}
