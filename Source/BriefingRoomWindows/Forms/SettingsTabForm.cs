using BriefingRoom4DCS.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Forms;

namespace BriefingRoom4DCS.WindowsTool.Forms
{
    public partial class SettingsTabForm : Form
    {
        private readonly MissionTemplate Template;

        public SettingsTabForm(MissionTemplate template, ImageList iconsImageList)
        {
            InitializeComponent();

            SettingsListView.LargeImageList = iconsImageList;
            Template = template;
        }

        private void SettingsTabForm_Load(object sender, EventArgs e)
        {
            List<string> createdGroups = new List<string>();

            foreach (PropertyInfo propertyInfo in WindowsGUIToolbox.GetMissionTemplateSettingsProperties())
            {
                DisplayAttribute displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                CategoryAttribute categoryAttribute = propertyInfo.GetCustomAttribute<CategoryAttribute>();

                ListViewGroup group = null;
                if (categoryAttribute != null)
                {
                    if (!createdGroups.Contains(categoryAttribute.Category))
                    {
                        SettingsListView.Groups.Add(categoryAttribute.Category, categoryAttribute.Category);
                        createdGroups.Add(categoryAttribute.Category);
                    }

                    group = SettingsListView.Groups[categoryAttribute.Category];
                }

                SettingsListView.Items.Add(CreateListViewItem(group, propertyInfo.Name, displayAttribute.Name, displayAttribute.Description));
            }
        }

        public void UpdateValues()
        {
            // Settings list view
            foreach (PropertyInfo propertyInfo in WindowsGUIToolbox.GetMissionTemplateSettingsProperties())
            {
                //DisplayAttribute displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();

                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(Template);
                string valueText = (value == null) ? "" : value.ToString();

                DatabaseSourceTypeAttribute databaseSourceTypeAttribute = propertyInfo.GetCustomAttribute<DatabaseSourceTypeAttribute>();
                if (databaseSourceTypeAttribute != null)
                {
                    DatabaseEntryInfo? databaseEntryInfo = BriefingRoom.GetSingleDatabaseEntryInfo(databaseSourceTypeAttribute.EntryType, valueText);
                    if (databaseEntryInfo.HasValue) valueText = databaseEntryInfo.Value.Name;
                }
                else if (propertyInfo.PropertyType.IsEnum)
                    valueText = WindowsGUIToolbox.BeautifyEnumValue(value);

                SettingsListView.Items[name].Tag = value;
                SettingsListView.Items[name].SubItems[1].Text = valueText;
            }
        }

        private ListViewItem CreateListViewItem(ListViewGroup group, string name, string text, string toolTip)
        {
            ListViewItem listViewItem = new ListViewItem(text, text) { Group = group, Name = name, ToolTipText = toolTip, ImageKey = name.ToLowerInvariant() };
            listViewItem.SubItems.Add("");
            return listViewItem;
        }

        private void OnSettingsListViewMouseClick(object sender, MouseEventArgs e)
        {
            if (SettingsListView.SelectedItems.Count == 0) return;
            PropertyInfo propertyInfo = typeof(MissionTemplate).GetProperty(SettingsListView.SelectedItems[0].Name);
            if (propertyInfo == null) return;

            DatabaseEntryType? databaseEntryType = null;
            DatabaseSourceTypeAttribute databaseSourceTypeAttribute = propertyInfo.GetCustomAttribute<DatabaseSourceTypeAttribute>();
            if (databaseSourceTypeAttribute != null) databaseEntryType = databaseSourceTypeAttribute.EntryType;

            string dataBaseEntryFilter = null;
            if (propertyInfo.Name == "FlightPlanTheaterStartingAirbase")
                dataBaseEntryFilter = Template.ContextTheater;

            WindowsGUIToolbox.PopulateContextMenu(SettingsContextMenuStrip, propertyInfo.Name, propertyInfo.PropertyType, databaseEntryType, dataBaseEntryFilter);

            SettingsContextMenuStrip.Show(SettingsListView, e.Location);
        }

        private void OnSettingsContextMenuStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (SettingsListView.SelectedItems.Count == 0) return;
            PropertyInfo propertyInfo = typeof(MissionTemplate).GetProperty(SettingsListView.SelectedItems[0].Name);
            if (propertyInfo == null) return;
            if (e.ClickedItem.Tag == null) return;

            propertyInfo.SetValue(Template, e.ClickedItem.Tag);
            UpdateValues();
        }
    }
}
