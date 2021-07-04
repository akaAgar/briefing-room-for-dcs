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

using BriefingRoom4DCS.Template;
using BriefingRoom4DCS.WindowsTool.Forms;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BriefingRoom4DCS.WindowsTool
{
    /// <summary>
    /// Static "toolbox" class for the Windows GUI utility.
    /// </summary>
    public static class WindowsGUIToolbox
    {
        public static PropertyInfo[] GetMissionTemplateSettingsProperties()
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

            foreach (PropertyInfo propertyInfo in typeof(MissionTemplate).GetProperties())
            {
                // Ignore arrays and lists
                if (propertyInfo.PropertyType.IsArray) continue;
                if (propertyInfo.PropertyType.IsGenericType && (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))) continue;

                // Ignore properties without display attributes
                DisplayAttribute displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null) continue;

                propertyInfos.Add(propertyInfo);
            }

            return propertyInfos.ToArray();
        }

        internal static void AssignFormToTabPage(Form tabForm, TabPage tabPage)
        {
            tabForm.TopLevel = false;
            tabForm.Parent = tabPage;
            tabForm.Dock = DockStyle.Fill;
            tabForm.Show();
        }

        internal static void PopulateContextMenu(ContextMenuStrip contextMenu, string propertyName, Type valueType, DatabaseEntryType? databaseEntryType = null, string dataBaseEntryFilter = null)
        {
            contextMenu.Items.Clear();

            if (valueType.IsEnum)
            {
                foreach (object value in Enum.GetValues(valueType))
                    contextMenu.Items.Add(BeautifyEnumValue(value)).Tag = value;

                return;
            }
            else if (valueType == typeof(int))
            {
                if (propertyName == "ObjectiveDistance")
                {
                    contextMenu.Items.Add("Random").Tag = 0;
                    for (int i = 10; i <= 160; i++)
                        contextMenu.Items.Add(i.ToString() + " nm").Tag = i;
                }
            }
            else if ((valueType == typeof(string)) && databaseEntryType.HasValue)
            {
                // Add a "no value" option for some values (like airbase)
                if (databaseEntryType.Value== DatabaseEntryType.Airbase)
                    contextMenu.Items.Add("(None)").Tag = "";

                // Create categories sub-folders
                foreach (DatabaseEntryInfo databaseEntryInfo in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType.Value, dataBaseEntryFilter))
                {
                    if (string.IsNullOrEmpty(databaseEntryInfo.Category)) continue;
                    string categoryKey = $"*{databaseEntryInfo.Category}";
                    if (contextMenu.Items.ContainsKey(categoryKey)) continue;

                    ToolStripMenuItem categoryItem = (ToolStripMenuItem)contextMenu.Items.Add(databaseEntryInfo.Category);
                    //categoryItem.Click += contextMenu.ItemClicked;
                    categoryItem.Name = categoryKey;
                }
                
                // Add definitions
                foreach (DatabaseEntryInfo databaseEntryInfo in BriefingRoom.GetDatabaseEntriesInfo(databaseEntryType.Value, dataBaseEntryFilter))
                {
                    if (string.IsNullOrEmpty(databaseEntryInfo.Category))
                        contextMenu.Items.Add(databaseEntryInfo.Name).Tag = databaseEntryInfo.ID;
                    else
                    {
                        string categoryKey = $"*{databaseEntryInfo.Category}";
                        //((ToolStripDropDownButton)contextMenu.Items[categoryKey]).Items.Add(databaseEntryInfo.Name).Tag = databaseEntryInfo.ID;
                        ((ToolStripMenuItem)contextMenu.Items[categoryKey]).DropDownItems.Add(databaseEntryInfo.Name).Tag = databaseEntryInfo.ID;
                    }
                }
            }
        }

        //private static void OnCategoryItemClicked(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem tsiSender = (ToolStripMenuItem)sender;
        //    tsiSender.GetCurrentParent().ItemClicked
        //}

        internal static string BeautifyEnumValue(object value)
        {
            string valueStr = value.ToString();

            // Special case for decade enum
            if (valueStr.StartsWith("Decade")) valueStr = valueStr.Substring(6) + "s";

            // Split camel case
            return string.Join(" ", Regex.Split(valueStr, @"(?<!^)(?=[A-Z])"));
        }
    }
}
