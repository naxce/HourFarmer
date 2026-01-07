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
using System.Runtime.InteropServices;

namespace HourFarmer
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cts;
        private AppId_t _currentAppId;
        private Task _steamLoopTask;
        private int _isSteamAPICalled = 0;
        private readonly object _farmingLock = new object();
        private bool _isStarting = false;
        private const string __GAME_COMBO_NAME = "comboBoxGames";
        private const string __STATUS_LABEL_NAME = "labelStatus";
        private const string __START_BUTTON_NAME = "buttonStart";
        private const int __THREAD_DELAY = 50;
        private const int __TASK_WAIT_TIMEOUT = 500;
        private bool _shouldAutoRestart = false;
        private const int __RESTART_INTERVAL = 5000;
        private int _failedAttemptsCount = 0;

        private string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "installed.dat");
        private const string HOURFARMER_CHANGELOG = "https://raw.githubusercontent.com/naxce/HourFarmer/refs/heads/master/changelog.txt";

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        public Form1()
        {
            SetDllDirectory(AppDomain.CurrentDomain.BaseDirectory);
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

            this.BeginInvoke(new MethodInvoker(CheckFirstRun));
        }
        private void CheckFirstRun()
        {
            if (!File.Exists(configPath))
            {
                try
                {
                    File.WriteAllText(configPath, DateTime.Now.ToString());
                    btnChangeLog_Click(this, EventArgs.Empty);
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
            if (_isStarting) return;
            var controls = new
            {
                GamesBox = this.Controls.Find(__GAME_COMBO_NAME, true).FirstOrDefault() as FatumComboBox,
                AppIdBox = this.Controls.Find("textBoxAppId", true).FirstOrDefault() as FatumTextBox,
                ManualCheck = this.Controls.Find("checkBoxManualAppId", true).FirstOrDefault() as CheckBox,
                StatusLabel = this.Controls.Find(__STATUS_LABEL_NAME, true).FirstOrDefault() as Label,
                StopBtn = this.Controls.Find("buttonStop", true).FirstOrDefault() as Button
            };
            if (controls.StatusLabel == null || controls.StopBtn == null) return;
            if (!SteamAPI.IsSteamRunning())
            {
                FatumMessageBox.Show("Steam is not running.", "Error", "OK", Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                return;
            }
            uint appId;
            if (controls.ManualCheck != null && controls.ManualCheck.Checked)
            {
                if (controls.AppIdBox == null || !uint.TryParse(controls.AppIdBox.Texts.Trim(), out appId))
                {
                    FatumMessageBox.Show("Invalid AppID.", "Error", "OK", Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                    return;
                }
            }
            else
            {
                if (controls.GamesBox == null || controls.GamesBox.SelectedItem == null)
                {
                    FatumMessageBox.Show("Game not chosen.", "Error", "OK", Properties.Resources.icon, Color.DarkTurquoise, Color.FromArgb(26, 26, 46));
                    return;
                }
                var item = controls.GamesBox.SelectedItem as ComboBoxItem;
                if (!uint.TryParse(item?.Value, out appId)) return;
            }
            _currentAppId = new AppId_t(appId);
            _shouldAutoRestart = true;
            StartFarming();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            _shouldAutoRestart = false;
            StopFarming();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _shouldAutoRestart = false;
            Task.Run(() =>
            {
                CleanupSteamState();
                KillSteamProcesses();
                if (File.Exists("steam_appid.txt")) try { File.Delete("steam_appid.txt"); } catch { }
                LaunchSteam();
                ResetUiState();
            });
            FormAnimator.FadeOut(this, Application.Exit);
        }

        private void btnClose_MouseEnter(object sender, EventArgs e) { btnClose.Image = Properties.Resources.CloseHOVER; }
        private void btnClose_MouseLeave(object sender, EventArgs e) { btnClose.Image = Properties.Resources.Close; }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            FormAnimator.FadeOut(this, () =>
            {
                this.WindowState = FormWindowState.Minimized;
                this.Opacity = 1.0;
            });
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e) { btnMinimize.Image = Properties.Resources.MinimizeHOVER; }
        private void btnMinimize_MouseLeave(object sender, EventArgs e) { btnMinimize.Image = Properties.Resources.Minimize; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            FormAnimator.ApplyRoundedCorners(this);
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
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
                if (running) SetStatus("Status: Farming hours on AppID: " + _currentAppId.ToString());
            }
        }

        private void StartFarming()
        {
            lock (_farmingLock)
            {
                if (_isStarting) return;
                _isStarting = true;
            }
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

                    CleanupSteamState();
                    Thread.Sleep(1500);

                    var steamProcs = Process.GetProcessesByName("steam").Concat(Process.GetProcessesByName("Steam")).ToArray();
                    if (steamProcs.Length > 0)
                    {
                        if (File.Exists("steam_appid.txt"))
                        {
                            try { File.Delete("steam_appid.txt"); } catch { }
                        }

                        File.WriteAllText("steam_appid.txt", _currentAppId.ToString());
                        Thread.Sleep(500);

                        if (SteamAPI.Init())
                        {
                            Interlocked.Exchange(ref _isSteamAPICalled, 1);
                            _cts = new CancellationTokenSource();
                            _steamLoopTask = Task.Run(() => SteamLoop(_cts.Token));
                            SetUiState(true);
                            return;
                        }
                        else
                        {
                            SetStatus("Failed to init SteamAPI. Restart Steam.");
                        }
                    }
                    else
                    {
                        SetStatus("Waiting for Steam...");
                    }

                    if (_shouldAutoRestart)
                    {
                        Task.Delay(__RESTART_INTERVAL).ContinueWith(t =>
                        {
                            if (_shouldAutoRestart) StartFarming();
                        });
                    }
                }
                catch (Exception ex)
                {
                    SetStatus("Error: " + ex.Message);
                }
                finally
                {
                    lock (_farmingLock)
                    {
                        _isStarting = false;
                    }
                }
            });
        }

        private void CleanupSteamState()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                try { _cts.Dispose(); } catch { }
                _cts = null;
            }

            if (Interlocked.Exchange(ref _isSteamAPICalled, 0) == 1)
            {
                try
                {
                    SteamAPI.Shutdown();
                }
                catch { }
            }

            if (File.Exists("steam_appid.txt"))
            {
                try { File.Delete("steam_appid.txt"); } catch { }
            }

            Thread.Sleep(500);
        }

        private void SteamLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    SteamAPI.RunCallbacks();
                    if (token.WaitHandle.WaitOne(100)) break;
                }
            }
            catch { }
            finally
            {
                if (Interlocked.Exchange(ref _isSteamAPICalled, 0) == 1) { try { SteamAPI.Shutdown(); } catch { } }
                if (!token.IsCancellationRequested && _shouldAutoRestart)
                {
                    Task.Delay(3000).ContinueWith(t => { if (_shouldAutoRestart) StartFarming(); });
                }
            }
        }

        private void ResetUiState()
        {
            Action __ui_sync_lambda = () =>
            {
                var lbl = this.Controls.Find(__STATUS_LABEL_NAME, true).FirstOrDefault() as Label;
                var start = this.Controls.Find(__START_BUTTON_NAME, true).FirstOrDefault() as Button;
                var stop = this.Controls.Find("buttonStop", true).FirstOrDefault() as Button;
                if (lbl != null) lbl.Text = "Status: Awaiting game selection.";
                if (start != null) { start.Enabled = true; start.BackColor = Color.DarkTurquoise; }
                if (stop != null) stop.Enabled = false;
            };
            if (this.InvokeRequired) this.BeginInvoke(new MethodInvoker(__ui_sync_lambda));
            else __ui_sync_lambda();
        }

        private void StopFarming()
        {
            _shouldAutoRestart = false;
            Task.Run(() =>
            {
                CleanupSteamState();
                KillSteamProcesses();
                if (File.Exists("steam_appid.txt")) try { File.Delete("steam_appid.txt"); } catch { }
                LaunchSteam();
                ResetUiState();
            });
        }
        private void KillSteamProcesses()
        {
            SetStatus("Restarting Steam...");
            string[] targets = { "steam", "SteamService", "steamwebhelper" };
            foreach (var target in targets)
            {
                foreach (var proc in Process.GetProcessesByName(target))
                {
                    try { proc.Kill(); proc.WaitForExit(2000); } catch { }
                }
            }
            Thread.Sleep(1000);
        }
        private void LaunchSteam()
        {
            string path = GetSteamPath();
            if (string.IsNullOrEmpty(path)) return;
            string exe = Path.Combine(path, "steam.exe");
            if (File.Exists(exe)) Process.Start(exe);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && this.Opacity == 1.0)
            {
                e.Cancel = true;
                StopFarming();
                FormAnimator.FadeOut(this, Application.Exit);
            }
            else base.OnFormClosing(e);
        }

        private void checkBoxManualAppId_CheckedChanged(object sender, EventArgs e)
        {
            bool manual = checkBoxManualAppId.Checked;
            comboBoxGames.IconColor = manual ? Color.CadetBlue : Color.DarkTurquoise;
            textBoxAppId.PlaceholderColor = manual ? Color.LightGray : Color.DimGray;
            comboBoxGames.Enabled = !manual;
            textBoxAppId.Enabled = manual;
        }

        private async Task<string> GetRawTextFromUrlAsync(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private async void btnChangeLog_Click(object sender, EventArgs e)
        {
            string changelogUrl = HOURFARMER_CHANGELOG;
            string changelogText = await GetRawTextFromUrlAsync(changelogUrl);
            FormStart formStart = new FormStart();
            FatumMessageBox.Show(
                changelogText,
                "HourFarmer " + formStart.version + " Changelog",
                "OK",
                Properties.Resources.icon,
                Color.DarkTurquoise,
                Color.FromArgb(26, 26, 46)
            );
        }

        public class ComboBoxItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string name, string value) { Name = name; Value = value; }
            public override string ToString() => Name;
        }
    }
}