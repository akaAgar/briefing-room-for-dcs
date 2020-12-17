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
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BriefingRoom4DCSWorld.TypeConverters
{
    public abstract class CheckedListBoxUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null) return value;

            string[] arrayIn = ParseValuesIn(value);
            CheckedListBox clb = new CheckedListBox();
            foreach (string arrayValue in GetPossibleArrayValues())
                clb.Items.Add(arrayValue, ValueIsSet(arrayIn, arrayValue));
            clb.Height = Toolbox.Clamp(clb.Items.Count * 20, 80, 400);
            clb.Width = Math.Max(clb.Width, (from string s in clb.Items select TextRenderer.MeasureText(s.ToString(), clb.Font).Width + 48).Max());

            edSvc.DropDownControl(clb);

            List<string> listOut = new List<string>();
            foreach (object o in clb.CheckedItems)
                listOut.Add(o.ToString());

            return ParseValuesOut(listOut.Distinct().OrderBy(x => x).ToArray());
        }

        protected virtual bool ValueIsSet(string[] array, string value)
        {
            return array.Contains(value);
        }

        protected virtual string[] ParseValuesIn(object value)
        {
            if (value == null) value = new string[0];
            return (string[])value;
        }

        protected virtual object ParseValuesOut(string[] selectedValues)
        {
            return selectedValues;
        }

        protected abstract string[] GetPossibleArrayValues();
    }
}
