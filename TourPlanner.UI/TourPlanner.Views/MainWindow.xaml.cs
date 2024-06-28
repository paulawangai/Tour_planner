using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Views {
    public partial class MainWindow : Window {
        private readonly TourViewModel _viewModel;

        public MainWindow(TourViewModel tourViewModel) {
            InitializeComponent();
            _viewModel = tourViewModel;
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            string currentDirectory = Directory.GetCurrentDirectory();
            string htmlPath = Path.Combine(currentDirectory, "map.html");
            webView.CoreWebView2.Navigate(new Uri(htmlPath).ToString());
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            await InitializeWebView();
        }

        private async Task InitializeWebView() {
            await webView.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html");
            webView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        public async Task AddRouteToMap(string geoJson) {
            if (webView != null && webView.CoreWebView2 != null) {
                string script = $"addRoute({geoJson})";
                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }
    }
}