namespace BriefingRoom4DCS.WindowsTool.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FormMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Tools = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Tools_Console = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.SettingsTabPage = new System.Windows.Forms.TabPage();
            this.ObjectivesTabPage = new System.Windows.Forms.TabPage();
            this.FlightGroupsTabPage = new System.Windows.Forms.TabPage();
            this.FeaturesTabPage = new System.Windows.Forms.TabPage();
            this.OptionsTabPage = new System.Windows.Forms.TabPage();
            this.BriefingTabPage = new System.Windows.Forms.TabPage();
            this.ValuesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.IconsImageList = new System.Windows.Forms.ImageList(this.components);
            this.FormMenuStrip.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormMenuStrip
            // 
            this.FormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.M_Tools});
            this.FormMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.FormMenuStrip.Name = "FormMenuStrip";
            this.FormMenuStrip.Size = new System.Drawing.Size(784, 24);
            this.FormMenuStrip.TabIndex = 0;
            this.FormMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // M_Tools
            // 
            this.M_Tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.M_Tools_Console});
            this.M_Tools.Name = "M_Tools";
            this.M_Tools.Size = new System.Drawing.Size(46, 20);
            this.M_Tools.Text = "&Tools";
            // 
            // M_Tools_Console
            // 
            this.M_Tools_Console.Name = "M_Tools_Console";
            this.M_Tools_Console.Size = new System.Drawing.Size(156, 22);
            this.M_Tools_Console.Text = "&Output console";
            this.M_Tools_Console.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.SettingsTabPage);
            this.MainTabControl.Controls.Add(this.ObjectivesTabPage);
            this.MainTabControl.Controls.Add(this.FlightGroupsTabPage);
            this.MainTabControl.Controls.Add(this.FeaturesTabPage);
            this.MainTabControl.Controls.Add(this.OptionsTabPage);
            this.MainTabControl.Controls.Add(this.BriefingTabPage);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTabControl.Location = new System.Drawing.Point(0, 24);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(784, 537);
            this.MainTabControl.TabIndex = 1;
            // 
            // SettingsTabPage
            // 
            this.SettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsTabPage.Name = "SettingsTabPage";
            this.SettingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsTabPage.Size = new System.Drawing.Size(776, 511);
            this.SettingsTabPage.TabIndex = 0;
            this.SettingsTabPage.Text = "Mission settings";
            this.SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // ObjectivesTabPage
            // 
            this.ObjectivesTabPage.Location = new System.Drawing.Point(4, 22);
            this.ObjectivesTabPage.Name = "ObjectivesTabPage";
            this.ObjectivesTabPage.Size = new System.Drawing.Size(776, 511);
            this.ObjectivesTabPage.TabIndex = 3;
            this.ObjectivesTabPage.Text = "Objectives";
            this.ObjectivesTabPage.UseVisualStyleBackColor = true;
            // 
            // FlightGroupsTabPage
            // 
            this.FlightGroupsTabPage.Location = new System.Drawing.Point(4, 22);
            this.FlightGroupsTabPage.Name = "FlightGroupsTabPage";
            this.FlightGroupsTabPage.Size = new System.Drawing.Size(776, 511);
            this.FlightGroupsTabPage.TabIndex = 5;
            this.FlightGroupsTabPage.Text = "Player flight groups";
            this.FlightGroupsTabPage.UseVisualStyleBackColor = true;
            // 
            // FeaturesTabPage
            // 
            this.FeaturesTabPage.Location = new System.Drawing.Point(4, 22);
            this.FeaturesTabPage.Name = "FeaturesTabPage";
            this.FeaturesTabPage.Size = new System.Drawing.Size(776, 511);
            this.FeaturesTabPage.TabIndex = 2;
            this.FeaturesTabPage.Text = "Mission features";
            this.FeaturesTabPage.UseVisualStyleBackColor = true;
            // 
            // OptionsTabPage
            // 
            this.OptionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.OptionsTabPage.Name = "OptionsTabPage";
            this.OptionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.OptionsTabPage.Size = new System.Drawing.Size(776, 511);
            this.OptionsTabPage.TabIndex = 1;
            this.OptionsTabPage.Text = "Options";
            this.OptionsTabPage.UseVisualStyleBackColor = true;
            // 
            // BriefingTabPage
            // 
            this.BriefingTabPage.Location = new System.Drawing.Point(4, 22);
            this.BriefingTabPage.Name = "BriefingTabPage";
            this.BriefingTabPage.Size = new System.Drawing.Size(776, 511);
            this.BriefingTabPage.TabIndex = 4;
            this.BriefingTabPage.Text = "Briefing";
            this.BriefingTabPage.UseVisualStyleBackColor = true;
            // 
            // ValuesContextMenuStrip
            // 
            this.ValuesContextMenuStrip.Name = "ValuesContextMenuStrip";
            this.ValuesContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // IconsImageList
            // 
            this.IconsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.IconsImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.IconsImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.FormMenuStrip);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.FormMenuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BriefingRoom";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormMenuStrip.ResumeLayout(false);
            this.FormMenuStrip.PerformLayout();
            this.MainTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip FormMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.TabPage OptionsTabPage;
        private System.Windows.Forms.ContextMenuStrip ValuesContextMenuStrip;
        private System.Windows.Forms.TabPage ObjectivesTabPage;
        private System.Windows.Forms.TabPage FlightGroupsTabPage;
        private System.Windows.Forms.TabPage FeaturesTabPage;
        private System.Windows.Forms.TabPage BriefingTabPage;
        private System.Windows.Forms.ToolStripMenuItem M_Tools;
        private System.Windows.Forms.ToolStripMenuItem M_Tools_Console;
        private System.Windows.Forms.ImageList IconsImageList;
    }
}