using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TurnOffDisplay
{
    public partial class HotkeyManager : Form
    {
        private static string saveFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\";
        private static string slFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\SL\";
        private static string backupFolder = @"C:\Users\yanmo\AppData\LocalLow\Eremite Games\Against the Storm\Backup\";
        private static string[] filesToCopy = { "MetaSave.save", "Save.save", "WorldSave.save", "Profiles.save" };

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // Define the hotkey id and modifiers
        private const int HOTKEY_ID = 9000;

        private const uint MOD_CONTROL = 0x02;

        // Define key codes
        private const uint VK_1 = 0x31;
        private const uint VK_2 = 0x32;
        private const uint VK_3 = 0x33;

        // Enum to define the modifier keys
        public enum ModifierKeys : uint
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        public HotkeyManager()
        {

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // If the message is a hotkey message
            if (m.Msg == 0x0312)
            {
                // Get the keys
                var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                var modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                // Check which hotkey was pressed and take action
                if (key == Keys.D1 && modifier == ModifierKeys.Control)
                {
                    if (this.IsGameRunningInForeGround())
                    {
                        Debug.WriteLine("1");
                        foreach (var file in filesToCopy)
                        {
                            File.Copy(slFolder + file, backupFolder + file, true);
                            File.Copy(saveFolder + file, slFolder + file, true);
                        }
                    }
                }
                else if (key == Keys.D2 && modifier == ModifierKeys.Control)
                {
                    if (this.IsGameRunningInForeGround())
                    {
                        Debug.WriteLine("2");
                        foreach (var file in filesToCopy)
                        {
                            File.Copy(saveFolder + file, backupFolder + file, true);
                            File.Copy(slFolder + file, saveFolder + file, true);
                        }
                    }
                }
                else if (key == Keys.D3 && modifier == ModifierKeys.Control)
                {
                    if (this.IsGameRunningInForeGround())
                    {
                        Debug.WriteLine("3");
                        foreach (var file in filesToCopy)
                        {
                            File.Copy(backupFolder + file, saveFolder + file, true);
                        }
                    }
                }
            }
        }

        public void Start()
        {
            var res1 = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL, VK_1);
            var res2 = RegisterHotKey(this.Handle, HOTKEY_ID + 1, MOD_CONTROL, VK_2);
            var res3 = RegisterHotKey(this.Handle, HOTKEY_ID + 2, MOD_CONTROL, VK_3);

            // Write register success/failure  
            Debug.WriteLine("Register "
              + (res1 ? "Ctrl+1" : "FAIL") + " "
              + (res2 ? "Ctrl+2" : "FAIL") + " "
              + (res3 ? "Ctrl+3" : "FAIL"));
        }

        public void Stop()
        {
            this.UnregisterHotKeys();
        }

        private void UnregisterHotKeys()
        {
            var res1 = UnregisterHotKey(this.Handle, HOTKEY_ID);
            var res2 = UnregisterHotKey(this.Handle, HOTKEY_ID + 1);
            var res3 = UnregisterHotKey(this.Handle, HOTKEY_ID + 2);
            Debug.WriteLine("Unregister "
              + (res1 ? "Ctrl+1" : "FAIL") + " "
              + (res2 ? "Ctrl+2" : "FAIL") + " "
              + (res3 ? "Ctrl+3" : "FAIL"));
        }

        private bool IsGameRunningInForeGround()
        {
            var handle = Utility.GetHandleIfActiveProcessMatchName("Against the Storm");
            return handle != IntPtr.Zero;
        }
    }
}