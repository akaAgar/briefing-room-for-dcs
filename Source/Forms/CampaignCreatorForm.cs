/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BriefingRoom4DCSWorld.Template;

namespace BriefingRoom4DCSWorld.Forms
{
    public partial class CampaignCreatorForm : Form
    {
        private readonly CampaignTemplate Template;

        public CampaignCreatorForm()
        {
            InitializeComponent();

            Template = new CampaignTemplate();
        }

        private void CampaignCreatorForm_Load(object sender, EventArgs e)
        {
            TemplatePropertyGrid.SelectedObject = Template;
        }

        private void OnMenuClick(object sender, EventArgs e)
        {
            switch (((ToolStripButton)sender).Name)
            {
                case "T_Close":
                    Close();
                    return;
            }
        }
    }
}
