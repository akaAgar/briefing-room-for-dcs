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
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.SettingsTabPage = new System.Windows.Forms.TabPage();
            this.OptionsTabPage = new System.Windows.Forms.TabPage();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MissionSettingsListView = new System.Windows.Forms.ListView();
            this.ValuesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OptionsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ModsGroupBox = new System.Windows.Forms.GroupBox();
            this.MissionFeaturesGroupBox = new System.Windows.Forms.GroupBox();
            this.OptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.MissionFeaturesTreeView = new System.Windows.Forms.TreeView();
            this.ModsTreeView = new System.Windows.Forms.TreeView();
            this.OptionsTreeView = new System.Windows.Forms.TreeView();
            this.FeaturesTabPage = new System.Windows.Forms.TabPage();
            this.ObjectivesTabPage = new System.Windows.Forms.TabPage();
            this.BriefingTabPage = new System.Windows.Forms.TabPage();
            this.FlightGroupsTabPage = new System.Windows.Forms.TabPage();
            this.FormMenuStrip.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.SettingsTabPage.SuspendLayout();
            this.OptionsTabPage.SuspendLayout();
            this.OptionsTableLayoutPanel.SuspendLayout();
            this.ModsGroupBox.SuspendLayout();
            this.MissionFeaturesGroupBox.SuspendLayout();
            this.OptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FormMenuStrip
            // 
            this.FormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.FormMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.FormMenuStrip.Name = "FormMenuStrip";
            this.FormMenuStrip.Size = new System.Drawing.Size(784, 24);
            this.FormMenuStrip.TabIndex = 0;
            this.FormMenuStrip.Text = "menuStrip1";
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
            this.SettingsTabPage.Controls.Add(this.MissionSettingsListView);
            this.SettingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.SettingsTabPage.Name = "SettingsTabPage";
            this.SettingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.SettingsTabPage.Size = new System.Drawing.Size(776, 511);
            this.SettingsTabPage.TabIndex = 0;
            this.SettingsTabPage.Text = "Mission settings";
            this.SettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // OptionsTabPage
            // 
            this.OptionsTabPage.Controls.Add(this.OptionsTableLayoutPanel);
            this.OptionsTabPage.Location = new System.Drawing.Point(4, 22);
            this.OptionsTabPage.Name = "OptionsTabPage";
            this.OptionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.OptionsTabPage.Size = new System.Drawing.Size(776, 511);
            this.OptionsTabPage.TabIndex = 1;
            this.OptionsTabPage.Text = "Options";
            this.OptionsTabPage.UseVisualStyleBackColor = true;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // MissionSettingsListView
            // 
            this.MissionSettingsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MissionSettingsListView.HideSelection = false;
            this.MissionSettingsListView.Location = new System.Drawing.Point(3, 3);
            this.MissionSettingsListView.MultiSelect = false;
            this.MissionSettingsListView.Name = "MissionSettingsListView";
            this.MissionSettingsListView.Size = new System.Drawing.Size(770, 505);
            this.MissionSettingsListView.TabIndex = 0;
            this.MissionSettingsListView.UseCompatibleStateImageBehavior = false;
            this.MissionSettingsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MissionSettingsListView_MouseClick);
            // 
            // ValuesContextMenuStrip
            // 
            this.ValuesContextMenuStrip.Name = "ValuesContextMenuStrip";
            this.ValuesContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // OptionsTableLayoutPanel
            // 
            this.OptionsTableLayoutPanel.ColumnCount = 3;
            this.OptionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.OptionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.OptionsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.OptionsTableLayoutPanel.Controls.Add(this.ModsGroupBox, 1, 0);
            this.OptionsTableLayoutPanel.Controls.Add(this.MissionFeaturesGroupBox, 2, 0);
            this.OptionsTableLayoutPanel.Controls.Add(this.OptionsGroupBox, 0, 0);
            this.OptionsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.OptionsTableLayoutPanel.Name = "OptionsTableLayoutPanel";
            this.OptionsTableLayoutPanel.RowCount = 1;
            this.OptionsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OptionsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.OptionsTableLayoutPanel.Size = new System.Drawing.Size(770, 505);
            this.OptionsTableLayoutPanel.TabIndex = 0;
            // 
            // ModsGroupBox
            // 
            this.ModsGroupBox.Controls.Add(this.ModsTreeView);
            this.ModsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModsGroupBox.Location = new System.Drawing.Point(257, 3);
            this.ModsGroupBox.Name = "ModsGroupBox";
            this.ModsGroupBox.Size = new System.Drawing.Size(255, 499);
            this.ModsGroupBox.TabIndex = 0;
            this.ModsGroupBox.TabStop = false;
            this.ModsGroupBox.Text = "groupBox1";
            // 
            // MissionFeaturesGroupBox
            // 
            this.MissionFeaturesGroupBox.Controls.Add(this.MissionFeaturesTreeView);
            this.MissionFeaturesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MissionFeaturesGroupBox.Location = new System.Drawing.Point(518, 3);
            this.MissionFeaturesGroupBox.Name = "MissionFeaturesGroupBox";
            this.MissionFeaturesGroupBox.Size = new System.Drawing.Size(249, 499);
            this.MissionFeaturesGroupBox.TabIndex = 1;
            this.MissionFeaturesGroupBox.TabStop = false;
            this.MissionFeaturesGroupBox.Text = "groupBox2";
            // 
            // OptionsGroupBox
            // 
            this.OptionsGroupBox.Controls.Add(this.OptionsTreeView);
            this.OptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.OptionsGroupBox.Name = "OptionsGroupBox";
            this.OptionsGroupBox.Size = new System.Drawing.Size(248, 499);
            this.OptionsGroupBox.TabIndex = 2;
            this.OptionsGroupBox.TabStop = false;
            this.OptionsGroupBox.Text = "groupBox3";
            // 
            // MissionFeaturesTreeView
            // 
            this.MissionFeaturesTreeView.CheckBoxes = true;
            this.MissionFeaturesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MissionFeaturesTreeView.Location = new System.Drawing.Point(3, 16);
            this.MissionFeaturesTreeView.Name = "MissionFeaturesTreeView";
            this.MissionFeaturesTreeView.ShowLines = false;
            this.MissionFeaturesTreeView.ShowNodeToolTips = true;
            this.MissionFeaturesTreeView.ShowPlusMinus = false;
            this.MissionFeaturesTreeView.ShowRootLines = false;
            this.MissionFeaturesTreeView.Size = new System.Drawing.Size(243, 480);
            this.MissionFeaturesTreeView.TabIndex = 0;
            // 
            // ModsTreeView
            // 
            this.ModsTreeView.CheckBoxes = true;
            this.ModsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModsTreeView.Location = new System.Drawing.Point(3, 16);
            this.ModsTreeView.Name = "ModsTreeView";
            this.ModsTreeView.ShowLines = false;
            this.ModsTreeView.ShowNodeToolTips = true;
            this.ModsTreeView.ShowPlusMinus = false;
            this.ModsTreeView.ShowRootLines = false;
            this.ModsTreeView.Size = new System.Drawing.Size(249, 480);
            this.ModsTreeView.TabIndex = 0;
            // 
            // OptionsTreeView
            // 
            this.OptionsTreeView.CheckBoxes = true;
            this.OptionsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionsTreeView.Location = new System.Drawing.Point(3, 16);
            this.OptionsTreeView.Name = "OptionsTreeView";
            this.OptionsTreeView.ShowLines = false;
            this.OptionsTreeView.ShowNodeToolTips = true;
            this.OptionsTreeView.ShowPlusMinus = false;
            this.OptionsTreeView.ShowRootLines = false;
            this.OptionsTreeView.Size = new System.Drawing.Size(242, 480);
            this.OptionsTreeView.TabIndex = 0;
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
            // ObjectivesTabPage
            // 
            this.ObjectivesTabPage.Location = new System.Drawing.Point(4, 22);
            this.ObjectivesTabPage.Name = "ObjectivesTabPage";
            this.ObjectivesTabPage.Size = new System.Drawing.Size(776, 511);
            this.ObjectivesTabPage.TabIndex = 3;
            this.ObjectivesTabPage.Text = "Objectives";
            this.ObjectivesTabPage.UseVisualStyleBackColor = true;
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
            // FlightGroupsTabPage
            // 
            this.FlightGroupsTabPage.Location = new System.Drawing.Point(4, 22);
            this.FlightGroupsTabPage.Name = "FlightGroupsTabPage";
            this.FlightGroupsTabPage.Size = new System.Drawing.Size(776, 511);
            this.FlightGroupsTabPage.TabIndex = 5;
            this.FlightGroupsTabPage.Text = "Player flight groups";
            this.FlightGroupsTabPage.UseVisualStyleBackColor = true;
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
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormMenuStrip.ResumeLayout(false);
            this.FormMenuStrip.PerformLayout();
            this.MainTabControl.ResumeLayout(false);
            this.SettingsTabPage.ResumeLayout(false);
            this.OptionsTabPage.ResumeLayout(false);
            this.OptionsTableLayoutPanel.ResumeLayout(false);
            this.ModsGroupBox.ResumeLayout(false);
            this.MissionFeaturesGroupBox.ResumeLayout(false);
            this.OptionsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip FormMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage SettingsTabPage;
        private System.Windows.Forms.TabPage OptionsTabPage;
        private System.Windows.Forms.ListView MissionSettingsListView;
        private System.Windows.Forms.ContextMenuStrip ValuesContextMenuStrip;
        private System.Windows.Forms.TableLayoutPanel OptionsTableLayoutPanel;
        private System.Windows.Forms.GroupBox ModsGroupBox;
        private System.Windows.Forms.GroupBox MissionFeaturesGroupBox;
        private System.Windows.Forms.GroupBox OptionsGroupBox;
        private System.Windows.Forms.TreeView ModsTreeView;
        private System.Windows.Forms.TreeView MissionFeaturesTreeView;
        private System.Windows.Forms.TreeView OptionsTreeView;
        private System.Windows.Forms.TabPage ObjectivesTabPage;
        private System.Windows.Forms.TabPage FlightGroupsTabPage;
        private System.Windows.Forms.TabPage FeaturesTabPage;
        private System.Windows.Forms.TabPage BriefingTabPage;
    }
}