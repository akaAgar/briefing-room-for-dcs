namespace BriefingRoom4DCS.WindowsTool.Forms
{
    partial class OptionsTabForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MissionOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.MissionOptionsTreeView = new System.Windows.Forms.TreeView();
            this.RealismOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.RealismOptionsTreeView = new System.Windows.Forms.TreeView();
            this.ModsGroupBox = new System.Windows.Forms.GroupBox();
            this.ModsTreeView = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1.SuspendLayout();
            this.MissionOptionsGroupBox.SuspendLayout();
            this.RealismOptionsGroupBox.SuspendLayout();
            this.ModsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.MissionOptionsGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.RealismOptionsGroupBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ModsGroupBox, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 441);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // MissionOptionsGroupBox
            // 
            this.MissionOptionsGroupBox.Controls.Add(this.MissionOptionsTreeView);
            this.MissionOptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MissionOptionsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.MissionOptionsGroupBox.Name = "MissionOptionsGroupBox";
            this.MissionOptionsGroupBox.Size = new System.Drawing.Size(199, 435);
            this.MissionOptionsGroupBox.TabIndex = 0;
            this.MissionOptionsGroupBox.TabStop = false;
            this.MissionOptionsGroupBox.Text = "Mission options";
            // 
            // MissionOptionsTreeView
            // 
            this.MissionOptionsTreeView.CheckBoxes = true;
            this.MissionOptionsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MissionOptionsTreeView.Location = new System.Drawing.Point(3, 16);
            this.MissionOptionsTreeView.Name = "MissionOptionsTreeView";
            this.MissionOptionsTreeView.ShowLines = false;
            this.MissionOptionsTreeView.ShowNodeToolTips = true;
            this.MissionOptionsTreeView.ShowPlusMinus = false;
            this.MissionOptionsTreeView.ShowRootLines = false;
            this.MissionOptionsTreeView.Size = new System.Drawing.Size(193, 416);
            this.MissionOptionsTreeView.TabIndex = 0;
            this.MissionOptionsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeNodeChecked);
            // 
            // RealismOptionsGroupBox
            // 
            this.RealismOptionsGroupBox.Controls.Add(this.RealismOptionsTreeView);
            this.RealismOptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RealismOptionsGroupBox.Location = new System.Drawing.Point(208, 3);
            this.RealismOptionsGroupBox.Name = "RealismOptionsGroupBox";
            this.RealismOptionsGroupBox.Size = new System.Drawing.Size(206, 435);
            this.RealismOptionsGroupBox.TabIndex = 1;
            this.RealismOptionsGroupBox.TabStop = false;
            this.RealismOptionsGroupBox.Text = "Realism options";
            // 
            // RealismOptionsTreeView
            // 
            this.RealismOptionsTreeView.CheckBoxes = true;
            this.RealismOptionsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RealismOptionsTreeView.Location = new System.Drawing.Point(3, 16);
            this.RealismOptionsTreeView.Name = "RealismOptionsTreeView";
            this.RealismOptionsTreeView.ShowLines = false;
            this.RealismOptionsTreeView.ShowNodeToolTips = true;
            this.RealismOptionsTreeView.ShowPlusMinus = false;
            this.RealismOptionsTreeView.ShowRootLines = false;
            this.RealismOptionsTreeView.Size = new System.Drawing.Size(200, 416);
            this.RealismOptionsTreeView.TabIndex = 0;
            this.RealismOptionsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeNodeChecked);
            // 
            // ModsGroupBox
            // 
            this.ModsGroupBox.Controls.Add(this.ModsTreeView);
            this.ModsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModsGroupBox.Location = new System.Drawing.Point(420, 3);
            this.ModsGroupBox.Name = "ModsGroupBox";
            this.ModsGroupBox.Size = new System.Drawing.Size(201, 435);
            this.ModsGroupBox.TabIndex = 2;
            this.ModsGroupBox.TabStop = false;
            this.ModsGroupBox.Text = "DCS World Mods";
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
            this.ModsTreeView.Size = new System.Drawing.Size(195, 416);
            this.ModsTreeView.TabIndex = 0;
            this.ModsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeNodeChecked);
            // 
            // OptionsTabForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsTabForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.OptionsTabForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.MissionOptionsGroupBox.ResumeLayout(false);
            this.RealismOptionsGroupBox.ResumeLayout(false);
            this.ModsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox MissionOptionsGroupBox;
        private System.Windows.Forms.GroupBox RealismOptionsGroupBox;
        private System.Windows.Forms.GroupBox ModsGroupBox;
        private System.Windows.Forms.TreeView MissionOptionsTreeView;
        private System.Windows.Forms.TreeView RealismOptionsTreeView;
        private System.Windows.Forms.TreeView ModsTreeView;
    }
}