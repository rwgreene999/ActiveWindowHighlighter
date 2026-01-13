using System;
using System.Threading;
using WpfApp = System.Windows.Application;

namespace ActiveWindowHighlighter
{
    public partial class App : WpfApp
    {
        private Mutex? _mutex;
        private TrayIconManager? _tray;
        private OverlayManager? _overlay;

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            _mutex = new Mutex(true, "ActiveWindowHighlighter_SingleInstance", out bool created);
            if (!created)
            {
                Shutdown();
                return;
            }

            base.OnStartup(e);

            _overlay = new OverlayManager();
            _overlay.Start();

            _tray = new TrayIconManager(_overlay);
            _tray.Initialize();
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            _overlay?.Dispose();
            _tray?.Dispose();
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }
}
