using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;


namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
        public class TourViewModel : INotifyPropertyChanged
        {
            private ObservableCollection<Tour> _tours = new ObservableCollection<Tour>();
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

            public TourViewModel()
            {
                AddTourCommand = new RelayCommand(AddTour);
                DeleteTourCommand = new RelayCommand(DeleteTour, CanModifyTour);
                SaveTourCommand = new RelayCommand(SaveTour, CanModifyTour);
                RefreshCommand = new RelayCommand(ExecuteRefresh);
                Tours.CollectionChanged += (s, e) => UpdateCommandsState();
            }

            private bool CanModifyTour()
            {
                return SelectedTour != null;
            }

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
                Tours.Add(newTour);
                SelectedTour = newTour;  // Automatically select the new tour
            }

            private void DeleteTour()
            {
                if (SelectedTour != null)
                {
                    Tours.Remove(SelectedTour);
                    SelectedTour = null;  // Clear selection after deletion
                }
            }

            private void SaveTour()
            {
                if (SelectedTour != null && ValidateTour(SelectedTour))
                {
                    // Saving logic here
                    // Typically you would interact with a database or data service
                    // Assuming SaveTour actually performs the save operation
                    Console.WriteLine("Tour saved: " + SelectedTour.TourName);
                    // Update the UI or inform the user the save was successful
                }
            }

        private void ExecuteRefresh()
        {
            // Implement the logic to refresh the list of tours, e.g., re-fetching from a database
            Console.WriteLine("Tours list refreshed.");
        }

        private bool ValidateTour(Tour tour)
            {
                // Simple validation logic (expand according to real rules)
                return !string.IsNullOrEmpty(tour.TourName) &&
                       !string.IsNullOrEmpty(tour.From) &&
                       !string.IsNullOrEmpty(tour.To) &&
                       tour.TourDistance > 0 &&
                       tour.EstimatedTime.TotalMinutes > 0;
            }

            private void UpdateCommandsState()
            {
                // Manually refresh command states that depend on the selection
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
