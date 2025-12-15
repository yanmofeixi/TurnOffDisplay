using System.Text.Json;

namespace DesktopAssistant
{
    public class TaskItem
    {
        public string TaskDescription { get; set; } = "";
        public DateTime ReminderTime { get; set; }
    }

    public class ReminderManager
    {
        private System.Windows.Forms.Timer? reminderTimer;
        private List<TaskItem> taskList = new();
        private HashSet<TaskItem> shownReminders = new();

        // 防止一次性/持续性弹窗过多造成打扰：整个程序生命周期内最多弹窗 5 次。
        // 如果你希望“每条任务最多弹 5 次”，可以把计数改成按任务维度维护。
        private const int MaxPopupCount = 5;
        private int shownPopupCount = 0;

        public void Start()
        {
            var jsonFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop), 
                "ToDo.json");

            if (File.Exists(jsonFilePath))
            {
                var json = File.ReadAllText(jsonFilePath);
                taskList = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new();
            }

            reminderTimer = new() { Interval = 1000 };
            reminderTimer.Tick += CheckReminders;
            reminderTimer.Start();
        }

        private void CheckReminders(object? sender, EventArgs e)
        {
            if (shownPopupCount >= MaxPopupCount)
            {
                // 达到上限后直接停止计时器，避免后续继续扫描与弹窗。
                reminderTimer?.Stop();
                return;
            }

            foreach (var task in taskList.Where(t => t.ReminderTime <= DateTime.Now && !shownReminders.Contains(t)))
            {
                if (shownPopupCount >= MaxPopupCount)
                {
                    reminderTimer?.Stop();
                    break;
                }

                shownReminders.Add(task);
                shownPopupCount++;
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(task.TaskDescription, "提醒");
            }
        }
    }
}

