using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Tour> _tours = new ObservableCollection<Tour>();
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService;

        public ObservableCollection<Tour> Tours
        {
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour != value)
                {
                    _selectedTour = value;
                    OnPropertyChanged(nameof(SelectedTour));
                    UpdateCommandsState();
                }
            }
        }

        public ICommand AddTourCommand { get; private set; }
        public ICommand DeleteTourCommand { get; private set; }
        public ICommand SaveTourCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        // Constructor accepts TourService, which should be provided by the higher-level composition root
        public TourViewModel(TourService tourService, TourLogService tourLogService)
        {
            _tourService = tourService;
            _tourLogService = tourLogService;

            LoadTours();
            InitializeCommands();
        }

        private void LoadTours()
        {
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());
        }

        private void InitializeCommands()
        {
            AddTourCommand = new RelayCommand(AddTour);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanModifyTour);
            SaveTourCommand = new RelayCommand(SaveTour, CanModifyTour);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
        }

        private bool CanModifyTour() => SelectedTour != null;

        private void AddTour()
        {
            Tour newTour = new Tour
            {
                TourName = "New Tour",
                Description = "Description",
                From = "Start Location",
                To = "Destination",
                TransportType = "Bike",  // Default values can be set here
                TourDistance = 0,
                EstimatedTime = TimeSpan.Zero,
                RouteImage = "path_to_default_image.png"
            };
            _tourService.AddTour(newTour);
            Tours.Add(newTour);
            SelectedTour = newTour;  // Automatically select the new tour
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                SelectedTour = null;  // Clear selection after deletion
            }
        }

        private void SaveTour()
        {
            if (SelectedTour != null && ValidateTour(SelectedTour))
            {
                _tourService.UpdateTour(SelectedTour);
                Console.WriteLine("Tour saved: " + SelectedTour.TourName);
            }
        }

        private void ExecuteRefresh()
        {
            LoadTours();
            Console.WriteLine("Tours list refreshed.");
        }

        private bool ValidateTour(Tour tour)
        {
            return !string.IsNullOrEmpty(tour.TourName) &&
                   !string.IsNullOrEmpty(tour.From) &&
                   !string.IsNullOrEmpty(tour.To) &&
                   tour.TourDistance > 0 &&
                   tour.EstimatedTime.TotalMinutes > 0;
        }

        private void UpdateCommandsState()
        {
            (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (SaveTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
