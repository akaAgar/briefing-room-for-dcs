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
    public partial class ConsoleForm : Form
    {
        public bool EnableClosing { get; set; } = false;

        public ConsoleForm()
        {
            InitializeComponent();
        }

        private void ConsoleForm_Load(object sender, EventArgs e)
        {

        }

        public void PrintToConsole(string message)
        {
            OutputListBox.Items.Add(message);
            int visibleItems = OutputListBox.ClientSize.Height / OutputListBox.ItemHeight;
            OutputListBox.TopIndex = Math.Max(OutputListBox.Items.Count - visibleItems + 1, 0);
            Application.DoEvents();
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EnableClosing) return;

            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}
