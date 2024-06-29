using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Subscribe to the RouteDisplayRequested event from the TourViewModel
            _viewModel.TourViewModel.RouteDisplayRequested += OnRouteDisplayRequested;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeWebView();
        }

        private async Task InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            webView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private async void OnRouteDisplayRequested(object sender, string geoJson)
        {
            await AddRouteToMap(geoJson);
        }

        public async Task AddRouteToMap(string geoJson)
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                string script = $"addRoute({geoJson})";
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }
    }
}
