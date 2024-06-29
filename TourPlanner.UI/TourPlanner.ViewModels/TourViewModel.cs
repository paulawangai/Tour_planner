using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.Commands;
using log4net;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourViewModel));
        private readonly TourService _tourService;
        private readonly OpenRouteService _routeService;

        public event EventHandler<string> RouteDisplayRequested;

        public ObservableCollection<Tour> Tours { get; private set; }
        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (SetProperty(ref _selectedTour, value) && value != null)
                {
                    // Update fields with selected tour details
                    TourName = value.TourName;
                    Description = value.Description;
                    From = value.From;
                    To = value.To;
                    TransportType = value.TransportType;
                    Popularity = value.Popularity;
                    ChildFriendliness = value.ChildFriendliness;
                }

                (UpdateTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateRouteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Properties for tour input
        private string _tourName;
        public string TourName
        {
            get => _tourName;
            set => SetProperty(ref _tourName, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _from;
        public string From
        {
            get => _from;
            set => SetProperty(ref _from, value);
        }

        private string _to;
        public string To
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }

        private string _transportType;
        public string TransportType
        {
            get => _transportType;
            set => SetProperty(ref _transportType, value);
        }

        private int _popularity;
        public int Popularity
        {
            get => _popularity;
            set => SetProperty(ref _popularity, value);
        }

        private double _childFriendliness;
        public double ChildFriendliness
        {
            get => _childFriendliness;
            set => SetProperty(ref _childFriendliness, value);
        }

        // Commands
        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand ExportToursCommand { get; }
        public ICommand ImportToursCommand { get; }
        public ICommand UpdateRouteCommand { get; }

        public string SearchText { get; set; }

        public TourViewModel(TourService tourService, OpenRouteService routeService)
        {
            log.Debug("Initializing TourViewModel");

            _tourService = tourService;
            _routeService = routeService;
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(param => AddTour());
            UpdateTourCommand = new RelayCommand(param => UpdateTour(), param => CanModifyTour());
            DeleteTourCommand = new RelayCommand(param => DeleteTour(), param => CanModifyTour());
            SearchCommand = new RelayCommand(param => SearchTours());
            GenerateTourReportCommand = new RelayCommand(param => GenerateTourReport(), param => CanModifyTour());
            GenerateSummaryReportCommand = new RelayCommand(param => GenerateSummaryReport());
            ExportToursCommand = new RelayCommand(param => ExportTours());
            ImportToursCommand = new RelayCommand(param => ImportTours());
            UpdateRouteCommand = new RelayCommand(param => UpdateRoute(), param => CanModifyTour());
        }

        private async Task DisplayRouteOnMap(string from, string to)
        {
            try
            {
                log.Info($"Fetching coordinates for {from} and {to}");
                var fromCoordinates = await _routeService.GetCoordinatesAsync(from);
                var toCoordinates = await _routeService.GetCoordinatesAsync(to);

                var fromLocation = $"{fromCoordinates.Longitude},{fromCoordinates.Latitude}";
                var toLocation = $"{toCoordinates.Longitude},{toCoordinates.Latitude}";

                log.Info($"Coordinates - From: {fromLocation}, To: {toLocation}");

                var route = await _routeService.GetRouteAsync(fromLocation, toLocation);

                string geoJson = route.ToString();
                log.Info($"GeoJSON: {geoJson}");
                RouteDisplayRequested?.Invoke(this, geoJson);
            }
            catch (HttpRequestException ex)
            {
                log.Error($"HTTP request error displaying route on map: {ex.Message}");
            }
            catch (Exception ex)
            {
                log.Error($"General error displaying route on map: {ex.Message}");
            }
        }

        private void AddTour()
        {
            log.Info($"Adding new tour: {TourName}");

            var newTour = new Tour
            {
                TourName = TourName,
                Description = Description,
                From = From,
                To = To,
                TransportType = TransportType,
                Popularity = 0,
                ChildFriendliness = 0
            };

            _tourService.AddTour(newTour);
            Tours.Add(newTour);
            ClearTourFields();
            log.Info("New tour added.");
        }

        private void UpdateTour()
        {
            if (SelectedTour != null)
            {
                log.Info($"Updating tour: {SelectedTour.TourName}");
                SelectedTour.TourName = TourName;
                SelectedTour.Description = Description;
                SelectedTour.From = From;
                SelectedTour.To = To;
                SelectedTour.TransportType = TransportType;

                _tourService.UpdateTour(SelectedTour);
                _tourService.UpdateTourAttributes(SelectedTour.TourId); // Update computed attributes
                RefreshTours();
                log.Info("Tour updated.");
            }
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                log.Info($"Deleting tour: {SelectedTour.TourName}");
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                SelectedTour = null;
                log.Info("Tour deleted.");
            }
        }

        private bool CanModifyTour() => SelectedTour != null;

        private void SearchTours()
        {
            log.Info($"Searching tours with text: {SearchText}");
            var searchResults = _tourService.SearchTours(SearchText);
            Tours.Clear();
            foreach (var tour in searchResults)
            {
                Tours.Add(tour);
            }
            log.Info("Tours search completed.");
        }

        private void RefreshTours()
        {
            Tours.Clear();
            foreach (var tour in _tourService.GetAllTours())
            {
                Tours.Add(tour);
            }
        }

        private void ClearTourFields()
        {
            TourName = string.Empty;
            Description = string.Empty;
            From = string.Empty;
            To = string.Empty;
            TransportType = string.Empty;
            Popularity = 0;
            ChildFriendliness = 0;
            OnPropertyChanged(nameof(TourName));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(From));
            OnPropertyChanged(nameof(To));
            OnPropertyChanged(nameof(TransportType));
            OnPropertyChanged(nameof(Popularity));
            OnPropertyChanged(nameof(ChildFriendliness));
        }

        private async void GenerateTourReport()
        {
            if (SelectedTour != null)
            {
                string outputPath = $"TourReport_{SelectedTour.TourId}.pdf";
                await _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
            }
        }

        private async void GenerateSummaryReport()
        {
            string outputPath = "SummaryReport.pdf";
            await _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }

        private void ExportTours()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _tourService.ExportTours(Tours.ToList(), saveFileDialog.FileName);
                    log.Info($"Tours exported to {saveFileDialog.FileName}");
                }
                catch (Exception ex)
                {
                    log.Error($"Error exporting tours: {ex.Message}");
                }
            }
        }

        private void ImportTours()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var importedTours = _tourService.ImportTours(openFileDialog.FileName);
                    foreach (var tour in importedTours)
                    {
                        Tours.Add(tour);
                    }
                    log.Info($"Tours imported from {openFileDialog.FileName}");
                }
                catch (Exception ex)
                {
                    log.Error($"Error importing tours: {ex.Message}");
                }
            }
        }

        private async void UpdateRoute()
        {
            if (!string.IsNullOrWhiteSpace(From) && !string.IsNullOrWhiteSpace(To))
            {
                await DisplayRouteOnMap(From, To);
            }
        }
    }
}
