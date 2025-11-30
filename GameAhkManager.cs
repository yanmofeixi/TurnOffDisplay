using System.Diagnostics;
using System.Management;

namespace DesktopAssistant
{
    public class GameAhkManager
    {
        private static readonly string ahkFolder = @"C:\Code\AHK\";
        
        // 游戏进程名 -> AHK脚本名的映射
        private static readonly Dictionary<string, string> gameAhkMapping = new()
        {
            { "GenshinImpact", "Genshin.ahk" },
            { "StarRail", "StarRail.ahk" }
        };

        // 记录哪些游戏的AHK已启动
        private readonly HashSet<string> runningGames = new();
        
        private ManagementEventWatcher? startWatcher;
        private ManagementEventWatcher? stopWatcher;

        public void Start()
        {
            // 检查当前已运行的游戏，启动对应AHK
            CheckRunningGames();

            // 监控进程启动事件
            startWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatcher.EventArrived += OnProcessStarted;
            startWatcher.Start();

            // 监控进程退出事件
            stopWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatcher.EventArrived += OnProcessStopped;
            stopWatcher.Start();
        }

        public void Stop()
        {
            startWatcher?.Stop();
            startWatcher?.Dispose();
            stopWatcher?.Stop();
            stopWatcher?.Dispose();

            // 关闭所有已启动的AHK进程
            foreach (var game in runningGames.ToList())
            {
                StopAhkScript(game);
            }
            runningGames.Clear();
        }

        private void CheckRunningGames()
        {
            foreach (var game in gameAhkMapping.Keys)
            {
                var processes = Process.GetProcessesByName(game);
                if (processes.Length > 0)
                {
                    StartAhkScript(game);
                }
            }
        }

        private void OnProcessStarted(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value?.ToString();
            if (string.IsNullOrEmpty(processName)) return;

            // 移除.exe后缀进行匹配
            var nameWithoutExt = Path.GetFileNameWithoutExtension(processName);
            
            if (gameAhkMapping.ContainsKey(nameWithoutExt))
            {
                StartAhkScript(nameWithoutExt);
            }
        }

        private void OnProcessStopped(object sender, EventArrivedEventArgs e)
        {
            var processName = e.NewEvent.Properties["ProcessName"].Value?.ToString();
            if (string.IsNullOrEmpty(processName)) return;

            var nameWithoutExt = Path.GetFileNameWithoutExtension(processName);
            
            if (gameAhkMapping.ContainsKey(nameWithoutExt))
            {
                StopAhkScript(nameWithoutExt);
            }
        }

        private void StartAhkScript(string gameName)
        {
            if (!gameAhkMapping.TryGetValue(gameName, out var ahkScript)) return;
            
            // 如果已经在运行，不重复启动
            if (runningGames.Contains(gameName))
            {
                return;
            }

            var ahkPath = Path.Combine(ahkFolder, ahkScript);
            if (!File.Exists(ahkPath)) return;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ahkPath,
                    UseShellExecute = true
                });
                runningGames.Add(gameName);
            }
            catch { }
        }

        private void StopAhkScript(string gameName)
        {
            if (!gameAhkMapping.TryGetValue(gameName, out var ahkScript)) return;
            if (!runningGames.Contains(gameName)) return;

            var scriptPath = Path.Combine(ahkFolder, ahkScript);
            
            try
            {
                // 查找所有 AutoHotkey 进程，通过命令行参数匹配脚本
                using var searcher = new ManagementObjectSearcher(
                    "SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name LIKE 'AutoHotkey%'");
                
                foreach (ManagementObject obj in searcher.Get())
                {
                    var commandLine = obj["CommandLine"]?.ToString() ?? "";
                    if (commandLine.Contains(ahkScript, StringComparison.OrdinalIgnoreCase) ||
                        commandLine.Contains(scriptPath, StringComparison.OrdinalIgnoreCase))
                    {
                        var pid = Convert.ToInt32(obj["ProcessId"]);
                        try
                        {
                            Process.GetProcessById(pid).Kill();
                        }
                        catch { }
                    }
                }
            }
            catch { }
            
            runningGames.Remove(gameName);
        }
    }
}

