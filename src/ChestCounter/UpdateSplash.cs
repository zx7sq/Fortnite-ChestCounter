using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace ChestCounter;

/// <summary>
/// Discord-style startup updater splash box with a spinning chest icon and smooth download progress bar.
/// </summary>
[SupportedOSPlatform("windows")]
public static class UpdateSplash
{
    private static Form? _form;
    private static Thread? _thread;
    private static volatile string _status = "";
    private static volatile int _progress = -1; // -1 = indeterminate, 0..100 = determinate

    public static void Show(string initialStatus)
    {
        if (_form != null)
        {
            SetStatus(initialStatus, -1);
            return;
        }
        _status = initialStatus;
        _progress = -1;

        var ready = new ManualResetEventSlim();
        _thread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            var form = new SplashForm();
            _form = form;
            form.Shown += (_, _) => ready.Set();
            Application.Run(form);
            _form = null;
        })
        {
            IsBackground = true,
            Name = "UpdateSplash",
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        ready.Wait(TimeSpan.FromSeconds(5));
    }

    public static void SetStatus(string status, int progress = -1)
    {
        _status = status;
        _progress = progress;
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
                try { form.Close(); } catch { }
            });
            _thread?.Join(TimeSpan.FromSeconds(2));
        }
        catch { }
        _form = null;
        _thread = null;
    }

    private sealed class SplashForm : Form
    {
        private readonly System.Windows.Forms.Timer _timer;
        private readonly Bitmap? _chest;
        private float _angle;
        private float _indetPos;

        public SplashForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            TopMost = true;
            DoubleBuffered = true;
            ClientSize = new Size(300, 330);
            BackColor = Color.FromArgb(11, 11, 14);
            Text = "Fortnite Chest Counter";

            try
            {
                if (AppInfo.ExePath is { } exe)
                {
                    Icon = Icon.ExtractAssociatedIcon(exe);
                }
                using var stream = typeof(UpdateSplash).Assembly.GetManifestResourceStream("ChestIcon.png");
                if (stream != null)
                {
                    _chest = new Bitmap(stream);
                }
            }
            catch { }

            using var rgnPath = RoundedRect(new RectangleF(0, 0, Width, Height), 20);
            Region = new Region(rgnPath);

            _timer = new System.Windows.Forms.Timer { Interval = 16 };
            _timer.Tick += (_, _) =>
            {
                _angle = (_angle + 3.2f) % 360f;
                _indetPos = (_indetPos + 0.025f) % 1.4f;
                Invalidate();
            };
            _timer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Stop();
                _timer.Dispose();
                _chest?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Flat dark card + subtle border
            g.Clear(Color.FromArgb(11, 11, 14));
            using var borderPath = RoundedRect(new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f), 20);
            using var borderPen = new Pen(Color.FromArgb(46, 255, 255, 255), 1f);
            g.DrawPath(borderPen, borderPath);

            // Subtle amber halo ring behind spinning chest
            var cx = Width / 2f;
            var cy = 112f;
            using var ringPen = new Pen(Color.FromArgb(55, 245, 158, 11), 2f);
            g.DrawEllipse(ringPen, cx - 54f, cy - 54f, 108f, 108f);

            // Spinning chest icon
            if (_chest != null)
            {
                var state = g.Save();
                g.TranslateTransform(cx, cy);
                g.RotateTransform(_angle);
                g.DrawImage(_chest, -42f, -42f, 84f, 84f);
                g.Restore(state);
            }

            // App title
            using var titleFont = new Font("Segoe UI", 13.5f, FontStyle.Bold);
            using var statusFont = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            using var centerFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var whiteBrush = new SolidBrush(Color.FromArgb(245, 245, 247));
            using var mutedBrush = new SolidBrush(Color.FromArgb(142, 142, 147));

            g.DrawString("Fortnite Chest Counter", titleFont, whiteBrush, new RectangleF(20, 186, Width - 40, 28), centerFmt);
            g.DrawString(_status, statusFont, mutedBrush, new RectangleF(20, 218, Width - 40, 24), centerFmt);

            // Progress bar track
            var barX = 36f;
            var barY = 264f;
            var barW = Width - 72f;
            var barH = 8f;
            using var trackPath = RoundedRect(new RectangleF(barX, barY, barW, barH), 4f);
            using var trackBrush = new SolidBrush(Color.FromArgb(30, 255, 255, 255));
            g.FillPath(trackBrush, trackPath);

            // Progress bar fill (amber #F59E0B)
            var pct = _progress;
            using var fillBrush = new SolidBrush(Color.FromArgb(245, 158, 11));
            g.SetClip(trackPath);
            if (pct >= 0)
            {
                var fw = Math.Max(barH, barW * Math.Clamp(pct, 0, 100) / 100f);
                using var fillPath = RoundedRect(new RectangleF(barX, barY, fw, barH), 4f);
                g.FillPath(fillBrush, fillPath);
            }
            else
            {
                var segW = barW * 0.36f;
                var segX = barX - segW + _indetPos * (barW + segW);
                using var fillPath = RoundedRect(new RectangleF(segX, barY, segW, barH), 4f);
                g.FillPath(fillBrush, fillPath);
            }
            g.ResetClip();

            // Version footer
            using var verFont = new Font("Segoe UI", 8.25f, FontStyle.Regular);
            using var dimBrush = new SolidBrush(Color.FromArgb(96, 96, 104));
            g.DrawString($"v{AppInfo.VersionText}", verFont, dimBrush, new RectangleF(20, 290, Width - 40, 20), centerFmt);
        }

        private static GraphicsPath RoundedRect(RectangleF r, float radius)
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
    }
}
