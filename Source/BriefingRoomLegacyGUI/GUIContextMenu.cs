using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BriefingRoom4DCS.LegacyGUI
{
    internal class GUIContextMenu : IDisposable
    {
        private ContextMenuStrip GUIContextMenuStrip;
        private string GUIContextMenuStripResult;

        internal GUIContextMenu(BriefingRoom briefingRoom)
        {
            GUIContextMenuStrip = new ContextMenuStrip();
            GUIContextMenuStrip.ItemClicked += OnGUIContextMenuStripItemClicked;
        }

        internal object ShowEnum<T>(Control control, Point location) where T : Enum
        {
            string value = ShowContextMenu(control, location, Enum.GetNames(typeof(T)));
            if (value == null) return null;

            return (T)Enum.Parse(typeof(T), value, true);
        }

        private string ShowContextMenu(Control control, Point location, params string[] values)
        {
            GUIContextMenuStripResult = null;
            GUIContextMenuStrip.Items.Clear();
            foreach (string value in values)
                GUIContextMenuStrip.Items.Add(value);
            GUIContextMenuStrip.Show(control, location);
            GUIContextMenuStrip.Focus();
            while (GUIContextMenuStrip.Visible)
                Application.DoEvents();

            return GUIContextMenuStripResult;
        }

        private void OnGUIContextMenuStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            GUIContextMenuStripResult = null;
            if (e.ClickedItem == null) return;
            GUIContextMenuStripResult = e.ClickedItem.Text;
        }

        public void Dispose()
        {
            GUIContextMenuStrip.Dispose();
        }
    }
}
