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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BriefingRoom4DCS.LegacyGUI
{
    public static class LegacyGUITools
    {


        public static void PopulateCheckedTreeViewFromEnum<T>(TreeView treeView) where T : Enum
        {
            treeView.Nodes.Clear();
            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
            {
                string enumValueString = enumValue.ToString();
                treeView.Nodes.Add(enumValueString, enumValueString);
            }
        }

        internal static void UpdateCheckedTreeViewFromEnumList<T>(TreeView treeView, List<T> enumList) where T : Enum
        {
            foreach (TreeNode treeNode in treeView.Nodes) treeNode.Checked = false;

            foreach (T enumValue in enumList)
            {
                string enumValueString = enumValue.ToString();
                if (!treeView.Nodes.ContainsKey(enumValueString)) continue;
                treeView.Nodes[enumValueString].Checked = true;
            }
        }
    }
}
