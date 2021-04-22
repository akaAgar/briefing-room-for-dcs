using BriefingRoom.Attributes;
using BriefingRoom.DB;
using BriefingRoom.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace BriefingRoom.GUI
{
    public class TreeViewPropertyEditor<T> : IDisposable
    {
        /// <summary>
        /// Event raised when a property value is changed.
        /// </summary>
        /// <returns></returns>
        public event EventHandler OnPropertyValueChanged = null;

        private readonly T SelectedObject;
        private readonly TreeView EditorTreeView;
        private readonly ContextMenuStrip ContextMenu;

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            return SelectedObjectType.GetProperty(propertyName);
        }

        private object GetPropertyValue(string propertyName)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(SelectedObject);
        }

        private void SetPropertyValue(string propertyName, object value)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            if (propertyInfo == null) return;
            propertyInfo.SetValue(SelectedObject, value);
        }

        private TreeViewPropertyAttribute GetTreeViewPropertyAttribute(string propertyName)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetCustomAttribute<TreeViewPropertyAttribute>();
        }

        private PropertyType GetPropertyAttribute<PropertyType>(string propertyName) where PropertyType : Attribute
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetCustomAttribute<PropertyType>();
        }

        /// <summary>
        /// The type of the displayed object.
        /// </summary>
        private Type SelectedObjectType { get { return typeof(T); } }

        public TreeViewPropertyEditor(TreeView treeView, T selectedObject)
        {
            SelectedObject = selectedObject;

            EditorTreeView = treeView;
            EditorTreeView.ImageList = GUITools.IconsList16;
            EditorTreeView.ShowNodeToolTips = true;
            EditorTreeView.Font = GUITools.BLUEPRINT_FONT;
            EditorTreeView.BackColor = GUITools.BLUEPRINT_BACKCOLOR_TREEVIEW;
            EditorTreeView.ForeColor = GUITools.BLUEPRINT_FORECOLOR;
            EditorTreeView.ShowPlusMinus = false;
            EditorTreeView.ShowRootLines = false;

            // Setup context menu
            ContextMenu = new ContextMenuStrip
            {
                BackColor = GUITools.BLUEPRINT_BACKCOLOR_MENU,
                Font = GUITools.BLUEPRINT_FONT,
                ForeColor = GUITools.BLUEPRINT_FORECOLOR,
                ShowCheckMargin = false,
                ShowImageMargin = false,
            };

            // Create tree view nodes
            CreateNodes();
            UpdateAllNodes();

            // Setup events
            EditorTreeView.NodeMouseClick += OnNodeMouseClick;
            EditorTreeView.AfterExpand += OnNodeAfterExpand;
            EditorTreeView.AfterCollapse += OnAfterCollapse;
            ContextMenu.ItemClicked += OnContextMenuItemClicked;
        }

        private void OnAfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null) GUITools.SetTreeNodeImage(e.Node, "folder0");
        }

        private void OnNodeAfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null) GUITools.SetTreeNodeImage(e.Node, "folder1");
        }

        private void OnContextMenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (EditorTreeView.SelectedNode == null) return; // No selected node
            if (e.ClickedItem.Tag == null) return; // Node has no tag (probably a folder)

            if (e.ClickedItem.Tag.ToString() == "*add") // Special case: adding a new item to a ContextMenuExpandable array
            {
                Array arrayValues = (Array)GetPropertyValue(EditorTreeView.SelectedNode.Parent.Name);

                if (GetTreeViewPropertyAttribute(EditorTreeView.SelectedNode.Parent.Name).DataSourceType == typeof(MissionTemplateFlightGroup))
                {
                    List<MissionTemplateFlightGroup> list = new List<MissionTemplateFlightGroup>();
                    for (int i = 0; i < arrayValues.Length; i++)
                        list.Add((MissionTemplateFlightGroup)arrayValues.GetValue(i));
                    list.Add(new MissionTemplateFlightGroup());

                    SetPropertyValue(EditorTreeView.SelectedNode.Parent.Name, list.ToArray());
                }

                UpdateNode(EditorTreeView.SelectedNode.Parent);
                OnPropertyValueChanged?.Invoke(EditorTreeView, new EventArgs());
                return;
            }
            else if (e.ClickedItem.Tag.ToString() == "*remove") // Special case: revove an item to a ContextMenuExpandable array
            {
                Array arrayValues = (Array)GetPropertyValue(EditorTreeView.SelectedNode.Parent.Name);
                int index = (int)EditorTreeView.SelectedNode.Tag;

                if (GetTreeViewPropertyAttribute(EditorTreeView.SelectedNode.Parent.Name).DataSourceType == typeof(MissionTemplateFlightGroup))
                {
                    List<MissionTemplateFlightGroup> list = new List<MissionTemplateFlightGroup>();
                    for (int i = 0; i < arrayValues.Length; i++)
                    {
                        if (i == index) continue;
                        list.Add((MissionTemplateFlightGroup)arrayValues.GetValue(i));
                    }

                    SetPropertyValue(EditorTreeView.SelectedNode.Parent.Name, list.ToArray());
                }

                UpdateNode(EditorTreeView.SelectedNode.Parent);
                OnPropertyValueChanged?.Invoke(EditorTreeView, new EventArgs());
                return;
            }

            // Property is a ContextMenuExpandable, call the class's own OnContextMenuItemClicked method
            if ((EditorTreeView.SelectedNode.Parent != null) &&
                (GetTreeViewPropertyAttribute(EditorTreeView.SelectedNode.Parent.Name) != null) &&
                GetTreeViewPropertyAttribute(EditorTreeView.SelectedNode.Parent.Name).DataSourceType.IsSubclassOf(typeof(ContextMenuExpandable)))
            {
                Array arrayValues = (Array)GetPropertyValue(EditorTreeView.SelectedNode.Parent.Name);
                int arrayIndex = (int)EditorTreeView.SelectedNode.Tag;
                ((ContextMenuExpandable)arrayValues.GetValue(arrayIndex)).OnContextMenuItemClicked(e.ClickedItem.Tag);
                UpdateNode(EditorTreeView.SelectedNode.Parent);
                OnPropertyValueChanged?.Invoke(EditorTreeView, new EventArgs());
                return;
            }

            SetPropertyValue(EditorTreeView.SelectedNode.Name, e.ClickedItem.Tag);
            UpdateNode(EditorTreeView.SelectedNode);
            OnPropertyValueChanged?.Invoke(EditorTreeView, new EventArgs());
        }

        private void OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null) return;
            EditorTreeView.SelectedNode = e.Node;

            if (e.Node.Nodes.Count > 0)
            {
                if (e.Node.IsExpanded) e.Node.Collapse();
                else e.Node.Expand();
                return;
            }

            if (string.IsNullOrEmpty(e.Node.Name)) return; // Node has no name, nothing to do
            if (e.Node.Name.StartsWith("*")) return; // Node is a folder, nothing to do
            if (e.Node.Name.StartsWith("#")) // Node is an array element
            {
                // Array of enums or DBEntries, treat as a checkbox
                if (GetTreeViewPropertyAttribute(e.Node.Parent.Name).DataSourceType.IsEnum ||
                    GetTreeViewPropertyAttribute(e.Node.Parent.Name).DataSourceType.IsSubclassOf(typeof(DBEntry)))
                {
                    if (e.Button != MouseButtons.Left) return; // Only left clicks can toggle checkboxes
                    string checkImage = (e.Node.ImageKey == "checkbox0") ? "checkbox1" : "checkbox0";
                    e.Node.ImageKey = checkImage;
                    e.Node.SelectedImageKey = checkImage;

                    object selectedValues = null;
                    PropertyInfo propertyInfo = GetPropertyInfo(e.Node.Parent.Name);
                    Type elementType = propertyInfo.PropertyType.GetElementType();
                    if (elementType == typeof(string))
                        selectedValues = (from TreeNode n in e.Node.Parent.Nodes where n.ImageKey == "checkbox1" select n.Name.Substring(1)).ToArray();
                    else if (elementType == typeof(RealismOption))
                        selectedValues = (from TreeNode n in e.Node.Parent.Nodes where n.ImageKey == "checkbox1" select (RealismOption)Enum.Parse(elementType, n.Name.Substring(1))).ToArray();

                    if (selectedValues != null)
                        propertyInfo.SetValue(SelectedObject, selectedValues);

                    UpdateNode(e.Node.Parent); // Update parent because sometimes enabling an option will disable others
                    OnPropertyValueChanged?.Invoke(EditorTreeView, new EventArgs());
                }
                else if (GetTreeViewPropertyAttribute(e.Node.Parent.Name).DataSourceType.
                    IsSubclassOf(typeof(ContextMenuExpandable))) // Array of ContextMenuExpandables
                {
                    ContextMenu.Items.Clear();
                    Array arrayValues = (Array)GetPropertyValue(e.Node.Parent.Name);
                    int arrayIndex = (int)e.Node.Tag;
                    ((ContextMenuExpandable)arrayValues.GetValue(arrayIndex)).CreateContextMenu(ContextMenu, OnContextMenuItemClicked);
                    ContextMenu.Items.Add(new ToolStripSeparator());

                    if (GetTreeViewPropertyAttribute(e.Node.Parent.Name).DataSourceType == typeof(MissionTemplateFlightGroup))
                    {
                        ContextMenu.Items.Add("Add another flight group").Tag = "*add";
                        if (e.Node.Parent.Nodes.Count > 1)
                            ContextMenu.Items.Add("Remove flight group").Tag = "*remove";
                    }

                    foreach (ToolStripItem item in ContextMenu.Items)
                    {
                        if ((item is ToolStripMenuItem) && (((ToolStripMenuItem)item).DropDownItems.Count > 0))
                            ((ToolStripMenuItem)item).DropDownItemClicked += OnContextMenuItemClicked;
                    }

                    ContextMenu.Show(EditorTreeView, e.Location);
                }

                return;
            }

            if (GetPropertyInfo(e.Node.Name).PropertyType.IsArray) return;

            ContextMenu.Items.Clear();
            TreeViewPropertyAttribute tvpa = GetTreeViewPropertyAttribute(e.Node.Name);
            if (tvpa.DataSourceType.IsEnum)
            {
                foreach (object enumValue in Enum.GetValues(tvpa.DataSourceType))
                {
                    GUITools.GetDisplayStrings(tvpa.DataSourceType, enumValue, out string enumDisplayName, out string enumDescription);
                    ToolStripMenuItem enumMenuItem = new ToolStripMenuItem { Tag = enumValue, Text = enumDisplayName, ToolTipText = enumDescription };
                    ContextMenu.Items.Add(enumMenuItem);
                }
            }
            else if (tvpa.DataSourceType == typeof(int))
            {
                TreeViewPropertyIntAttribute tvpia = GetPropertyAttribute<TreeViewPropertyIntAttribute>(e.Node.Name);

                if (tvpia != null)
                    for (int i = tvpia.IntValueMin; i <= tvpia.IntValueMax; i += tvpia.IntValueIncrement)
                        ContextMenu.Items.Add(tvpia.FormatIntValue(i)).Tag = i;
            }
            else if (tvpa.DataSourceType.IsSubclassOf(typeof(DBEntry)))
                GUITools.PopulateToolStripMenuWithDBEntries(
                    ContextMenu.Items, tvpa.DataSourceType,
                    OnContextMenuItemClicked, tvpa.Flags.HasFlag(TreeViewPropertyAttributeFlags.EmptyIsRandom));

            ContextMenu.Show(EditorTreeView, e.Location);
        }

        private void CreateNodes()
        {
            EditorTreeView.Nodes.Clear();
            foreach (PropertyInfo propertyInfo in SelectedObjectType.GetProperties())
            {
                TreeViewPropertyAttribute tvpa = propertyInfo.GetCustomAttribute<TreeViewPropertyAttribute>();
                if (tvpa == null) continue;

                TreeNodeCollection nodeCollection = EditorTreeView.Nodes;

                if (tvpa.ParentNode != null) // Property belongs to a category/folder
                {
                    string parentNodeKey = $"*{tvpa.ParentNode}";
                    if (!EditorTreeView.Nodes.ContainsKey(parentNodeKey)) // Folder does not exist, create it
                    {
                        TreeNode parentNode = new TreeNode(tvpa.ParentNode) { Name = parentNodeKey, ImageKey = "folder0", SelectedImageKey = "folder0" };
                        EditorTreeView.Nodes.Add(parentNode);
                    }

                    nodeCollection = EditorTreeView.Nodes[parentNodeKey].Nodes;
                }

                TreeNode node = new TreeNode
                {
                    Text = tvpa.DisplayName,
                    Name = propertyInfo.Name,
                    ToolTipText = tvpa.Description
                };
                GUITools.SetTreeNodeImage(node, "Setting");

                nodeCollection.Add(node);

                if (propertyInfo.PropertyType.IsArray)
                {
                    GUITools.SetTreeNodeImage(node, "folder0");

                    if (tvpa.DataSourceType.IsEnum)
                    {
                        foreach (object enumValue in Enum.GetValues(tvpa.DataSourceType))
                        {
                            GUITools.GetDisplayStrings(tvpa.DataSourceType, enumValue, out string enumDisplayName, out string enumDescription);
                            node.Nodes.Add($"#{enumValue}", enumDisplayName, "checkbox0", "checkbox0").ToolTipText = enumDescription;
                        }
                    }
                    else if (tvpa.DataSourceType.IsSubclassOf(typeof(DBEntry)))
                    {
                        foreach (DBEntry entry in Database.Instance.GetAllEntries(tvpa.DataSourceType))
                            node.Nodes.Add($"#{entry.ID}", entry.UIDisplayName, "checkbox0", "checkbox0").ToolTipText = entry.UIDescription;
                    }
                }
            }

            EditorTreeView.Sort();
        }

        public void UpdateAllNodes()
        {
            foreach (TreeNode node in EditorTreeView.Nodes)
                UpdateNode(node);
        }

        private void UpdateNode(TreeNode node)
        {
            Array arrayValues;

            if (!string.IsNullOrEmpty(node.Name))
            {
                switch (node.Name[0])
                {
                    case '*': // Node is a folder
                        break;
                    case '#': // Node is a checkbox
                        arrayValues = (Array)GetPropertyValue(node.Parent.Name);
                        string checkImage = "checkbox0";
                        for (int i = 0; i < arrayValues.Length; i++)
                        {
                            if (arrayValues.GetValue(i).ToString() == node.Name.Substring(1))
                            {
                                checkImage = "checkbox1";
                                break;
                            }
                        }
                        node.ImageKey = checkImage;
                        node.SelectedImageKey = checkImage;
                        break;
                    default: // Node is a property
                        if (GetPropertyInfo(node.Name).PropertyType.IsArray) // Property is an array
                        {
                            if (GetTreeViewPropertyAttribute(node.Name).DataSourceType.IsSubclassOf(typeof(ContextMenuExpandable)))
                            {
                                node.Nodes.Clear();
                                arrayValues = (Array)GetPropertyValue(node.Name);

                                for (int i = 0; i < arrayValues.Length; i++)
                                {
                                    TreeNode elementNode = node.Nodes.Add($"#{i}", arrayValues.GetValue(i).ToString());
                                    elementNode.Tag = i;
                                    elementNode.ToolTipText = elementNode.Text.Replace(", ", "\r\n- ");
                                }

                                return; // Do not update subnodes
                            }
                        }
                        else
                        {
                            TreeViewPropertyAttribute tvpa = GetTreeViewPropertyAttribute(node.Name);

                            string valueText = GUITools.GetDisplayName(tvpa.DataSourceType, GetPropertyValue(node.Name));
                            if (tvpa.DataSourceType == typeof(int))
                            {
                                TreeViewPropertyIntAttribute tvpia = GetPropertyAttribute<TreeViewPropertyIntAttribute>(node.Name);
                                if (tvpia != null)
                                    valueText = tvpia.FormatIntValue((int)GetPropertyValue(node.Name));
                            }
                            else if (string.IsNullOrEmpty(valueText) && tvpa.Flags.HasFlag(TreeViewPropertyAttributeFlags.EmptyIsRandom))
                                valueText = "Random";

                            node.Text = $"{tvpa.DisplayName}: {valueText}";
                        }
                        break;
                }
            }

            foreach (TreeNode childNode in node.Nodes)
                UpdateNode(childNode);
        }

        public void Dispose()
        {

        }
    }
}