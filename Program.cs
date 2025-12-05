using System.Reflection;
using DesktopAssistant;

namespace DesktopAssistant
{
    internal class Program : Form
    {
        private static HotkeyManager? hotkeyManager;
        private static ReminderManager? reminderManager;
        private static GameAhkManager? gameAhkManager;

        private static readonly NotifyIcon icon = new()
        {
            Text = "DesktopAssistant",
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
            gameAhkManager = new GameAhkManager();
            
            hotkeyManager.Start();
            reminderManager.Start();
            gameAhkManager.Start();
            SetUpTrayIcon();
            Application.Run();
        }

        private static void SetUpTrayIcon()
        {
            menu.Items.Add("关闭显示器", null, (_, _) => DisplayManager.TurnOff());
            menu.Items.Add("开启显示器", null, (_, _) => DisplayManager.TurnOn());
            menu.Items.Add("-");
            menu.Items.Add("启动实时翻译", null, (_, _) => StartRealtimeSubtitle());
            menu.Items.Add("-");
            menu.Items.Add("退出", null, (_, _) => { icon.Visible = false; hotkeyManager?.Stop(); gameAhkManager?.Stop(); Application.Exit(); });

            icon.ContextMenuStrip = menu;
        }

        private static void StartRealtimeSubtitle()
        {
            try
            {
                var batPath = @"C:\Code\RealtimeSubtitle\start.bat";
                if (System.IO.File.Exists(batPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = batPath,
                        WorkingDirectory = @"C:\Code\RealtimeSubtitle",
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    });
                }
                else
                {
                    MessageBox.Show($"找不到文件: {batPath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


