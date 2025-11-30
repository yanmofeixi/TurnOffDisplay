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

        // 记录已启动的AHK进程，避免重复启动
        private readonly Dictionary<string, Process?> runningAhkProcesses = new();
        
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
            foreach (var process in runningAhkProcesses.Values)
            {
                try
                {
                    if (process != null && !process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch { }
            }
            runningAhkProcesses.Clear();
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
            if (runningAhkProcesses.TryGetValue(gameName, out var existingProcess) && 
                existingProcess != null && !existingProcess.HasExited)
            {
                return;
            }

            var ahkPath = Path.Combine(ahkFolder, ahkScript);
            if (!File.Exists(ahkPath)) return;

            try
            {
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = ahkPath,
                    UseShellExecute = true
                });
                runningAhkProcesses[gameName] = process;
            }
            catch { }
        }

        private void StopAhkScript(string gameName)
        {
            if (!runningAhkProcesses.TryGetValue(gameName, out var process)) return;

            try
            {
                if (process != null && !process.HasExited)
                {
                    process.Kill();
                }
            }
            catch { }
            
            runningAhkProcesses.Remove(gameName);
        }
    }
}

