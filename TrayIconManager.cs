using System;
using System.Reflection;
using System.Windows.Media;

namespace ActiveWindowHighlighter
{
    public sealed class TrayIconManager : IDisposable
    {
        private readonly OverlayManager _overlayManager;
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private bool _enabled = true;
        private bool _ownsIcon;
        private System.Drawing.Icon? _trayIcon;

        public TrayIconManager(OverlayManager overlayManager)
        {
            _overlayManager = overlayManager;
        }

        public void Initialize()
        {
            var exePath = Assembly.GetEntryAssembly()?.Location;
            if (!string.IsNullOrEmpty(exePath))
            {
                try
                {
                    _trayIcon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
                    _ownsIcon = true;
                }
                catch
                {
                    _trayIcon = System.Drawing.SystemIcons.Information;
                    _ownsIcon = false;
                }
            }
            else
            {
                _trayIcon = System.Drawing.SystemIcons.Information;
                _ownsIcon = false;
            }
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = _trayIcon,
                Visible = true,
                Text = "Active Window Highlighter",
                ContextMenuStrip = BuildContextMenu()
            };
        }

        private System.Windows.Forms.ContextMenuStrip BuildContextMenu()
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();

            var enabledItem = new System.Windows.Forms.ToolStripMenuItem(
                "Enabled",
                image: null,
                onClick: (_, _) => ToggleEnabled())
            {
                Checked = _enabled
            };

            var colorsMenu = new System.Windows.Forms.ToolStripMenuItem("Border Color");
            colorsMenu.DropDownItems.Add(CreateColorItem("OrangeRed", Colors.OrangeRed));
            colorsMenu.DropDownItems.Add(CreateColorItem("Lime", Colors.Lime));
            colorsMenu.DropDownItems.Add(CreateColorItem("DeepSkyBlue", Colors.DeepSkyBlue));
            colorsMenu.DropDownItems.Add(CreateColorItem("Magenta", Colors.Magenta));

            var thicknessMenu = new System.Windows.Forms.ToolStripMenuItem("Border Thickness");
            thicknessMenu.DropDownItems.Add(CreateThicknessItem("2 px", 2));
            thicknessMenu.DropDownItems.Add(CreateThicknessItem("3 px", 3));
            thicknessMenu.DropDownItems.Add(CreateThicknessItem("4 px", 4));
            thicknessMenu.DropDownItems.Add(CreateThicknessItem("6 px", 6));

            var exitItem = new System.Windows.Forms.ToolStripMenuItem(
                "Exit",
                image: null,
                onClick: (_, _) => ExitApplication());

            menu.Items.Add(enabledItem);
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add(colorsMenu);
            menu.Items.Add(thicknessMenu);
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add(exitItem);

            menu.Opening += (_, _) =>
            {
                enabledItem.Checked = _enabled;
            };

            return menu;
        }

        private System.Windows.Forms.ToolStripMenuItem CreateColorItem(string name, Color color)
        {
            return new System.Windows.Forms.ToolStripMenuItem(
                name,
                image: null,
                onClick: (_, _) =>
                {
                    _overlayManager.BorderColor = color;
                });
        }

        private System.Windows.Forms.ToolStripMenuItem CreateThicknessItem(string name, double thickness)
        {
            return new System.Windows.Forms.ToolStripMenuItem(
                name,
                image: null,
                onClick: (_, _) =>
                {
                    _overlayManager.BorderThickness = thickness;
                });
        }

        private void ToggleEnabled()
        {
            _enabled = !_enabled;

            if (_enabled)
                _overlayManager.Start();
            else
                _overlayManager.Stop();
        }

        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            if (_trayIcon != null && _ownsIcon)
            {
                _trayIcon.Dispose();
                _trayIcon = null;
                _ownsIcon = false;
            }

            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
        }
    }
}
