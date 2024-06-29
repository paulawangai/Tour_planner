/*using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace Tour_planner.TourPlanner.UI.TourPlanner.ViewModels
{
    public class ViewModelLocator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewModelLocator));

        public ViewModelLocator()
        {
            log.Debug("Initializing ViewModelLocator.");
        }

        public MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();
        public TourViewModel TourViewModel => App.ServiceProvider.GetRequiredService<TourViewModel>();
        public TourLogViewModel TourLogViewModel => App.ServiceProvider.GetRequiredService<TourLogViewModel>();
    }
}
*/