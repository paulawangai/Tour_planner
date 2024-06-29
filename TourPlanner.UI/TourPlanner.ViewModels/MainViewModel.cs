using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tour_planner.TourPlanner.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using System.Windows;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels {
    public class MainViewModel : ViewModelBase {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        public TourViewModel TourViewModel { get; }
        public TourLogViewModel TourLogViewModel { get; }

        private readonly AppDbContext _context;
        private Tour _selectedTour;
        private readonly TourService _tourService;

        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour {
            get => _selectedTour;
            set {
                _selectedTour = value;
                OnPropertyChanged();
                (UpdateTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTourCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }

        public MainViewModel(TourViewModel tourViewModel, TourLogViewModel tourLogViewModel, AppDbContext context, TourService tourService) {
            TourViewModel = tourViewModel;
            TourLogViewModel = tourLogViewModel;

            _context = context;
            _tourService = tourService;

            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());

            AddTourCommand = new RelayCommand(_ => AddTour());
            UpdateTourCommand = new RelayCommand(_ => UpdateTour(), _ => CanModifyTour());
            DeleteTourCommand = new RelayCommand(_ => DeleteTour(), _ => CanModifyTour());
            GenerateTourReportCommand = new RelayCommand(_ => GenerateTourReport(), _ => CanModifyTour());
            GenerateSummaryReportCommand = new RelayCommand(_ => GenerateSummaryReport());
        }

        private void AddTour() {
            log.Info("Adding a new tour.");
            var newTour = new Tour {
                TourName = "New Tour",
                Description = "Description of the new tour"
            };
            _tourService.AddTour(newTour);
            Tours.Add(newTour);
            log.Info("New tour added.");
        }

        private void UpdateTour() {
            log.Info($"Updating tour: {SelectedTour.TourName}");
            _tourService.UpdateTour(SelectedTour);
            log.Info("Tour updated.");
        }

        private void DeleteTour() {
            if (SelectedTour != null) {
                log.Info($"Deleting tour: {SelectedTour.TourName}");
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                log.Info("Tour deleted.");
            }
        }

        private void GenerateTourReport() {
            if (SelectedTour != null) {
                string outputPath = "tour_report.pdf";
                _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
                MessageBox.Show($"Tour report generated at {outputPath}", "Tour Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GenerateSummaryReport() {
            string outputPath = "summary_report.pdf";
            _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
            MessageBox.Show($"Summary report generated at {outputPath}", "Summary Report Generated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanModifyTour() {
            return SelectedTour != null;
        }
    }
}