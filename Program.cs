using System.Reflection;
using TurnOffDisplay;

namespace DesktopApp
{
    internal partial class Program : Form
    {
        private static HotkeyManager hotkeyManager;
        private static TitleBarRemover titleBarRemover;
        private static DisplayManager displayManager;
        private static ReminderManager reminderManager;

        private static NotifyIcon icon = new()
        {
            Text = "TurnOffDisplay",
            Icon = new Icon("icon.ico"),
            Visible = true
        };
        private static ContextMenuStrip contextMenuStrip = new()
        {
            ShowCheckMargin = false,
            ShowImageMargin = false
        };

        private static void Main(string[] args)
        {
            displayManager = new DisplayManager();
            titleBarRemover = new TitleBarRemover();
            hotkeyManager = new HotkeyManager();
            reminderManager = new ReminderManager();
            //titleBarRemover.Start();
            hotkeyManager.Start();
            reminderManager.Start();
            SetUpTrayIcon();
            Application.Run();
        }

        private static void AddMenuItem(string text, EventHandler clickHandler)
        {
            var menuItem = new ToolStripMenuItem
            {
                Text = text
            };
            menuItem.Click += clickHandler;
            contextMenuStrip.Items.Add(menuItem);
        }

        private static void SetUpTrayIcon()
        {
            AddMenuItem("Turn Off Display", TurnOffDisplayToolStripMenuItem_Click);
            AddMenuItem("Turn On Display", TurnOnDisplayToolStripMenuItem_Click);
            AddMenuItem("Exit", ExitToolStripMenuItem_ClickAsync);
            // Create a custom form to host the context menu
            var contextMenuForm = new Form
            {
                TopMost = true,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = new Size(0, 0),
                Location = new Point(-1000, -1000)
            };

            icon.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                    mi.Invoke(icon, null);
                    contextMenuForm.Show();
                    contextMenuStrip.Show(Cursor.Position);
                }
            };
        }


        private static void ExitToolStripMenuItem_ClickAsync(object? sender, EventArgs e)
        {
            CleanUp();
            Application.Exit();
        }

        private static void Icon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && contextMenuStrip != null)
            {
                contextMenuStrip.Show(Control.MousePosition);
            }
        }

        private static void TurnOffDisplayToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            displayManager.TurnOffDisplay();
        }

        private static void TurnOnDisplayToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            displayManager.TurnOnDisplay();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            CleanUp();
        }

        private static void CleanUp()
        {
            icon.Visible = false;
            icon.Dispose();
            hotkeyManager.Stop();
            titleBarRemover.Stop();
        }

    }
}
