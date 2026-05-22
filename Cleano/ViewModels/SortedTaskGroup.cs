using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Cleano.Models;

namespace Cleano.ViewModels
{
    public class SortedTaskGroup : INotifyPropertyChanged
    {
        private TaskGroup _taskGroup;

        public SortedTaskGroup(TaskGroup taskGroup)
        {
            _taskGroup = taskGroup;
            _taskGroup.Tasks.CollectionChanged += (s, e) => UpdateSortedTasks();
            UpdateSortedTasks();
        }

        public string Name => _taskGroup.Name;

        public ObservableCollection<CleaningTask> SortedTasks { get; private set; } = new ObservableCollection<CleaningTask>();

        private void UpdateSortedTasks()
        {
            SortedTasks.Clear();
            foreach (var task in _taskGroup.Tasks.OrderBy(t => t.NextDueDate))
            {
                SortedTasks.Add(task);
            }
            OnPropertyChanged(nameof(SortedTasks));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
