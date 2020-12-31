namespace BriefingRoom4DCSWorld.Forms
{
    partial class CampaignCreatorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CampaignCreatorForm));
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TemplatePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.CampaignToolStripMenu = new System.Windows.Forms.ToolStrip();
            this.T_New = new System.Windows.Forms.ToolStripButton();
            this.T_Open = new System.Windows.Forms.ToolStripButton();
            this.T_SaveAs = new System.Windows.Forms.ToolStripButton();
            this.T_s1 = new System.Windows.Forms.ToolStripSeparator();
            this.T_ExportCampaign = new System.Windows.Forms.ToolStripButton();
            this.T_s4 = new System.Windows.Forms.ToolStripSeparator();
            this.T_Close = new System.Windows.Forms.ToolStripButton();
            this.T_s2 = new System.Windows.Forms.ToolStripSeparator();
            this.T_s3 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.CampaignToolStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.TemplatePropertyGrid);
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.CampaignToolStripMenu);
            this.MainSplitContainer.Size = new System.Drawing.Size(624, 441);
            this.MainSplitContainer.SplitterDistance = 437;
            this.MainSplitContainer.TabIndex = 1;
            // 
            // TemplatePropertyGrid
            // 
            this.TemplatePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TemplatePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.TemplatePropertyGrid.Name = "TemplatePropertyGrid";
            this.TemplatePropertyGrid.Size = new System.Drawing.Size(437, 441);
            this.TemplatePropertyGrid.TabIndex = 1;
            this.TemplatePropertyGrid.ToolbarVisible = false;
            // 
            // CampaignToolStripMenu
            // 
            this.CampaignToolStripMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CampaignToolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.CampaignToolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.T_s1,
            this.T_New,
            this.T_Open,
            this.T_SaveAs,
            this.T_s2,
            this.T_ExportCampaign,
            this.T_s3,
            this.T_Close,
            this.T_s4});
            this.CampaignToolStripMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.CampaignToolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.CampaignToolStripMenu.Name = "CampaignToolStripMenu";
            this.CampaignToolStripMenu.Size = new System.Drawing.Size(183, 441);
            this.CampaignToolStripMenu.TabIndex = 4;
            this.CampaignToolStripMenu.Text = "toolStrip1";
            // 
            // T_New
            // 
            this.T_New.Image = ((System.Drawing.Image)(resources.GetObject("T_New.Image")));
            this.T_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_New.Name = "T_New";
            this.T_New.Size = new System.Drawing.Size(181, 35);
            this.T_New.Text = "New";
            this.T_New.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.T_New.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // T_Open
            // 
            this.T_Open.Image = ((System.Drawing.Image)(resources.GetObject("T_Open.Image")));
            this.T_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Open.Name = "T_Open";
            this.T_Open.Size = new System.Drawing.Size(181, 35);
            this.T_Open.Text = "Open";
            this.T_Open.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.T_Open.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // T_SaveAs
            // 
            this.T_SaveAs.Image = ((System.Drawing.Image)(resources.GetObject("T_SaveAs.Image")));
            this.T_SaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_SaveAs.Name = "T_SaveAs";
            this.T_SaveAs.Size = new System.Drawing.Size(181, 35);
            this.T_SaveAs.Text = "Save as...";
            this.T_SaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.T_SaveAs.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // T_s1
            // 
            this.T_s1.Name = "T_s1";
            this.T_s1.Size = new System.Drawing.Size(181, 6);
            // 
            // T_ExportCampaign
            // 
            this.T_ExportCampaign.Image = ((System.Drawing.Image)(resources.GetObject("T_ExportCampaign.Image")));
            this.T_ExportCampaign.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_ExportCampaign.Name = "T_ExportCampaign";
            this.T_ExportCampaign.Size = new System.Drawing.Size(181, 35);
            this.T_ExportCampaign.Text = "Export campaign";
            this.T_ExportCampaign.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.T_ExportCampaign.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // T_s4
            // 
            this.T_s4.Name = "T_s4";
            this.T_s4.Size = new System.Drawing.Size(181, 6);
            // 
            // T_Close
            // 
            this.T_Close.Image = ((System.Drawing.Image)(resources.GetObject("T_Close.Image")));
            this.T_Close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T_Close.Name = "T_Close";
            this.T_Close.Size = new System.Drawing.Size(181, 35);
            this.T_Close.Text = "Close campaign creator";
            this.T_Close.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.T_Close.Click += new System.EventHandler(this.OnMenuClick);
            // 
            // T_s2
            // 
            this.T_s2.Name = "T_s2";
            this.T_s2.Size = new System.Drawing.Size(181, 6);
            // 
            // T_s3
            // 
            this.T_s3.Name = "T_s3";
            this.T_s3.Size = new System.Drawing.Size(181, 6);
            // 
            // CampaignCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.MainSplitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CampaignCreatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Campaign creator";
            this.Load += new System.EventHandler(this.CampaignCreatorForm_Load);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            this.MainSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.CampaignToolStripMenu.ResumeLayout(false);
            this.CampaignToolStripMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.PropertyGrid TemplatePropertyGrid;
        private System.Windows.Forms.ToolStrip CampaignToolStripMenu;
        private System.Windows.Forms.ToolStripButton T_New;
        private System.Windows.Forms.ToolStripButton T_Open;
        private System.Windows.Forms.ToolStripButton T_SaveAs;
        private System.Windows.Forms.ToolStripSeparator T_s1;
        private System.Windows.Forms.ToolStripButton T_ExportCampaign;
        private System.Windows.Forms.ToolStripSeparator T_s4;
        private System.Windows.Forms.ToolStripButton T_Close;
        private System.Windows.Forms.ToolStripSeparator T_s2;
        private System.Windows.Forms.ToolStripSeparator T_s3;
    }
}