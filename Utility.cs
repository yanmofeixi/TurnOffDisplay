using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TurnOffDisplay
{
    public static class Utility
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, string text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static nint GetHandleIfActiveProcessMatchName(string name)
        {
            var handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out var processId);
            var process = Process.GetProcessById((int)processId);
            var activeProcessName = process.ProcessName;
            if (activeProcessName != null)
            {
                if (activeProcessName == name)
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }
    }
}
