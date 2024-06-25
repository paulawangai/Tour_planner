using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Tour_planner.TourPlanner.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AppDbContext _context;
        private Tour _selectedTour;

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

        public MainViewModel(AppDbContext context)
        {
            _context = context;

            Tours = new ObservableCollection<Tour>(_context.Tours.ToList());

            AddTourCommand = new RelayCommand(param => AddTour());
            UpdateTourCommand = new RelayCommand(param => UpdateTour(), param => CanExecuteTourCommand());
            DeleteTourCommand = new RelayCommand(param => DeleteTour(), param => CanExecuteTourCommand());
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
            Tours.Add(newTour);
        }

        private void UpdateTour()
        {
            _context.SaveChanges();
        }

        private void DeleteTour()
        {
            if (SelectedTour != null)
            {
                _context.Tours.Remove(SelectedTour);
                _context.SaveChanges();
                Tours.Remove(SelectedTour);
            }
        }

        private bool CanExecuteTourCommand()
        {
            return SelectedTour != null;
        }
    }
}
