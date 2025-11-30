using System.Reflection;
using TurnOffDisplay;

namespace DesktopApp
{
    internal class Program : Form
    {
        private static HotkeyManager? hotkeyManager;
        private static ReminderManager? reminderManager;

        private static readonly NotifyIcon icon = new()
        {
            Text = "TurnOffDisplay",
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Visible = true
        };

        private static readonly ContextMenuStrip menu = new()
        {
            ShowCheckMargin = false,
            ShowImageMargin = false
        };

        private static void Main()
        {
            hotkeyManager = new HotkeyManager();
            reminderManager = new ReminderManager();
            
            hotkeyManager.Start();
            reminderManager.Start();
            SetUpTrayIcon();
            Application.Run();
        }

        private static void SetUpTrayIcon()
        {
            menu.Items.Add("关闭显示器", null, (_, _) => DisplayManager.TurnOff());
            menu.Items.Add("开启显示器", null, (_, _) => DisplayManager.TurnOn());
            menu.Items.Add("-");
            menu.Items.Add("退出", null, (_, _) => { icon.Visible = false; hotkeyManager?.Stop(); Application.Exit(); });

            icon.ContextMenuStrip = menu;
        }
    }
}
