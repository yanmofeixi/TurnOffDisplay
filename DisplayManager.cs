using System.Runtime.InteropServices;

namespace TurnOffDisplay
{
    internal static class DisplayManager
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;

        public static void TurnOff() => 
            Task.Run(() => SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, 2));

        public static void TurnOn() => 
            Task.Run(() => SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, -1));
    }
}
