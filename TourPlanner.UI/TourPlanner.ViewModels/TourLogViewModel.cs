using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;  

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourLogViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourLog> _tourLogs;
        private readonly TourLogService _tourLogService;
        private TourLog _selectedTourLog;
        private string _statusMessage; // Property to hold the status message


        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                if (_selectedTourLog != value)
                {
                    _selectedTourLog = value;
                    OnPropertyChanged(nameof(SelectedTourLog));
                    (DeleteTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (UpdateTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public ICommand AddTourLogCommand { get; private set; }
        public ICommand UpdateTourLogCommand { get; private set; }
        public ICommand DeleteTourLogCommand { get; private set; }
        public ICommand RefreshTourLogsCommand { get; private set; }

        public TourLogViewModel(TourLogService tourLogService)
        {
            _tourLogService = tourLogService;
            TourLogs = new ObservableCollection<TourLog>(_tourLogService.GetAllTourLogs());

            AddTourLogCommand = new RelayCommand(AddTourLog);
            UpdateTourLogCommand = new RelayCommand(UpdateTourLog, CanModifyDeleteTourLog);
            DeleteTourLogCommand = new RelayCommand(DeleteTourLog, CanModifyDeleteTourLog);
            RefreshTourLogsCommand = new RelayCommand(RefreshTourLogs);
        }

        private void AddTourLog()
        {
            TourLog newLog = new TourLog();  
            if (IsValidTourLog(newLog))
            {
                _tourLogService.AddTourLog(newLog);
                TourLogs.Add(newLog);
                SelectedTourLog = newLog;
                StatusMessage = "New tour log added successfully!";
            }
            else
            {
                StatusMessage = "Failed to add tour log. Please check the tour log details are correct and complete.";
            }
        }

        private void UpdateTourLog()
        {
            if (SelectedTourLog != null && IsValidTourLog(SelectedTourLog))
            {
                _tourLogService.UpdateTourLog(SelectedTourLog);
                OnPropertyChanged(nameof(TourLogs)); // Refresh the binding
                StatusMessage = "Tour log updated successfully!";
            }
            else
            {
                StatusMessage = "Update failed. Ensure all fields are correctly filled and valid.";
            }
        }


        private void DeleteTourLog()
        {
            if (SelectedTourLog != null)
            {
                _tourLogService.DeleteTourLog(SelectedTourLog);
                TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
            }
        }

        private bool CanModifyDeleteTourLog()
        {
            return SelectedTourLog != null;
        }

        private bool IsValidTourLog(TourLog tourLog)
        {
            return !string.IsNullOrEmpty(tourLog.Comment) &&
                   tourLog.Difficulty > 0 &&
                   tourLog.TotalDistance > 0 &&
                   tourLog.TotalTime > TimeSpan.Zero &&
                   tourLog.Rating >= 1 && tourLog.Rating <= 5; // Rating is between 1 and 5
        }


        private void RefreshTourLogs()
        {
            TourLogs = new ObservableCollection<TourLog>(_tourLogService.GetAllTourLogs());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
