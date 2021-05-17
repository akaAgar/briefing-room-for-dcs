using BriefingRoom4DCS.LegacyGUI.Forms;
using System;
using System.Windows.Forms;

namespace BriefingRoom4DCS.LegacyGUI
{
    internal class LegacyGUIProgram : IDisposable
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (LegacyGUIProgram legacyGUI = new LegacyGUIProgram()) { }
        }

        public LegacyGUIProgram()
        {
            using (BriefingRoom briefingRoom = new BriefingRoom())
            {
                Application.Run(new MainForm(briefingRoom));
            }
        }

        public void Dispose()
        {

        }
    }
}
