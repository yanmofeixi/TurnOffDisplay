using System.Runtime.InteropServices;

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

        public DisplayManager()
        {
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
