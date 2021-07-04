namespace BriefingRoom4DCS.WindowsTool.Forms
{
    partial class ConsoleForm
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
            this.PaddingPanel = new System.Windows.Forms.Panel();
            this.OutputListBox = new System.Windows.Forms.ListBox();
            this.PaddingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // PaddingPanel
            // 
            this.PaddingPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(32)))), ((int)(((byte)(43)))));
            this.PaddingPanel.Controls.Add(this.OutputListBox);
            this.PaddingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaddingPanel.ForeColor = System.Drawing.Color.White;
            this.PaddingPanel.Location = new System.Drawing.Point(0, 0);
            this.PaddingPanel.Name = "PaddingPanel";
            this.PaddingPanel.Padding = new System.Windows.Forms.Padding(8);
            this.PaddingPanel.Size = new System.Drawing.Size(464, 561);
            this.PaddingPanel.TabIndex = 0;
            // 
            // OutputListBox
            // 
            this.OutputListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(32)))), ((int)(((byte)(43)))));
            this.OutputListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.OutputListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputListBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputListBox.ForeColor = System.Drawing.Color.White;
            this.OutputListBox.FormattingEnabled = true;
            this.OutputListBox.ItemHeight = 16;
            this.OutputListBox.Location = new System.Drawing.Point(8, 8);
            this.OutputListBox.Name = "OutputListBox";
            this.OutputListBox.Size = new System.Drawing.Size(448, 545);
            this.OutputListBox.TabIndex = 1;
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(464, 561);
            this.ControlBox = false;
            this.Controls.Add(this.PaddingPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConsoleForm";
            this.ShowInTaskbar = false;
            this.Text = "BriefingRoom console output";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.PaddingPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PaddingPanel;
        private System.Windows.Forms.ListBox OutputListBox;
    }
}