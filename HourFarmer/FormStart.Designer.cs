using FatumStyles;
using System.Drawing;

namespace HourFarmer
{
    partial class FormStart
    {
        private System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.Panel panelTitleBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                FontLoader.ClearResource();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            FontLoader.LoadCustomFont();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
            panelTitleBar = new Panel();
            btnClose = new PictureBox();
            logo = new PictureBox();
            label1 = new Label();
            versionLabel = new Label();
            buttonDownload = new FatumButton();
            panelTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)btnClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            SuspendLayout();
            // 
            // panelTitleBar
            // 
            panelTitleBar.BackColor = Color.FromArgb(31, 31, 51);
            panelTitleBar.Controls.Add(btnClose);
            panelTitleBar.Controls.Add(logo);
            panelTitleBar.Dock = DockStyle.Top;
            panelTitleBar.Location = new Point(0, 0);
            panelTitleBar.Name = "panelTitleBar";
            panelTitleBar.Size = new Size(400, 47);
            panelTitleBar.TabIndex = 6;
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;
            // 
            // btnClose
            // 
            btnClose.Image = Properties.Resources.PurpleClose;
            btnClose.Location = new Point(358, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(30, 30);
            btnClose.SizeMode = PictureBoxSizeMode.StretchImage;
            btnClose.TabIndex = 2;
            btnClose.TabStop = false;
            btnClose.Click += btnClose_Click;
            btnClose.MouseEnter += btnClose_MouseEnter;
            btnClose.MouseLeave += btnClose_MouseLeave;
            // 
            // logo
            // 
            logo.Image = Properties.Resources.fatumlogo;
            logo.Location = new Point(185, 8);
            logo.Name = "logo";
            logo.Size = new Size(30, 30);
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            logo.TabIndex = 1;
            logo.TabStop = false;
            logo.MouseDown += panelTitleBar_MouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = FontLoader.GetFont(9F, FontStyle.Regular);
            label1.ForeColor = Color.Silver;
            label1.Location = new Point(96, 383);
            label1.Name = "label1";
            label1.Size = new Size(189, 15);
            label1.TabIndex = 7;
            label1.Text = "© 2025 fatum. All Rights Reserved.";
            // 
            // versionLabel
            // 
            versionLabel.Font = FontLoader.GetFont(15.75F, FontStyle.Regular);
            versionLabel.ForeColor = Color.White;
            versionLabel.Location = new Point(50, 133);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(300, 135);
            versionLabel.TabIndex = 9;
            versionLabel.Text = "versionLabel";
            versionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonDownload
            // 
            buttonDownload.BackColor = Color.FromArgb(145, 0, 255);
            buttonDownload.BackgroundColor = Color.FromArgb(145, 0, 255);
            buttonDownload.BorderColor = Color.CadetBlue;
            buttonDownload.BorderRadius = 15;
            buttonDownload.BorderSize = 0;
            buttonDownload.FlatAppearance.BorderSize = 0;
            buttonDownload.FlatStyle = FlatStyle.Flat;
            buttonDownload.Font = FontLoader.GetFont(12F, FontStyle.Regular);
            buttonDownload.ForeColor = Color.White;
            buttonDownload.Location = new Point(100, 303);
            buttonDownload.Name = "buttonDownload";
            buttonDownload.Size = new Size(201, 66);
            buttonDownload.TabIndex = 10;
            buttonDownload.Text = "Download a new version";
            buttonDownload.TextColor = Color.White;
            buttonDownload.UseVisualStyleBackColor = false;
            buttonDownload.Click += downloadButton_Click;
            // 
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 46);
            ClientSize = new Size(400, 400);
            Controls.Add(versionLabel);
            Controls.Add(label1);
            Controls.Add(panelTitleBar);
            Controls.Add(buttonDownload);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormStart";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fatum Initialize";
            Load += FormStart_Load;
            panelTitleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)btnClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox logo;
        public System.Windows.Forms.Label versionLabel;
        private FatumButton buttonDownload;
        public PictureBox btnClose;
    }
}