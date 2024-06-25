using System.Windows;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
