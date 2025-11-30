using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TurnOffDisplay
{
    public class HotkeyManager : Form
    {
        private static readonly string saveFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\";
        private static readonly string slFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\SL\";
        private static readonly string backupFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\Backup\";
        private static readonly string[] filesToCopy = { "MetaSave.save", "Save.save", "WorldSave.save", "Profiles.save" };

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_CONTROL = 0x02;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg != 0x0312 || !IsGameActive()) return;

            var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
            
            switch (key)
            {
                case Keys.D1: // 保存到 SL
                    CopyFiles(saveFolder, slFolder, slFolder, backupFolder);
                    break;
                case Keys.D2: // 从 SL 读取
                    CopyFiles(slFolder, saveFolder, saveFolder, backupFolder);
                    break;
                case Keys.D3: // 从备份恢复
                    foreach (var f in filesToCopy) File.Copy(backupFolder + f, saveFolder + f, true);
                    break;
            }
        }

        private static void CopyFiles(string from, string to, string backupFrom, string backupTo)
        {
            foreach (var f in filesToCopy)
            {
                File.Copy(backupFrom + f, backupTo + f, true);
                File.Copy(from + f, to + f, true);
            }
        }

        private static bool IsGameActive()
        {
            var handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out var pid);
            return Process.GetProcessById((int)pid).ProcessName == "Against the Storm";
        }

        public void Start()
        {
            RegisterHotKey(Handle, HOTKEY_ID, MOD_CONTROL, 0x31);
            RegisterHotKey(Handle, HOTKEY_ID + 1, MOD_CONTROL, 0x32);
            RegisterHotKey(Handle, HOTKEY_ID + 2, MOD_CONTROL, 0x33);
        }

        public void Stop()
        {
            UnregisterHotKey(Handle, HOTKEY_ID);
            UnregisterHotKey(Handle, HOTKEY_ID + 1);
            UnregisterHotKey(Handle, HOTKEY_ID + 2);
        }
    }
}