using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using MediaColor = System.Windows.Media.Color;

namespace ActiveWindowHighlighter
{
    public sealed class OverlayManager : IDisposable
    {
        private readonly OverlayWindow _overlay;
        private readonly Dispatcher _dispatcher;
        private IntPtr _hook = IntPtr.Zero;
        private WinEventDelegate? _callback;

        public MediaColor BorderColor { get; set; } = MediaColor.FromRgb(255, 80, 0);
        public double BorderThickness { get; set; } = 3;

        public OverlayManager()
        {
            _dispatcher = Application.Current.Dispatcher;
            _overlay = new OverlayWindow();
        }

        public void Start()
        {
            _overlay.Show();
            _overlay.Hide();

            _callback = WinEventProc;
            _hook = SetWinEventHook(3, 3, IntPtr.Zero, _callback, 0, 0, 0x0000 | 0x0002);
        }

        public void Stop()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWinEvent(_hook);
                _hook = IntPtr.Zero;
            }
            _overlay.Hide();
        }

        private void WinEventProc(IntPtr h, uint e, IntPtr hwnd, int obj, int child, uint t, uint ms)
        {
            if (e != 3 || hwnd == IntPtr.Zero)
                return;

            _dispatcher.InvokeAsync(() => Update(hwnd));
        }

        private void Update(IntPtr hwnd)
        {
            if (!IsWindowVisible(hwnd) || IsIconic(hwnd))
            {
                _overlay.Hide();
                return;
            }

            if (!GetWindowRect(hwnd, out RECT r))
            {
                _overlay.Hide();
                return;
            }

            var rect = new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);

            _overlay.SetBorderColor(BorderColor);
            _overlay.SetBorderThickness(BorderThickness);
            _overlay.Position(rect);

            if (!_overlay.IsVisible)
                _overlay.Show();
        }

        public void Dispose() => Stop();

        private delegate void WinEventDelegate(IntPtr h, uint e, IntPtr w, int o, int c, uint t, uint ms);

        [DllImport("user32.dll")] private static extern IntPtr SetWinEventHook(uint a, uint b, IntPtr m, WinEventDelegate d, uint p, uint t, uint f);
        [DllImport("user32.dll")] private static extern bool UnhookWinEvent(IntPtr h);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr h, out RECT r);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr h);
        [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr h);

        private struct RECT { public int Left, Top, Right, Bottom; }
    }
}
