using FatumStyles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HourFarmer
{
    public partial class FormStart : Form
    {
        private const string __LOCAL_VERSION = "v1.2.1";
        private const string __ONLINE_VERSION_URL = "https://pastebin.com/raw/XwH8WKqM";
        private bool _mainFormOpened = false;

        private static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private static void RelaunchAsAdmin()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    Verb = "runas",
                    UseShellExecute = true
                };

                Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Environment.Exit(0);
            }
        }
        public FormStart()
        {
            if (!IsAdministrator())
            {
                RelaunchAsAdmin();
                return;
            }
            InitializeComponent();
            this.Load += new EventHandler(FormStart_Load);
        }
        private void FormStart_Load(object sender, EventArgs e)
        {
            buttonDownload.Hide();

            FormAnimator.ApplyRoundedCorners(this);

            versionLabel.Text = $"Checking version...";

            Task.Run(CheckForUpdates);
        }

        private async Task CheckForUpdates()
        {
            string onlineVersion = null;

            using (var handler = new HttpClientHandler { AllowAutoRedirect = true })
            using (var client = new HttpClient(handler))
            {
                try
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    client.DefaultRequestHeaders.Add("Accept-Language", "pl-PL,pl;q=0.9,en-US;q=0.8,en;q=0.7");
                    client.DefaultRequestHeaders.Add("Referer", "https://google.com/");

                    client.Timeout = TimeSpan.FromSeconds(10);

                    var response = await client.GetAsync(__ONLINE_VERSION_URL);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        onlineVersion = content.Trim();
                    }
                    else
                    {
                        onlineVersion = $"ERROR (HTTP Status: {response.StatusCode})";
                    }
                }
                catch (Exception ex)
                {
                    if (ex is HttpRequestException)
                    {
                        onlineVersion = $"ERROR (HTTP Failed: {ex.Message.Split('\n')[0].Trim()})";
                    }
                    else
                    {
                        onlineVersion = "ERROR (Connection failed or unknown issue)";
                    }
                }
            }

            this.BeginInvoke((MethodInvoker)delegate
            {
                HandleUpdateResult(onlineVersion);
            });
        }

        private void HandleUpdateResult(string onlineVersion)
        {
            bool isHtmlContent = Regex.IsMatch(onlineVersion, @"<[^>]+>");

            string displayVersion = onlineVersion;

            if (isHtmlContent || displayVersion.Length > 25)
            {
                displayVersion = "ERROR (Server security blocking direct access)";
            }

            Button buttonStart = this.Controls.Find("buttonStart", true).FirstOrDefault() as Button;

            if (__LOCAL_VERSION.Equals(displayVersion, StringComparison.OrdinalIgnoreCase))
            {
                versionLabel.Text = $"Up to date! ({__LOCAL_VERSION})";

                if (_mainFormOpened)
                    return;

                _mainFormOpened = true;

                FormAnimator.FadeOut(this, () =>
                {
                    var mainForm = new Form1();
                    mainForm.Show();
                    this.Hide();
                });
            }
            else
            {
                versionLabel.Text = $"Update required!\nYour version: {__LOCAL_VERSION}\nCurrent version: {displayVersion}";
                versionLabel.ForeColor = Color.Red;
                buttonDownload.Show();

                if (displayVersion.StartsWith("ERROR"))
                {

                    FatumMessageBox.Show(
                        "Server security is blocking direct access to the version file. Reinstall the application or contact Fatum.",
                        "Critical Version Check Error",
                        "OK", Properties.Resources.fatumlogo,
                        Color.FromArgb(145, 0, 255),
                        Color.FromArgb(26, 26, 46)
                        );
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FormAnimator.FadeOut(this, Application.Exit);
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.Image = Properties.Resources.CloseHOVER;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.Image = Properties.Resources.PurpleClose;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            FormAnimator.ApplyRoundedCorners(this);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.Opacity == 1.0)
            {
                e.Cancel = true;
                FormAnimator.FadeOut(this, () =>
                {
                    Application.Exit();
                });
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            string installerPath = @"C:\Program Files\Fatum Installer\FatumInstaller.exe";

            if (File.Exists(installerPath))
            {
                try
                {
                    Process proc = null;

                    try
                    {
                        proc = Process.Start(installerPath);
                    }
                    catch { }

                    if (proc == null)
                    {
                        try
                        {
                            var psi = new ProcessStartInfo(installerPath)
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Path.GetDirectoryName(installerPath)
                            };
                            proc = Process.Start(psi);
                        }
                        catch { }
                    }

                    if (proc == null)
                    {
                        try
                        {
                            var cmd = new ProcessStartInfo("cmd.exe", $"/c start \"\" \"{installerPath}\"")
                            {
                                CreateNoWindow = true,
                                UseShellExecute = false
                            };
                            proc = Process.Start(cmd);
                        }
                        catch { }
                    }

                    if (proc == null)
                    {
                        try
                        {
                            var psiElev = new ProcessStartInfo(installerPath)
                            {
                                UseShellExecute = true,
                                Verb = "runas",
                                WorkingDirectory = Path.GetDirectoryName(installerPath)
                            };
                            proc = Process.Start(psiElev);
                        }
                        catch (Win32Exception wex) when (wex.NativeErrorCode == 1223)
                        {
                            throw new Exception("Operation Canceled.");
                        }
                        catch { }
                    }

                    if (proc == null)
                        throw new Exception("Critical Error");

                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    FatumMessageBox.Show(
                        "Failed to launch installer: " + ex.Message,
                        "Error",
                        "OK",
                        Properties.Resources.fatumlogo,
                        Color.FromArgb(145, 0, 255),
                        Color.FromArgb(26, 26, 46)
                    );
                }
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://fatum.cc",
                        UseShellExecute = true
                    });
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    FatumMessageBox.Show(
                        "Failed to open website: " + ex.Message,
                        "Error",
                        "OK",
                        Properties.Resources.fatumlogo,
                        Color.FromArgb(145, 0, 255),
                        Color.FromArgb(26, 26, 46)
                    );
                }
            }
        }
    }
}