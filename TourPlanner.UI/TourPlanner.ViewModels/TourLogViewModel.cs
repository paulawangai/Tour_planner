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
        private ObservableCollection<Tour> _tours;
        private TourLog _selectedTourLog;
        private DateTime _editTourLogDateTime;
        private string _editTourLogComment;
        private int _editTourLogDifficulty;
        private double _editTourLogTotalDistance;
        private TimeSpan _editTourLogTotalTime;
        private int _editTourLogRating;
        private string _editTourLogStatusMessage;
        private Tour _editTour;

        private readonly TourLogService _tourLogService;
        private readonly TourService _tourService;

        public TourLogViewModel(TourLogService tourLogService, TourService tourService)
        {
            _tourLogService = tourLogService;
            _tourService = tourService;

            TourLogs = new ObservableCollection<TourLog>(_tourLogService.GetAllTourLogs());
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourLogCommand = new RelayCommand(AddTourLog);
            UpdateTourLogCommand = new RelayCommand(UpdateTourLog, CanModifyDeleteTourLog);
            DeleteTourLogCommand = new RelayCommand(DeleteTourLog, CanModifyDeleteTourLog);
            RefreshTourLogsCommand = new RelayCommand(RefreshTourLogs);
        }

        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            set { _tourLogs = value; OnPropertyChanged(nameof(TourLogs)); }
        }

        public ObservableCollection<Tour> Tours
        {
            get => _tours;
            set { _tours = value; OnPropertyChanged(nameof(Tours)); }
        }

        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
                (DeleteTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();

                if (_selectedTourLog != null)
                {
                    EditTourLogDateTime = _selectedTourLog.DateTime;
                    EditTourLogComment = _selectedTourLog.Comment;
                    EditTourLogDifficulty = _selectedTourLog.Difficulty;
                    EditTourLogTotalDistance = _selectedTourLog.TotalDistance;
                    EditTourLogTotalTime = _selectedTourLog.TotalTime;
                    EditTourLogRating = _selectedTourLog.Rating;
                    EditTourLogStatusMessage = _selectedTourLog.StatusMessage;
                    EditTour = _selectedTourLog.Tour;
                }
            }
        }

        public DateTime EditTourLogDateTime
        {
            get => _editTourLogDateTime;
            set { _editTourLogDateTime = value; OnPropertyChanged(nameof(EditTourLogDateTime)); }
        }

        public string EditTourLogComment
        {
            get => _editTourLogComment;
            set { _editTourLogComment = value; OnPropertyChanged(nameof(EditTourLogComment)); }
        }

        public int EditTourLogDifficulty
        {
            get => _editTourLogDifficulty;
            set { _editTourLogDifficulty = value; OnPropertyChanged(nameof(EditTourLogDifficulty)); }
        }

        public double EditTourLogTotalDistance
        {
            get => _editTourLogTotalDistance;
            set { _editTourLogTotalDistance = value; OnPropertyChanged(nameof(EditTourLogTotalDistance)); }
        }

        public TimeSpan EditTourLogTotalTime
        {
            get => _editTourLogTotalTime;
            set { _editTourLogTotalTime = value; OnPropertyChanged(nameof(EditTourLogTotalTime)); }
        }

        public int EditTourLogRating
        {
            get => _editTourLogRating;
            set { _editTourLogRating = value; OnPropertyChanged(nameof(EditTourLogRating)); }
        }

        public string EditTourLogStatusMessage
        {
            get => _editTourLogStatusMessage;
            set { _editTourLogStatusMessage = value; OnPropertyChanged(nameof(EditTourLogStatusMessage)); }
        }

        public Tour EditTour
        {
            get => _editTour;
            set { _editTour = value; OnPropertyChanged(nameof(EditTour)); }
        }

        public ICommand AddTourLogCommand { get; private set; }
        public ICommand UpdateTourLogCommand { get; private set; }
        public ICommand DeleteTourLogCommand { get; private set; }
        public ICommand RefreshTourLogsCommand { get; private set; }

        private void AddTourLog(object parameter)
        {
            if (EditTour == null)
            {
                return;
            }

            var newLog = new TourLog
            {
                DateTime = DateTime.SpecifyKind(EditTourLogDateTime, DateTimeKind.Utc),
                Comment = EditTourLogComment,
                Difficulty = EditTourLogDifficulty,
                TotalDistance = EditTourLogTotalDistance,
                TotalTime = EditTourLogTotalTime,
                Rating = EditTourLogRating,
                TourId = EditTour.TourId,
                StatusMessage = EditTourLogStatusMessage
            };

            if (IsValidTourLog(newLog))
            {
                _tourLogService.AddTourLog(newLog);
                newLog.Tour = EditTour; // Set the associated Tour
                TourLogs.Add(newLog);
                SelectedTourLog = newLog;
                ClearFormFields();
            }
        }

        private void UpdateTourLog(object parameter)
        {
            if (SelectedTourLog != null)
            {
                _tourLogService.Save();
                RefreshTourLogs(null);
            }
        }

        private void DeleteTourLog(object parameter)
        {
            if (SelectedTourLog != null)
            {
                _tourLogService.DeleteTourLog(SelectedTourLog);
                TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
            }
        }

        private bool CanModifyDeleteTourLog(object parameter)
        {
            return SelectedTourLog != null;
        }

        private bool IsValidTourLog(TourLog tourLog)
        {
            return tourLog.TourId > 0 &&
                   !string.IsNullOrEmpty(tourLog.Comment) &&
                   tourLog.Difficulty > 0 &&
                   tourLog.TotalDistance > 0 &&
                   tourLog.TotalTime > TimeSpan.Zero &&
                   tourLog.Rating >= 1 && tourLog.Rating <= 5;
        }

        private void RefreshTourLogs(object parameter)
        {
            TourLogs = new ObservableCollection<TourLog>(_tourLogService.GetAllTourLogs().Select(tl =>
            {
                tl.Tour = _tourService.GetTourById(tl.TourId);
                return tl;
            }));
        }

        private void ClearFormFields()
        {
            EditTourLogDateTime = DateTime.UtcNow;
            EditTourLogComment = string.Empty;
            EditTourLogDifficulty = 0;
            EditTourLogTotalDistance = 0;
            EditTourLogTotalTime = TimeSpan.Zero;
            EditTourLogRating = 0;
            EditTourLogStatusMessage = string.Empty;
            EditTour = null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
