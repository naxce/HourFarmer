using FatumStyles;
using System.Drawing;
using System.Windows.Forms;

namespace HourFarmer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.Label labelStatus;
        public System.Windows.Forms.Label labelSelectGame;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            labelStatus = new Label();
            labelSelectGame = new Label();
            panelTitleBar = new Panel();
            btnMinimize = new PictureBox();
            btnClose = new PictureBox();
            logo = new PictureBox();
            label1 = new Label();
            buttonStart = new FatumButton();
            buttonStop = new FatumButton();
            comboBoxGames = new FatumComboBox();
            textBoxAppId = new FatumTextBox();
            checkBoxManualAppId = new FatumToggleButton();
            label2 = new Label();
            panelTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)btnMinimize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnClose).BeginInit();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            SuspendLayout();
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = FontLoader.GetFont(9F, FontStyle.Regular);
            labelStatus.ForeColor = Color.Silver;
            labelStatus.Location = new Point(18, 188);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(190, 15);
            labelStatus.TabIndex = 3;
            labelStatus.Text = "Status: Waiting for game selection.";
            // 
            // labelSelectGame
            // 
            labelSelectGame.AutoSize = true;
            labelSelectGame.Font = FontLoader.GetFont(9F, FontStyle.Bold);
            labelSelectGame.ForeColor = Color.White;
            labelSelectGame.Location = new Point(15, 68);
            labelSelectGame.Name = "labelSelectGame";
            labelSelectGame.Size = new Size(152, 15);
            labelSelectGame.TabIndex = 5;
            labelSelectGame.Text = "Choose the game (AppID):";
            // 
            // panelTitleBar
            // 
            panelTitleBar.BackColor = Color.FromArgb(16, 16, 36);
            panelTitleBar.Controls.Add(btnMinimize);
            panelTitleBar.Controls.Add(btnClose);
            panelTitleBar.Controls.Add(logo);
            panelTitleBar.Dock = DockStyle.Top;
            panelTitleBar.Location = new Point(0, 0);
            panelTitleBar.Name = "panelTitleBar";
            panelTitleBar.Size = new Size(424, 47);
            panelTitleBar.TabIndex = 6;
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;
            // 
            // btnMinimize
            // 
            btnMinimize.Image = Properties.Resources.Minimize;
            btnMinimize.Location = new Point(351, 8);
            btnMinimize.Name = "btnMinimize";
            btnMinimize.Size = new Size(30, 30);
            btnMinimize.SizeMode = PictureBoxSizeMode.StretchImage;
            btnMinimize.TabIndex = 3;
            btnMinimize.TabStop = false;
            btnMinimize.Click += btnMinimize_Click;
            btnMinimize.MouseEnter += btnMinimize_MouseEnter;
            btnMinimize.MouseLeave += btnMinimize_MouseLeave;
            // 
            // btnClose
            // 
            btnClose.Image = Properties.Resources.Close;
            btnClose.Location = new Point(387, 8);
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
            logo.Image = Properties.Resources.icon;
            logo.Location = new Point(197, 8);
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
            label1.Location = new Point(220, 216);
            label1.Name = "label1";
            label1.Size = new Size(189, 15);
            label1.TabIndex = 7;
            label1.Text = "© 2025 fatum. All Rights Reserved.";
            // 
            // buttonStart
            // 
            buttonStart.BackColor = Color.DarkTurquoise;
            buttonStart.BackgroundColor = Color.DarkTurquoise;
            buttonStart.BorderColor = Color.CadetBlue;
            buttonStart.BorderRadius = 15;
            buttonStart.BorderSize = 0;
            buttonStart.FlatAppearance.BorderSize = 0;
            buttonStart.FlatStyle = FlatStyle.Flat;
            buttonStart.Font = FontLoader.GetFont(12F, FontStyle.Regular);
            buttonStart.ForeColor = Color.White;
            buttonStart.Location = new Point(18, 135);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(187, 40);
            buttonStart.TabIndex = 8;
            buttonStart.Text = "Start";
            buttonStart.TextColor = Color.White;
            buttonStart.UseVisualStyleBackColor = false;
            buttonStart.Click += buttonStart_Click;
            // 
            // buttonStop
            // 
            buttonStop.BackColor = Color.FromArgb(36, 36, 56);
            buttonStop.BackgroundColor = Color.FromArgb(36, 36, 56);
            buttonStop.BorderColor = Color.CadetBlue;
            buttonStop.BorderRadius = 15;
            buttonStop.BorderSize = 0;
            buttonStop.Enabled = false;
            buttonStop.FlatAppearance.BorderSize = 0;
            buttonStop.FlatStyle = FlatStyle.Flat;
            buttonStop.Font = FontLoader.GetFont(12F, FontStyle.Regular);
            buttonStop.ForeColor = Color.White;
            buttonStop.Location = new Point(220, 135);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(187, 40);
            buttonStop.TabIndex = 9;
            buttonStop.Text = "Stop";
            buttonStop.TextColor = Color.White;
            buttonStop.UseVisualStyleBackColor = false;
            buttonStop.Click += btnClose_Click;
            // 
            // comboBoxGames
            // 
            comboBoxGames.AutoCompleteMode = AutoCompleteMode.None;
            comboBoxGames.AutoCompleteSource = AutoCompleteSource.None;
            comboBoxGames.BackColor = Color.FromArgb(36, 36, 56);
            comboBoxGames.BorderColor = Color.DarkTurquoise;
            comboBoxGames.BorderRadius = 10;
            comboBoxGames.BorderSize = 0;
            comboBoxGames.DataSource = null;
            comboBoxGames.DisplayMember = "";
            comboBoxGames.Font = FontLoader.GetFont(10F, FontStyle.Regular);
            comboBoxGames.ForeColor = Color.White;
            comboBoxGames.IconColor = Color.DarkTurquoise;
            comboBoxGames.ListBackColor = Color.FromArgb(26, 26, 46);
            comboBoxGames.ListTextColor = Color.White;
            comboBoxGames.Location = new Point(18, 95);
            comboBoxGames.MinimumSize = new Size(200, 30);
            comboBoxGames.Name = "comboBoxGames";
            comboBoxGames.SelectedIndex = -1;
            comboBoxGames.SelectedItem = null;
            comboBoxGames.Size = new Size(200, 30);
            comboBoxGames.TabIndex = 10;
            comboBoxGames.ValueMember = "";
            // 
            // textBoxAppId
            // 
            textBoxAppId.BackColor = Color.FromArgb(36, 36, 56);
            textBoxAppId.BorderColor = Color.FromArgb(36, 36, 56);
            textBoxAppId.BorderFocusColor = Color.DarkTurquoise;
            textBoxAppId.BorderRadius = 5;
            textBoxAppId.BorderSize = 2;
            textBoxAppId.Font = FontLoader.GetFont(9.5F, FontStyle.Regular);
            textBoxAppId.ForeColor = Color.White;
            textBoxAppId.Location = new Point(220, 94);
            textBoxAppId.Margin = new Padding(4);
            textBoxAppId.Multiline = false;
            textBoxAppId.Name = "textBoxAppId";
            textBoxAppId.Padding = new Padding(10, 7, 10, 7);
            textBoxAppId.PasswordChar = false;
            textBoxAppId.PlaceholderColor = Color.DimGray;
            textBoxAppId.PlaceholderText = "Enter AppID (e.g. 730)";
            textBoxAppId.Size = new Size(187, 25);
            textBoxAppId.TabIndex = 11;
            textBoxAppId.Texts = "";
            textBoxAppId.UnderlinedStyle = false;
            // 
            // checkBoxManualAppId
            // 
            checkBoxManualAppId.AutoSize = true;
            checkBoxManualAppId.Location = new Point(364, 65);
            checkBoxManualAppId.MinimumSize = new Size(45, 22);
            checkBoxManualAppId.Name = "checkBoxManualAppId";
            checkBoxManualAppId.OffBackColor = Color.Gray;
            checkBoxManualAppId.OffToggleColor = Color.Gainsboro;
            checkBoxManualAppId.OnBackColor = Color.DarkTurquoise;
            checkBoxManualAppId.OnToggleColor = Color.WhiteSmoke;
            checkBoxManualAppId.Size = new Size(45, 22);
            checkBoxManualAppId.TabIndex = 12;
            checkBoxManualAppId.UseVisualStyleBackColor = true;
            checkBoxManualAppId.CheckedChanged += checkBoxManualAppId_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = FontLoader.GetFont(9F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(262, 66);
            label2.Name = "label2";
            label2.Size = new Size(87, 15);
            label2.TabIndex = 13;
            label2.Text = "Custom AppID";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 46);
            ClientSize = new Size(424, 234);
            Controls.Add(textBoxAppId);
            Controls.Add(label2);
            Controls.Add(checkBoxManualAppId);
            Controls.Add(comboBoxGames);
            Controls.Add(buttonStop);
            Controls.Add(buttonStart);
            Controls.Add(label1);
            Controls.Add(panelTitleBar);
            Controls.Add(labelSelectGame);
            Controls.Add(labelStatus);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hour Farmer";
            Load += Form1_Load;
            panelTitleBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)btnMinimize).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnClose).EndInit();
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        public Label label1;
        private FatumButton buttonStart;
        private FatumButton buttonStop;
        private PictureBox logo;
        private FatumComboBox comboBoxGames;
        private PictureBox btnClose;
        private PictureBox btnMinimize;
        private FatumTextBox textBoxAppId;
        private FatumToggleButton checkBoxManualAppId;
        public Label label2;
    }
}