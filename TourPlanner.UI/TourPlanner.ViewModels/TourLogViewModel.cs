using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using Tour_planner.TourPlanner.Commands;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class TourLogViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TourLog> TourLogs { get; set; }
        private TourLog _selectedTourLog;

        public TourLog SelectedTourLog
        {
            get { return _selectedTourLog; }
            set
            {
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
                (DeleteTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateTourLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddTourLogCommand { get; private set; }
        public ICommand UpdateTourLogCommand { get; private set; }
        public ICommand DeleteTourLogCommand { get; private set; }
        public ICommand RefreshTourLogsCommand { get; private set; }

        public TourLogViewModel()
        {
            TourLogs = new ObservableCollection<TourLog>();
            AddTourLogCommand = new RelayCommand(AddTourLog);
            UpdateTourLogCommand = new RelayCommand(UpdateTourLog, CanModifyDeleteTourLog);
            DeleteTourLogCommand = new RelayCommand(DeleteTourLog, CanModifyDeleteTourLog);
            RefreshTourLogsCommand = new RelayCommand(RefreshTourLogs);

            // Dummy data for testing
            LoadDummyData();
        }

        private void AddTourLog()
        {
            TourLog newLog = new TourLog(); 
            TourLogs.Add(newLog);
            SelectedTourLog = newLog;
        }

        private void UpdateTourLog()
        {
            OnPropertyChanged("TourLogs");
        }

        private void DeleteTourLog()
        {
            if (SelectedTourLog != null)
            {
                TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
            }
        }

        private bool CanModifyDeleteTourLog()
        {
            return SelectedTourLog != null;
        }

        private void RefreshTourLogs()
        {
            // Implement logic to reload tour logs from the data source
            LoadDummyData(); // Replace with actual data loading method
        }

        private void LoadDummyData()
        {
            // Placeholder for data loading
            TourLogs.Clear();
            TourLogs.Add(new TourLog { Comment = "Nice weather", Difficulty = 3 });
            TourLogs.Add(new TourLog { Comment = "Challenging path", Difficulty = 5 });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}