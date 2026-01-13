using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MediaColor = System.Windows.Media.Color;

namespace ActiveWindowHighlighter
{
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int ex = GetWindowLong(hwnd, -20);
            ex |= 0x20 | 0x80000 | 0x80; // WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW
            SetWindowLong(hwnd, -20, ex);
        }

        public void SetBorderColor(MediaColor c)
        {
            Border.BorderBrush = new System.Windows.Media.SolidColorBrush(c);
        }

        public void SetBorderThickness(double t)
        {
            Border.BorderThickness = new Thickness(t);
        }

        public void Position(Rect r)
        {
            Left = r.Left;
            Top = r.Top;
            Width = r.Width;
            Height = r.Height;
        }

        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr h, int n);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr h, int n, int v);
    }
}
