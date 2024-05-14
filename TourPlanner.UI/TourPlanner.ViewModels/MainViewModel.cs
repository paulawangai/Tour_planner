using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TourViewModel _tourViewModel;

        public MainViewModel()
        {
            _tourViewModel = new TourViewModel();
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
