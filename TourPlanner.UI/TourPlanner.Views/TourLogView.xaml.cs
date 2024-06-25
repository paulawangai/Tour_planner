using System.Windows.Controls;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Views
{
    public partial class TourLogView : UserControl
    {
        public TourLogView()
        {
            InitializeComponent();
        }

        public TourLogView(TourLogViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
