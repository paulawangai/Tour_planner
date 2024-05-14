using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;  // Adjust namespace as needed
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Load configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Extract the connection string from the configuration
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Create instances of the services with the connection string
            var tourService = new TourService(connectionString);
            var tourLogService = new TourLogService(connectionString);

            // Set up the main application window and inject the services into the ViewModel
            MainWindow mainWindow = new MainWindow
            {
                DataContext = new TourViewModel(tourService, tourLogService)  // Adjust constructor in ViewModel to accept both services
            };

            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
