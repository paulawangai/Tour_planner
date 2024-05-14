using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the configuration from the appsettings.json file
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Extract the connection string from the configuration
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Assume that the TourService and TourLogService require a connection string to initialize
            var tourService = new TourService(connectionString);
            var tourLogService = new TourLogService(connectionString);

            // Inject the services into the view models
            var tourViewModel = new TourViewModel(tourService, tourLogService);
            var tourLogViewModel = new TourLogViewModel(tourLogService);  // Assuming you might need to use it somewhere

            // Set up the main application window and assign the data context
            MainWindow mainWindow = new MainWindow
            {
                DataContext = tourViewModel  // Set DataContext to the TourViewModel
            };

            mainWindow.Show();
        }
    }
}
