using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Checker
{
    /**
     * Ein Programm von RICH. LAW
     * A Programm made by RICH. LAW
    **/

    public partial class MainWindow : Window
    {

        private readonly string appFolder;
        private readonly string saveFile;
        private TaskItem editingTask = null;

        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;

            this.StateChanged += (s, e) =>
            {
                if (this.WindowState == WindowState.Minimized)
                    this.Topmost = false; // nicht mehr oben
                else
                    this.Topmost = true;  // wieder oben
            };

            //Zum erstellen des ordners in Appdata / For creating the folder in Appdata
            appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Checker");
            saveFile = Path.Combine(appFolder, "tasks.json");

            Tasks = new ObservableCollection<TaskItem>();
            TaskList.ItemsSource = Tasks;
            LoadTasks();
        }


        //Sortiert / Sorting
        private void SortByDate_Click(object sender, RoutedEventArgs e)
        {
            // Sortieren nach DueDate aufsteigend / Sorting by Duedate inceasing
            var sortedTasks = Tasks.OrderBy(t => t.DueDate).ToList();

            // Alte Sammlung leeren und die sortierten Tasks wieder hinzufügen / Emtying the Old Collection and putting the sorted in
            Tasks.Clear();
            foreach (var task in sortedTasks)
                Tasks.Add(task);

            SaveTasks();

        }

        //Fuegt eine Task hinzu :) / Adds a Task :)
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskTextBox.Text) && TaskDatePicker.SelectedDate != null)
            {
                Tasks.Add(new TaskItem
                {
                    Title = TaskTextBox.Text.Trim(),
                    DueDate = TaskDatePicker.SelectedDate.Value,
                    IsDone = false
                });
                TaskTextBox.Clear();
                SaveTasks();
            }
        }

        //Loescht Tasks / Delets Tasks
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem task)
            {
                Tasks.Remove(task);
                SaveTasks();
            }
        }


        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem task)
            {
                TaskTextBox.Text = task.Title;
                TaskDatePicker.SelectedDate = task.DueDate;


                Tasks.Remove(task);
                SaveTasks();
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem task)
            {
                int index = Tasks.IndexOf(task);
                if (index > 0)
                {
                    Tasks.Move(index, index - 1);
                    TaskList.SelectedItem = task;
                    SaveTasks();
                }
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem task)
            {
                int index = Tasks.IndexOf(task);
                if (index < Tasks.Count - 1)
                {
                    Tasks.Move(index, index + 1);
                    TaskList.SelectedItem = task;
                    SaveTasks();
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) => SaveTasks();
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e) => SaveTasks();

        private void EnsureStorage()
        {
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            if (!File.Exists(saveFile))
                File.WriteAllText(saveFile, "[]");
        }

        private void SaveTasks()
        {
            try
            {
                EnsureStorage();
                var json = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(saveFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
            }
        }

        private void LoadTasks()
        {
            try
            {
                EnsureStorage();
                var json = File.ReadAllText(saveFile);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<TaskItem>>(json);
                if (loaded != null)
                    Tasks = loaded;

                TaskList.ItemsSource = Tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden: {ex.Message}");
            }
        }
    }

    public class TaskItem
    {
        public string Title { get; set; } = "";
        public DateTime DueDate { get; set; }
        public bool IsDone { get; set; }
    }


}
