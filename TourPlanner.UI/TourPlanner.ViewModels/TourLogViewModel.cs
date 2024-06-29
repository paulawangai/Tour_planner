using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.Commands;
using System.Linq;
using log4net;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourLogViewModel : ViewModelBase
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(TourLogViewModel));
        private readonly TourLogService _tourLogService;
        private readonly TourService _tourService;

        public ObservableCollection<TourLog> TourLogs { get; private set; }
        public ObservableCollection<Tour> Tours { get; private set; }

        private TourLog _selectedTourLog;
        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                SetProperty(ref _selectedTourLog, value);
                (UpdateTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private Tour _selectedTour;
<<<<<<< HEAD
        public Tour SelectedTour {
            get => _selectedTour;
            set {
=======
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
>>>>>>> d515d74c3d1101b9e5ffc068a20d2105d2bc2b6c
                SetProperty(ref _selectedTour, value);
            }
        }

        // Properties for new tour log input
        public DateTime NewTourLogDateTime { get; set; } = DateTime.Now;
        public string NewTourLogComment { get; set; }
        public int NewTourLogDifficulty { get; set; }
        public double NewTourLogTotalDistance { get; set; }
        public TimeSpan NewTourLogTotalTime { get; set; }
        public int NewTourLogRating { get; set; }
        public string NewTourLogStatusMessage { get; set; }

        // Commands
        public ICommand AddTourLogCommand { get; }
        public ICommand UpdateTourLogCommand { get; }
        public ICommand DeleteTourLogCommand { get; }
        public ICommand SearchCommand { get; }

        public string SearchText { get; set; }

<<<<<<< HEAD
        public TourLogViewModel(TourLogService tourLogService, TourService tourService) {
=======
        public TourLogViewModel(TourLogService tourLogService, TourService tourService)
        {
>>>>>>> d515d74c3d1101b9e5ffc068a20d2105d2bc2b6c
            _tourLogService = tourLogService;
            _tourService = tourService;

            TourLogs = new ObservableCollection<TourLog>(_tourLogService.GetAllTourLogs());
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourLogCommand = new RelayCommand(param => AddTourLog());
            UpdateTourLogCommand = new RelayCommand(param => UpdateTourLog(), param => CanModifyTourLog());
            DeleteTourLogCommand = new RelayCommand(param => DeleteTourLog(), param => CanModifyTourLog());
            SearchCommand = new RelayCommand(param => SearchTourLogs());
        }

<<<<<<< HEAD
        private void AddTourLog() {
            if (SelectedTour != null) {
                log.Info("Adding a new tour log.");
                var newTourLog = new TourLog {
=======
        private void AddTourLog()
        {
            if (SelectedTour != null)
            {
                log.Info("Adding a new tour log.");
                var newTourLog = new TourLog
                {
>>>>>>> d515d74c3d1101b9e5ffc068a20d2105d2bc2b6c
                    TourId = SelectedTour.TourId,
                    DateTime = NewTourLogDateTime,
                    Comment = NewTourLogComment,
                    Difficulty = NewTourLogDifficulty,
                    TotalDistance = NewTourLogTotalDistance,
                    TotalTime = NewTourLogTotalTime,
                    Rating = NewTourLogRating,
<<<<<<< HEAD
                    StatusMessage = NewTourLogStatusMessage,
=======
                    StatusMessage = NewTourLogStatusMessage
>>>>>>> d515d74c3d1101b9e5ffc068a20d2105d2bc2b6c
                };

                _tourLogService.AddTourLog(newTourLog);
                TourLogs.Add(newTourLog);
                ClearNewTourLogFields();
                log.Info("New tour log added.");
            }
<<<<<<< HEAD
            else {
=======
            else
            {
>>>>>>> d515d74c3d1101b9e5ffc068a20d2105d2bc2b6c
                log.Warn("No tour selected. Cannot add tour log.");
            }
        }

        private void UpdateTourLog()
        {
            if (SelectedTourLog != null)
            {
                log.Info($"Updating tour log ID: {SelectedTourLog.TourLogId}");
                SelectedTourLog.TourId = SelectedTour?.TourId ?? SelectedTourLog.TourId;
                SelectedTourLog.DateTime = NewTourLogDateTime;
                SelectedTourLog.Comment = NewTourLogComment;
                SelectedTourLog.Difficulty = NewTourLogDifficulty;
                SelectedTourLog.TotalDistance = NewTourLogTotalDistance;
                SelectedTourLog.TotalTime = NewTourLogTotalTime;
                SelectedTourLog.Rating = NewTourLogRating;
                SelectedTourLog.StatusMessage = NewTourLogStatusMessage;

                _tourLogService.Save();
                RefreshTourLogs();
                log.Info("Tour log updated.");
            }
        }

        private void DeleteTourLog()
        {
            if (SelectedTourLog != null)
            {
                log.Info($"Deleting tour log ID: {SelectedTourLog.TourLogId}");
                _tourLogService.DeleteTourLog(SelectedTourLog);
                TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
                log.Info("Tour log deleted.");
            }
        }

        private bool CanModifyTourLog() => SelectedTourLog != null;

        private void SearchTourLogs()
        {
            log.Info($"Searching tour logs with text: {SearchText}");
            var searchResults = _tourLogService.SearchTourLogs(SearchText);
            TourLogs.Clear();
            foreach (var tourLog in searchResults)
            {
                TourLogs.Add(tourLog);
            }
            log.Info("Tour logs search completed.");
        }

        private void RefreshTourLogs()
        {
            TourLogs.Clear();
            foreach (var tourLog in _tourLogService.GetAllTourLogs())
            {
                TourLogs.Add(tourLog);
            }
        }

        private void ClearNewTourLogFields()
        {
            NewTourLogDateTime = DateTime.Now;
            NewTourLogComment = string.Empty;
            NewTourLogDifficulty = 0;
            NewTourLogTotalDistance = 0;
            NewTourLogTotalTime = TimeSpan.Zero;
            NewTourLogRating = 0;
            NewTourLogStatusMessage = string.Empty;
            OnPropertyChanged(nameof(NewTourLogDateTime));
            OnPropertyChanged(nameof(NewTourLogComment));
            OnPropertyChanged(nameof(NewTourLogDifficulty));
            OnPropertyChanged(nameof(NewTourLogTotalDistance));
            OnPropertyChanged(nameof(NewTourLogTotalTime));
            OnPropertyChanged(nameof(NewTourLogRating));
            OnPropertyChanged(nameof(NewTourLogStatusMessage));
        }
    }
}
