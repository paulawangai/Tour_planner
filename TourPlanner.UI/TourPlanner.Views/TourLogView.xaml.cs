using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for TourLogView.xaml
    /// </summary>
    public partial class TourLogView : UserControl
    {
        public TourLogService TourLogService { get; }

        public TourLogView()
        {
            InitializeComponent();
        }
        public TourLogView(TourLogService tourLogService) : this()
        {
            TourLogService = tourLogService;
            DataContext = new TourLogViewModel(tourLogService);
        }
    }
}
