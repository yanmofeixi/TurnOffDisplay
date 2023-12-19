using System.Runtime.InteropServices;

namespace TurnOffDisplay
{
    public partial class TitleBarRemover : Form
    {
        // Win32 API declarations
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlagske);

        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_SYSMENU = 0x00080000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_MINIMIZE = 0x20000000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const long WS_EX_DLGMODALFRAME = 0x0001L;

        private Thread? listenerThread;

        public TitleBarRemover()
        {

        }

        public void Start()
        {
            this.listenerThread = new Thread(() =>
            {
                while (true)
                {
                    var handle = Utility.GetHandleIfActiveProcessMatchName("GenshinImpact");
                    if (handle != IntPtr.Zero)
                    {
                        this.RemoveTitleBar(handle);
                    }
                    Thread.Sleep(1000);
                }
            })
            {
                IsBackground = true
            };
            this.listenerThread.Start();
        }

        public void Stop()
        {
            this.listenerThread.Join();
        }

        // Method to remove title bar of another program
        public void RemoveTitleBar(nint hwnd)
        {
            // Get current style
            long lCurStyle = GetWindowLong(hwnd, GWL_STYLE);
            // Remove title bar elements
            lCurStyle &= ~WS_CAPTION;
            lCurStyle &= ~WS_SYSMENU;
            lCurStyle &= ~WS_THICKFRAME;
            lCurStyle &= ~WS_MINIMIZE;
            lCurStyle &= ~WS_MAXIMIZEBOX;

            // Apply new style
            _ = SetWindowLong(hwnd, GWL_STYLE, (int)lCurStyle);
            // Reapply a 3d border
            lCurStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            _ = SetWindowLong(hwnd, GWL_EXSTYLE, (int)(lCurStyle | WS_EX_DLGMODALFRAME));
            // Get current extended style
            long lCurExStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            // Remove window border
            lCurExStyle &= ~WS_EX_DLGMODALFRAME;
            // Apply new extended style
            _ = SetWindowLong(hwnd, GWL_EXSTYLE, (int)lCurExStyle);// Get screen size
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;
            // Redraw
            SetWindowPos(hwnd, IntPtr.Zero,
                    0, 0,
                    screenWidth,
                    screenHeight,
                    SWP_NOMOVE | SWP_FRAMECHANGED);
        }
    }
}