using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cleano.Models;
using Cleano.Services;
using Microsoft.Win32;

namespace Cleano.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private readonly PdfService _pdfService;
        private ObservableCollection<TaskGroup> _taskGroups;
        private string _newGroupName;
        private TaskGroup _selectedAdminGroup;
        private string _newTaskName;
        private TaskFrequency _selectedFrequency;

        public ObservableCollection<TaskGroup> TaskGroups
        {
            get => _taskGroups;
            set
            {
                _taskGroups = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SortedTaskGroups));
            }
        }

        public ObservableCollection<object> SortedTaskGroups
        {
            get
            {
                var sorted = new ObservableCollection<object>();
                foreach (var group in TaskGroups)
                {
                    var sortedGroup = new
                    {
                        Name = group.Name,
                        Tasks = new ObservableCollection<CleaningTask>(
                            group.Tasks.OrderBy(t => t.NextDueDate))
                    };
                    sorted.Add(sortedGroup);
                }
                return sorted;
            }
        }

        public string NewGroupName
        {
            get => _newGroupName;
            set
            {
                _newGroupName = value;
                OnPropertyChanged();
            }
        }

        public TaskGroup SelectedAdminGroup
        {
            get => _selectedAdminGroup;
            set
            {
                _selectedAdminGroup = value;
                OnPropertyChanged();
            }
        }

        public string NewTaskName
        {
            get => _newTaskName;
            set
            {
                _newTaskName = value;
                OnPropertyChanged();
            }
        }

        public TaskFrequency SelectedFrequency
        {
            get => _selectedFrequency;
            set
            {
                _selectedFrequency = value;
                OnPropertyChanged();
            }
        }

        public Array FrequencyOptions => Enum.GetValues(typeof(TaskFrequency));

        public RelayCommand AddGroupCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand DeleteGroupCommand { get; }
        public RelayCommand DeleteTaskCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand PrintCommand { get; }
        public RelayCommand SelectGroupCommand { get; }

        public MainViewModel()
        {
            _dataService = new DataService();
            _pdfService = new PdfService();
            TaskGroups = new ObservableCollection<TaskGroup>(_dataService.LoadData());

            AddGroupCommand = new RelayCommand(AddGroup);
            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteGroupCommand = new RelayCommand(DeleteGroup, param => param is TaskGroup);
            DeleteTaskCommand = new RelayCommand(DeleteTask, param => param is CleaningTask);
            SaveCommand = new RelayCommand(Save);
            PrintCommand = new RelayCommand(Print);
        }

        private void AddGroup(object obj)
        {
            if (!string.IsNullOrWhiteSpace(NewGroupName))
            {
                TaskGroups.Add(new TaskGroup { Name = NewGroupName });
                NewGroupName = string.Empty;
                OnPropertyChanged(nameof(SortedTaskGroups));
                Save(null);
            }
        }

        private bool CanAddTask(object obj)
        {
            return SelectedAdminGroup != null && !string.IsNullOrWhiteSpace(NewTaskName);
        }

        private void AddTask(object obj)
        {
            if (SelectedAdminGroup != null && !string.IsNullOrWhiteSpace(NewTaskName))
            {
                SelectedAdminGroup.Tasks.Add(new CleaningTask
                {
                    Name = NewTaskName,
                    Frequency = SelectedFrequency
                });
                NewTaskName = string.Empty;
                OnPropertyChanged(nameof(SortedTaskGroups));
                Save(null);
            }
        }

        private void DeleteGroup(object obj)
        {
            if (obj is TaskGroup group)
            {
                TaskGroups.Remove(group);
                OnPropertyChanged(nameof(SortedTaskGroups));
                Save(null);
            }
        }

        private void SelectGroup(object obj)
        {
            if (obj is TaskGroup group)
            {
                SelectedAdminGroup = group;
            }
        }

        private void DeleteTask(object obj)
        {
            if (obj is CleaningTask task)
            {
                foreach (var group in TaskGroups)
                {
                    if (group.Tasks.Contains(task))
                    {
                        group.Tasks.Remove(task);
                        break;
                    }
                }
                OnPropertyChanged(nameof(SortedTaskGroups));
                Save(null);
            }
        }

        private void Save(object obj)
        {
            _dataService.SaveData(TaskGroups.ToList());
        }

        private void Print(object obj)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"CleaningChecklist_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GeneratePdf(TaskGroups.ToList(), saveFileDialog.FileName);

                    foreach (var group in TaskGroups)
                    {
                        foreach (var task in group.Tasks.Where(t => t.IsSelected))
                        {
                            task.CompletedDates.Add(DateTime.Now);
                            task.IsSelected = false;

                            // Trigger property change notifications for computed properties
                            task.OnPropertyChanged(nameof(task.LastCompleted));
                            task.OnPropertyChanged(nameof(task.NextDueDate));
                            task.OnPropertyChanged(nameof(task.IsOverdue));
                            task.OnPropertyChanged(nameof(task.DaysSinceDue));
                        }
                    }

                    OnPropertyChanged(nameof(SortedTaskGroups));
                    Save(null);

                    MessageBox.Show($"PDF generated successfully!\n{saveFileDialog.FileName}", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating PDF: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
