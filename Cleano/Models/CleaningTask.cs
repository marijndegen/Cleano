using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Cleano.Models
{
    public class CleaningTask : INotifyPropertyChanged
    {
        private string _name;
        private TaskFrequency _frequency;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public TaskFrequency Frequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                OnPropertyChanged();
            }
        }

        public List<DateTime> CompletedDates { get; set; } = new List<DateTime>();

        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public DateTime? LastCompleted => CompletedDates.Count > 0 ? CompletedDates[^1] : null;

        [JsonIgnore]
        public DateTime NextDueDate
        {
            get
            {
                var baseDate = LastCompleted ?? DateTime.MinValue;
                return Frequency switch
                {
                    TaskFrequency.Weekly => baseDate.AddDays(7),
                    TaskFrequency.EveryTwoWeeks => baseDate.AddDays(14),
                    TaskFrequency.EveryThreeWeeks => baseDate.AddDays(21),
                    TaskFrequency.Monthly => baseDate.AddMonths(1),
                    TaskFrequency.Quarterly => baseDate.AddMonths(3),
                    TaskFrequency.Yearly => baseDate.AddYears(1),
                    _ => baseDate
                };
            }
        }

        [JsonIgnore]
        public bool IsOverdue => DateTime.Now > NextDueDate;

        [JsonIgnore]
        public int DaysSinceDue => (DateTime.Now - NextDueDate).Days;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
