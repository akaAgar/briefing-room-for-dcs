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

using BriefingRoom4DCSWorld.Attributes;
using BriefingRoom4DCSWorld.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.GUI
{
    /// <summary>
    /// A "toolbox" static class with some useful methods to help with the user interface.
    /// </summary>
    public static class GUITools
    {
        /// <summary>
        /// Background color for the "Blueprint" treeview control.
        /// </summary>
        public static readonly Color BLUEPRINT_BACKCOLOR_TREEVIEW = Color.FromArgb(255, 48, 87, 225);

        /// <summary>
        /// Background color for the "Blueprint" context menu.
        /// </summary>
        public static readonly Color BLUEPRINT_BACKCOLOR_MENU = Color.FromArgb(255, 38, 71, 200);

        /// <summary>
        /// Font for the "Blueprint" treeview and context menu.
        /// </summary>
        public static readonly Font BLUEPRINT_FONT = new Font("Courier New", 10f);

        /// <summary>
        /// Foreground/text color for the "Blueprint" treeview and context menu.
        /// </summary>
        public static readonly Color BLUEPRINT_FORECOLOR = Color.White;

        /// <summary>
        /// List of 16x16px icons loaded from <see cref="BRPaths.MEDIA_ICONS16"/>.
        /// </summary>
        public static ImageList IconsList16 { get; private set; }

        /// <summary>
        /// Loads all PNG files from <see cref="BRPaths.MEDIA_ICONS16"/> into <see cref="IconsList16"/>.
        /// Should only be called once, before the first use of <see cref="IconsList16"/>.
        /// </summary>
        public static void LoadIcons()
        {
            if (IconsList16 != null) return; // Icons list are already loaded

            IconsList16 = new ImageList { ImageSize = new Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };
            foreach (string f in Directory.GetFiles(BRPaths.MEDIA_ICONS16, "*.png"))
                IconsList16.Images.Add(Path.GetFileNameWithoutExtension(f).ToLowerInvariant(), Image.FromFile(f));
        }

        /// <summary>
        /// Path to the assembly namespace where embedded resources are stored.
        /// </summary>
        private const string EMBEDDED_RESOURCES_PATH = "BriefingRoom4DCSWorld.Resources.";

        /// <summary>
        /// Returns an icon from an embedded resource.
        /// </summary>
        /// <param name="resourcePath">Relative path to the icon from BriefingRoom4DCSWorld.Resources.</param>
        /// <returns>An icon or null if no resource was found.</returns>
        public static Icon GetIconFromResource(string resourcePath)
        {
            Icon icon = null;

            using (Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream($"{EMBEDDED_RESOURCES_PATH}{resourcePath}"))
            {
                if (stream == null) return null;
                icon = new Icon(stream);
            }

            return icon;
        }

        /// <summary>
        /// "Shortcut" method to set all parameters of an OpenFileDialog and display it.
        /// </summary>
        /// <param name="fileExtension">The desired file extension.</param>
        /// <param name="initialDirectory">The initial directory of the dialog.</param>
        /// <param name="fileTypeDescription">A description of the file type (e.g. "Windows PCM wave files")</param>
        /// <returns>The path to the file to load, or null if no file was selected.</returns>
        public static string ShowOpenFileDialog(string fileExtension, string initialDirectory, string fileTypeDescription = null)
        {
            string fileName = null;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = initialDirectory;
                if (string.IsNullOrEmpty(fileTypeDescription)) fileTypeDescription = $"{fileExtension.ToUpperInvariant()} files";
                ofd.Filter = $"{fileTypeDescription} (*.{fileExtension})|*.{fileExtension}";
                if (ofd.ShowDialog() == DialogResult.OK) fileName = ofd.FileName;
            }

            return fileName;
        }

        public static void PopulateToolStripMenuWithIntegers(ToolStripItemCollection itemCollection, int minValue, int maxValue, int interval = 1)
        {
            for (int i = minValue; i <= maxValue; i += interval)
                itemCollection.Add(i.ToString()).Tag = i;
        }

        /// <summary>
        /// "Shortcut" method to set all parameters of a SaveFileDialog and display it.
        /// </summary>
        /// <param name="fileExtension">The desired file extension.</param>
        /// <param name="initialDirectory">The initial directory of the dialog.</param>
        /// <param name="defaultFileName">The defaule file name.</param>
        /// <param name="fileTypeDescription">A description of the file type (e.g. "Windows PCM wave files")</param>
        /// <returns>The path to the file to save to, or null if no file was selected.</returns>
        public static string ShowSaveFileDialog(string fileExtension, string initialDirectory, string defaultFileName = "", string fileTypeDescription = null)
        {
            string fileName = null;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = initialDirectory;
                sfd.FileName = defaultFileName;
                if (string.IsNullOrEmpty(fileTypeDescription)) fileTypeDescription = $"{fileExtension.ToUpperInvariant()} files";
                sfd.Filter = $"{fileTypeDescription} (*.{fileExtension})|*.{fileExtension}";
                if (sfd.ShowDialog() == DialogResult.OK) fileName = sfd.FileName;
            }

            return fileName;
        }

        public static string GetDisplayName(Type valueType, object value)
        {
            GetDisplayStrings(valueType, value, out string displayName, out string _);
            return displayName;
        }

        public static string GetDisplayName<T>(T value)
        {
            return GetDisplayName(typeof(T), value);
        }

        public static void GetDisplayStrings(Type valueType, object value, out string displayName, out string description)
        {
            displayName = value.ToString();
            description = null;

            if (valueType.IsEnum)
            {
                MemberInfo enumValueInfo = valueType.GetMember(value.ToString()).FirstOrDefault(m => m.DeclaringType == valueType);
                TreeViewEnumAttribute tvna = enumValueInfo.GetCustomAttribute<TreeViewEnumAttribute>();
                if (tvna == null) return;
                displayName = tvna.DisplayName;
                description = tvna.Description;
            }
            else if (valueType.IsSubclassOf(typeof(DBEntry)))
            {
                DBEntry dbEntry = Database.Instance.GetEntry(valueType, value.ToString());
                if (dbEntry == null) return;

                displayName = dbEntry.UIDisplayName;
                description = dbEntry.UIDescription;
            }
        }
        
        public static void SortContextMenuStrip(ToolStripItemCollection itemCollection)
        {
            List<ToolStripItem> itemList = new List<ToolStripItem>();
            foreach (ToolStripItem item in itemCollection) itemList.Add(item);
            itemList = itemList.OrderBy(x => x.Text).ToList();
            itemCollection.Clear();
            itemCollection.AddRange(itemList.ToArray());
        }

        public static void SetTreeNodeImage(TreeNode node, string imageKey)
        {
            node.ImageKey = imageKey;
            node.SelectedImageKey = imageKey;
        }

        public static void PopulateToolStripMenuWithDBEntries(ToolStripItemCollection itemCollection, Type dbEntryType, ToolStripItemClickedEventHandler onClickEventHandler = null, bool addRandomOption = false, string tagPrefix = "")
        {
            if (addRandomOption)
                itemCollection.Add("(Random)").Tag = "";

            foreach (string id in Database.Instance.GetAllEntriesIDs(dbEntryType))
            {
                ToolStripItemCollection parentCollection = itemCollection;

                DBEntry dbEntry = Database.Instance.GetEntry(dbEntryType, id);
                if (dbEntry == null) continue;

                if (!string.IsNullOrEmpty(dbEntry.UICategory)) // Database entry belong to a collection
                {
                    string categoryName = dbEntry.UICategory;
                    string categoryID = "*" + dbEntry.UICategory;

                    if (!parentCollection.ContainsKey(categoryID))
                    {
                        ToolStripItem categoryItem = parentCollection.Add(categoryName);

                        categoryItem.Name = categoryID;
                        categoryItem.BackColor = BLUEPRINT_BACKCOLOR_MENU;
                        categoryItem.Font = BLUEPRINT_FONT;
                        categoryItem.ForeColor = BLUEPRINT_FORECOLOR;
                    }

                    parentCollection = ((ToolStripMenuItem)parentCollection[categoryID]).DropDownItems;
                }

                ToolStripMenuItem item = new ToolStripMenuItem
                {
                    Text = dbEntry.UIDisplayName,
                    ToolTipText = dbEntry.UIDescription,
                    Tag = tagPrefix + id,
                    BackColor = BLUEPRINT_BACKCOLOR_MENU,
                    Font = BLUEPRINT_FONT,
                    ForeColor = BLUEPRINT_FORECOLOR,
                };

                parentCollection.Add(item);
            }

            foreach (ToolStripMenuItem item in itemCollection)
                if (item.DropDownItems.Count > 0)
                {
                    if (onClickEventHandler != null)
                        item.DropDownItemClicked += onClickEventHandler;
                    SortContextMenuStrip(item.DropDownItems);
                }

            SortContextMenuStrip(itemCollection);
        }
    }
}
