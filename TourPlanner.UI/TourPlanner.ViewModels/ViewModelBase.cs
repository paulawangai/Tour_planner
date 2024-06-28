using log4net;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels {
    public class ViewModelBase : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewModelBase));

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            log.Debug($"Property changed: {propertyName}");
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
