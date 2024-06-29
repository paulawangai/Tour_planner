using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tour_planner.TourPlanner.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Microsoft.Win32;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        private readonly AppDbContext _context;
        private Tour _selectedTour;
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService;
        private readonly OpenRouteService _routeService;

        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged();
                (UpdateTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SearchText { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }
        public ICommand ImportToursCommand { get; }
        public ICommand ExportToursCommand { get; }

        public TourViewModel TourViewModel { get; }
        public TourLogViewModel TourLogViewModel { get; }

        public MainViewModel(AppDbContext context, TourService tourService, TourLogService tourLogService, OpenRouteService routeService)
        {
            _context = context;
            _tourService = tourService;
            _tourLogService = tourLogService;
            _routeService = routeService;

            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            TourViewModel = new TourViewModel(_tourService, _routeService);
            TourLogViewModel = new TourLogViewModel(_tourLogService, _tourService);

            SearchCommand = new RelayCommand(param => SearchTours());
            AddTourCommand = new RelayCommand(param => AddTour());
            UpdateTourCommand = new RelayCommand(param => UpdateTour(), param => CanExecuteTourCommand());
            DeleteTourCommand = new RelayCommand(param => DeleteTour(), param => CanExecuteTourCommand());
            GenerateTourReportCommand = new RelayCommand(param => GenerateTourReport(), param => CanExecuteTourCommand());
            GenerateSummaryReportCommand = new RelayCommand(param => GenerateSummaryReport());
            ImportToursCommand = new RelayCommand(param => ImportTours());
            ExportToursCommand = new RelayCommand(param => ExportTours());
        }

        private void SearchTours()
        {
            log.Info($"Searching tours with text: {SearchText}");
            // Implement your search logic here
        }

        private void AddTour()
        {
            log.Info("Adding a new tour.");
            var newTour = new Tour
            {
                TourName = "New Tour",
                Description = "Description of the new tour"
            };
            _context.Tours.Add(newTour);
            _context.SaveChanges();
            _tourService.AddTour(newTour);
            Tours.Add(newTour);
            log.Info("New tour added.");
        }

        private void UpdateTour()
        {
            log.Info($"Updating tour: {SelectedTour.TourName}");
            _context.SaveChanges();
            log.Info("Tour updated.");
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                log.Info($"Deleting tour: {SelectedTour.TourName}");
                _context.Tours.Remove(SelectedTour);
                _context.SaveChanges();
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                log.Info("Tour deleted.");
            }
        }

        private void GenerateTourReport()
        {
            if (SelectedTour != null)
            {
                string outputPath = "tour_report.pdf";
                _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
            }
        }

        private void GenerateSummaryReport()
        {
            string outputPath = "summary_report.pdf";
            _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }

        private void ImportTours()
        {
            log.Info("Importing tours.");
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

        private void ExportTours()
        {
            log.Info("Exporting tours.");
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

        private bool CanExecuteTourCommand()
        {
            return SelectedTour != null;
        }
    }
}
