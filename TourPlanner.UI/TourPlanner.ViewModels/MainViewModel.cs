using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tour_planner.TourPlanner.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        private readonly AppDbContext _context;
        private Tour _selectedTour;
        private readonly TourService _tourService;


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

        public ICommand AddTourCommand { get; }
        public ICommand UpdateTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand GenerateTourReportCommand { get; }
        public ICommand GenerateSummaryReportCommand { get; }

        public MainViewModel(AppDbContext context, TourService tourService)
        {
            _context = context;
            _tourService = tourService;

            Tours = new ObservableCollection<Tour>(_tourService.GetAllTours());



            Tours = new ObservableCollection<Tour>(_context.Tours.ToList());

            AddTourCommand = new RelayCommand(param => AddTour());
            UpdateTourCommand = new RelayCommand(param => UpdateTour(), param => CanExecuteTourCommand());
            DeleteTourCommand = new RelayCommand(param => DeleteTour(), param => CanExecuteTourCommand());
            GenerateTourReportCommand = new RelayCommand(param => GenerateTourReport(), param => CanExecuteTourCommand());
            GenerateSummaryReportCommand = new RelayCommand(param => GenerateSummaryReport());
        }

        private void AddTour()
        {
            var newTour = new Tour
            {
                TourName = "New Tour",
                Description = "Description of the new tour"
            };
            _context.Tours.Add(newTour);
            _context.SaveChanges();
            _tourService.AddTour(newTour);
            Tours.Add(newTour);
        }

        private void UpdateTour()
        {
            _tourService.Save();
            _context.SaveChanges();
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                _context.Tours.Remove(SelectedTour);
                _context.SaveChanges();
                _tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
            }
        }

        private void GenerateTourReport()
        {
            if (SelectedTour != null)
            {
                string outputPath = "tour_report.pdf"; // Specify the path where you want to save the report
                _tourService.GenerateTourReport(SelectedTour.TourId, outputPath);
                log.Info($"Tour report generated for tour ID: {SelectedTour.TourId} at {outputPath}");
            }
        }

        private void GenerateSummaryReport()
        {
            string outputPath = "summary_report.pdf"; // Specify the path where you want to save the summary report
            _tourService.GenerateSummaryReport(outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }

        private bool CanExecuteTourCommand()
        {
            return SelectedTour != null;
        }
    }
}
