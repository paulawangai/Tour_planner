using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Configure and build the application settings
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Set up the main application window
            MainWindow mainWindow = new MainWindow
            {
                DataContext = new TourViewModel() // Assuming TourViewModel uses some configuration settings
            };

            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
