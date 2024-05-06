using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using Tour_planner.TourPlanner.Commands;


namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourViewModel : INotifyPropertyChanged
        {
            private string _tourName;

            public string TourName
            {
                get => _tourName;
                set
                {
                    if (_tourName != value)
                    {
                        _tourName = value;
                        OnPropertyChanged(nameof(TourName));
                    }
                }
            }

            public ICommand SaveTourCommand { get; }

            public TourViewModel()
            {
                SaveTourCommand = new RelayCommand(SaveTour);
            }

            private void SaveTour()
            {
                // Logic to save the tour could go here
                // For demo purposes, we'll just simulate saving the data
                System.Diagnostics.Debug.WriteLine($"Saving Tour: {TourName}");
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

}
