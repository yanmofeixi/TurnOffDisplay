using Newtonsoft.Json;

namespace TurnOffDisplay
{
    public class TaskItem
    {
        public string TaskDescription { get; set; }
        public DateTime ReminderTime { get; set; }
    }

    public class ReminderManager
    {

        private System.Windows.Forms.Timer? reminderTimer;
        private List<TaskItem> taskList;

        private void SetupReminderTimer()
        {
            this.reminderTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            this.reminderTimer.Tick += this.ReminderTimer_Tick;
            this.reminderTimer.Start();
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            foreach (var task in this.taskList)
            {
                if (task.ReminderTime <= DateTime.Now)
                {
                    this.ShowReminder(task);
                }
            }
        }

        private void ShowReminder(TaskItem task)
        {
            MessageBox.Show(task.TaskDescription, "Reminder");
            System.Media.SystemSounds.Beep.Play(); // Alarm sound
        }

        public void Start()
        {
            // Get the path to the Desktop
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Combine it with the filename for the JSON file
            var jsonFilePath = Path.Combine(desktopPath, "ToDo.json");

            if (File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllText(jsonFilePath);
                this.taskList = JsonConvert.DeserializeObject<List<TaskItem>>(jsonContent) ?? new List<TaskItem>();
            }

            this.SetupReminderTimer();
        }
    }

}
