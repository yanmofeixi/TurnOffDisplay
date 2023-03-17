using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace DesktopApp
{
    internal partial class Program
    {
        private static readonly NotifyIcon icon = new()
        {
            Text = "TurnOffDisplay",
            Icon = new Icon("icon.ico"),
            Visible = true
        };
        private static readonly ContextMenuStrip contextMenuStrip = new();
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private static readonly int WM_SYSCOMMAND = 0x0112;
        private static readonly int SC_MONITORPOWER = 0xF170;
        private static readonly int MONITOR_ON = -1;
        private static readonly int MONITOR_OFF = 2;

        private static void Main(string[] args)
        {
            SetUpTrayIcon();
            SetUpTcpListener();
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
            AddMenuItem("Exit", ExitToolStripMenuItem_Click);
            icon.ContextMenuStrip = contextMenuStrip;
            icon.MouseClick += new MouseEventHandler(Icon_MouseClick);
        }

        private static void SetUpTcpListener()
        {
            var listenerThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 12345);
                listener.Start();
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var stream = client.GetStream();
                    var buffer = new byte[1024];
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (message == "turn off")
                    {
                        TurnOffDisplay();
                    }
                    stream.Close();
                    client.Close();
                }
            })
            {
                IsBackground = true
            };
            listenerThread.Start();
        }

        private static void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CleanUpNotifyIcon();
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
            TurnOffDisplay();
        }

        private static void TurnOnDisplayToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            TurnOnDisplay();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            icon.Visible = false;
        }

        private static void CleanUpNotifyIcon()
        {
            icon.Visible = false;
            icon.Dispose();
        }

        private static void TurnOffDisplay()
        {
            Task.Run(() =>
            {
                _ = SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
            });
        }

        private static void TurnOnDisplay()
        {
            Task.Run(() =>
            {
                _ = SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON);
            });
        }

    }
}
