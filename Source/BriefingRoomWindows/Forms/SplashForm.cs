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
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            BackgroundImage = Image.FromFile("Media\\SplashScreen.png");
            VersionLabel.Text = $"Version {BriefingRoom.VERSION}";
        }
    }
}
