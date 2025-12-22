using System.Runtime.InteropServices;

namespace DesktopAssistant
{
    internal static class DisplayManager
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MONITORPOWER = 0xF170;

        public static void TurnOff() => 
            Task.Run(() => 
            {
                // 延迟 500ms 确保用户已经松开鼠标按键，避免立即唤醒
                Thread.Sleep(500);
                SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
            });
    }
}

