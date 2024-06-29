using System.Collections.ObjectModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Tour_planner.TourPlanner.Commands;
using System.Linq;
using log4net;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using System.Windows;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourViewModel));
        private readonly TourService _tourService;
        private readonly OpenRouteService _routeService;
        private readonly TourReportService _reportService;

        public event EventHandler<string> RouteDisplayRequested;
        public ObservableCollection<Tour> Tours { get; private set; }
        private Tour _selectedTour;
        private string _searchText;

        // Properties for new tour input
        public string NewTourName { get; set; }
        public string NewTourDescription { get; set; }
        public string NewTourFrom { get; set; }
        public string NewTourTo { get; set; }
        public string NewTourTransportType { get; set; }
        public int Popularity { get; set; }
        public double ChildFriendliness { get; set; }
        public TimeSpan NewTourEstimatedTime { get; set; }
        public double NewTourDistance { get; set; }

        // Commands
        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand ExportToursCommand { get; }
        public ICommand ImportToursCommand { get; }

        public string SearchText { get; set; }

        public Tour SelectedTour {
            get => _selectedTour;
            set {
                _selectedTour = value;
                OnPropertyChanged();
                (UpdateTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public TourViewModel(TourService tourService, OpenRouteService routeService, TourReportService reportService)
        {
            log.Debug("Initializing TourViewModel");

            _tourService = tourService;
            _routeService = routeService;
            _reportService = reportService;
            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(_ => AddTour());
            UpdateTourCommand = new RelayCommand(_ => UpdateTour(),_ => CanModifyTour());
            DeleteTourCommand = new RelayCommand(_ => DeleteTour(),_ => CanModifyTour());
            SearchCommand = new RelayCommand(_ => SearchTours());
            GenerateTourReportCommand = new RelayCommand(_ => GenerateTourReport(), _ => CanModifyTour());
            GenerateSummaryReportCommand = new RelayCommand(_ => GenerateSummaryReport());
            ExportToursCommand = new RelayCommand(_ => ExportTours());
            ImportToursCommand = new RelayCommand(_ => ImportTours());
        }

        private void DisplayRouteOnMap(string from, string to)
        {
            try
            {
                var fromCoordinates = _routeService.GetCoordinatesAsync(from).Result;
                var toCoordinates = _routeService.GetCoordinatesAsync(to).Result;

                var route = _routeService.GetRouteAsync(
                    $"{fromCoordinates.Longitude},{fromCoordinates.Latitude}",
                    $"{toCoordinates.Longitude},{toCoordinates.Latitude}"
                ).Result;

                string geoJson = route.ToString();
                RouteDisplayRequested?.Invoke(this, geoJson);
            }
            catch (Exception ex)
            {
                log.Error($"Error displaying route on map: {ex.Message}");
            }
        }

        private void AddTour() {
            log.Info($"Adding new tour: {NewTourName}");

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
                    Popularity = 0,
                    ChildFriendliness = 0
                };

                _tourService.AddTour(newTour);
                Tours.Add(newTour);
                ClearFormFields();
                log.Info("New tour added.");
            }
            else {
                log.Warn("Tour validation failed. Ensure all required fields are filled");
            }
        }

        private void UpdateTour() 
        {
            if (SelectedTour != null && ValidateTourDetails()) 
            {
                log.Info($"Updating tour: {SelectedTour.TourName}");
                SelectedTour.TourName = NewTourName;
                SelectedTour.Description = NewTourDescription;
                SelectedTour.From = NewTourFrom;
                SelectedTour.To = NewTourTo;
                SelectedTour.TransportType = NewTourTransportType;
                SelectedTour.TourDistance = NewTourDistance;
                SelectedTour.EstimatedTime = NewTourEstimatedTime;

                _tourService.UpdateTour(SelectedTour);
                _tourService.UpdateTourAttributes(SelectedTour.TourId);
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
                ClearFormFields(); 
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

        private bool ValidateTourDetails()
        {
            return !string.IsNullOrWhiteSpace(NewTourName) &&
                   !string.IsNullOrWhiteSpace(NewTourDescription) &&
                   !string.IsNullOrWhiteSpace(NewTourFrom) &&
                   !string.IsNullOrWhiteSpace(NewTourTo) &&
                   !string.IsNullOrWhiteSpace(NewTourTransportType) &&
                   NewTourDistance > 0 &&
                   NewTourEstimatedTime > TimeSpan.Zero;
        }   

        private void ClearFormFields() {
            NewTourName = string.Empty;
            NewTourDescription = string.Empty;
            NewTourFrom = string.Empty;
            NewTourTo = string.Empty;
            NewTourTransportType = string.Empty;
            NewTourDistance = 0;
            NewTourEstimatedTime = TimeSpan.Zero;
            Popularity = 0;
            //ChildFriendliness = 0;
            OnPropertyChanged(nameof(NewTourName));
            OnPropertyChanged(nameof(NewTourDescription));
            OnPropertyChanged(nameof(NewTourFrom));
            OnPropertyChanged(nameof(NewTourTo));
            OnPropertyChanged(nameof(NewTourTransportType));
            OnPropertyChanged(nameof(NewTourDistance));
            OnPropertyChanged(nameof(NewTourEstimatedTime));
            OnPropertyChanged(nameof(Popularity));
            //OnPropertyChanged(nameof(ChildFriendliness));
        }

        private void GenerateTourReport()
        {
            if (SelectedTour != null)
            {
                string outputPath = $"TourReport_{SelectedTour.TourId}.pdf";
                _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
                MessageBox.Show($"Tour report generated at {outputPath}", "Tour Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GenerateSummaryReport()
        {
            string outputPath = "SummaryReport.pdf";
            _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
            MessageBox.Show($"Summary report generated at {outputPath}", "Summary Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
