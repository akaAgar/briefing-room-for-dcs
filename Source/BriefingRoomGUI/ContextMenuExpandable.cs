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

using System.Windows.Forms;

namespace BriefingRoom.GUI
{
    /// <summary>
    /// A class that can be expanded into a ContextMenu when viewed as a property in a <see cref="TreeViewPropertyEditor{T}"/>.
    /// </summary>
    public abstract class ContextMenuExpandable
    {
        /// <summary>
        /// Adds the required items in a context menu.
        /// </summary>
        /// <param name="contextMenu">The context menu in which to display items.</param>
        /// <param name="onClickEventHandler">Method to call when a menu item is clicked.</param>
        public abstract void CreateContextMenu(ContextMenuStrip contextMenu, ToolStripItemClickedEventHandler onClickEventHandler);

        /// <summary>
        /// Called when an item from the context menu is clicked.
        /// </summary>
        /// <param name="itemTag">Tag of the clicked item.</param>
        public abstract void OnContextMenuItemClicked(object itemTag);
    }
}
