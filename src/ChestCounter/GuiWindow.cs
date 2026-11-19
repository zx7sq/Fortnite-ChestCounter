using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Windows.Forms;

namespace ChestCounter;

/// <summary>
/// Pure native Windows desktop UI (no WebView2, no Chromium, no embedded web app).
/// Custom-painted dark flat + amber-gold desktop interface with system tray and Windows notifications.
/// </summary>
[SupportedOSPlatform("windows")]
public static class GuiWindow
{
    private static NativeMainForm? _form;
    private static NotifyIcon? _tray;
    private static bool _closing, _trayNoticeShown;

    public static volatile bool MinimizeToTray;

    public static bool Available() => OperatingSystem.IsWindows();

    public static void Start(
        Func<JsonElement> getState,
        Func<string, JsonElement, object?> runAction,
        string reportPath,
        string title,
        bool startHidden,
        Action quit,
        Func<string> closeQuestion,
        Func<string> closeTitle,
        Func<string> openText,
        Func<string> quitText,
        Func<string> trayNotice)
    {
        var ready = new ManualResetEventSlim();
        var thread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var icon = AppInfo.ExePath is { } exe ? Icon.ExtractAssociatedIcon(exe) : null;
            var form = new NativeMainForm(getState, runAction, reportPath, title, icon);

            var tray = new NotifyIcon { Icon = icon, Text = title.Length > 63 ? title[..63] : title, Visible = false };
            var menu = new ContextMenuStrip();
            menu.Items.Add(openText(), null, (_, _) => Restore());
            menu.Items.Add(quitText(), null, (_, _) => { _closing = true; form.Close(); });
            tray.ContextMenuStrip = menu;
            tray.MouseClick += (_, e) => { if (e.Button == MouseButtons.Left) Restore(); };
            _tray = tray;

            form.FormClosing += (_, e) =>
            {
                if (_closing || e.CloseReason != CloseReason.UserClosing)
                {
                    return;
                }
                if (MessageBox.Show(form, closeQuestion(), closeTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    _closing = true;
                }
                else
                {
                    e.Cancel = true;
                }
            };
            form.Resize += (_, _) =>
            {
                if (form.WindowState == FormWindowState.Minimized && MinimizeToTray)
                {
                    form.Hide();
                    tray.Visible = true;
                    if (!_trayNoticeShown)
                    {
                        _trayNoticeShown = true;
                        tray.BalloonTipTitle = title;
                        tray.BalloonTipText = trayNotice();
                        tray.BalloonTipIcon = ToolTipIcon.Info;
                        tray.ShowBalloonTip(8000);
                    }
                }
            };
            form.FormClosed += (_, _) =>
            {
                tray.Visible = false;
                tray.Dispose();
                _tray = null;
                _form = null;
                quit();
            };

            _form = form;
            if (startHidden)
            {
                _trayNoticeShown = true;
                form.WindowState = FormWindowState.Minimized;
                form.ShowInTaskbar = !MinimizeToTray;
                if (MinimizeToTray)
                {
                    tray.Visible = true;
                    form.Shown += (_, _) => form.Hide();
                }
            }
            ready.Set();
            Application.Run(form);
        })
        {
            IsBackground = true,
            Name = "GuiWindow",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        ready.Wait(TimeSpan.FromSeconds(10));
    }

    public static void Restore()
    {
        var form = _form;
        if (form == null || form.IsDisposed)
        {
            return;
        }
        void Show()
        {
            if (_tray != null) _tray.Visible = false;
            form.ShowInTaskbar = true;
            form.Show();
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }
            form.Activate();
            form.BringToFront();
            form.RefreshNow();
        }
        if (form.InvokeRequired) form.BeginInvoke(Show); else Show();
    }

    public static void Close()
    {
        var form = _form;
        if (form == null || form.IsDisposed)
        {
            return;
        }
        try
        {
            form.BeginInvoke(() =>
            {
                _closing = true;
                form.Close();
            });
        }
        catch { }
    }

    /// <summary>Shows a native Windows notification pop-up (tray balloon / Action Center toast) and plays a subtle chime.</summary>
    public static void ShowNotification(string title, string message)
    {
        var form = _form;
        if (form == null || form.IsDisposed)
        {
            return;
        }
        try
        {
            form.BeginInvoke(() =>
            {
                if (_tray == null)
                {
                    return;
                }
                var wasVisible = _tray.Visible;
                if (!wasVisible)
                {
                    _tray.Visible = true;
                }
                _tray.BalloonTipTitle = title;
                _tray.BalloonTipText = message;
                _tray.BalloonTipIcon = ToolTipIcon.Info;
                _tray.ShowBalloonTip(8000);
                try { System.Media.SystemSounds.Exclamation.Play(); } catch { }
                if (!wasVisible)
                {
                    var hideTimer = new System.Windows.Forms.Timer { Interval = 10_000 };
                    hideTimer.Tick += (_, _) =>
                    {
                        hideTimer.Stop();
                        hideTimer.Dispose();
                        if (_tray != null && _form is { Visible: true })
                        {
                            _tray.Visible = false;
                        }
                    };
                    hideTimer.Start();
                }
            });
        }
        catch { }
    }

    // ---- Theme Colors -----------------------------------------------------------------------
    private static readonly Color Bg = Color.FromArgb(11, 11, 14);
    private static readonly Color Surface = Color.FromArgb(20, 20, 25);
    private static readonly Color SurfaceHover = Color.FromArgb(28, 28, 35);
    private static readonly Color Border = Color.FromArgb(38, 38, 46);
    private static readonly Color Brand = Color.FromArgb(245, 158, 11);
    private static readonly Color BrandDark = Color.FromArgb(11, 11, 14);
    private static readonly Color Ink = Color.FromArgb(245, 245, 247);
    private static readonly Color Muted = Color.FromArgb(142, 142, 147);
    private static readonly Color Green = Color.FromArgb(48, 209, 88);
    private static readonly Color Red = Color.FromArgb(255, 69, 58);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    private static void EnableDarkTitleBar(IntPtr hwnd)
    {
        var dark = 1;
        DwmSetWindowAttribute(hwnd, 20 /* DWMWA_USE_IMMERSIVE_DARK_MODE */, ref dark, sizeof(int));
        DwmSetWindowAttribute(hwnd, 19, ref dark, sizeof(int));
    }

    // ---- Main Native Window -----------------------------------------------------------------
    private sealed class NativeMainForm : Form
    {
        private readonly Func<JsonElement> _getState;
        private readonly Func<string, JsonElement, object?> _runAction;
        private readonly string _reportPath;
        private readonly Bitmap? _chestBmp;
        private readonly System.Windows.Forms.Timer _pollTimer;
        private readonly System.Windows.Forms.Timer _cdTimer;

        private string _activeTab = "home";
        private string _lastStateHash = "";
        private JsonElement _state;

        // Top chrome
        private readonly Label _titleLbl;
        private readonly Label _subLbl;
        private readonly StatusPill _statusPill;
        private readonly FlowLayoutPanel _navBar;
        private readonly Dictionary<string, NavButton> _navButtons = new();
        private readonly Panel _contentHost;

        // Live countdown label on Home
        private Label? _thCountdownLbl;
        private DateTime _thStartUtc, _thEndUtc;

        public NativeMainForm(Func<JsonElement> getState, Func<string, JsonElement, object?> runAction, string reportPath, string title, Icon? icon)
        {
            _getState = getState;
            _runAction = runAction;
            _reportPath = reportPath;

            Text = title;
            Icon = icon;
            BackColor = Bg;
            ForeColor = Ink;
            Font = new Font("Segoe UI", 10f);
            MinimumSize = new Size(880, 620);
            ClientSize = new Size(1060, 760);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            try
            {
                using var s = typeof(GuiWindow).Assembly.GetManifestResourceStream("ChestIcon.png");
                if (s != null) _chestBmp = new Bitmap(s);
            }
            catch { }

            HandleCreated += (_, _) => EnableDarkTitleBar(Handle);

            // Header panel
            var header = new Panel { Dock = DockStyle.Top, Height = 128, BackColor = Bg, Padding = new Padding(28, 18, 28, 0) };

            var logoBox = new PictureBox
            {
                Image = _chestBmp,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(44, 44),
                Location = new Point(28, 18),
                BackColor = Color.Transparent,
            };
            _titleLbl = new Label
            {
                Text = "Fortnite Chest Counter",
                Font = new Font("Segoe UI", 19f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(84, 14),
            };
            _subLbl = new Label
            {
                Text = $"v{AppInfo.VersionText}",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Muted,
                AutoSize = true,
                Location = new Point(86, 48),
            };
            _statusPill = new StatusPill
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(ClientSize.Width - 220, 24),
                Size = new Size(192, 32),
            };

            _navBar = new FlowLayoutPanel
            {
                Location = new Point(28, 80),
                Size = new Size(680, 40),
                BackColor = Surface,
                Padding = new Padding(4),
                WrapContents = false,
            };
            foreach (var (key, label) in new[] { ("home", "Home"), ("stats", "Stats"), ("overlay", "Overlay"), ("settings", "Settings"), ("help", "Help") })
            {
                var btn = new NavButton(label, key == _activeTab);
                btn.Click += (_, _) =>
                {
                    if (_activeTab == key) return;
                    _activeTab = key;
                    UpdateNavStyles();
                    RebuildTab();
                };
                _navButtons[key] = btn;
                _navBar.Controls.Add(btn);
            }

            var divider = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Border };
            header.Controls.AddRange(new Control[] { logoBox, _titleLbl, _subLbl, _statusPill, _navBar, divider });

            _contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Bg,
                AutoScroll = true,
                Padding = new Padding(28, 20, 28, 28),
            };

            Controls.Add(_contentHost);
            Controls.Add(header);

            _pollTimer = new System.Windows.Forms.Timer { Interval = 1500 };
            _pollTimer.Tick += (_, _) => RefreshState(force: false);
            _pollTimer.Start();

            _cdTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _cdTimer.Tick += (_, _) => TickCountdown();
            _cdTimer.Start();

            Shown += (_, _) => RefreshNow();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pollTimer.Stop();
                _pollTimer.Dispose();
                _cdTimer.Stop();
                _cdTimer.Dispose();
                _chestBmp?.Dispose();
            }
            base.Dispose(disposing);
        }

        public void RefreshNow() => RefreshState(force: true);

        private void UpdateNavStyles()
        {
            foreach (var (k, b) in _navButtons)
            {
                b.Active = k == _activeTab;
            }
        }

        private void Act(string action, object? value = null)
        {
            var el = JsonSerializer.SerializeToElement(value);
            _ = Task.Run(() =>
            {
                try { _runAction(action, el); } catch { }
                try { BeginInvoke(() => RefreshState(force: true)); } catch { }
            });
        }

        private void RefreshState(bool force)
        {
            try
            {
                var next = _getState();
                var raw = next.GetRawText();
                if (!force && raw == _lastStateHash)
                {
                    return;
                }
                _lastStateHash = raw;
                _state = next;

                // Update header
                var appName = Str(_state, "appName", "Fortnite Chest Counter");
                var player = Str(_state, "player", "");
                var ver = Str(_state, "version", AppInfo.VersionText);
                _titleLbl.Text = appName;
                _subLbl.Text = string.IsNullOrEmpty(player) ? $"v{ver}" : $"{player}  ·  v{ver}";

                if (_state.TryGetProperty("status", out var st))
                {
                    var kind = Str(st, "kind", "idle");
                    var text = Str(st, "text", "");
                    _statusPill.SetStatus(text, kind switch
                    {
                        "live" => Red,
                        "busy" => Brand,
                        "warn" => Color.FromArgb(255, 159, 10),
                        _ => Green,
                    });
                }

                // Update localized nav tab labels
                if (_state.TryGetProperty("txt", out var txt))
                {
                    SetNavText("home", Str(txt, "home", "Home"));
                    SetNavText("stats", Str(txt, "report", "Stats"));
                    SetNavText("overlay", Str(txt, "overlay", "Overlay"));
                    SetNavText("settings", Str(txt, "settings", "Settings"));
                    SetNavText("help", Str(txt, "help", "Help"));
                }

                // Don't clobber textboxes while the user is typing in Settings
                if (!force && ActiveControl is TextBox)
                {
                    return;
                }

                RebuildTab();
            }
            catch { }
        }

        private void SetNavText(string key, string text)
        {
            if (_navButtons.TryGetValue(key, out var b))
            {
                b.Text = text;
            }
        }

        private void RebuildTab()
        {
            if (_state.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            var scrollPos = _contentHost.AutoScrollPosition;
            _contentHost.SuspendLayout();
            _contentHost.Controls.Clear();
            _thCountdownLbl = null;

            var stack = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Bg,
                Width = Math.Max(780, _contentHost.ClientSize.Width - 64),
                Location = new Point(28, 16),
            };
            var cardW = stack.Width;

            // Active Treasure Hour Alert Banner (shown on any tab)
            if (_state.TryGetProperty("treasureAlert", out var ta) && ta.ValueKind == JsonValueKind.Object)
            {
                var alertCard = MakeCard(cardW, accentBorder: true);
                var row = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, WrapContents = false, BackColor = Color.Transparent, Width = cardW - 40 };
                var info = new Panel { Width = cardW - 140, AutoSize = true, BackColor = Color.Transparent };
                var at = new Label { Text = Str(ta, "title", ""), Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Brand, AutoSize = true, MaximumSize = new Size(cardW - 150, 0) };
                var ax = new Label { Text = Str(ta, "text", ""), Font = new Font("Segoe UI", 9.5f), ForeColor = Ink, AutoSize = true, Location = new Point(0, 24), MaximumSize = new Size(cardW - 150, 0) };
                info.Controls.AddRange(new Control[] { at, ax });
                var dismiss = new PillButton("Dismiss", primary: false) { Width = 86 };
                var id = Str(ta, "id", "");
                dismiss.Click += (_, _) => Act("dismissTreasureAlert", id);
                row.Controls.AddRange(new Control[] { info, dismiss });
                alertCard.Controls.Add(row);
                stack.Controls.Add(alertCard);
            }

            switch (_activeTab)
            {
                case "home": BuildHome(stack, cardW); break;
                case "stats": BuildStats(stack, cardW); break;
                case "overlay": BuildOverlay(stack, cardW); break;
                case "settings": BuildSettings(stack, cardW); break;
                case "help": BuildHelp(stack, cardW); break;
            }

            _contentHost.Controls.Add(stack);
            _contentHost.ResumeLayout(true);
            _contentHost.AutoScrollPosition = new Point(-scrollPos.X, -scrollPos.Y);
        }

        // ---- HOME TAB -----------------------------------------------------------------------
        private void BuildHome(FlowLayoutPanel stack, int w)
        {
            var txt = _state.GetProperty("txt");

            // Update available banner
            if (_state.TryGetProperty("update", out var upd) && upd.ValueKind == JsonValueKind.Object)
            {
                var uCard = MakeCard(w, accentBorder: true);
                var ver = Str(upd, "version", "");
                uCard.Controls.Add(MakeHeading($"New version {ver} available"));
                var btn = new PillButton(Str(txt, "install", "Install update"), primary: true);
                btn.Click += (_, _) => Act("installUpdate");
                uCard.Controls.Add(btn);
                stack.Controls.Add(uCard);
            }

            // Progress bar while scanning replays
            if (_state.TryGetProperty("progress", out var prog) && prog.ValueKind == JsonValueKind.Object)
            {
                var pCard = MakeCard(w);
                var done = Int(prog, "done");
                var total = Math.Max(1, Int(prog, "total"));
                pCard.Controls.Add(MakeHeading($"{Str(txt, "analyzing", "Analyzing replays…")} {done} / {total}"));
                var bar = new System.Windows.Forms.ProgressBar { Width = w - 44, Height = 10, Maximum = total, Value = Math.Clamp(done, 0, total) };
                pCard.Controls.Add(bar);
                stack.Controls.Add(pCard);
            }

            // 4 Stat Tiles Grid
            var tiles = new TableLayoutPanel
            {
                ColumnCount = 4,
                RowCount = 1,
                Width = w,
                Height = 148,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 16),
            };
            for (var i = 0; i < 4; i++) tiles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            // Hero tile: Total Chests + Edit button
            var hero = MakeTile(accentBorder: true);
            var heroTop = new Panel { Dock = DockStyle.Top, Height = 26, BackColor = Color.Transparent };
            heroTop.Controls.Add(new Label { Text = Str(txt, "total", "Chests in total").ToUpperInvariant(), Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Brand, AutoSize = true, Location = new Point(0, 4) });
            var editBtn = new PillButton("Edit", primary: false) { Size = new Size(54, 26), Location = new Point(140, 0), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            editBtn.Click += (_, _) => OpenManualDialog();
            heroTop.Controls.Add(editBtn);
            var heroVal = new Label { Text = Str(_state, "total", "0"), Font = new Font("Segoe UI", 28f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Dock = DockStyle.Top };
            var heroSub = new Label { Text = Str(txt, "totalSub", "opened or destroyed by you"), Font = new Font("Segoe UI", 8.5f), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top };
            hero.Controls.Add(heroSub);
            hero.Controls.Add(heroVal);
            hero.Controls.Add(heroTop);
            tiles.Controls.Add(hero, 0, 0);

            // Session tile
            var sesTile = MakeTile();
            sesTile.Controls.Add(new Label { Text = $"{Str(txt, "session", "Session")} · {Str(_state, "sessionMatches", "0")} {Str(txt, "matches", "matches")}", Font = new Font("Segoe UI", 8.5f), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            sesTile.Controls.Add(new Label { Text = Str(_state, "session", "0"), Font = new Font("Segoe UI", 24f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Dock = DockStyle.Top });
            sesTile.Controls.Add(new Label { Text = "SESSION", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            tiles.Controls.Add(sesTile, 1, 0);

            // Last Match tile
            var lastVal = "—";
            var lastSub = Str(txt, "last", "Last match");
            if (_state.TryGetProperty("last", out var last) && last.ValueKind == JsonValueKind.Object)
            {
                lastVal = Str(last, "chests", "—");
                lastSub = $"{Str(last, "when", "")} · {Str(last, "mode", "")}";
            }
            var lastTile = MakeTile();
            lastTile.Controls.Add(new Label { Text = lastSub, Font = new Font("Segoe UI", 8.5f), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            lastTile.Controls.Add(new Label { Text = lastVal, Font = new Font("Segoe UI", 24f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Dock = DockStyle.Top });
            lastTile.Controls.Add(new Label { Text = "LAST MATCH", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            tiles.Controls.Add(lastTile, 2, 0);

            // Treasures tile
            var treTile = MakeTile();
            treTile.Controls.Add(new Label { Text = $"{Str(_state, "matches", "0")} {Str(txt, "matches", "matches")} · {Str(_state, "average", "0")}/match", Font = new Font("Segoe UI", 8.5f), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            treTile.Controls.Add(new Label { Text = Str(_state, "treasures", "0"), Font = new Font("Segoe UI", 24f, FontStyle.Bold), ForeColor = Brand, AutoSize = true, Dock = DockStyle.Top });
            treTile.Controls.Add(new Label { Text = "TREASURES", Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Muted, AutoSize = true, Dock = DockStyle.Top });
            tiles.Controls.Add(treTile, 3, 0);

            stack.Controls.Add(tiles);

            // Action Buttons Bar
            var actions = new FlowLayoutPanel { AutoSize = true, Width = w, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 16) };
            var openRep = new PillButton(Str(txt, "openReport", "Open full report"), primary: true);
            openRep.Click += (_, _) => OpenExternal(_reportPath);
            actions.Controls.Add(openRep);

            if (_state.TryGetProperty("overlay", out var ov) && Bool(ov, "running"))
            {
                var visBtn = new PillButton(Bool(ov, "visible") ? Str(txt, "hideOverlay", "Hide overlay") : Str(txt, "showOverlay", "Show overlay"), primary: false);
                visBtn.Click += (_, _) => Act("toggleOverlayVisible");
                var lockBtn = new PillButton(Bool(ov, "locked") ? Str(txt, "unlock", "Unlock overlay") : Str(txt, "lock", "Lock overlay"), primary: false);
                lockBtn.Click += (_, _) => Act("toggleOverlayLock");
                actions.Controls.Add(visBtn);
                actions.Controls.Add(lockBtn);
            }

            if (Bool(_state, "updatesOn"))
            {
                var chkBtn = new PillButton(Str(txt, "checkUpdate", "Check GitHub for updates"), primary: false);
                chkBtn.Click += (_, _) => Act("checkUpdate");
                actions.Controls.Add(chkBtn);
            }
            stack.Controls.Add(actions);

            // Treasure Hours Schedule Card
            if (_state.TryGetProperty("treasureHours", out var thList) && thList.ValueKind == JsonValueKind.Array && thList.GetArrayLength() > 0)
            {
                var thCard = MakeCard(w, accentBorder: true);
                var first = thList[0];
                _thStartUtc = DateTime.Parse(Str(first, "start", "")).ToUniversalTime();
                _thEndUtc = DateTime.Parse(Str(first, "end", "")).ToUniversalTime();

                var thHead = new Panel { Width = w - 44, Height = 30, BackColor = Color.Transparent };
                thHead.Controls.Add(new Label { Text = Str(txt, "thTitle", "Treasure Hours"), Font = new Font("Segoe UI", 12.5f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(0, 2) });
                _thCountdownLbl = new Label
                {
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    ForeColor = Brand,
                    AutoSize = true,
                    Location = new Point(w - 240, 5),
                };
                thHead.Controls.Add(_thCountdownLbl);
                thCard.Controls.Add(thHead);
                TickCountdown();

                if (first.TryGetProperty("boosts", out var boosts) && boosts.ValueKind == JsonValueKind.Array)
                {
                    var boostText = string.Join("   ·   ", boosts.EnumerateArray().Select(b => $"{Str(b, "name", "")} ×{b.GetProperty("mult").GetDouble():0.#}"));
                    thCard.Controls.Add(new Label { Text = boostText, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Brand, AutoSize = true, Margin = new Padding(0, 4, 0, 8) });
                }

                var today = DateTime.Now.Date;
                foreach (var h in thList.EnumerateArray())
                {
                    var sd = DateTime.Parse(Str(h, "start", "")).ToLocalTime();
                    var ed = DateTime.Parse(Str(h, "end", "")).ToLocalTime();
                    var tag = DateTime.Now >= sd && DateTime.Now < ed ? "  [LIVE]" : (sd.Date == today ? "  [TODAY]" : "");
                    var line = new Panel { Width = w - 44, Height = 28, BackColor = Color.Transparent };
                    line.Controls.Add(new Label { Text = sd.ToString("ddd, MMM d") + tag, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = tag.Contains("LIVE") ? Green : Ink, AutoSize = true, Location = new Point(0, 4) });
                    line.Controls.Add(new Label { Text = $"{sd:t} – {ed:t}", Font = new Font("Segoe UI", 9.5f), ForeColor = Muted, AutoSize = true, Location = new Point(w - 210, 4) });
                    thCard.Controls.Add(line);
                }

                thCard.Controls.Add(new Label { Text = Str(txt, "thLocal", "Times shown in your local timezone"), Font = new Font("Segoe UI", 8.5f), ForeColor = Muted, AutoSize = true, Margin = new Padding(0, 6, 0, 0) });
                stack.Controls.Add(thCard);
            }

            // Activity Log Card
            var logCard = MakeCard(w);
            logCard.Controls.Add(MakeHeading(Str(txt, "activity", "Activity")));
            var logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Width = w - 44,
                Height = 160,
                BackColor = Color.FromArgb(14, 14, 18),
                ForeColor = Color.FromArgb(205, 205, 214),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5f),
            };
            if (_state.TryGetProperty("activity", out var logArr) && logArr.ValueKind == JsonValueKind.Array)
            {
                var lines = logArr.EnumerateArray()
                    .Select(line => line.ValueKind == JsonValueKind.Array
                        ? string.Concat(line.EnumerateArray().Select(s => Str(s, "t", "")))
                        : "")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Reverse()
                    .Take(60);
                logBox.Text = string.Join(Environment.NewLine, lines);
            }
            if (string.IsNullOrWhiteSpace(logBox.Text))
            {
                logBox.Text = Str(txt, "emptyMatches", "No matches counted yet. Keep ChestCounter open while you play Fortnite.");
            }
            logCard.Controls.Add(logBox);
            stack.Controls.Add(logCard);
        }

        private void TickCountdown()
        {
            if (_thCountdownLbl == null || _thCountdownLbl.IsDisposed || _thStartUtc == default)
            {
                return;
            }
            var now = DateTime.UtcNow;
            var live = now >= _thStartUtc && now < _thEndUtc;
            var diff = live ? _thEndUtc - now : _thStartUtc - now;
            if (diff < TimeSpan.Zero) diff = TimeSpan.Zero;
            var fmt = diff.Days > 0 ? $"{diff.Days}d {diff.Hours}h {diff.Minutes}m"
                : diff.Hours > 0 ? $"{diff.Hours}h {diff.Minutes}m"
                : $"{diff.Minutes}m {diff.Seconds}s";
            _thCountdownLbl.ForeColor = live ? Green : Brand;
            _thCountdownLbl.Text = live ? $"LIVE · Ends in {fmt}" : $"Next in {fmt}";
        }

        // ---- STATS TAB ----------------------------------------------------------------------
        private void BuildStats(FlowLayoutPanel stack, int w)
        {
            var topCard = MakeCard(w);
            topCard.Controls.Add(MakeHeading("Match History & Odds"));
            var openBtn = new PillButton("Open interactive HTML report in browser", primary: true);
            openBtn.Click += (_, _) => OpenExternal(_reportPath);
            topCard.Controls.Add(openBtn);
            stack.Controls.Add(topCard);

            var listCard = MakeCard(w);
            listCard.Controls.Add(MakeHeading("Recent Matches"));
            var grid = new DataGridView
            {
                Width = w - 44,
                Height = 400,
                BackgroundColor = Surface,
                GridColor = Border,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Bg;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Ink;
            grid.DefaultCellStyle.SelectionBackColor = SurfaceHover;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);

            grid.Columns.Add("when", "Date");
            grid.Columns.Add("mode", "Mode");
            grid.Columns.Add("place", "Placement");
            grid.Columns.Add("chests", "Chests");
            grid.Columns.Add("treasures", "Treasures");

            if (_state.TryGetProperty("recentMatches", out var rm) && rm.ValueKind == JsonValueKind.Array)
            {
                foreach (var m in rm.EnumerateArray())
                {
                    grid.Rows.Add(Str(m, "when", ""), Str(m, "mode", ""), Str(m, "place", ""), Str(m, "chests", "0"), Str(m, "treasures", ""));
                }
            }
            listCard.Controls.Add(grid);
            stack.Controls.Add(listCard);
        }

        // ---- OVERLAY TAB --------------------------------------------------------------------
        private void BuildOverlay(FlowLayoutPanel stack, int w)
        {
            var txt = _state.GetProperty("txt");
            var ov = _state.GetProperty("overlay");

            var card = MakeCard(w);
            card.Controls.Add(MakeHeading(Str(txt, "overlay", "Overlay")));
            AddToggleRow(card, w - 44, Str(txt, "inGame", "In-game overlay"), Str(txt, "inGameHelp", ""), Bool(ov, "show"), v => Act("showOverlay", v));
            AddComboRow(card, w - 44, Str(txt, "style", "Style"), "", Str(ov, "style", "hud"),
                new[] { ("hud", Str(txt, "hud", "HUD (seamless)")), ("classic", Str(txt, "classic", "Classic card")) },
                v => Act("overlayStyle", v));

            if (Bool(ov, "show"))
            {
                var sizes = ov.GetProperty("sizes").EnumerateArray().Select(x => (x.GetInt32().ToString(), $"{x.GetInt32()} %")).ToArray();
                AddComboRow(card, w - 44, Str(txt, "size", "Size"), "", Int(ov, "size").ToString(), sizes, v => Act("overlaySize", int.Parse(v)));

                var opacities = ov.GetProperty("opacities").EnumerateArray().Select(x => (x.GetInt32().ToString(), $"{x.GetInt32()} %")).ToArray();
                AddComboRow(card, w - 44, Str(txt, "opacity", "Opacity"), "", Int(ov, "opacity").ToString(), opacities, v => Act("overlayOpacity", int.Parse(v)));
            }
            stack.Controls.Add(card);

            // OBS Browser Source Card
            var obsCard = MakeCard(w);
            obsCard.Controls.Add(MakeHeading(Str(txt, "obs", "OBS / Streaming")));
            var obsPath = _state.TryGetProperty("paths", out var p) ? Str(p, "overlay", "") : "";
            var pathBox = new TextBox { Text = obsPath, ReadOnly = true, Width = w - 44, BackColor = Bg, ForeColor = Ink, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Consolas", 9.5f) };
            obsCard.Controls.Add(pathBox);
            var copyBtn = new PillButton(Str(txt, "copy", "Copy path"), primary: true) { Margin = new Padding(0, 10, 0, 0) };
            copyBtn.Click += (_, _) => { try { Clipboard.SetText(obsPath); copyBtn.Text = Str(txt, "copied", "Copied!"); } catch { } };
            obsCard.Controls.Add(copyBtn);
            stack.Controls.Add(obsCard);
        }

        // ---- SETTINGS TAB -------------------------------------------------------------------
        private void BuildSettings(FlowLayoutPanel stack, int w)
        {
            var txt = _state.GetProperty("txt");
            var s = _state.GetProperty("settings");

            var card = MakeCard(w);
            card.Controls.Add(MakeHeading(Str(txt, "settings", "Settings")));

            var langs = s.GetProperty("languages").EnumerateArray().Select(x => (x[0].GetString()!, x[1].GetString()!)).ToArray();
            AddComboRow(card, w - 44, Str(txt, "language", "Language"), "", Str(s, "language", "en"), langs, v => Act("language", v));

            AddToggleRow(card, w - 44, Str(txt, "tray", "Minimize to system tray"), Str(txt, "trayHelp", ""), Bool(s, "minimizeToTray"), v => Act("minimizeToTray", v));
            if (Bool(s, "canStartWithFortnite"))
            {
                AddToggleRow(card, w - 44, Str(txt, "withFortnite", "Open with Fortnite"), Str(txt, "withFortniteHelp", ""), Bool(s, "startWithFortnite"), v => Act("startWithFortnite", v));
            }
            AddToggleRow(card, w - 44, Str(txt, "zapMode", "Zap mode"), Str(txt, "zapModeHelp", ""), Bool(s, "zapMode"), v => Act("zapMode", v));
            AddToggleRow(card, w - 44, Str(txt, "treasureAlerts", "Treasure Hour reminders"), Str(txt, "treasureAlertsHelp", ""), Bool(s, "treasureHourAlerts"), v => Act("treasureHourAlerts", v));
            AddToggleRow(card, w - 44, Str(txt, "discordStatus", "Discord status"), Str(txt, "discordStatusHelp", ""), Bool(s, "discordPresence"), v => Act("discordPresence", v));

            if (Bool(s, "discordPresence"))
            {
                AddTextInputRow(card, w - 44, Str(txt, "discordAppId", "Discord Application ID"), Str(s, "discordAppId", ""), Str(txt, "saveId", "Save ID"), v => Act("discordAppId", v));
            }

            AddComboRow(card, w - 44, Str(txt, "updates", "Updates"), "", Str(s, "updateMode", "auto"),
                new[] { ("auto", Str(txt, "upAuto", "Installed automatically")), ("prompt", Str(txt, "upPrompt", "Offered")), ("off", Str(txt, "upOff", "Off")) },
                v => Act("updateMode", v));

            AddTextInputRow(card, w - 44, Str(txt, "demos", "Replays folder"), Str(s, "demos", ""), Str(txt, "save", "Save folder"), v => Act("demosFolder", v));
            stack.Controls.Add(card);
        }

        // ---- HELP TAB -----------------------------------------------------------------------
        private void BuildHelp(FlowLayoutPanel stack, int w)
        {
            var txt = _state.GetProperty("txt");
            var card = MakeCard(w);
            card.Controls.Add(MakeHeading(Str(txt, "help", "Help")));
            card.Controls.Add(new Label
            {
                Text = Str(txt, "howText", "Reads Fortnite's saved .replay files in read-only mode after each match. Never touches Fortnite's memory, process, or game files — 100% Easy Anti-Cheat safe."),
                Font = new Font("Segoe UI", 10f),
                ForeColor = Ink,
                AutoSize = true,
                MaximumSize = new Size(w - 48, 0),
                Margin = new Padding(0, 0, 0, 14),
            });

            var actions = new FlowLayoutPanel { AutoSize = true, BackColor = Color.Transparent };
            var dataBtn = new PillButton(Str(txt, "openData", "Open data folder"), primary: false);
            dataBtn.Click += (_, _) => Act("openData");
            var repoBtn = new PillButton("GitHub", primary: false);
            repoBtn.Click += (_, _) => Act("openRepo");
            actions.Controls.Add(dataBtn);
            actions.Controls.Add(repoBtn);

            if (Bool(_state, "canUninstall"))
            {
                var unBtn = new PillButton(Str(txt, "uninstall", "Uninstall ChestCounter"), primary: false, danger: true);
                unBtn.Click += (_, _) =>
                {
                    if (MessageBox.Show(this, Str(txt, "uninstallConfirm", "Uninstall now?"), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Act("uninstall");
                    }
                };
                actions.Controls.Add(unBtn);
            }
            card.Controls.Add(actions);
            stack.Controls.Add(card);
        }

        // ---- MANUAL CHESTS DIALOG -----------------------------------------------------------
        private void OpenManualDialog()
        {
            using var dlg = new Form
            {
                Text = "Add chests manually",
                BackColor = Bg,
                ForeColor = Ink,
                Font = new Font("Segoe UI", 10f),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                ClientSize = new Size(400, 210),
            };
            dlg.HandleCreated += (_, _) => EnableDarkTitleBar(dlg.Handle);

            var lbl = new Label { Text = "Chests to add (use negative to subtract):", ForeColor = Ink, AutoSize = true, Location = new Point(20, 20) };
            var num = new NumericUpDown { Minimum = -1_000_000, Maximum = 10_000_000, Width = 356, Location = new Point(20, 50), BackColor = Surface, ForeColor = Ink };
            var weekChk = new CheckBox { Text = "Opened this week", ForeColor = Ink, AutoSize = true, Location = new Point(20, 92) };
            var addBtn = new PillButton("Add chests", primary: true) { Location = new Point(20, 140), Width = 140 };
            addBtn.Click += (_, _) =>
            {
                if (num.Value != 0)
                {
                    Act("manualAdd", new { chests = (int)num.Value, thisWeek = weekChk.Checked });
                    dlg.Close();
                }
            };
            dlg.Controls.AddRange(new Control[] { lbl, num, weekChk, addBtn });
            dlg.ShowDialog(this);
        }

        // ---- UI Building Helpers ------------------------------------------------------------
        private static FlowLayoutPanel MakeCard(int width, bool accentBorder = false) => new RoundedCardPanel(accentBorder)
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(width, 0),
            MaximumSize = new Size(width, 0),
            Padding = new Padding(20, 16, 20, 18),
            Margin = new Padding(0, 0, 0, 16),
        };

        private static Panel MakeTile(bool accentBorder = false) => new RoundedTilePanel(accentBorder)
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 14, 16, 14),
            Margin = new Padding(6, 0, 6, 0),
        };

        private static Label MakeHeading(string text) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10),
        };

        private static void AddToggleRow(FlowLayoutPanel card, int width, string title, string help, bool value, Action<bool> onChange)
        {
            var row = new Panel { Width = width, AutoSize = true, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 10) };
            var t = new Label { Text = title, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(0, 2) };
            var h = new Label { Text = help, Font = new Font("Segoe UI", 8.75f), ForeColor = Muted, AutoSize = true, Location = new Point(0, 24), MaximumSize = new Size(width - 80, 0) };
            var sw = new ToggleSwitch { Checked = value, Location = new Point(width - 54, 6) };
            sw.CheckedChanged += (_, _) => onChange(sw.Checked);
            row.Controls.Add(t);
            if (!string.IsNullOrEmpty(help)) row.Controls.Add(h);
            row.Controls.Add(sw);
            card.Controls.Add(row);
        }

        private static void AddComboRow(FlowLayoutPanel card, int width, string title, string help, string current, (string Value, string Label)[] options, Action<string> onChange)
        {
            var row = new Panel { Width = width, Height = string.IsNullOrEmpty(help) ? 44 : 62, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 6) };
            row.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(0, 8) });
            var cb = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 220,
                Location = new Point(width - 224, 6),
                BackColor = Bg,
                ForeColor = Ink,
                FlatStyle = FlatStyle.Flat,
            };
            var idx = 0;
            for (var i = 0; i < options.Length; i++)
            {
                cb.Items.Add(options[i].Label);
                if (string.Equals(options[i].Value, current, StringComparison.OrdinalIgnoreCase)) idx = i;
            }
            if (cb.Items.Count > 0) cb.SelectedIndex = idx;
            cb.SelectedIndexChanged += (_, _) =>
            {
                if (cb.SelectedIndex >= 0 && cb.SelectedIndex < options.Length)
                {
                    onChange(options[cb.SelectedIndex].Value);
                }
            };
            row.Controls.Add(cb);
            card.Controls.Add(row);
        }

        private static void AddTextInputRow(FlowLayoutPanel card, int width, string title, string value, string buttonText, Action<string> onSave)
        {
            var row = new Panel { Width = width, Height = 68, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 6) };
            row.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(0, 2) });
            var tb = new TextBox { Text = value, Width = width - 140, Location = new Point(0, 30), BackColor = Bg, ForeColor = Ink, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 9.5f) };
            var btn = new PillButton(buttonText, primary: false) { Location = new Point(width - 130, 26), Width = 126, Height = 32 };
            btn.Click += (_, _) => onSave(tb.Text);
            row.Controls.AddRange(new Control[] { tb, btn });
            card.Controls.Add(row);
        }

        private static void OpenExternal(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true })?.Dispose(); } catch { }
        }

        private static string Str(JsonElement e, string prop, string fallback) =>
            e.ValueKind == JsonValueKind.Object && e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() ?? fallback : fallback;

        private static bool Bool(JsonElement e, string prop) =>
            e.ValueKind == JsonValueKind.Object && e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.True;

        private static int Int(JsonElement e, string prop) =>
            e.ValueKind == JsonValueKind.Object && e.TryGetProperty(prop, out var v) && v.TryGetInt32(out var n) ? n : 0;
    }

    // ---- Custom-Painted Native Controls -----------------------------------------------------
    private static GraphicsPath RoundPath(RectangleF r, float radius)
    {
        var path = new GraphicsPath();
        var d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    private sealed class RoundedCardPanel : FlowLayoutPanel
    {
        private readonly bool _accent;
        public RoundedCardPanel(bool accent) { _accent = accent; DoubleBuffered = true; BackColor = Surface; }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Bg);
            using var path = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 14f);
            using var fill = new SolidBrush(Surface);
            using var pen = new Pen(_accent ? Color.FromArgb(110, 245, 158, 11) : Border, 1f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }
    }

    private sealed class RoundedTilePanel : Panel
    {
        private readonly bool _accent;
        public RoundedTilePanel(bool accent) { _accent = accent; DoubleBuffered = true; BackColor = Surface; }
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Bg);
            using var path = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 14f);
            using var fill = new SolidBrush(Surface);
            using var pen = new Pen(_accent ? Color.FromArgb(130, 245, 158, 11) : Border, 1f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }
    }

    private sealed class PillButton : Button
    {
        private readonly bool _primary, _danger;
        private bool _hover;

        public PillButton(string text, bool primary, bool danger = false)
        {
            _primary = primary;
            _danger = danger;
            Text = text;
            Height = 36;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(14, 0, 14, 0);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            Margin = new Padding(0, 0, 10, 0);
            DoubleBuffered = true;
            MouseEnter += (_, _) => { _hover = true; Invalidate(); };
            MouseLeave += (_, _) => { _hover = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Parent?.BackColor ?? Bg);

            var bg = _primary
                ? (_hover ? Color.FromArgb(251, 191, 36) : Brand)
                : _danger
                    ? (_hover ? Color.FromArgb(75, 255, 69, 58) : Color.FromArgb(30, 255, 69, 58))
                    : (_hover ? SurfaceHover : Surface);
            var border = _primary ? Brand : _danger ? Red : Border;
            var fg = _primary ? BrandDark : _danger ? Red : Ink;

            using var path = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 10f);
            using var fill = new SolidBrush(bg);
            using var pen = new Pen(border, 1f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);

            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var brush = new SolidBrush(fg);
            g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), fmt);
        }
    }

    private sealed class NavButton : Control
    {
        private bool _active, _hover;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Active { get => _active; set { _active = value; Invalidate(); } }

        public NavButton(string text, bool active)
        {
            Text = text;
            _active = active;
            Size = new Size(116, 32);
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            DoubleBuffered = true;
            Margin = new Padding(2, 0, 2, 0);
            MouseEnter += (_, _) => { _hover = true; Invalidate(); };
            MouseLeave += (_, _) => { _hover = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Surface);

            if (_active || _hover)
            {
                using var path = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 8f);
                using var fill = new SolidBrush(_active ? Brand : SurfaceHover);
                g.FillPath(fill, path);
            }
            using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var brush = new SolidBrush(_active ? BrandDark : (_hover ? Color.White : Muted));
            g.DrawString(Text, Font, brush, new RectangleF(0, 0, Width, Height), fmt);
        }
    }

    private sealed class ToggleSwitch : Control
    {
        private bool _checked;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool Checked { get => _checked; set { if (_checked == value) return; _checked = value; Invalidate(); CheckedChanged?.Invoke(this, EventArgs.Empty); } }
        public event EventHandler? CheckedChanged;

        public ToggleSwitch()
        {
            Size = new Size(48, 26);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            Click += (_, _) => Checked = !Checked;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? Surface);
            using var track = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 13f);
            using var fill = new SolidBrush(_checked ? Brand : Color.FromArgb(46, 46, 56));
            g.FillPath(fill, track);

            var knobX = _checked ? Width - 22f : 3f;
            using var knob = new SolidBrush(Color.White);
            g.FillEllipse(knob, knobX, 3f, 20f, 20f);
        }
    }

    private sealed class StatusPill : Control
    {
        private string _text = "";
        private Color _dot = Green;

        public StatusPill()
        {
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        }

        public void SetStatus(string text, Color dot)
        {
            _text = text;
            _dot = dot;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Bg);
            using var path = RoundPath(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 15f);
            using var fill = new SolidBrush(Surface);
            using var pen = new Pen(Border, 1f);
            g.FillPath(fill, path);
            g.DrawPath(pen, path);

            using var dotBrush = new SolidBrush(_dot);
            g.FillEllipse(dotBrush, 12f, (Height - 8f) / 2f, 8f, 8f);
            using var textBrush = new SolidBrush(Ink);
            using var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            g.DrawString(_text, Font, textBrush, new RectangleF(26, 0, Width - 34, Height), fmt);
        }
    }
}
