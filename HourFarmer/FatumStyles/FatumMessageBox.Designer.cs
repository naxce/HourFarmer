namespace FatumStyles
{
    partial class FatumMessageBox
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelMessageContent;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                FontLoader.ClearResource();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region

        private void InitializeComponent()
        {
            FontLoader.LoadCustomFont();

            labelMessageContent = new System.Windows.Forms.Label();
            buttonOK = new FatumButton();
            logo = new System.Windows.Forms.PictureBox();
            titleLabel = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // labelMessageContent
            // 
            labelMessageContent.Font = FontLoader.GetFont(9F, System.Drawing.FontStyle.Regular);
            labelMessageContent.ForeColor = System.Drawing.Color.White;
            labelMessageContent.Location = new System.Drawing.Point(12, 64);
            labelMessageContent.Name = "labelMessageContent";
            labelMessageContent.Size = new System.Drawing.Size(350, 70);
            labelMessageContent.TabIndex = 0;
            labelMessageContent.Text = "labelMessageContent";
            // 
            // buttonOK
            // 
            buttonOK.BackColor = System.Drawing.Color.FromArgb(145, 0, 255);
            buttonOK.BackgroundColor = System.Drawing.Color.FromArgb(145, 0, 255);
            buttonOK.BorderColor = System.Drawing.Color.CadetBlue;
            buttonOK.BorderRadius = 15;
            buttonOK.BorderSize = 0;
            buttonOK.FlatAppearance.BorderSize = 0;
            buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonOK.Font = FontLoader.GetFont(12F, System.Drawing.FontStyle.Regular);
            buttonOK.ForeColor = System.Drawing.Color.White;
            buttonOK.Location = new System.Drawing.Point(124, 137);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(126, 41);
            buttonOK.TabIndex = 9;
            buttonOK.Text = "buttonOK";
            buttonOK.TextColor = System.Drawing.Color.White;
            buttonOK.UseVisualStyleBackColor = false;
            buttonOK.Click += buttonOK_Click;
            // 
            // logo
            // 
            logo.Image = HourFarmer.Properties.Resources.fatumlogo;
            logo.Location = new System.Drawing.Point(0, 0);
            logo.Name = "logo";
            logo.Size = new System.Drawing.Size(30, 30);
            logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            logo.TabIndex = 1;
            logo.TabStop = false;
            // 
            // titleLabel
            // 
            titleLabel.BackColor = System.Drawing.Color.FromArgb(16, 16, 16);
            titleLabel.Font = FontLoader.GetFont(12F, System.Drawing.FontStyle.Regular);
            titleLabel.ForeColor = System.Drawing.Color.White;
            titleLabel.Location = new System.Drawing.Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(374, 47);
            titleLabel.TabIndex = 11;
            titleLabel.Text = "titleLabel";
            titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.FromArgb(16, 16, 16);
            panel1.Controls.Add(logo);
            panel1.Location = new System.Drawing.Point(12, 8);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(30, 30);
            panel1.TabIndex = 12;
            // 
            // FatumMessageBox
            // 
            BackColor = System.Drawing.Color.FromArgb(26, 26, 26);
            ClientSize = new System.Drawing.Size(374, 190);
            Controls.Add(panel1);
            Controls.Add(titleLabel);
            Controls.Add(buttonOK);
            Controls.Add(labelMessageContent);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "FatumMessageBox";
            Text = "Fatum Message Box";
            ((System.ComponentModel.ISupportInitialize)logo).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FatumButton buttonOK;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panel1;
    }
}