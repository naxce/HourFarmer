using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

namespace FatumStyles
{
    public static class FormAnimator
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public static void ApplyRoundedCorners(Form form, int radius = 20)
        {
            if (form.WindowState == FormWindowState.Maximized)
                return;

            form.Region = Region.FromHrgn(CreateRoundRectRgn(
                0, 0, form.Width + 1, form.Height + 1, radius, radius));
        }

        private const int AnimationInterval = 5;
        private const double OpacityStep = 0.1;

        public static void FadeIn(Form form)
        {
            form.Opacity = 0.0;

            System.Windows.Forms.Timer fadeInTimer = new System.Windows.Forms.Timer();
            fadeInTimer.Interval = AnimationInterval;

            fadeInTimer.Tick += (sender, e) =>
            {
                if (form.Opacity < 1.0)
                {
                    form.Opacity += OpacityStep;
                }
                else
                {
                    fadeInTimer.Stop();
                    fadeInTimer.Dispose();
                    form.Opacity = 1.0;
                }
            };
            fadeInTimer.Start();
        }

        public static void FadeOut(Form form, Action onAnimationFinished = null)
        {
            System.Windows.Forms.Timer fadeOutTimer = new System.Windows.Forms.Timer();
            fadeOutTimer.Interval = AnimationInterval;

            fadeOutTimer.Tick += (sender, e) =>
            {
                if (form.Opacity > 0.0)
                {
                    form.Opacity -= OpacityStep;
                }
                else
                {
                    fadeOutTimer.Stop();
                    fadeOutTimer.Dispose();
                    onAnimationFinished?.Invoke();
                }
            };
            fadeOutTimer.Start();
        }
    }
}