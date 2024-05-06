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
        private Tour _tour;
        private string _newDestinationName;

        public string? TourName
        {
            get => _tour.TourName;
            set
            {
                if (_tour.TourName != value)
                {
                    _tour.TourName = value;
                    OnPropertyChanged(nameof(TourName));
                }
            }
        }

        public string? Description
        {
            get => _tour.Description;
            set
            {
                if (_tour.Description != value)
                {
                    _tour.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public ObservableCollection<string> Destinations { get; } = new ObservableCollection<string>();

        public string NewDestinationName
        {
            get => _newDestinationName;
            set
            {
                if (_newDestinationName != value)
                {
                    _newDestinationName = value;
                    OnPropertyChanged(nameof(NewDestinationName));
                }
            }
        }

        public ICommand AddDestinationCommand { get; }
        public ICommand SaveTourCommand { get; }

        public TourViewModel()
        {
            _tour = new Tour();
            AddDestinationCommand = new RelayCommand(AddDestination, CanAddDestination);
            SaveTourCommand = new RelayCommand(SaveTour);
        }

        private bool CanAddDestination() => !string.IsNullOrEmpty(NewDestinationName);

        private void AddDestination()
        {
            Destinations.Add(NewDestinationName);
            NewDestinationName = string.Empty;
        }

        private void SaveTour()
        {
            // Save logic
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
