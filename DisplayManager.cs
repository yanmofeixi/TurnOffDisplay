using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace TurnOffDisplay
{
    internal class DisplayManager
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private static readonly int WM_SYSCOMMAND = 0x0112;
        private static readonly int SC_MONITORPOWER = 0xF170;
        private static readonly int MONITOR_ON = -1;
        private static readonly int MONITOR_OFF = 2;

        private Thread? listenerThread;

        public DisplayManager()
        {

        }

        public void Start()
        {
            this.SetUpTcpListener();
        }

        public void Stop()
        {
            this.listenerThread.Join();
        }

        private void SetUpTcpListener()
        {
            this.listenerThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 12345);
                listener.Stop();
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
                        this.TurnOffDisplay();
                    }
                    stream.Close();
                    client.Close();
                }
            })
            {
                IsBackground = true
            };
            this.listenerThread.Start();
        }

        public void TurnOffDisplay()
        {
            Task.Run(() =>
            {
                _ = SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_OFF);
            });
        }

        public void TurnOnDisplay()
        {
            Task.Run(() =>
            {
                _ = SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, MONITOR_ON);
            });
        }
    }
}
