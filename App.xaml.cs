using System.Configuration;
using System.Data;
using System.Windows;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow
            {
                DataContext = new TourViewModel()
            };
            mainWindow.Show();
        }
    }

}
