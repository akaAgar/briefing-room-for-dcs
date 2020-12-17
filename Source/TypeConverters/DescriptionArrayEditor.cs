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
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace BriefingRoom4DCSWorld.TypeConverters
{
    public class DescriptionArrayEditor : ArrayEditor
    {
        public DescriptionArrayEditor(Type type) : base(type) { }

        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Shown += delegate { ShowDescription(form); };
            return form;
        }

        private static void ShowDescription(Control control)
        {
            if (control is PropertyGrid grid)
            {
                grid.HelpVisible = true;
                grid.PropertySort = PropertySort.Alphabetical;
                grid.ToolbarVisible = false;
            }

            foreach (Control child in control.Controls)
                ShowDescription(child);
        }
    }
}
