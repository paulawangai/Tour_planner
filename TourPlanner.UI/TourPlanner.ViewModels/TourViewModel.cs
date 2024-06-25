using System.Collections.ObjectModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Extensions;
using Mapsui.UI.Wpf;
using NetTopologySuite.Geometries;
using Mapsui.Tiling;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourViewModel : ViewModelBase
    {
        private readonly TourService _tourService;
        private readonly OpenRouteService _routeService;
        private MapControl _mapControl;

        public ObservableCollection<Tour> Tours { get; set; }

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (SetProperty(ref _selectedTour, value) && value != null)
                {
                    // Update fields with selected tour details
                    NewTourName = value.TourName;
                    NewTourDescription = value.Description;
                    NewTourFrom = value.From;
                    NewTourTo = value.To;
                    NewTourTransportType = value.TransportType;
                    NewTourDistance = value.TourDistance;
                    NewTourEstimatedTime = value.EstimatedTime;

                    // Display route on the map
                    DisplayRouteOnMap(value.From, value.To);
                }
            }
        }

        public ICommand AddTourCommand { get; }
        public ICommand SaveTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }

        private string _newTourName;
        private string _newTourDescription;
        private string _newTourFrom;
        private string _newTourTo;
        private string _newTourTransportType;
        private double _newTourDistance;
        private TimeSpan _newTourEstimatedTime;

        public string NewTourName
        {
            get => _newTourName;
            set => SetProperty(ref _newTourName, value);
        }

        public string NewTourDescription
        {
            get => _newTourDescription;
            set => SetProperty(ref _newTourDescription, value);
        }

        public string NewTourFrom
        {
            get => _newTourFrom;
            set => SetProperty(ref _newTourFrom, value);
        }

        public string NewTourTo
        {
            get => _newTourTo;
            set => SetProperty(ref _newTourTo, value);
        }

        public string NewTourTransportType
        {
            get => _newTourTransportType;
            set => SetProperty(ref _newTourTransportType, value);
        }

        public double NewTourDistance
        {
            get => _newTourDistance;
            set => SetProperty(ref _newTourDistance, value);
        }

        public TimeSpan NewTourEstimatedTime
        {
            get => _newTourEstimatedTime;
            set => SetProperty(ref _newTourEstimatedTime, value);
        }

        public TourViewModel(TourService tourService, OpenRouteService routeService)
        {
            _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));

            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(AddTour);
            SaveTourCommand = new RelayCommand(SaveTour);
            UpdateTourCommand = new RelayCommand(UpdateTour, CanExecuteTourCommand);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanExecuteTourCommand);
        }

        public void Initialize(MapControl mapControl)
        {
            _mapControl = mapControl;
            _mapControl.Map = new Map();
            _mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        }

        private async void DisplayRouteOnMap(string from, string to)
        {
            var route = await _routeService.GetRouteAsync(from, to);
            var coordinates = route["routes"][0]["geometry"]["coordinates"].ToObject<double[][]>();

            var lineString = new LineString(coordinates.Select(coord => new Coordinate(coord[0], coord[1])).ToArray());

            // Use Mapsui.Data.Feature to create your feature
            var mapsuiFeature = new Feature
            {
                Geometry = lineString
            };

            var layer = new MemoryLayer
            {
                Name = "Route Layer",
                Features = new ObservableCollection<IFeature> { mapsuiFeature }
            };

            _mapControl.Map.Layers.Clear();
            _mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            _mapControl.Map.Layers.Add(layer);

            var envelope = lineString.EnvelopeInternal;
            _mapControl.Navigator.ZoomToBox(new MRect(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY));
        }



        private void AddTour(object parameter)
        {
            if (ValidateTourDetails())
            {
                var newTour = new Tour
                {
                    TourName = NewTourName,
                    Description = NewTourDescription,
                    From = NewTourFrom,
                    To = NewTourTo,
                    TransportType = NewTourTransportType,
                    TourDistance = NewTourDistance,
                    EstimatedTime = NewTourEstimatedTime,
                    RouteImage = "default_image.png" // Provide a default value or set appropriately
                };

                _tourService.AddTour(newTour);
                Tours.Add(newTour);

                // Clear the form fields after adding the tour
                ClearFormFields();
            }
            else
            {
                // Handle validation errors (e.g., show a message to the user)
                Console.WriteLine("Validation failed. Ensure all required fields are filled.");
            }
        }

        private void SaveTour(object parameter)
        {
            _tourService.Save();
        }

        private void UpdateTour(object parameter)
        {
            if (SelectedTour != null && ValidateTourDetails())
            {
                SelectedTour.TourName = NewTourName;
                SelectedTour.Description = NewTourDescription;
                SelectedTour.From = NewTourFrom;
                SelectedTour.To = NewTourTo;
                SelectedTour.TransportType = NewTourTransportType;
                SelectedTour.TourDistance = NewTourDistance;
                SelectedTour.EstimatedTime = NewTourEstimatedTime;

                _tourService.Save();
                // Refresh the list
                RefreshTours();
            }
        }

        private void DeleteTour(object parameter)
        {
            if (SelectedTour != null)
            {
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                ClearFormFields();
            }
        }

        private bool CanExecuteTourCommand(object parameter)
        {
            return SelectedTour != null;
        }

        private bool ValidateTourDetails()
        {
            return !string.IsNullOrWhiteSpace(NewTourName) &&
                   !string.IsNullOrWhiteSpace(NewTourFrom) &&
                   !string.IsNullOrWhiteSpace(NewTourTo);
        }

        private void ClearFormFields()
        {
            NewTourName = string.Empty;
            NewTourDescription = string.Empty;
            NewTourFrom = string.Empty;
            NewTourTo = string.Empty;
            NewTourTransportType = string.Empty;
            NewTourDistance = 0;
            NewTourEstimatedTime = TimeSpan.Zero;
        }

        private void RefreshTours()
        {
            Tours.Clear();
            var updatedTours = _tourService.GetAllTours();
            foreach (var tour in updatedTours)
            {
                Tours.Add(tour);
            }
        }
    }
}
