using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Nts.Providers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI;
using Mapsui.UI.Wpf;
using Microsoft.Win32;
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
        private readonly TourReportService _tourReportService;


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
                    NewTourPopularity = value.Popularity;
                    NewTourChildFriendliness = value.ChildFriendliness;


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
        public ICommand ExportToursCommand { get; }
        public ICommand ImportToursCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }

        private string _newTourName;
        private string _newTourDescription;
        private string _newTourFrom;
        private string _newTourTo;
        private string _newTourTransportType;
        private double _newTourDistance;
        private TimeSpan _newTourEstimatedTime;
        private int _newTourPopularity;
        private double _newTourChildFriendliness;
        private string _searchText;

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

        public int NewTourPopularity
        {
            get => _newTourPopularity;
            set => SetProperty(ref _newTourPopularity, value);
        }

        public double NewTourChildFriendliness
        {
            get => _newTourChildFriendliness;
            set => SetProperty(ref _newTourChildFriendliness, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public TourViewModel(TourService tourService, OpenRouteService routeService, TourReportService tourReportService)
        {
            _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            _tourReportService = tourReportService ?? throw new ArgumentNullException(nameof(tourReportService));


            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(AddTour);
            SaveTourCommand = new RelayCommand(SaveTour);
            UpdateRouteCommand = new RelayCommand(UpdateRoute);
            UpdateTourCommand = new RelayCommand(UpdateTour, CanExecuteTourCommand);
            DeleteTourCommand = new RelayCommand(DeleteTour, CanExecuteTourCommand);
            ExportToursCommand = new RelayCommand(ExportTours);
            ImportToursCommand = new RelayCommand(ImportTours);
            SearchCommand = new RelayCommand(SearchTours);
            GenerateTourReportCommand = new RelayCommand(GenerateTourReport);
            GenerateSummaryReportCommand = new RelayCommand(GenerateSummaryReport);
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
                    RouteImage = "default_image.png",
                    Popularity = NewTourPopularity,
                    ChildFriendliness = NewTourChildFriendliness
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
                SelectedTour.Popularity = NewTourPopularity;
                SelectedTour.ChildFriendliness = NewTourChildFriendliness;

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

        private void ExportTours(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Export Tours to PDF"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                using (var writer = new PdfWriter(filePath))
                {
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    document.Add(new Paragraph("Tour List").SetFontSize(20).SetBold());

                    foreach (var tour in Tours)
                    {
                        document.Add(new Paragraph($"Tour Name: {tour.TourName}"));
                        document.Add(new Paragraph($"Description: {tour.Description}"));
                        document.Add(new Paragraph($"From: {tour.From}"));
                        document.Add(new Paragraph($"To: {tour.To}"));
                        document.Add(new Paragraph($"Transport Type: {tour.TransportType}"));
                        document.Add(new Paragraph($"Distance (km): {tour.TourDistance}"));
                        document.Add(new Paragraph($"Estimated Time (hrs): {tour.EstimatedTime}"));
                        document.Add(new Paragraph($"------------------------------------------------"));
                    }

                    document.Close();
                }
            }
        }

        private void ImportTours(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Import Tours from PDF"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                using (var reader = new PdfReader(filePath))
                {
                    var pdf = new PdfDocument(reader);
                    var strategy = new SimpleTextExtractionStrategy();
                    var extractedText = string.Empty;

                    for (int page = 1; page <= pdf.GetNumberOfPages(); page++)
                    {
                        extractedText += PdfTextExtractor.GetTextFromPage(pdf.GetPage(page), strategy);
                    }

                    var tours = ParseToursFromText(extractedText);
                    foreach (var tour in tours)
                    {
                        _tourService.AddTour(tour);
                        Tours.Add(tour);
                    }
                }
            }
        }

        private List<Tour> ParseToursFromText(string text)
        {
            var tours = new List<Tour>();
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Tour currentTour = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("Tour Name:"))
                {
                    if (currentTour != null)
                    {
                        tours.Add(currentTour);
                    }
                    currentTour = new Tour();
                    currentTour.TourName = line.Replace("Tour Name:", "").Trim();
                }
                else if (line.StartsWith("Description:") && currentTour != null)
                {
                    currentTour.Description = line.Replace("Description:", "").Trim();
                }
                else if (line.StartsWith("From:") && currentTour != null)
                {
                    currentTour.From = line.Replace("From:", "").Trim();
                }
                else if (line.StartsWith("To:") && currentTour != null)
                {
                    currentTour.To = line.Replace("To:", "").Trim();
                }
                else if (line.StartsWith("Transport Type:") && currentTour != null)
                {
                    currentTour.TransportType = line.Replace("Transport Type:", "").Trim();
                }
                else if (line.StartsWith("Distance (km):") && currentTour != null)
                {
                    currentTour.TourDistance = double.Parse(line.Replace("Distance (km):", "").Trim());
                }
                else if (line.StartsWith("Estimated Time (hrs):") && currentTour != null)
                {
                    currentTour.EstimatedTime = TimeSpan.Parse(line.Replace("Estimated Time (hrs):", "").Trim());
                }
            }

            if (currentTour != null)
            {
                tours.Add(currentTour);
            }

            return tours;
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
            NewTourPopularity = 0;
            NewTourChildFriendliness = 0.0;
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

        private void SearchTours(object parameter)
        {
            var searchText = SearchText?.ToLower() ?? string.Empty;
            var searchedTours = _tourService.GetAllTours().Where(t =>
                t.TourName.ToLower().Contains(searchText) ||
                t.Description.ToLower().Contains(searchText) ||
                t.From.ToLower().Contains(searchText) ||
                t.To.ToLower().Contains(searchText) ||
                t.TransportType.ToLower().Contains(searchText) ||
                t.Popularity.ToString().Contains(searchText) ||
                t.ChildFriendliness.ToString().Contains(searchText)).ToList();

            Tours.Clear();
            foreach (var tour in searchedTours)
            {
                Tours.Add(tour);
            }
        }

        private bool CanGenerateTourReport(object parameter)
        {
            return SelectedTour != null;
        }

        private void GenerateTourReport(object parameter)
        {
            var tourLogs = SelectedTour.TourLogs.ToList();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = Path.Combine(desktopPath, "TourReport.pdf");

            _tourReportService.GenerateTourReport(SelectedTour, tourLogs, outputPath);

            // Optionally, display a message to the user indicating success
            System.Windows.MessageBox.Show("Tour report generated successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private bool CanGenerateSummaryReport(object parameter)
        {
            return Tours.Any();
        }

        private void GenerateSummaryReport(object parameter)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputPath = Path.Combine(desktopPath, "SummaryReport.pdf");

            _tourReportService.GenerateSummaryReport(Tours.ToList(), outputPath);

            // Optionally, display a message to the user indicating success
            System.Windows.MessageBox.Show("Summary report generated successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}