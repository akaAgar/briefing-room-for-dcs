namespace BriefingRoom4DCS.WindowsTool.Forms
{
    partial class SettingsTabForm
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
            this.SettingsListView = new System.Windows.Forms.ListView();
            this.SettingsListViewNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SettingsListViewValueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SettingsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // SettingsListView
            // 
            this.SettingsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.SettingsListViewNameColumn,
            this.SettingsListViewValueColumn});
            this.SettingsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsListView.HideSelection = false;
            this.SettingsListView.Location = new System.Drawing.Point(0, 0);
            this.SettingsListView.MultiSelect = false;
            this.SettingsListView.Name = "SettingsListView";
            this.SettingsListView.Size = new System.Drawing.Size(624, 441);
            this.SettingsListView.TabIndex = 1;
            this.SettingsListView.UseCompatibleStateImageBehavior = false;
            this.SettingsListView.View = System.Windows.Forms.View.Tile;
            this.SettingsListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnSettingsListViewMouseClick);
            // 
            // SettingsContextMenuStrip
            // 
            this.SettingsContextMenuStrip.Name = "SettingsContextMenuStrip";
            this.SettingsContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            this.SettingsContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.OnSettingsContextMenuStripItemClicked);
            // 
            // SettingsTabForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.ControlBox = false;
            this.Controls.Add(this.SettingsListView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsTabForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.SettingsTabForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView SettingsListView;
        private System.Windows.Forms.ColumnHeader SettingsListViewNameColumn;
        private System.Windows.Forms.ColumnHeader SettingsListViewValueColumn;
        private System.Windows.Forms.ContextMenuStrip SettingsContextMenuStrip;
    }
}