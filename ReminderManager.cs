using System.Text.Json;

namespace TurnOffDisplay
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
            foreach (var task in taskList.Where(t => t.ReminderTime <= DateTime.Now && !shownReminders.Contains(t)))
            {
                shownReminders.Add(task);
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(task.TaskDescription, "提醒");
            }
        }
    }
}
