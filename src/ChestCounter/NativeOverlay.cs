using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ChestCounter;

/// <summary>
/// Small always-on-top counter window drawn with plain Win32/GDI (no WinForms needed).
/// Left-drag to move it, right-click to hide it (Ctrl+Shift+O or the counter's menu shows it again).
/// Visible over Fortnite when the game runs in "windowed fullscreen" (not exclusive fullscreen).
/// </summary>
/// <summary>What the overlay shows. <see cref="Current"/> null = no match in progress.</summary>
/// <param name="Detail">Optional secondary text drawn small and grey after the value.</param>
public sealed record OverlayRow(string Label, string Value, bool Treasure = false, string? Detail = null);

/// <param name="Current">Right counter (match in progress, or last match); null = "—".</param>
/// <param name="CurrentLive">Right counter is the match in progress (green).</param>
/// <param name="Badge">Header badge ("● EN DIRECT" / "● LIVE", "● PARTIE EN COURS" / "● MATCH IN PROGRESS") or null.</param>
public sealed record OverlayView(string Total, string? Current, string CurrentCaption, bool CurrentLive, string? Badge, IReadOnlyList<OverlayRow> Rows, string Treasures = "0", string Session = "0", string? Last = null, string? Squad = null);

[SupportedOSPlatform("windows")]
public static class NativeOverlay
{
    private static Thread? _thread;
    private static IntPtr _hwnd;
    private static volatile OverlayView _view = new("0", null, OverlayModel.LastMatchCaption, false, null, Array.Empty<OverlayRow>());
    private static OverlayView _displayView = new("0", null, OverlayModel.LastMatchCaption, false, null, Array.Empty<OverlayRow>());
    private static bool _hasInitialView, _animating, _hasSavedPosition;
    private static long _animStartMs;
    private static int _fromTotal, _toTotal, _fromSession, _toSession, _fromTreasures, _toTreasures, _fromLast, _toLast;
    private const long AnimDurationMs = 550;
    private static readonly UIntPtr AnimTimerId = new(1);

    // Layout (computed by Relayout, used by Paint; both on the overlay thread).
    private static int _headerH, _bigH, _captionH, _rowH, _col1W, _col2W, _labelW;
    private static IntPtr _fontHeader, _fontCaption, _fontLabel;
    private static IntPtr _brushBorder, _brushLine;
    private static string Header => L.T("COFFRES", "CHESTS", "COFRES");
    private const string CaptionTotal = "total";
    private static Action<int, int>? _onMoved;
    private static Action<bool>? _onLockChanged;
    private static volatile bool _locked;
    private static bool _lastEffectiveLocked;
    private static bool EffectiveLocked => _locked || _view.CurrentLive;
    private static bool _offscreen;
    private static int _stableW;
    private static RECT _lockRect; // lock button, client coordinates
    private static IntPtr _fontIcon;
    private static readonly WndProcDelegate _wndProc = WndProc; // keep the delegate alive for the lifetime of the process
    private static IntPtr _fontBig, _fontSmall, _brushBg;
    private static float _scale = 1f;    // display scaling × size chosen by the player
    private static float _dpiScale = 1f; // display scaling only
    private static volatile int _sizePercent = 100;
    private static volatile int _opacityPercent = 100;

    /// <summary>"hud" (figures only, under the minimap, like the game's HUD) or "classic" (dark card).</summary>
    private static volatile string _style = "classic";
    private static IntPtr _fontHudBig, _fontHudSmall;
    private static int _hudBigH, _hudSmallH;

    /// <summary>Pixels of this color are fully transparent (styles without a background). Almost black, so that the
    /// anti-aliased edge of the black outline around the text blends into it without any colored fringe.</summary>
    private static readonly uint KeyColor = 0x00010001;

    public static string[] Styles => new[] { "hud", "classic" };

    /// <summary>Changes the look of the overlay at once.</summary>
    public static void SetStyle(string style)
    {
        _style = Array.IndexOf(Styles, style) >= 0 ? style : "hud";
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_APPEARANCE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static bool Keyed => _style != "classic";

    private const int MinWidth = 240, Pad = 16, ColGap = 36;

    /// <summary>Size and opacity of the overlay (percent). Applied at once when the overlay is shown.</summary>
    public static void SetAppearance(int sizePercent, int opacityPercent)
    {
        _sizePercent = OverlayAppearance.ClampSize(sizePercent);
        _opacityPercent = OverlayAppearance.ClampOpacity(opacityPercent);
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_APPEARANCE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static byte Alpha => (byte)Math.Round(_opacityPercent * 255 / 100.0);

    public static bool IsRunning => _hwnd != IntPtr.Zero;

    /// <summary>True when the overlay window is shown (not hidden with the shortcut or the menu).</summary>
    public static bool IsVisible => _hwnd != IntPtr.Zero && IsWindowVisible(_hwnd);

    /// <summary>Closes the overlay window and its thread (overlay turned off in the settings). Start() can open it again.</summary>
    public static void Stop()
    {
        var thread = _thread;
        if (thread == null)
        {
            return;
        }
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_STOP, IntPtr.Zero, IntPtr.Zero);
        }
        thread.Join(TimeSpan.FromSeconds(3));
    }

    /// <summary>True when the global Ctrl+Shift+O shortcut could be registered.</summary>
    public static bool HotkeyRegistered { get; private set; }

    /// <summary>True when the global Ctrl+Shift+L shortcut (lock/unlock) could be registered.</summary>
    public static bool LockHotkeyRegistered { get; private set; }

    /// <summary>Locked = click-through: the mouse goes through the overlay and it cannot be moved.</summary>
    public static bool IsLocked => _locked;

    public static void Start(int? x, int? y, bool locked, Action<int, int> onMoved, Action<bool> onLockChanged)
    {
        if (_thread != null)
        {
            return;
        }

        _onMoved = onMoved;
        _offscreen = x <= -10000; // --overlay-preview: never brought back on screen
        _onLockChanged = onLockChanged;
        _locked = locked;
        var ready = new ManualResetEventSlim();
        _thread = new Thread(() =>
        {
            try
            {
                Run(x, y, ready);
            }
            catch (Exception ex)
            {
                Console.WriteLine(L.T($"Overlay indisponible : {ex.Message}", $"Overlay unavailable: {ex.Message}", $"Overlay no disponible: {ex.Message}"));
                ready.Set();
            }
            finally
            {
                _hwnd = IntPtr.Zero;
                HotkeyRegistered = LockHotkeyRegistered = false;
                _thread = null;
            }
        })
        {
            IsBackground = true,
            Name = "Overlay",
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        ready.Wait(TimeSpan.FromSeconds(5));
    }

    /// <summary>Replaces what is shown. The window resizes itself to fit.</summary>
    public static void Update(OverlayView view)
    {
        _view = view;
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_RELAYOUT, IntPtr.Zero, IntPtr.Zero);
        }
    }

    /// <summary>Saves the overlay as it is drawn to a PNG file (works off-screen). Used by --overlay-preview.</summary>
    public static bool Snapshot(string path)
    {
        if (_hwnd == IntPtr.Zero || !GetWindowRect(_hwnd, out var r))
        {
            return false;
        }
        int w = r.Right - r.Left, h = r.Bottom - r.Top;
        if (Keyed)
        {
            using var hudBmp = RenderHudBitmap(w, h);
            hudBmp.Save(path, ImageFormat.Png);
            return true;
        }
        var screen = GetDC(IntPtr.Zero);
        var mem = CreateCompatibleDC(screen);
        var bmp = CreateCompatibleBitmap(screen, w, h);
        var old = SelectObject(mem, bmp);
        var ok = PrintWindow(_hwnd, mem, 1 /* PW_CLIENTONLY: WM_PRINT -> WM_PRINTCLIENT */);
        SelectObject(mem, old);
        var bi = new BITMAPINFOHEADER { biSize = 40, biWidth = w, biHeight = -h, biPlanes = 1, biBitCount = 32 };
        var bgra = new byte[w * h * 4];
        GetDIBits(mem, bmp, 0, (uint)h, bgra, ref bi, 0);
        DeleteObject(bmp);
        DeleteDC(mem);
        ReleaseDC(IntPtr.Zero, screen);
        if (ok)
        {
            Png.Write(path, w, h, bgra);
        }
        return ok;
    }

    public static void ToggleLock()
    {
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_LOCK, IntPtr.Zero, IntPtr.Zero);
        }
    }

    public static void Toggle()
    {
        if (_hwnd != IntPtr.Zero)
        {
            PostMessageW(_hwnd, WM_APP_TOGGLE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static void Run(int? x, int? y, ManualResetEventSlim ready)
    {
        var screenDc = GetDC(IntPtr.Zero);
        _dpiScale = GetDeviceCaps(screenDc, LOGPIXELSY) / 96f;
        ReleaseDC(IntPtr.Zero, screenDc);

        int S(int v) => (int)Math.Round(v * _scale);

        CreateFonts();
        _brushBg = CreateSolidBrush(Rgb(10, 10, 13));
        _brushBorder = CreateSolidBrush(Rgb(46, 46, 54));
        _brushLine = CreateSolidBrush(Rgb(30, 30, 36));
        try
        {
            var hInstance = GetModuleHandleW(null);
            var wc = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProc),
                hInstance = hInstance,
                hCursor = LoadCursorW(IntPtr.Zero, IDC_SIZEALL),
                hbrBackground = IntPtr.Zero,
                lpszClassName = "ChestCounterOverlay",
            };
            // Already registered when the overlay is turned off and on again in the same run (ERROR_CLASS_ALREADY_EXISTS).
            if (RegisterClassExW(ref wc) == 0 && Marshal.GetLastWin32Error() != 1410)
            {
                throw new InvalidOperationException("RegisterClassEx failed");
            }

            int w = S(MinWidth), h = S(90);
            _hasSavedPosition = x.HasValue && y.HasValue;
            var posX = x ?? S(24);
            var posY = y ?? S(24);

            _hwnd = CreateWindowExW(
                // Opaque (no WS_EX_LAYERED alpha): nothing for the compositor to blend every frame.
                WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE,
                wc.lpszClassName, L.T("Coffres", "Chests", "Cofres"), WS_POPUP,

                posX, posY, w, h, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
            if (_hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("CreateWindowEx failed");
            }

            var winDpi = GetDpiForWindow(_hwnd);
            if (winDpi > 0 && Math.Abs(winDpi / 96f - _dpiScale) > 0.01f)
            {
                _dpiScale = winDpi / 96f;
                CreateFonts();
            }

            ApplyLock(_hwnd, _locked, notify: false);
            Relayout(_hwnd);
            ShowWindow(_hwnd, SW_SHOWNOACTIVATE);
            // No timer at all: the thread sleeps in GetMessage and only wakes up when the counter changes,
            // when you drag/hide the window or press the shortcut.
            // Global shortcut, works even when the game has the focus.
            HotkeyRegistered = RegisterHotKey(_hwnd, 1, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, VK_O);
            LockHotkeyRegistered = RegisterHotKey(_hwnd, 2, MOD_CONTROL | MOD_SHIFT | MOD_NOREPEAT, VK_L);
            ready.Set();

            while (GetMessageW(out var msg, IntPtr.Zero, 0, 0) > 0)
            {
                TranslateMessage(ref msg);
                DispatchMessageW(ref msg);
            }
        }
        finally
        {
            foreach (var obj in new[] { _fontBig, _fontSmall, _fontHeader, _fontCaption, _fontLabel, _fontIcon, _fontHudBig, _fontHudSmall, _brushBg, _brushBorder, _brushLine })
            {
                if (obj != IntPtr.Zero) DeleteObject(obj);
            }
            _fontBig = _fontSmall = _fontHeader = _fontCaption = _fontLabel = _fontIcon = _fontHudBig = _fontHudSmall = IntPtr.Zero;
            _brushBg = _brushBorder = _brushLine = IntPtr.Zero;
        }
    }

    /// <summary>(Re)creates the fonts for the current display scaling and chosen size.</summary>
    private static void CreateFonts()
    {
        foreach (var old in new[] { _fontBig, _fontSmall, _fontHeader, _fontCaption, _fontLabel, _fontIcon, _fontHudBig, _fontHudSmall })
        {
            if (old != IntPtr.Zero) DeleteObject(old);
        }
        _scale = _dpiScale * _sizePercent / 100f;
        int S(int v) => (int)Math.Round(v * _scale);
        _fontBig = CreateFontW(-S(38), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI Variable Display");
        _fontSmall = CreateFontW(-S(14), 0, 0, 0, 600, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI Variable Text");
        _fontHeader = CreateFontW(-S(12), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI Variable Text");
        _fontCaption = CreateFontW(-S(11), 0, 0, 0, 600, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI Variable Text");
        _fontLabel = CreateFontW(-S(13), 0, 0, 0, 500, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe UI Variable Text");
        _fontIcon = CreateFontW(-S(14), 0, 0, 0, 400, 0, 0, 0, 1, 0, 0, 5, 0, "Segoe MDL2 Assets");
        // Clean Apple-style HUD figures: grayscale anti-aliasing (ClearType would leave colored edges once the
        // background is made transparent).
        _fontHudBig = CreateFontW(-S(40), 0, 0, 0, 700, 0, 0, 0, 1, 0, 0, 4, 0, "Segoe UI Variable Display");
        _fontHudSmall = CreateFontW(-S(18), 0, 0, 0, 600, 0, 0, 0, 1, 0, 0, 4, 0, "Segoe UI Variable Text");
    }

    private static IntPtr WndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_NCHITTEST:
            {
                if (EffectiveLocked)
                {
                    return (IntPtr)(-1); // HTTRANSPARENT: never capture clicks or drags while locked or tracking live
                }
                // Lock button = normal client area (clickable); everywhere else = caption (drag to move).
                // When locked the window is WS_EX_TRANSPARENT, so it never even receives this message.
                GetWindowRect(hwnd, out var wr);
                var px = (short)((long)lParam & 0xFFFF) - wr.Left;
                var py = (short)(((long)lParam >> 16) & 0xFFFF) - wr.Top;
                var onButton = px >= _lockRect.Left && px < _lockRect.Right && py >= _lockRect.Top && py < _lockRect.Bottom;
                return (IntPtr)(onButton ? HTCLIENT : HTCAPTION);
            }

            case WM_SETCURSOR:
                if (((long)lParam & 0xFFFF) == HTCLIENT)
                {
                    SetCursor(LoadCursorW(IntPtr.Zero, IDC_HAND));
                    return (IntPtr)1;
                }
                break;

            case WM_LBUTTONUP:
                ApplyLock(hwnd, true, notify: true);
                return IntPtr.Zero;

            case WM_APP_LOCK:
                ApplyLock(hwnd, !_locked, notify: true);
                return IntPtr.Zero;

            case WM_NCRBUTTONUP:
                SetVisible(hwnd, false);
                return IntPtr.Zero;

            case WM_HOTKEY:
                if ((long)wParam == 2)
                {
                    ApplyLock(hwnd, !_locked, notify: true);
                }
                else
                {
                    SetVisible(hwnd, !IsWindowVisible(hwnd));
                }
                return IntPtr.Zero;

            case WM_EXITSIZEMOVE:
                if (GetWindowRect(hwnd, out var r))
                {
                    _onMoved?.Invoke(r.Left, r.Top);
                }
                return IntPtr.Zero;

            case WM_APP_STOP:
                DestroyWindow(hwnd);
                return IntPtr.Zero;

            case WM_APP_TOGGLE:
                SetVisible(hwnd, !IsWindowVisible(hwnd));
                return IntPtr.Zero;

            case WM_APP_RELAYOUT:
                if (EffectiveLocked != _lastEffectiveLocked)
                {
                    ApplyLock(hwnd, _locked, notify: false);
                }
                StartNumberAnimation(hwnd);
                Relayout(hwnd);
                return IntPtr.Zero;

            case WM_TIMER:
                if (wParam == (IntPtr)1 && _animating)
                {
                    StepNumberAnimation(hwnd);
                }
                return IntPtr.Zero;

            case WM_APP_APPEARANCE:
                CreateFonts();
                _stableW = 0; // the width may shrink when the overlay gets smaller
                ApplyLock(hwnd, _locked, notify: false); // also applies the opacity
                Relayout(hwnd);
                return IntPtr.Zero;

            case WM_DPICHANGED:
                var newDpi = (uint)((long)wParam & 0xFFFF);
                if (newDpi > 0)
                {
                    _dpiScale = newDpi / 96f;
                    CreateFonts();
                    _stableW = 0;
                    Relayout(hwnd);
                }
                return IntPtr.Zero;

            case WM_PAINT:
                Paint(hwnd);
                return IntPtr.Zero;

            case WM_PRINTCLIENT: // snapshot (--overlay-preview): draw into the given DC
                PaintTo(hwnd, wParam);
                return IntPtr.Zero;

            case WM_DESTROY:
                if (_animating)
                {
                    KillTimer(hwnd, AnimTimerId);
                    _animating = false;
                }
                UnregisterHotKey(hwnd, 1);
                UnregisterHotKey(hwnd, 2);
                PostQuitMessage(0);
                return IntPtr.Zero;
        }
        return DefWindowProcW(hwnd, msg, wParam, lParam);
    }

    /// <summary>
    /// Locked: click-through (WS_EX_LAYERED + WS_EX_TRANSPARENT). Unlocked: normal window. Layered also when the
    /// player made it see-through; a fully opaque unlocked overlay stays a plain window (nothing to blend).
    /// </summary>
    private static void ApplyLock(IntPtr hwnd, bool locked, bool notify)
    {
        _locked = locked;
        var effective = EffectiveLocked;
        _lastEffectiveLocked = effective;
        var alpha = Alpha;
        var layered = effective || alpha < 255 || Keyed;
        var ex = (long)GetWindowLongPtrW(hwnd, GWL_EXSTYLE);
        if (Keyed)
        {
            if ((ex & WS_EX_LAYERED) != 0)
            {
                SetWindowLongPtrW(hwnd, GWL_EXSTYLE, (IntPtr)(ex & ~(long)WS_EX_LAYERED));
            }
            ex |= WS_EX_LAYERED;
            ex = effective ? ex | WS_EX_TRANSPARENT : ex & ~(long)WS_EX_TRANSPARENT;
            SetWindowLongPtrW(hwnd, GWL_EXSTYLE, (IntPtr)ex);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_FRAMECHANGED);
            UpdateHudLayeredWindow(hwnd);
        }
        else
        {
            ex = layered ? ex | WS_EX_LAYERED : ex & ~(long)WS_EX_LAYERED;
            ex = effective ? ex | WS_EX_TRANSPARENT : ex & ~(long)WS_EX_TRANSPARENT;
            SetWindowLongPtrW(hwnd, GWL_EXSTYLE, (IntPtr)ex);
            if (layered)
            {
                SetLayeredWindowAttributes(hwnd, KeyColor, alpha, LWA_ALPHA);
            }
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_FRAMECHANGED);
            InvalidateRect(hwnd, IntPtr.Zero, true);
        }

        if (notify)
        {
            _onLockChanged?.Invoke(locked);
            Console.WriteLine(locked
                ? L.T($"Overlay verrouillé : la souris passe au travers. {(LockHotkeyRegistered ? "Ctrl+Maj+L" : "« Déverrouiller l'overlay » dans le compteur")} pour le déverrouiller.",
                      $"Overlay locked: the mouse goes through it. {(LockHotkeyRegistered ? "Ctrl+Shift+L" : "\"Unlock the overlay\" in the counter")} to unlock it.",
                      $"Overlay bloqueado: el ratón lo atraviesa. {(LockHotkeyRegistered ? "Ctrl+Mayús+L" : "«Desbloquear el overlay» en el contador")} para desbloquearlo.")
                : L.T("Overlay déverrouillé : clic gauche maintenu pour le déplacer, cadenas pour le reverrouiller.",
                      "Overlay unlocked: hold the left mouse button to move it, click the padlock to lock it again.",
                      "Overlay desbloqueado: arrástralo con el clic izquierdo; el candado lo vuelve a bloquear."));
        }
    }

    private static void SetVisible(IntPtr hwnd, bool visible)
    {
        if (visible)
        {
            ShowWindow(hwnd, SW_SHOWNOACTIVATE);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }
        else
        {
            ShowWindow(hwnd, SW_HIDE);
            Console.WriteLine(HotkeyRegistered
                ? L.T("Overlay masqué : Ctrl+Maj+O (n'importe où) pour le réafficher.", "Overlay hidden: Ctrl+Shift+O (anywhere) to show it again.", "Overlay oculto: Ctrl+Mayús+O (desde cualquier sitio) para volver a mostrarlo.")
                : L.T("Overlay masqué : « Afficher l'overlay » dans le compteur pour le réafficher.", "Overlay hidden: \"Show the overlay\" in the counter to show it again.", "Overlay oculto: «Mostrar el overlay» en el contador para volver a mostrarlo."));
        }
    }

    private static int TextWidth(IntPtr hdc, IntPtr font, string text, out int height)
    {
        SelectObject(hdc, font);
        var r = new RECT();
        DrawTextW(hdc, text.Length == 0 ? " " : text, -1, ref r, DT_CALCRECT | DT_SINGLELINE | DT_NOPREFIX);
        height = r.Bottom - r.Top;
        return r.Right - r.Left;
    }

    /// <summary>Measures the text and resizes the window (and its rounded region) to fit it.</summary>
    private static void Relayout(IntPtr hwnd)
    {
        if (Keyed)
        {
            RelayoutStyled(hwnd);
            return;
        }
        int S(int v) => (int)Math.Round(v * _scale);
        var v = _view;
        var hdc = GetDC(hwnd);
        try
        {
            // Header: "COFFRES" ... "● EN DIRECT" [lock]
            var headerW = TextWidth(hdc, _fontHeader, Header, out _headerH) + S(16) + (v.Badge != null ? TextWidth(hdc, _fontHeader, v.Badge, out _) : 0) + S(30);

            // Two counters side by side.
            _col1W = Math.Max(TextWidth(hdc, _fontBig, v.Total, out _bigH), TextWidth(hdc, _fontCaption, CaptionTotal, out _captionH));
            _col2W = Math.Max(TextWidth(hdc, _fontBig, v.Current ?? "—", out _), Math.Max(TextWidth(hdc, _fontCaption, v.CurrentCaption, out _), TextWidth(hdc, _fontCaption, OverlayModel.LastMatchCaption, out _)));
            var countersW = _col1W + S(ColGap) + _col2W;

            // Detail rows: grey label column + value.
            _labelW = 0;
            var valueW = 0;
            _rowH = 0;
            foreach (var row in v.Rows)
            {
                _labelW = Math.Max(_labelW, TextWidth(hdc, _fontLabel, row.Label, out var lh));
                valueW = Math.Max(valueW, TextWidth(hdc, _fontSmall, row.Value, out var vh)
                    + (row.Detail != null ? S(6) + TextWidth(hdc, _fontCaption, row.Detail, out _) : 0));
                _rowH = Math.Max(_rowH, Math.Max(lh, vh));
            }
            var rowsW = v.Rows.Count > 0 ? _labelW + S(12) + valueW : 0;

            // Never shrinks: the overlay keeps the same size between a match in progress and the end of the match.
            var w = Math.Max(_stableW, Math.Max(S(MinWidth), Math.Max(headerW, Math.Max(countersW, rowsW)) + 2 * S(Pad)));
            _stableW = w;
            var h = S(12) + _headerH + S(6) + _bigH + _captionH + S(12);
            if (v.Rows.Count > 0)
            {
                h += 1 + S(10) + v.Rows.Count * (_rowH + S(4)) + S(4);
            }
            _lockRect = new RECT { Left = w - S(Pad) - S(18), Top = S(6), Right = w - S(6), Bottom = S(12) + _headerH + S(6) };
            // Keep it fully on its screen (or place cleanly under the Fortnite minimap on first launch).
            var (nx, ny, moved) = ClampOrPlaceUnderMinimap(hwnd, w, h);
            SetWindowPos(hwnd, HWND_TOPMOST, nx, ny, w, h, SWP_NOACTIVATE);
            SetWindowRgn(hwnd, CreateRoundRectRgn(0, 0, w + 1, h + 1, S(18), S(18)), true); // window owns the region
            if (moved)
            {
                _onMoved?.Invoke(nx, ny);
            }
        }
        finally
        {
            ReleaseDC(hwnd, hdc);
        }
        InvalidateRect(hwnd, IntPtr.Zero, true);
        UpdateWindow(hwnd); // paint now (one repaint per update, no timer)
    }

    private static void Text(IntPtr hdc, IntPtr font, uint color, string text, int left, int top, int right, int bottom, uint align = DT_LEFT)
    {
        SelectObject(hdc, font);
        SetTextColor(hdc, color);
        var r = new RECT { Left = left, Top = top, Right = right, Bottom = bottom };
        DrawTextW(hdc, text, -1, ref r, DT_SINGLELINE | DT_NOPREFIX | align);
    }

    private static int ParseCount(string? text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        var val = 0;
        foreach (var ch in text)
        {
            if (ch is >= '0' and <= '9')
            {
                val = val * 10 + (ch - '0');
            }
        }
        return val;
    }

    private static void StartNumberAnimation(IntPtr hwnd)
    {
        var target = _view;
        var nextTotal = ParseCount(target.Total);
        var nextSession = ParseCount(target.Session);
        var nextTreasures = ParseCount(target.Treasures);
        var nextLast = ParseCount(target.Last);

        if (!_hasInitialView)
        {
            _hasInitialView = true;
            _displayView = target;
            _toTotal = nextTotal;
            _toSession = nextSession;
            _toTreasures = nextTreasures;
            _toLast = nextLast;
            return;
        }

        var cur = _displayView;
        _fromTotal = ParseCount(cur.Total);
        _fromSession = ParseCount(cur.Session);
        _fromTreasures = ParseCount(cur.Treasures);
        _fromLast = ParseCount(cur.Last);

        _toTotal = nextTotal;
        _toSession = nextSession;
        _toTreasures = nextTreasures;
        _toLast = nextLast;

        if (_toTotal > _fromTotal || _toSession > _fromSession || _toTreasures > _fromTreasures || (_toLast != _fromLast && _toLast > 0))
        {
            _animStartMs = Environment.TickCount64;
            _animating = true;
            SetTimer(hwnd, AnimTimerId, 16, IntPtr.Zero);
            StepNumberAnimation(hwnd, repaint: false);
        }
        else
        {
            if (_animating)
            {
                KillTimer(hwnd, AnimTimerId);
                _animating = false;
            }
            _displayView = target;
        }
    }

    private static void StepNumberAnimation(IntPtr hwnd, bool repaint = true)
    {
        var target = _view;
        var elapsed = Environment.TickCount64 - _animStartMs;
        var t = Math.Clamp((double)elapsed / AnimDurationMs, 0.0, 1.0);
        var ease = 1.0 - Math.Pow(1.0 - t, 3.0);

        int Lerp(int a, int b) => (int)Math.Round(a + (b - a) * ease);

        if (t >= 1.0)
        {
            _displayView = target;
            _animating = false;
            KillTimer(hwnd, AnimTimerId);
        }
        else
        {
            var tot = L.N(Lerp(_fromTotal, _toTotal));
            var ses = L.N(Lerp(_fromSession, _toSession));
            var tre = L.N(Lerp(_fromTreasures, _toTreasures));
            var lst = target.Last != null ? L.N(Lerp(_fromLast, _toLast)) : null;

            var rows = target.Rows.Select(r =>
            {
                if (r.Treasure)
                {
                    return r with { Value = $"★ {tre}" };
                }
                if (r.Label == L.T("Session", "Session", "Sesión"))
                {
                    return r with { Value = L.Count(Lerp(_fromSession, _toSession), "coffre", "chest", "cofre") };
                }
                return r;
            }).ToList();

            _displayView = target with
            {
                Total = tot,
                Session = ses,
                Treasures = tre,
                Last = lst,
                Rows = rows,
            };
        }

        if (repaint)
        {
            if (Keyed)
            {
                UpdateHudLayeredWindow(hwnd);
            }
            else
            {
                InvalidateRect(hwnd, IntPtr.Zero, false);
                UpdateWindow(hwnd);
            }
        }
    }

    private static void Paint(IntPtr hwnd)
    {
        var hdc = BeginPaint(hwnd, out var ps);
        try
        {
            if (Keyed)
            {
                UpdateHudLayeredWindow(hwnd);
            }
            else
            {
                PaintTo(hwnd, hdc);
            }
        }
        finally
        {
            EndPaint(hwnd, ref ps);
        }
    }

    private static void PaintTo(IntPtr hwnd, IntPtr hdc)
    {
        if (Keyed)
        {
            PaintStyled(hwnd, hdc);
            return;
        }
        var v = _displayView;
        {
            GetClientRect(hwnd, out var rc);
            FillRect(hdc, ref rc, _brushBg);
            SetBkMode(hdc, TRANSPARENT);

            int S(int n) => (int)Math.Round(n * _scale);
            var x = S(Pad);
            var right = rc.Right - S(Pad);
            var y = S(12);

            // Thin border following the rounded shape.
            var border = CreateRoundRectRgn(0, 0, rc.Right + 1, rc.Bottom + 1, S(18), S(18));
            FrameRgn(hdc, border, _brushBorder, 1, 1);
            DeleteObject(border);

            // Header
            Text(hdc, _fontHeader, Rgb(142, 142, 147), Header, x, y, right, y + _headerH);
            if (v.Badge != null)
            {
                Text(hdc, _fontHeader, Rgb(48, 209, 88), v.Badge, x, y, _lockRect.Left - S(6), y + _headerH, DT_RIGHT);
            }
            // Lock button (Segoe MDL2 Assets: E785 = open padlock, E72E = closed padlock).
            var isLocked = EffectiveLocked;
            SelectObject(hdc, _fontIcon);
            SetTextColor(hdc, isLocked ? Rgb(99, 99, 102) : Rgb(229, 229, 234));
            var lr = _lockRect;
            DrawTextW(hdc, isLocked ? "" : "", -1, ref lr, DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX);
            y += _headerH + S(6);

            // Counters: total | match in progress
            var x2 = x + _col1W + S(ColGap);
            Text(hdc, _fontBig, Rgb(255, 255, 255), v.Total, x - S(1), y, x + _col1W + S(4), y + _bigH);
            Text(hdc, _fontBig, v.Current == null ? Rgb(72, 72, 74) : v.CurrentLive ? Rgb(48, 209, 88) : Rgb(242, 242, 247), v.Current ?? "—", x2 - S(1), y, right + S(4), y + _bigH);
            var sep = new RECT { Left = x + _col1W + S(ColGap) / 2, Top = y + S(8), Right = x + _col1W + S(ColGap) / 2 + 1, Bottom = y + _bigH + _captionH };
            FillRect(hdc, ref sep, _brushLine);
            y += _bigH;
            Text(hdc, _fontCaption, Rgb(142, 142, 147), CaptionTotal, x, y, x + _col1W + S(4), y + _captionH);
            Text(hdc, _fontCaption, Rgb(142, 142, 147), v.CurrentCaption, x2, y, right + S(4), y + _captionH);
            y += _captionH + S(12);

            if (v.Rows.Count > 0)
            {
                var line = new RECT { Left = x, Top = y, Right = right, Bottom = y + 1 };
                FillRect(hdc, ref line, _brushLine);
                y += 1 + S(10);
                foreach (var row in v.Rows)
                {
                    Text(hdc, _fontLabel, Rgb(142, 142, 147), row.Label, x, y, x + _labelW, y + _rowH);
                    var vx = x + _labelW + S(12);
                    Text(hdc, _fontSmall, row.Treasure ? Rgb(191, 90, 242) : Rgb(245, 245, 247), row.Value, vx, y, right + S(4), y + _rowH);
                    if (row.Detail != null)
                    {
                        var dx = vx + TextWidth(hdc, _fontSmall, row.Value, out _) + S(6);
                        Text(hdc, _fontCaption, Rgb(142, 142, 147), row.Detail, dx, y, right + S(4), y + _rowH - S(2), DT_BOTTOM);
                    }
                    y += _rowH + S(4);
                }
            }
        }
    }


    // ---- "hud" style: figures only, outlined like the game's own HUD, right-aligned under the minimap -------------
    //
    //  ● [chest] 3 934       total; the red dot while a match is in progress (gold while it is counted live)
    //   Dernière • 14        last finished match, kept during the next one
    //    Session • 83
    //      [star] 36         treasures

    private sealed record HudRow(string Label, string Value);

    private static HudRow StateRow(OverlayView v) => new(L.T("Dernière", "Last", "Última"), v.Last ?? "—");

    /// <summary>The figures of the match in progress are readable (its chests are already in the total).</summary>
    private static bool LiveFigures(OverlayView v) => v.CurrentLive && v.Current is { } cur && cur != "…";

    private static int Sc(double v) => (int)Math.Round(v * _scale);

    private const string Sep = "  •  ";

    private static void RelayoutStyled(IntPtr hwnd)
    {
        var v = _view;
        EnsureHudAssets();
        using var tmp = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(tmp);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.PageUnit = GraphicsUnit.Pixel;

        using var fontBig = CreateHudFont(Math.Max(18f, Sc(38)), FontStyle.Bold);
        using var fontSmall = CreateHudFont(Math.Max(11f, Sc(16)), FontStyle.Bold);

        var pad = Sc(8);
        var lockW = Sc(22);
        var bigW = (int)Math.Ceiling(MeasureHudText(g, fontBig, v.Total));
        _hudBigH = (int)Math.Ceiling(fontBig.GetHeight(g));
        _hudSmallH = (int)Math.Ceiling(fontSmall.GetHeight(g));

        var state = StateRow(v);
        var sessionLabel = L.T("Session", "Session", "Sesión");
        var rowsW = new[]
        {
            (int)Math.Ceiling(MeasureHudText(g, fontSmall, state.Label + Sep + state.Value)) + Sc(6),
            (int)Math.Ceiling(MeasureHudText(g, fontSmall, sessionLabel + Sep + v.Session)) + Sc(6),
            v.Squad != null ? (int)Math.Ceiling(MeasureHudText(g, fontSmall, OverlayModel.SquadLabel + Sep + v.Squad)) + Sc(6) : 0,
            Sc(24) + (int)Math.Ceiling(MeasureHudText(g, fontSmall, v.Treasures)),
        }.Max();

        // Room for the in-match dot + real chest icon + spacing is always kept.
        var w = lockW + pad + Math.Max(Sc(18) + Sc(30) + Sc(12) + bigW, rowsW) + pad + Sc(10);
        var h = pad + _hudBigH + (v.Squad != null ? 4 : 3) * (_hudSmallH + Sc(4)) + pad;
        w = Math.Max(_stableW, w);
        _stableW = w;
        _lockRect = new RECT { Left = Sc(2), Top = 0, Right = lockW, Bottom = h };

        var (nx, ny, moved) = ClampOrPlaceUnderMinimap(hwnd, w, h);
        SetWindowPos(hwnd, HWND_TOPMOST, nx, ny, w, h, SWP_NOACTIVATE);
        SetWindowRgn(hwnd, IntPtr.Zero, true);
        if (moved)
        {
            _onMoved?.Invoke(nx, ny);
        }
        UpdateHudLayeredWindow(hwnd);
    }

    private static (int X, int Y, bool Moved) ClampOrPlaceUnderMinimap(IntPtr hwnd, int w, int h)
    {
        GetWindowRect(hwnd, out var cur);
        int nx = cur.Left, ny = cur.Top;
        var mi = new MONITORINFO { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
        if (!_offscreen && GetMonitorInfoW(MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST), ref mi))
        {
            if (!_hasSavedPosition)
            {
                var screen = mi.rcMonitor;
                nx = screen.Right - w - Sc(20);
                ny = screen.Top + (int)Math.Round((screen.Bottom - screen.Top) * 0.255);
                _hasSavedPosition = true;
            }
            else
            {
                var work = mi.rcWork;
                if (cur.Right - cur.Left > 0 && (cur.Left + cur.Right) / 2 > (work.Left + work.Right) / 2)
                {
                    nx = cur.Right - w; // anchored on the right edge (under the minimap)
                }
                nx = Math.Max(work.Left, Math.Min(nx, work.Right - w));
                ny = Math.Max(work.Top, Math.Min(ny, work.Bottom - h));
            }
        }
        return (nx, ny, nx != cur.Left || ny != cur.Top);
    }

    /// <summary>Text outlined in near black, like the game's own HUD figures (readable on any background).</summary>
    private static void Outlined(IntPtr hdc, IntPtr font, uint color, string text, int left, int top, int right, int bottom, uint align, bool thin = false)
    {
        SelectObject(hdc, font);
        var o = thin ? 1 : Math.Max(1, Sc(1.6));
        SetTextColor(hdc, Rgb(11, 15, 24));
        for (var dx = -o; dx <= o; dx++)
        {
            for (var dy = -o; dy <= o + 1; dy++)
            {
                if (Math.Abs(dx) != o && Math.Abs(dy) < o) continue; // only the ring (and one pixel of drop shadow)
                var r = new RECT { Left = left + dx, Top = top + dy, Right = right + dx, Bottom = bottom + dy };
                DrawTextW(hdc, text, -1, ref r, DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | align);
            }
        }
        SetTextColor(hdc, color);
        var rc = new RECT { Left = left, Top = top, Right = right, Bottom = bottom };
        DrawTextW(hdc, text, -1, ref rc, DT_SINGLELINE | DT_NOPREFIX | DT_NOCLIP | align); // italic glyphs lean past the box
    }

    /// <summary>A treasure chest made of three shapes: body, rounded lid, latch.</summary>
    private static void Chest(IntPtr hdc, int x, int y, int w, int h, uint body, uint lid, uint latch, uint outline)
    {
        var pen = CreatePen(0, Math.Max(1, Sc(1.5)), outline);
        var oldPen = SelectObject(hdc, pen);
        var lidH = (int)(h * 0.42);
        var bLid = CreateSolidBrush(lid);
        var old = SelectObject(hdc, bLid);
        Chord(hdc, x, y, x + w, y + 2 * lidH, x + w, y + lidH, x, y + lidH);
        var bBody = CreateSolidBrush(body);
        SelectObject(hdc, bBody);
        RoundRect(hdc, x, y + lidH, x + w, y + h, Sc(4), Sc(4));
        var bLatch = CreateSolidBrush(latch);
        SelectObject(hdc, bLatch);
        var lw = Math.Max(3, w / 5);
        Rectangle(hdc, x + (w - lw) / 2, y + lidH - Sc(2), x + (w + lw) / 2, y + lidH + Math.Max(4, h / 4));
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(bLid);
        DeleteObject(bBody);
        DeleteObject(bLatch);
        DeleteObject(pen);
    }

    /// <summary>A five-pointed star (treasures), outlined like the text.</summary>
    private static void Star(IntPtr hdc, int cx, int cy, int r, uint fill, uint outline)
    {
        var pts = new POINT[10];
        for (var i = 0; i < 10; i++)
        {
            var a = -Math.PI / 2 + i * Math.PI / 5;
            var rr = i % 2 == 0 ? r : r * 0.45;
            pts[i] = new POINT { X = cx + (int)Math.Round(rr * Math.Cos(a)), Y = cy + (int)Math.Round(rr * Math.Sin(a)) };
        }
        var pen = CreatePen(0, Math.Max(1, Sc(1.5)), outline);
        var brush = CreateSolidBrush(fill);
        var oldPen = SelectObject(hdc, pen);
        var old = SelectObject(hdc, brush);
        Polygon(hdc, pts, pts.Length);
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(brush);
        DeleteObject(pen);
    }

    private static void Disc(IntPtr hdc, int x, int y, int d, uint fill, uint outline, int penW)
    {
        var pen = CreatePen(0, penW, outline);
        var brush = CreateSolidBrush(fill);
        var oldPen = SelectObject(hdc, pen);
        var old = SelectObject(hdc, brush);
        Ellipse(hdc, x, y, x + d, y + d);
        SelectObject(hdc, old);
        SelectObject(hdc, oldPen);
        DeleteObject(brush);
        DeleteObject(pen);
    }

    /// <summary>"Label  •  value" right-aligned: pale label, white value.</summary>
    private static int LabelValue(IntPtr hdc, string label, string value, int left, int right, int top, int height, uint valueColor)
    {
        if (value.Length == 0)
        {
            Outlined(hdc, _fontHudSmall, Rgb(174, 174, 178), label, left, top, right, top + height, DT_RIGHT, thin: true);
            return right - TextWidth(hdc, _fontHudSmall, label, out _);
        }
        var valueW = TextWidth(hdc, _fontHudSmall, value, out _);
        Outlined(hdc, _fontHudSmall, valueColor, value, left, top, right, top + height, DT_RIGHT, thin: true);
        var labelRight = right - valueW;
        Outlined(hdc, _fontHudSmall, Rgb(174, 174, 178), label + Sep, left, top, labelRight, top + height, DT_RIGHT, thin: true);
        return labelRight - TextWidth(hdc, _fontHudSmall, label + Sep, out _);
    }

    private static void UpdateHudLayeredWindow(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero || !GetWindowRect(hwnd, out var wr))
        {
            return;
        }
        int w = wr.Right - wr.Left, h = wr.Bottom - wr.Top;
        if (w <= 0 || h <= 0)
        {
            return;
        }
        using var bmp = RenderHudBitmap(w, h);
        var screenDc = GetDC(IntPtr.Zero);
        var memDc = CreateCompatibleDC(screenDc);
        var hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
        var oldBmp = SelectObject(memDc, hBitmap);
        try
        {
            var dst = new POINT { X = wr.Left, Y = wr.Top };
            var src = new POINT { X = 0, Y = 0 };
            var sz = new SIZE { cx = w, cy = h };
            var blend = new BLENDFUNCTION
            {
                BlendOp = AC_SRC_OVER,
                BlendFlags = 0,
                SourceConstantAlpha = Alpha,
                AlphaFormat = AC_SRC_ALPHA,
            };
            UpdateLayeredWindow(hwnd, screenDc, ref dst, ref sz, memDc, ref src, 0, ref blend, ULW_ALPHA);
        }
        finally
        {
            SelectObject(memDc, oldBmp);
            DeleteObject(hBitmap);
            DeleteDC(memDc);
            ReleaseDC(IntPtr.Zero, screenDc);
        }
    }

    private static PrivateFontCollection? _hudFontCollection;
    private static FontFamily? _hudFontFamily;
    private static Bitmap? _chestIconBitmap;
    private static bool _hudAssetsLoaded;

    private static void EnsureHudAssets()
    {
        if (_hudAssetsLoaded)
        {
            return;
        }
        _hudAssetsLoaded = true;
        var asm = typeof(NativeOverlay).Assembly;

        try
        {
            using var fontStream = asm.GetManifestResourceStream("hellafine.ttf");
            if (fontStream != null)
            {
                var bytes = new byte[fontStream.Length];
                fontStream.ReadExactly(bytes);
                var ptr = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                _hudFontCollection = new PrivateFontCollection();
                _hudFontCollection.AddMemoryFont(ptr, bytes.Length);
                if (_hudFontCollection.Families.Length > 0)
                {
                    _hudFontFamily = _hudFontCollection.Families[0];
                }
            }
        }
        catch
        {
            // Fallback to system font if embedded font fails to load.
        }

        try
        {
            using var iconStream = asm.GetManifestResourceStream("ChestIcon.png");
            if (iconStream != null)
            {
                _chestIconBitmap = new Bitmap(iconStream);
            }
        }
        catch
        {
            // Fallback to vector chest if embedded icon fails to load.
        }
    }

    private static Font CreateHudFont(float pixelSize, FontStyle style)
    {
        EnsureHudAssets();
        if (_hudFontFamily != null)
        {
            var chosen = _hudFontFamily.IsStyleAvailable(style) ? style : FontStyle.Regular;
            return new Font(_hudFontFamily, pixelSize, chosen, GraphicsUnit.Pixel);
        }
        try
        {
            return new Font("Segoe UI Variable Display", pixelSize, style, GraphicsUnit.Pixel);
        }
        catch
        {
            return new Font("Segoe UI", pixelSize, style, GraphicsUnit.Pixel);
        }
    }

    private static double _cachedHudFontScale = -1;
    private static Font? _cachedHudBigFont;
    private static Font? _cachedHudSmallFont;
    private static readonly StringFormat HudMeasureFmt = new(StringFormat.GenericTypographic)
    {
        FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces,
    };
    private static readonly StringFormat HudRightFmt = new(StringFormat.GenericTypographic)
    {
        Alignment = StringAlignment.Far,
        LineAlignment = StringAlignment.Center,
        FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces,
    };
    private static readonly SolidBrush HudShadowBrush = new(Color.FromArgb(145, 0, 0, 0));

    private static (Font Big, Font Small) GetCachedHudFonts()
    {
        if (_cachedHudBigFont == null || _cachedHudSmallFont == null || Math.Abs(_cachedHudFontScale - _scale) > 0.001)
        {
            _cachedHudBigFont?.Dispose();
            _cachedHudSmallFont?.Dispose();
            _cachedHudBigFont = CreateHudFont(Math.Max(18f, Sc(38)), FontStyle.Bold);
            _cachedHudSmallFont = CreateHudFont(Math.Max(11f, Sc(16)), FontStyle.Bold);
            _cachedHudFontScale = _scale;
        }
        return (_cachedHudBigFont, _cachedHudSmallFont);
    }

    private static Bitmap RenderHudBitmap(int w, int h)
    {
        EnsureHudAssets();
        var bmp = new Bitmap(Math.Max(1, w), Math.Max(1, h), PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(bmp);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PageUnit = GraphicsUnit.Pixel;
        g.Clear(Color.Transparent);

        var v = _displayView;
        var pad = (float)Sc(8);
        var right = w - pad - Sc(8);
        var left = (float)_lockRect.Right;

        if (!EffectiveLocked)
        {
            using var bgPath = RoundedRectPath(new RectangleF(0.5f, 0.5f, w - 1f, h - 1f), Sc(12));
            using var bgBrush = new SolidBrush(Color.FromArgb(195, 14, 14, 18));
            using var bgPen = new Pen(Color.FromArgb(75, 255, 255, 255), 1f);
            g.FillPath(bgBrush, bgPath);
            g.DrawPath(bgPen, bgPath);

            using var lockFont = new Font("Segoe MDL2 Assets", Math.Max(12f, Sc(14)), FontStyle.Regular, GraphicsUnit.Pixel);
            using var lockBrush = new SolidBrush(Color.FromArgb(229, 229, 234));
            using var lockFmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("\uE785", lockFont, lockBrush, new RectangleF(_lockRect.Left, _lockRect.Top, _lockRect.Right - _lockRect.Left, _lockRect.Bottom - _lockRect.Top), lockFmt);
        }

        var (fontBig, fontSmall) = GetCachedHudFonts();

        var white = Color.FromArgb(255, 255, 255, 255);
        var muted = Color.FromArgb(230, 200, 204, 214);
        var green = Color.FromArgb(255, 48, 209, 88);
        var purple = Color.FromArgb(255, 218, 143, 255);

        var state = StateRow(v);
        var live = LiveFigures(v);
        var y = pad;

        var bigRect = new RectangleF(left, y, Math.Max(10f, right - left), _hudBigH);
        var bigW = MeasureHudText(g, fontBig, v.Total);
        DrawHudText(g, v.Total, fontBig, live ? green : white, bigRect, HudRightFmt);

        var iw = (float)Sc(28);
        var ih = (float)Sc(28);
        var ix = right - bigW - Sc(10) - iw;
        var iy = y + (_hudBigH - ih) / 2f;
        if (_chestIconBitmap != null)
        {
            g.DrawImage(_chestIconBitmap, ix, iy, iw, ih);
        }
        else
        {
            DrawHudChest(g, ix, iy + Sc(4), Sc(24), Sc(20));
        }

        if (v.CurrentLive)
        {
            var d = (float)Sc(10);
            var dx = ix - Sc(8) - d;
            var dy = y + (_hudBigH - d) / 2f;
            using var dotBrush = new SolidBrush(Color.FromArgb(255, 255, 69, 58));
            g.FillEllipse(HudShadowBrush, dx, dy + 1.5f, d, d);
            g.FillEllipse(dotBrush, dx, dy, d, d);
        }
        y += _hudBigH + Sc(4);

        var rowH = (float)_hudSmallH;
        DrawHudLabelValue(g, fontSmall, state.Label, state.Value, left, right, y, rowH, muted, white);
        y += rowH + Sc(4);
        DrawHudLabelValue(g, fontSmall, L.T("Session", "Session", "Sesión"), v.Session, left, right, y, rowH, muted, white);
        y += rowH + Sc(4);
        if (v.Squad != null)
        {
            DrawHudLabelValue(g, fontSmall, OverlayModel.SquadLabel, v.Squad, left, right, y, rowH, muted, white);
            y += rowH + Sc(4);
        }

        var tw = MeasureHudText(g, fontSmall, v.Treasures);
        DrawHudText(g, v.Treasures, fontSmall, purple, new RectangleF(left, y, Math.Max(10f, right - left), rowH), HudRightFmt);
        DrawHudStar(g, right - tw - Sc(15), y + rowH / 2f, Sc(7));

        return bmp;
    }

    private static float MeasureHudText(Graphics g, Font font, string text) =>
        g.MeasureString(text, font, 2000, HudMeasureFmt).Width;

    private static void DrawHudLabelValue(Graphics g, Font font, string label, string value, float left, float right, float top, float height, Color labelColor, Color valueColor)
    {
        if (string.IsNullOrEmpty(value))
        {
            DrawHudText(g, label, font, labelColor, new RectangleF(left, top, Math.Max(10f, right - left), height), HudRightFmt);
            return;
        }
        var valueW = MeasureHudText(g, font, value);
        DrawHudText(g, value, font, valueColor, new RectangleF(left, top, Math.Max(10f, right - left), height), HudRightFmt);
        var labelRight = right - valueW - Sc(2);
        DrawHudText(g, label + Sep, font, labelColor, new RectangleF(left, top, Math.Max(10f, labelRight - left), height), HudRightFmt);
    }

    private static void DrawHudText(Graphics g, string text, Font font, Color fillColor, RectangleF rect, StringFormat fmt)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        var dy = Math.Max(1.2f, Sc(1.6));
        using var fillBrush = new SolidBrush(fillColor);
        var shadowRect = new RectangleF(rect.X, rect.Y + dy, rect.Width, rect.Height);
        g.DrawString(text, font, HudShadowBrush, shadowRect, fmt);
        g.DrawString(text, font, fillBrush, rect, fmt);
    }

    private static GraphicsPath RoundedRectPath(RectangleF r, float radius)
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

    private static void DrawHudChest(Graphics g, float x, float y, float w, float h)
    {
        var lidH = h * 0.42f;
        using var outline = new Pen(Color.FromArgb(215, 8, 10, 15), Math.Max(1.4f, Sc(1.8))) { LineJoin = LineJoin.Round };
        using var lidBrush = new SolidBrush(Color.FromArgb(255, 182, 186, 194));
        using var bodyBrush = new SolidBrush(Color.FromArgb(255, 232, 234, 240));
        using var latchBrush = new SolidBrush(Color.FromArgb(255, 22, 24, 30));

        using var lidPath = new GraphicsPath();
        lidPath.AddArc(x, y, w, lidH * 2f, 180, 180);
        lidPath.CloseFigure();
        g.FillPath(lidBrush, lidPath);
        g.DrawPath(outline, lidPath);

        using var bodyPath = RoundedRectPath(new RectangleF(x, y + lidH, w, h - lidH), Sc(3));
        g.FillPath(bodyBrush, bodyPath);
        g.DrawPath(outline, bodyPath);

        var lw = Math.Max(3f, w / 5f);
        var lh = Math.Max(4f, h / 4f);
        g.FillRectangle(latchBrush, x + (w - lw) / 2f, y + lidH - Sc(1.5), lw, lh);
    }

    private static void DrawHudStar(Graphics g, float cx, float cy, float r)
    {
        var pts = new PointF[10];
        for (var i = 0; i < 10; i++)
        {
            var a = -Math.PI / 2 + i * Math.PI / 5;
            var rr = i % 2 == 0 ? r : r * 0.45;
            pts[i] = new PointF(cx + (float)(rr * Math.Cos(a)), cy + (float)(rr * Math.Sin(a)));
        }
        using var path = new GraphicsPath();
        path.AddPolygon(pts);
        using var fill = new SolidBrush(Color.FromArgb(255, 191, 90, 242));
        using var outline = new Pen(Color.FromArgb(215, 8, 10, 15), Math.Max(1.4f, Sc(1.8))) { LineJoin = LineJoin.Round };
        g.FillPath(fill, path);
        g.DrawPath(outline, path);
    }

    private static void PaintStyled(IntPtr hwnd, IntPtr hdc)
    {
        GetClientRect(hwnd, out var rc);
        var key = CreateSolidBrush(KeyColor);
        FillRect(hdc, ref rc, key);
        DeleteObject(key);
        SetBkMode(hdc, TRANSPARENT);
        var v = _displayView;
        var pad = Sc(8);
        var right = rc.Right - pad - Sc(12); // italic glyphs and their outline lean past the measured width
        var white = Rgb(255, 255, 255);
        var silver = Rgb(229, 229, 234);
        var green = Rgb(48, 209, 88);
        var ink = Rgb(6, 6, 8);

        // Unlocked: a dark backdrop to grab it, and the padlock (locked, only the figures remain, mouse goes through).
        if (!EffectiveLocked)
        {
            var pen = CreatePen(0, 1, Rgb(56, 56, 64));
            var brush = CreateSolidBrush(Rgb(16, 16, 20));
            var oldPen = SelectObject(hdc, pen);
            var old = SelectObject(hdc, brush);
            RoundRect(hdc, 0, 0, rc.Right, rc.Bottom, Sc(12), Sc(12));
            SelectObject(hdc, old);
            SelectObject(hdc, oldPen);
            DeleteObject(brush);
            DeleteObject(pen);
            SelectObject(hdc, _fontIcon);
            SetTextColor(hdc, Rgb(229, 229, 234));
            var lr = _lockRect;
            DrawTextW(hdc, "", -1, ref lr, DT_SINGLELINE | DT_CENTER | DT_VCENTER | DT_NOPREFIX);
        }

        var left = _lockRect.Right;
        var y = pad;
        var state = StateRow(v);
        var live = LiveFigures(v);

        // Total, with the chest, and the red dot while a match is in progress.
        var bigW = TextWidth(hdc, _fontHudBig, v.Total, out _);
        Outlined(hdc, _fontHudBig, live ? green : white, v.Total, left, y, right, y + _hudBigH, DT_RIGHT);
        var iw = Sc(26);
        var ih = Sc(22);
        var ix = right - bigW - Sc(8) - iw;
        Chest(hdc, ix, y + (_hudBigH - ih) / 2, iw, ih, silver, Rgb(174, 174, 178), ink, ink);
        if (v.CurrentLive)
        {
            var d = Sc(11);
            Disc(hdc, ix - Sc(7) - d, y + (_hudBigH - d) / 2, d, Rgb(255, 69, 58), ink, Math.Max(1, Sc(1.5)));
        }
        y += _hudBigH + Sc(2);

        // Last finished match (also during the next one), session, treasures.
        var rowH = _hudSmallH;
        LabelValue(hdc, state.Label, state.Value, left, right, y, rowH, white);
        y += rowH + Sc(2);
        LabelValue(hdc, L.T("Session", "Session", "Sesión"), v.Session, left, right, y, rowH, white);
        y += rowH + Sc(2);
        if (v.Squad != null)
        {
            LabelValue(hdc, OverlayModel.SquadLabel, v.Squad, left, right, y, rowH, white);
            y += rowH + Sc(2);
        }
        var tw = TextWidth(hdc, _fontHudSmall, v.Treasures, out _);
        Outlined(hdc, _fontHudSmall, Rgb(218, 143, 255), v.Treasures, left, y, right, y + rowH, DT_RIGHT, thin: true);
        Star(hdc, right - tw - Sc(12), y + rowH / 2, Sc(8), Rgb(191, 90, 242), ink);
    }

    private static uint Rgb(byte r, byte g, byte b) => (uint)(r | (g << 8) | (b << 16));

    // ---- Win32 ------------------------------------------------------------------------------
    private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const uint WS_POPUP = 0x80000000;
    private const uint WS_EX_TOPMOST = 0x00000008, WS_EX_TRANSPARENT = 0x00000020, WS_EX_TOOLWINDOW = 0x00000080, WS_EX_LAYERED = 0x00080000, WS_EX_NOACTIVATE = 0x08000000;
    private const int GWL_EXSTYLE = -20, HTCLIENT = 1;
    private const uint WM_SETCURSOR = 0x0020, WM_LBUTTONUP = 0x0202, WM_APP_LOCK = 0x8003, VK_L = 0x4C, SWP_FRAMECHANGED = 0x20;
    private static readonly IntPtr IDC_HAND = new(32649);
    private const uint LWA_ALPHA = 0x2, LWA_COLORKEY = 0x1, ULW_ALPHA = 0x2;
    private const byte AC_SRC_OVER = 0, AC_SRC_ALPHA = 1;
    private const int SW_HIDE = 0, SW_SHOWNOACTIVATE = 4;
    private const uint WM_DESTROY = 0x0002, WM_PAINT = 0x000F, WM_NCHITTEST = 0x0084, WM_NCRBUTTONUP = 0x00A5,
        WM_TIMER = 0x0113, WM_EXITSIZEMOVE = 0x0232, WM_DPICHANGED = 0x02E0, WM_APP_TOGGLE = 0x8001, WM_PRINTCLIENT = 0x0318, WM_APP_RELAYOUT = 0x8002, WM_HOTKEY = 0x0312, WM_APP_APPEARANCE = 0x8004, WM_APP_STOP = 0x8005;
    private const uint MOD_CONTROL = 0x2, MOD_SHIFT = 0x4, MOD_NOREPEAT = 0x4000, VK_O = 0x4F;
    private const int HTCAPTION = 2;
    private const int TRANSPARENT = 1, LOGPIXELSY = 90;
    private const uint DT_LEFT = 0x0, DT_CENTER = 0x1, DT_RIGHT = 0x2, DT_VCENTER = 0x4, DT_BOTTOM = 0x8, DT_SINGLELINE = 0x20, DT_CALCRECT = 0x400, DT_NOPREFIX = 0x800, DT_NOCLIP = 0x100;
    private const uint SWP_NOSIZE = 0x1, SWP_NOMOVE = 0x2, SWP_NOACTIVATE = 0x10;
    private static readonly IntPtr HWND_TOPMOST = new(-1);
    private static readonly IntPtr IDC_SIZEALL = new(32646);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct WNDCLASSEXW
    {
        public uint cbSize, style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra, cbWndExtra;
        public IntPtr hInstance, hIcon, hCursor, hbrBackground;
        public string? lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int Left, Top, Right, Bottom; }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X, Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam, lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public int fErase;
        public RECT rcPaint;
        public int fRestore, fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern ushort RegisterClassExW(ref WNDCLASSEXW wc);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint exStyle, string className, string windowName, uint style, int x, int y, int w, int h, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DefWindowProcW(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetMessageW(out MSG msg, IntPtr hwnd, uint min, uint max);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG msg);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr DispatchMessageW(ref MSG msg);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern bool PostMessageW(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern void PostQuitMessage(int code);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int cmd);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool DestroyWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hwnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE { public int cx, cy; }
    [StructLayout(LayoutKind.Sequential)]
    private struct BLENDFUNCTION { public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat; }
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint key, byte alpha, uint flags);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, uint crKey, ref BLENDFUNCTION pblend, uint dwFlags);
    [DllImport("user32.dll")] private static extern int SetWindowRgn(IntPtr hwnd, IntPtr rgn, bool redraw);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);
    [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hwnd, out RECT rect);
    [DllImport("user32.dll")] private static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool erase);
    [DllImport("user32.dll")] private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT ps);
    [DllImport("user32.dll")] private static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT ps);
    [DllImport("user32.dll")] private static extern int FillRect(IntPtr hdc, ref RECT rect, IntPtr brush);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int DrawTextW(IntPtr hdc, string text, int count, ref RECT rect, uint format);
    [DllImport("user32.dll")] private static extern UIntPtr SetTimer(IntPtr hwnd, UIntPtr id, uint ms, IntPtr proc);
    [DllImport("user32.dll")] private static extern bool KillTimer(IntPtr hwnd, UIntPtr id);
    [DllImport("user32.dll")] private static extern IntPtr LoadCursorW(IntPtr instance, IntPtr name);
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] private static extern IntPtr GetWindowLongPtrW(IntPtr hwnd, int index);
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")] private static extern IntPtr SetWindowLongPtrW(IntPtr hwnd, int index, IntPtr value);
    [DllImport("user32.dll")] private static extern IntPtr SetCursor(IntPtr cursor);
    [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint vk);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hwnd, int id);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandleW(string? name);
    [DllImport("gdi32.dll")] private static extern int GetDeviceCaps(IntPtr hdc, int index);
    [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFontW(int height, int width, int escapement, int orientation, int weight, uint italic, uint underline, uint strikeOut, uint charSet, uint outPrecision, uint clipPrecision, uint quality, uint pitchAndFamily, string face);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateSolidBrush(uint color);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int w, int h);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth, biHeight;
        public ushort biPlanes, biBitCount;
        public uint biCompression, biSizeImage;
        public int biXPelsPerMeter, biYPelsPerMeter;
        public uint biClrUsed, biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO { public uint cbSize; public RECT rcMonitor, rcWork; public uint dwFlags; }

    private const uint MONITOR_DEFAULTTONEAREST = 2;
    [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);
    [DllImport("user32.dll")] private static extern bool GetMonitorInfoW(IntPtr monitor, ref MONITORINFO info);
    [DllImport("user32.dll")] private static extern bool UpdateWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern int GetDIBits(IntPtr hdc, IntPtr bmp, uint start, uint lines, byte[] bits, ref BITMAPINFOHEADER bi, uint usage);
    [DllImport("gdi32.dll")] private static extern bool FrameRgn(IntPtr hdc, IntPtr rgn, IntPtr brush, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr obj);
    [DllImport("gdi32.dll")] private static extern int SetBkMode(IntPtr hdc, int mode);
    [DllImport("gdi32.dll")] private static extern uint SetTextColor(IntPtr hdc, uint color);
    [DllImport("gdi32.dll")] private static extern IntPtr CreatePen(int style, int width, uint color);
    [DllImport("gdi32.dll")] private static extern bool Ellipse(IntPtr hdc, int l, int t, int r, int b);
    [DllImport("gdi32.dll")] private static extern bool Polygon(IntPtr hdc, POINT[] points, int count);
    [DllImport("gdi32.dll")] private static extern bool Rectangle(IntPtr hdc, int l, int t, int r, int b);
    [DllImport("gdi32.dll")] private static extern bool RoundRect(IntPtr hdc, int l, int t, int r, int b, int w, int h);
    [DllImport("gdi32.dll")] private static extern bool Chord(IntPtr hdc, int l, int t, int r, int b, int x1, int y1, int x2, int y2);
    [DllImport("user32.dll")] private static extern int FrameRect(IntPtr hdc, ref RECT rect, IntPtr brush);
}
