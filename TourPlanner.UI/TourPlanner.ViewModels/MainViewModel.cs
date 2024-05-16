using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TourViewModel _tourViewModel;

        public MainViewModel()
        {
        }

        public MainViewModel(TourService tourService, TourLogService tourLogService)
        {
            _tourViewModel = new TourViewModel(tourService, tourLogService);
            // Initialize other view models or services 
        }

        public TourViewModel TourViewModel
        {
            get => _tourViewModel;
            set
            {
                if (_tourViewModel != value)
                {
                    _tourViewModel = value;
                    OnPropertyChanged(nameof(TourViewModel));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
