using FatumStyles;
using Microsoft.Win32;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HourFarmer
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cts;
        private AppId_t _currentAppId;
        private Task _steamLoopTask;
        private int _isSteamAPICalled = 0;
        private const string __CONFIG_FILE = "farm_state.ini";
        private const string __GAME_COMBO_NAME = "comboBoxGames";
        private const string __STATUS_LABEL_NAME = "labelStatus";
        private const string __START_BUTTON_NAME = "buttonStart";
        private const int __THREAD_DELAY = 50;
        private const int __TASK_WAIT_TIMEOUT = 500;
        private bool _shouldAutoRestart = false;
        private const int __RESTART_INTERVAL = 5000;
        private int _failedAttemptsCount = 0;
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxGames.Enabled = true;
            textBoxAppId.Enabled = false;
            try
            {
                LoadSteamGames();
            }
            catch (Exception ex)
            {
                FatumMessageBox.Show(
                    "Steam games failed to load: " + ex.Message,
                    "Critical Error",
                    "OK",
                    Properties.Resources.icon,
                    Color.DarkTurquoise,
                    Color.FromArgb(26, 26, 46)
                    );
            }
            FormAnimator.ApplyRoundedCorners(this);
            FormAnimator.FadeIn(this);
            if (File.Exists(__CONFIG_FILE))
            {
                try
                {
                    string savedAppId = File.ReadAllText(__CONFIG_FILE);
                    if (uint.TryParse(savedAppId, out uint appId))
                    {
                        _currentAppId = new AppId_t(appId);
                        _shouldAutoRestart = true;
                        SetStatus("Auto-resuming...");
                        Task.Delay(3000).ContinueWith(t => StartFarming());
                    }
                }
                catch { }
            }
        }

        private void LoadSteamGames()
        {
            var steamPathProvider = (Func<string>)GetSteamPath;
            string steamPath = steamPathProvider();

            if (string.IsNullOrEmpty(steamPath))
            {
                FatumMessageBox.Show("Steam client not found. Make sure Steam is installed",
                    "Error",
                    "OK",
                    Properties.Resources.icon,
                    Color.DarkTurquoise,
                    Color.FromArgb(26, 26, 46)
                    );
                return;
            }

            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            var appManifests = new Dictionary<string, string>();

            Func<string, Dictionary<string, string>> ParseVdf = (filePath) =>
            {
                var data = new Dictionary<string, string>();
                if (!File.Exists(filePath)) return data;

                try
                {
                    string content = File.ReadAllText(filePath);
                    var matches = Regex.Matches(content, "\"([^\"]*)\"[\\s]*\"([^\"]*)\"");

                    foreach (Match match in matches)
                    {
                        if (match.Groups.Count >= 3)
                        {
                            string key = match.Groups[1].Value.Trim();
                            string value = match.Groups[2].Value.Trim();
                            if (!data.ContainsKey(key)) data[key] = value;
                        }
                    }
                }
                catch { }
                return data;
            };

            var libraryFolders = new List<string> { Path.Combine(steamPath, "steamapps") };
            var vdfData = ParseVdf(libraryFoldersPath);

            foreach (var kvp in vdfData.Where(x => int.TryParse(x.Key, out _)))
            {
                string steamAppsFolder = Path.Combine(kvp.Value, "steamapps");
                if (Directory.Exists(steamAppsFolder) && !libraryFolders.Contains(steamAppsFolder))
                {
                    libraryFolders.Add(steamAppsFolder);
                }
            }

            foreach (var folder in libraryFolders.Distinct())
            {
                if (Directory.Exists(folder))
                {
                    foreach (var file in Directory.GetFiles(folder, "appmanifest_*.acf"))
                    {
                        try
                        {
                            var appData = ParseVdf(file);
                            if (appData.TryGetValue("appid", out string appId) && appData.TryGetValue("name", out string name))
                            {
                                appManifests[appId] = name;
                            }
                        }
                        catch { }
                    }
                }
            }

            if (this.Controls.Find(__GAME_COMBO_NAME, true).FirstOrDefault() is FatumComboBox fc)
            {
                fc.Items.Clear();
                foreach (var item in appManifests.OrderBy(x => x.Value))
                {
                    fc.Items.Add(new ComboBoxItem(item.Value, item.Key));
                }
            }
            else
            {
                FatumMessageBox.Show(
                    "Dynamic control resolution failed. Reinstall the application.",
                    "Resolution Error",
                    "OK",
                    Properties.Resources.icon,
                    Color.DarkTurquoise,
                    Color.FromArgb(26, 26, 46)
                    );
            }

            if (appManifests.Count == 0)
            {
                FatumMessageBox.Show(
                    "Games not detected. Make sure games are installed.",
                    "No Games",
                    "OK",
                    Properties.Resources.icon,
                    Color.DarkTurquoise,
                    Color.FromArgb(26, 26, 46)
                    );
            }
        }

        private string GetSteamPath()
        {
            var regKeys = new[]
            {
                @"SOFTWARE\WOW6432Node\Valve\Steam",
                @"SOFTWARE\Valve\Steam",
                @"Software\Valve\Steam"
            };

            var hiveChecks = new (RegistryKey hive, string valName)[]
            {
                (Registry.LocalMachine, "InstallPath"),
                (Registry.LocalMachine, "InstallPath"),
                (Registry.CurrentUser, "SteamPath")
            };

            for (int i = 0; i < regKeys.Length; i++)
            {
                using (RegistryKey key = hiveChecks[i].hive.OpenSubKey(regKeys[i]))
                {
                    string path = key?.GetValue(hiveChecks[i].valName)?.ToString();
                    if (!string.IsNullOrEmpty(path)) return path.Replace('/', '\\');
                }
            }
            return string.Empty;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            var controls = new
            {
                GamesBox = this.Controls.Find(__GAME_COMBO_NAME, true).FirstOrDefault() as FatumComboBox,
                AppIdBox = this.Controls.Find("textBoxAppId", true).FirstOrDefault() as FatumTextBox,
                ManualCheck = this.Controls.Find("checkBoxManualAppId", true).FirstOrDefault() as CheckBox,
                StatusLabel = this.Controls.Find(__STATUS_LABEL_NAME, true).FirstOrDefault() as Label,
                StopBtn = this.Controls.Find("buttonStop", true).FirstOrDefault() as Button
            };

            if (controls.StatusLabel == null || controls.StopBtn == null)
            {
                FatumMessageBox.Show("UI error.", "Critical Error", "OK",
                    Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                return;
            }

            if (!SteamAPI.IsSteamRunning())
            {
                FatumMessageBox.Show("Steam is not running.", "Error", "OK",
                    Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                return;
            }

            uint appId;

            if (controls.ManualCheck != null && controls.ManualCheck.Checked)
            {
                if (controls.AppIdBox == null || !uint.TryParse(controls.AppIdBox.Texts.Trim(), out appId))
                {
                    FatumMessageBox.Show("Invalid AppID.", "Error", "OK",
                        Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                    return;
                }
            }
            else
            {
                if (controls.GamesBox == null || controls.GamesBox.SelectedItem == null)
                {
                    FatumMessageBox.Show("Game not chosen.", "Error", "OK",
                        Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                    return;
                }

                var item = controls.GamesBox.SelectedItem as ComboBoxItem;
                if (!uint.TryParse(item?.Value, out appId))
                {
                    FatumMessageBox.Show("Invalid AppID.", "Error", "OK",
                        Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                    return;
                }
            }

            if (_steamLoopTask != null && !_steamLoopTask.IsCompleted)
                StopFarming();

            _currentAppId = new AppId_t(appId);
            _shouldAutoRestart = true;
            StartFarming();
        }


        private void buttonStop_Click(object sender, EventArgs e)
        {
            _shouldAutoRestart = false;
            if (File.Exists(__CONFIG_FILE)) try { File.Delete(__CONFIG_FILE); } catch { }
            StopFarming();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ShutdownBackgroundTasks();

            FormAnimator.FadeOut(this, Application.Exit);
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.Image = Properties.Resources.CloseHOVER;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.Image = Properties.Resources.Close;
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            FormAnimator.FadeOut(this, () =>
            {
                this.WindowState = FormWindowState.Minimized;
                this.Opacity = 1.0;
            });
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            btnMinimize.Image = Properties.Resources.MinimizeHOVER;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.Image = Properties.Resources.Minimize;
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
        private void SetStatus(string text)
        {
            if (this.InvokeRequired) this.BeginInvoke(new Action(() => SetStatus(text)));
            else
            {
                var lbl = this.Controls.Find(__STATUS_LABEL_NAME, true).FirstOrDefault() as Label;
                if (lbl != null) lbl.Text = text;
            }
        }

        private void SetUiState(bool running)
        {
            if (this.InvokeRequired) this.BeginInvoke(new Action(() => SetUiState(running)));
            else
            {
                var start = this.Controls.Find(__START_BUTTON_NAME, true).FirstOrDefault() as Button;
                var stop = this.Controls.Find("buttonStop", true).FirstOrDefault() as Button;
                if (start != null) { start.Enabled = !running; start.BackColor = running ? Color.CadetBlue : Color.DarkTurquoise; }
                if (stop != null) stop.Enabled = running;
                if (running) SetStatus("Status: Farming hours...");
            }
        }
        private void StartFarming()
        {
            Task.Run(() =>
            {
                try
                {
                    bool isElevated;
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                    {
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                    }

                    if (!isElevated)
                    {
                        SetStatus("ERROR: RUN AS ADMIN!");
                        return;
                    }

                    if (_cts != null) { _cts.Cancel(); _cts.Dispose(); _cts = null; }
                    Interlocked.Exchange(ref _isSteamAPICalled, 0);
                    try { SteamAPI.Shutdown(); } catch { }

                    Thread.Sleep(1000);

                    var steamProcs = Process.GetProcessesByName("steam").Concat(Process.GetProcessesByName("Steam")).ToArray();
                    if (steamProcs.Length > 0)
                    {
                        File.WriteAllText("steam_appid.txt", _currentAppId.ToString());
                        Thread.Sleep(1000);

                        if (SteamAPI.Init())
                        {
                            Interlocked.Exchange(ref _isSteamAPICalled, 1);
                            _cts = new CancellationTokenSource();
                            _steamLoopTask = Task.Run(() => SteamLoop(_cts.Token));
                            SetUiState(true);
                            return;
                        }
                    }

                    SetStatus("Waiting for Steam...");
                    if (_shouldAutoRestart)
                    {
                        Task.Delay(30000).ContinueWith(t => { if (_shouldAutoRestart) StartFarming(); });
                    }
                }
                catch { }
            });
        }
        private void SteamLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var steamProcs = Process.GetProcessesByName("steam");
                    if (steamProcs.Length == 0)
                    {
                        steamProcs = Process.GetProcessesByName("Steam");
                    }

                    if (steamProcs.Length == 0) break;

                    SteamAPI.RunCallbacks();

                    if (token.WaitHandle.WaitOne(__THREAD_DELAY)) break;
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isSteamAPICalled, 0);

                if (!token.IsCancellationRequested && _shouldAutoRestart)
                {
                    var finalCheck = Process.GetProcessesByName("steam").Concat(Process.GetProcessesByName("Steam")).ToArray();

                    if (finalCheck.Length == 0)
                    {
                        SetStatus("Steam process closed. Waiting...");
                        Task.Delay(5000).ContinueWith(t => { if (_shouldAutoRestart) StartFarming(); });
                    }
                    else
                    {
                        StartFarming();
                    }
                }
            }
        }
        private void ResetUiState()
        {
            var controls = new
            {
                StatusLabel = this.Controls.Find(__STATUS_LABEL_NAME, true).FirstOrDefault() as Label,
                StartBtn = this.Controls.Find(__START_BUTTON_NAME, true).FirstOrDefault() as Button,
                StopBtn = this.Controls.Find("buttonStop", true).FirstOrDefault() as Button
            };

            Action __ui_sync_lambda = () =>
            {
                if (controls.StatusLabel != null) controls.StatusLabel.Text = "Status: Awaiting game selection.";
                if (controls.StartBtn != null) controls.StartBtn.Enabled = true;
                buttonStart.BackColor = Color.DarkTurquoise;
                if (controls.StopBtn != null) controls.StopBtn.Enabled = false;
            };

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(__ui_sync_lambda));
            }
            else
            {
                __ui_sync_lambda();
            }
        }

        private void StopFarming()
        {
            ShutdownBackgroundTasks();
            ResetUiState();
        }

        private void ShutdownBackgroundTasks()
        {
            if (_cts != null) { _cts.Cancel(); _cts.Dispose(); _cts = null; }

            if (Interlocked.Exchange(ref _isSteamAPICalled, 0) == 1)
            {
                try { SteamAPI.Shutdown(); } catch { }
            }
            if (File.Exists("steam_appid.txt"))
            {
                try { File.Delete("steam_appid.txt"); } catch { }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.Opacity == 1.0)
            {
                e.Cancel = true;
                ShutdownBackgroundTasks();
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

        private void checkBoxManualAppId_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxManualAppId.Checked == true)
            {
                comboBoxGames.IconColor = Color.CadetBlue;
                textBoxAppId.PlaceholderColor = Color.LightGray;
                comboBoxGames.Enabled = false;
                textBoxAppId.Enabled = true;
            }
            else if (checkBoxManualAppId.Checked == false)
            {
                comboBoxGames.IconColor = Color.DarkTurquoise;
                textBoxAppId.PlaceholderColor = Color.DimGray;
                comboBoxGames.Enabled = true;
                textBoxAppId.Enabled = false;
            }
        }

        public class ComboBoxItem
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public ComboBoxItem(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString() => Name;
        }
    }
}