using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Nts.Providers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using NetTopologySuite.Geometries;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

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
                     _ = DisplayRouteOnMap(value.From, value.To);
                }
            }
        }

        public ICommand AddTourCommand { get; }
        public ICommand SaveTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand UpdateRouteCommand { get; }

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
            UpdateRouteCommand = new RelayCommand(UpdateRoute);
            UpdateTourCommand = new RelayCommand(UpdateTour, CanExecuteTourCommand);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanExecuteTourCommand);
        }

        public void Initialize(MapControl mapControl)
        {
            _mapControl = mapControl;
            _mapControl.Map = new Map();
            _mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        }



        private async Task DisplayRouteOnMap(string from, string to)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                {
                    Console.WriteLine("From or To locations are empty.");
                    return;
                }

                var fromCoordinates = await _routeService.GetCoordinatesAsync(from);
                var toCoordinates = await _routeService.GetCoordinatesAsync(to);

                string start = $"{fromCoordinates.Longitude},{fromCoordinates.Latitude}";
                string end = $"{toCoordinates.Longitude},{toCoordinates.Latitude}";

                Console.WriteLine($"Requesting route from {start} to {end}");

                var route = await _routeService.GetRouteAsync(start, end);

                var coordinates = route["features"]?[0]?["geometry"]?["coordinates"]?.ToObject<double[][]>();
                var distance = route["features"]?[0]?["properties"]?["segments"]?[0]?["distance"]?.ToObject<double>();
                var duration = route["features"]?[0]?["properties"]?["segments"]?[0]?["duration"]?.ToObject<double>();

                if (coordinates == null || distance == null || duration == null)
                {
                    throw new Exception("Invalid route data received.");
                }

                Console.WriteLine("Route coordinates received:");
                foreach (var coord in coordinates)
                {
                    Console.WriteLine($"Longitude: {coord[0]}, Latitude: {coord[1]}");
                }

                var lineString = new LineString(coordinates.Select(coord => new Coordinate(coord[0], coord[1])).ToArray());

                var customFeature = new CustomFeature(lineString)
                {
                    Styles = { new VectorStyle
            {
                Line = new Pen
                {
                    Color = Mapsui.Styles.Color.Red,
                    Width = 2
                }
            }}
                };

                var layer = new MemoryLayer
                {
                    Name = "Route Layer",
                    Features = new ObservableCollection<IFeature> { customFeature }
                };

                Console.WriteLine("Adding layers to the map control.");

                _mapControl.Map.Layers.Clear();
                _mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
                _mapControl.Map.Layers.Add(layer);

                var envelope = lineString.EnvelopeInternal.ToMRect();
                _mapControl.Map.Home = n => n.ZoomToBox(envelope.Grow(1.1));
                _mapControl.Refresh();

                // Update the form fields
                NewTourDistance = distance.Value / 1000; // Convert to kilometers
                NewTourEstimatedTime = TimeSpan.FromSeconds(duration.Value);

                Console.WriteLine("Route displayed on map successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error displaying route on map: {ex.Message}");
            }
        }




        private async void UpdateRoute(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(NewTourFrom) && !string.IsNullOrWhiteSpace(NewTourTo))
            {
                await DisplayRouteOnMap(NewTourFrom, NewTourTo);
            }
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
