namespace BriefingRoom4DCSWorld.Forms
{
    partial class SplashScreenForm
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
            this.PanelSplashImage = new System.Windows.Forms.Panel();
            this.LabelVersion = new System.Windows.Forms.Label();
            this.CloseTimer = new System.Windows.Forms.Timer(this.components);
            this.LoadTimer = new System.Windows.Forms.Timer(this.components);
            this.PanelSplashImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelSplashImage
            // 
            this.PanelSplashImage.BackColor = System.Drawing.Color.Black;
            this.PanelSplashImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelSplashImage.Controls.Add(this.LabelVersion);
            this.PanelSplashImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelSplashImage.Location = new System.Drawing.Point(0, 0);
            this.PanelSplashImage.Name = "PanelSplashImage";
            this.PanelSplashImage.Size = new System.Drawing.Size(590, 270);
            this.PanelSplashImage.TabIndex = 7;
            // 
            // LabelVersion
            // 
            this.LabelVersion.BackColor = System.Drawing.Color.Transparent;
            this.LabelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(188)))), ((int)(((byte)(68)))));
            this.LabelVersion.Location = new System.Drawing.Point(0, 0);
            this.LabelVersion.Margin = new System.Windows.Forms.Padding(3);
            this.LabelVersion.Name = "LabelVersion";
            this.LabelVersion.Size = new System.Drawing.Size(590, 270);
            this.LabelVersion.TabIndex = 0;
            this.LabelVersion.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // CloseTimer
            // 
            this.CloseTimer.Interval = 1000;
            this.CloseTimer.Tick += new System.EventHandler(this.OnCloseTimerTick);
            // 
            // LoadTimer
            // 
            this.LoadTimer.Tick += new System.EventHandler(this.OnLoadTimerTick);
            // 
            // SplashScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 270);
            this.ControlBox = false;
            this.Controls.Add(this.PanelSplashImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashScreenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.SplashScreenForm_Load);
            this.PanelSplashImage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer CloseTimer;
        private System.Windows.Forms.Panel PanelSplashImage;
        private System.Windows.Forms.Label LabelVersion;
        private System.Windows.Forms.Timer LoadTimer;
    }
}