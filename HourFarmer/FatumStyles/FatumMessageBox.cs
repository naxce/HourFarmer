using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FatumStyles
{
    public partial class FatumMessageBox : Form
    {
        private const string MessageLabelName = "labelMessageContent";
        private const string ButtonOKName = "buttonOK";
        private const string TitleLabelName = "titleLabel";
        private const string LogoPictureBoxName = "logo";

        private FatumMessageBox(string messageContent, string windowTitle, string okButtonText, Image logoImage, Color okButtonColor, Color backgroundColor)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.Opacity = 0.0;
            this.BackColor = backgroundColor;

            Color darkerTitleColor = GetDarkerColor(backgroundColor, 10);

            if (this.Controls.Find(TitleLabelName, true).FirstOrDefault() is Label titleLabel)
            {
                titleLabel.Text = windowTitle;
                titleLabel.BackColor = darkerTitleColor;
                panel1.BackColor = darkerTitleColor;
            }

            if (this.Controls.Find(LogoPictureBoxName, true).FirstOrDefault() is PictureBox logoBox)
            {
                logoBox.BackColor = darkerTitleColor;
                logoBox.Image = null;

                if (logoImage != null)
                {
                    logoBox.Image = logoImage;
                    logoBox.Visible = true;
                    logoBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    logoBox.Visible = false;
                }
            }

            if (this.Controls.Find(MessageLabelName, true).FirstOrDefault() is Label msgLabel)
            {
                msgLabel.AutoSize = false;
                msgLabel.Text = messageContent;

                Size maxSize = new Size(msgLabel.Width, int.MaxValue);
                Size textSize = TextRenderer.MeasureText(messageContent, msgLabel.Font, maxSize, TextFormatFlags.WordBreak);

                int heightDelta = textSize.Height - msgLabel.Height;

                if (heightDelta > 0)
                {
                    msgLabel.Height = textSize.Height;
                    this.Height += heightDelta;
                }
            }

            if (this.Controls.Find(ButtonOKName, true).FirstOrDefault() is Button okButton)
            {
                okButton.Text = okButtonText;
                okButton.BackColor = okButtonColor;
                okButton.Top = this.ClientSize.Height - okButton.Height - 10;
            }

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
        }

        private Color GetDarkerColor(Color baseColor, int subtractValue)
        {
            int r = Math.Max(baseColor.R - subtractValue, 0);
            int g = Math.Max(baseColor.G - subtractValue, 0);
            int b = Math.Max(baseColor.B - subtractValue, 0);

            return Color.FromArgb(r, g, b);
        }

        private static readonly Color DefaultButtonColor = Color.FromArgb(100, 50, 255);
        private static readonly Color DefaultBackgroundColor = Color.FromArgb(40, 40, 40);

        public static DialogResult Show(string message, string title = "Fatum Message Box", string okButtonText = "OK")
        {
            return Show(message, title, okButtonText, null, DefaultButtonColor, DefaultBackgroundColor);
        }

        public static DialogResult Show(string message, string title, string okButtonText, Image logoImage)
        {
            return Show(message, title, okButtonText, logoImage, DefaultButtonColor, DefaultBackgroundColor);
        }

        public static DialogResult Show(string message, string title, string okButtonText, Image logoImage, Color okButtonColor)
        {
            return Show(message, title, okButtonText, logoImage, okButtonColor, DefaultBackgroundColor);
        }

        public static DialogResult Show(string message, string title, string okButtonText, Image logoImage, Color okButtonColor, Color backgroundColor)
        {
            using (var dialog = new FatumMessageBox(message, title, okButtonText, logoImage, okButtonColor, backgroundColor))
            {
                FormAnimator.ApplyRoundedCorners(dialog);
                FormAnimator.FadeIn(dialog);
                return dialog.ShowDialog();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.Opacity == 1.0 && e.Cancel == false)
            {
                e.Cancel = true;
                DialogResult finalResult = this.DialogResult;

                FormAnimator.FadeOut(this, () =>
                {
                    this.DialogResult = finalResult;
                    this.BeginInvoke((MethodInvoker)delegate { this.Dispose(); });
                });
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            FormAnimator.ApplyRoundedCorners(this);
        }
    }
}