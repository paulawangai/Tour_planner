// TourView.xaml.cs
using System.Windows.Controls;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;
using Mapsui.UI.Wpf;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Views
{
    public partial class TourView : UserControl
    {
        public TourView()
        {
            InitializeComponent();
            if (DataContext is TourViewModel viewModel)
            {
                viewModel.Initialize(mapControl);
            }
        }
    }
}
