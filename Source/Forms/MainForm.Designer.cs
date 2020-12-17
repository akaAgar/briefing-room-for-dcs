namespace BriefingRoom4DCSWorld.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuStripMain = new System.Windows.Forms.MenuStrip();
            this.M_File = new System.Windows.Forms.ToolStripMenuItem();
            this.M_File_New = new System.Windows.Forms.ToolStripMenuItem();
            this.M_File_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.M_File_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.M_File_s1 = new System.Windows.Forms.ToolStripSeparator();
            this.M_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Mission = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Mission_Generate = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Mission_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.M_Mission_ExportBriefing = new System.Windows.Forms.ToolStripMenuItem();
            this.M_About = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMain = new System.Windows.Forms.ToolStrip();
            this.T_File_New = new System.Windows.Forms.ToolStripButton();
            this.T_File_Open = new System.Windows.Forms.ToolStripButton();
            this.T_File_SaveAs = new System.Windows.Forms.ToolStripButton();
            this.T_s1 = new System.Windows.Forms.ToolStripSeparator();
            this.T_Mission_Generate = new System.Windows.Forms.ToolStripButton();
            this.T_Mission_Export = new System.Windows.Forms.ToolStripButton();
            this.T_Mission_ExportBriefing = new System.Windows.Forms.ToolStripButton();
            this.T_Debug_Export = new System.Windows.Forms.ToolStripButton();
            this.BottomStatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TemplatePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.BriefingPanel = new System.Windows.Forms.Panel();
            this.BriefingWebBrowser = new System.Windows.Forms.WebBrowser();
            this.MenuStripMain.SuspendLayout();
            this.ToolStripMain.SuspendLayout();
            this.BottomStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.BriefingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStripMain
            // 
            this.MenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.M_File,
            this.M_Mission,
            this.M_About});
            this.MenuStripMain.Location = new System.Drawing.Point(0, 0);
            this.MenuStripMain.Name = "MenuStripMain";
            this.MenuStripMain.Size = new System.Drawing.Size(784, 24);
            this.MenuStripMain.TabIndex = 0;
            this.MenuStripMain.Text = "menuStrip1";
            // 
            // M_File
            // 
            this.M_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.M_File_New,
            this.M_File_Open,
            this.M_File_SaveAs,
            this.M_File_s1,
            this.M_File_Exit});
            this.M_File.Name = "M_File";
            this.M_File.Size = new System.Drawing.Size(37, 20);
            this.M_File.Text = "&File";
            // 
            // M_File_New
            // 
            this.M_File_New.Name = "M_File_New";
            this.M_File_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.M_File_New.Size = new System.Drawing.Size(211, 22);
            this.M_File_New.Text = "&New template";
            this.M_File_New.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_File_Open
            // 
            this.M_File_Open.Name = "M_File_Open";
            this.M_File_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.M_File_Open.Size = new System.Drawing.Size(211, 22);
            this.M_File_Open.Text = "&Open template";
            this.M_File_Open.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_File_SaveAs
            // 
            this.M_File_SaveAs.Name = "M_File_SaveAs";
            this.M_File_SaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.M_File_SaveAs.Size = new System.Drawing.Size(211, 22);
            this.M_File_SaveAs.Text = "&Save template as...";
            this.M_File_SaveAs.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_File_s1
            // 
            this.M_File_s1.Name = "M_File_s1";
            this.M_File_s1.Size = new System.Drawing.Size(208, 6);
            // 
            // M_File_Exit
            // 
            this.M_File_Exit.Name = "M_File_Exit";
            this.M_File_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.M_File_Exit.Size = new System.Drawing.Size(211, 22);
            this.M_File_Exit.Text = "E&xit";
            this.M_File_Exit.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_Mission
            // 
            this.M_Mission.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.M_Mission_Generate,
            this.M_Mission_Export,
            this.M_Mission_ExportBriefing});
            this.M_Mission.Name = "M_Mission";
            this.M_Mission.Size = new System.Drawing.Size(60, 20);
            this.M_Mission.Text = "&Mission";
            // 
            // M_Mission_Generate
            // 
            this.M_Mission_Generate.Name = "M_Mission_Generate";
            this.M_Mission_Generate.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.M_Mission_Generate.Size = new System.Drawing.Size(242, 22);
            this.M_Mission_Generate.Text = "&Generate another";
            this.M_Mission_Generate.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_Mission_Export
            // 
            this.M_Mission_Export.Name = "M_Mission_Export";
            this.M_Mission_Export.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.M_Mission_Export.Size = new System.Drawing.Size(242, 22);
            this.M_Mission_Export.Text = "&Export to .miz file";
            this.M_Mission_Export.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_Mission_ExportBriefing
            // 
            this.M_Mission_ExportBriefing.Name = "M_Mission_ExportBriefing";
            this.M_Mission_ExportBriefing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.M_Mission_ExportBriefing.Size = new System.Drawing.Size(242, 22);
            this.M_Mission_ExportBriefing.Text = "Export &briefing to HTML";
            this.M_Mission_ExportBriefing.Click += new System.EventHandler(this.MenuClick);
            // 
            // M_About
            // 
            this.M_About.Name = "M_About";
            this.M_About.Size = new System.Drawing.Size(52, 20);
            this.M_About.Text = "&About";
            this.M_About.Click += new System.EventHandler(this.MenuClick);
            // 
            // ToolStripMain
            // 
            this.ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.T_File_New,
            this.T_File_Open,
            this.T_File_SaveAs,
            this.T_s1,
            this.T_Mission_Generate,
            this.T_Mission_Export,
            this.T_Mission_ExportBriefing,
            this.T_Debug_Export});
            this.ToolStripMain.Location = new System.Drawing.Point(0, 24);
            this.ToolStripMain.Name = "ToolStripMain";
            this.ToolStripMain.Size = new System.Drawing.Size(784, 25);
            this.ToolStripMain.TabIndex = 1;
            this.ToolStripMain.Text = "toolStrip1";
            // 
            // T_File_New
            // 
            this.T_File_New.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_File_New.Image = ((System.Drawing.Image)(resources.GetObject("T_File_New.Image")));
            this.T_File_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_File_New.Name = "T_File_New";
            this.T_File_New.Size = new System.Drawing.Size(23, 22);
            this.T_File_New.Text = "New template (Ctrl+N)";
            this.T_File_New.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_File_Open
            // 
            this.T_File_Open.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_File_Open.Image = ((System.Drawing.Image)(resources.GetObject("T_File_Open.Image")));
            this.T_File_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_File_Open.Name = "T_File_Open";
            this.T_File_Open.Size = new System.Drawing.Size(23, 22);
            this.T_File_Open.Text = "Open template (Ctrl+O)";
            this.T_File_Open.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_File_SaveAs
            // 
            this.T_File_SaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_File_SaveAs.Image = ((System.Drawing.Image)(resources.GetObject("T_File_SaveAs.Image")));
            this.T_File_SaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_File_SaveAs.Name = "T_File_SaveAs";
            this.T_File_SaveAs.Size = new System.Drawing.Size(23, 22);
            this.T_File_SaveAs.Text = "Save template as... (Ctrl+S)";
            this.T_File_SaveAs.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_s1
            // 
            this.T_s1.Name = "T_s1";
            this.T_s1.Size = new System.Drawing.Size(6, 25);
            // 
            // T_Mission_Generate
            // 
            this.T_Mission_Generate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_Mission_Generate.Image = ((System.Drawing.Image)(resources.GetObject("T_Mission_Generate.Image")));
            this.T_Mission_Generate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Mission_Generate.Name = "T_Mission_Generate";
            this.T_Mission_Generate.Size = new System.Drawing.Size(23, 22);
            this.T_Mission_Generate.Text = "Generate another (F5)";
            this.T_Mission_Generate.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_Mission_Export
            // 
            this.T_Mission_Export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_Mission_Export.Image = ((System.Drawing.Image)(resources.GetObject("T_Mission_Export.Image")));
            this.T_Mission_Export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Mission_Export.Name = "T_Mission_Export";
            this.T_Mission_Export.Size = new System.Drawing.Size(23, 22);
            this.T_Mission_Export.Text = "Export to .miz file (Ctrl+E)";
            this.T_Mission_Export.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_Mission_ExportBriefing
            // 
            this.T_Mission_ExportBriefing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_Mission_ExportBriefing.Image = ((System.Drawing.Image)(resources.GetObject("T_Mission_ExportBriefing.Image")));
            this.T_Mission_ExportBriefing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Mission_ExportBriefing.Name = "T_Mission_ExportBriefing";
            this.T_Mission_ExportBriefing.Size = new System.Drawing.Size(23, 22);
            this.T_Mission_ExportBriefing.Text = "Export &briefing to HTML (Ctrl+B)";
            this.T_Mission_ExportBriefing.Click += new System.EventHandler(this.MenuClick);
            // 
            // T_Debug_Export
            // 
            this.T_Debug_Export.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T_Debug_Export.Image = ((System.Drawing.Image)(resources.GetObject("T_Debug_Export.Image")));
            this.T_Debug_Export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Debug_Export.Name = "T_Debug_Export";
            this.T_Debug_Export.Size = new System.Drawing.Size(23, 22);
            this.T_Debug_Export.Text = "Export .miz content to DebugOutput folder for debugging";
            this.T_Debug_Export.Visible = false;
            this.T_Debug_Export.Click += new System.EventHandler(this.MenuClick);
            // 
            // BottomStatusStrip
            // 
            this.BottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.BottomStatusStrip.Location = new System.Drawing.Point(0, 539);
            this.BottomStatusStrip.Name = "BottomStatusStrip";
            this.BottomStatusStrip.Size = new System.Drawing.Size(784, 22);
            this.BottomStatusStrip.TabIndex = 2;
            this.BottomStatusStrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitContainer.Location = new System.Drawing.Point(0, 49);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.TemplatePropertyGrid);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.BriefingPanel);
            this.MainSplitContainer.Size = new System.Drawing.Size(784, 490);
            this.MainSplitContainer.SplitterDistance = 326;
            this.MainSplitContainer.TabIndex = 4;
            // 
            // TemplatePropertyGrid
            // 
            this.TemplatePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TemplatePropertyGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TemplatePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.TemplatePropertyGrid.Name = "TemplatePropertyGrid";
            this.TemplatePropertyGrid.Size = new System.Drawing.Size(326, 490);
            this.TemplatePropertyGrid.TabIndex = 0;
            this.TemplatePropertyGrid.ToolbarVisible = false;
            this.TemplatePropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.TemplatePropertyGrid_PropertyValueChanged);
            // 
            // BriefingPanel
            // 
            this.BriefingPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BriefingPanel.Controls.Add(this.BriefingWebBrowser);
            this.BriefingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BriefingPanel.Location = new System.Drawing.Point(0, 0);
            this.BriefingPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BriefingPanel.Name = "BriefingPanel";
            this.BriefingPanel.Size = new System.Drawing.Size(454, 490);
            this.BriefingPanel.TabIndex = 2;
            // 
            // BriefingWebBrowser
            // 
            this.BriefingWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BriefingWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.BriefingWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.BriefingWebBrowser.Name = "BriefingWebBrowser";
            this.BriefingWebBrowser.Size = new System.Drawing.Size(450, 486);
            this.BriefingWebBrowser.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.MainSplitContainer);
            this.Controls.Add(this.BottomStatusStrip);
            this.Controls.Add(this.ToolStripMain);
            this.Controls.Add(this.MenuStripMain);
            this.MainMenuStrip = this.MenuStripMain;
            this.MinimumSize = new System.Drawing.Size(604, 495);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BriefingRoom for DCS World";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MenuStripMain.ResumeLayout(false);
            this.MenuStripMain.PerformLayout();
            this.ToolStripMain.ResumeLayout(false);
            this.ToolStripMain.PerformLayout();
            this.BottomStatusStrip.ResumeLayout(false);
            this.BottomStatusStrip.PerformLayout();
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.BriefingPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStripMain;
        private System.Windows.Forms.ToolStrip ToolStripMain;
        private System.Windows.Forms.ToolStripMenuItem M_File;
        private System.Windows.Forms.ToolStripMenuItem M_File_New;
        private System.Windows.Forms.ToolStripMenuItem M_File_Open;
        private System.Windows.Forms.ToolStripMenuItem M_File_SaveAs;
        private System.Windows.Forms.ToolStripSeparator M_File_s1;
        private System.Windows.Forms.ToolStripMenuItem M_File_Exit;
        private System.Windows.Forms.ToolStripButton T_File_New;
        private System.Windows.Forms.ToolStripButton T_File_Open;
        private System.Windows.Forms.ToolStripButton T_File_SaveAs;
        private System.Windows.Forms.ToolStripSeparator T_s1;
        private System.Windows.Forms.ToolStripButton T_Mission_Generate;
        private System.Windows.Forms.ToolStripButton T_Mission_Export;
        private System.Windows.Forms.ToolStripMenuItem M_Mission;
        private System.Windows.Forms.ToolStripMenuItem M_Mission_Generate;
        private System.Windows.Forms.ToolStripMenuItem M_Mission_Export;
        private System.Windows.Forms.ToolStripMenuItem M_Mission_ExportBriefing;
        private System.Windows.Forms.ToolStripButton T_Mission_ExportBriefing;
        private System.Windows.Forms.StatusStrip BottomStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.PropertyGrid TemplatePropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem M_About;
        private System.Windows.Forms.Panel BriefingPanel;
        private System.Windows.Forms.WebBrowser BriefingWebBrowser;
        private System.Windows.Forms.ToolStripButton T_Debug_Export;
    }
}