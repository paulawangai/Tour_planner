using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using log4net;
using Microsoft.EntityFrameworkCore;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;
using Tour_planner.TourPlanner.UI.TourPlanner.Views;

namespace Tour_planner {
    public partial class App : Application {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;

        public static IServiceProvider ServiceProvider => _serviceProvider;
        public static IConfiguration Configuration => _configuration;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            log.Info("Application started");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            //var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            //var mainWindow = new MainWindow();
            //mainWindow.DataContext = mainViewModel;
            var tourViewModel = _serviceProvider.GetRequiredService<TourViewModel>();
            var tourLogViewModel = _serviceProvider.GetRequiredService<TourLogViewModel>();
            var mainWindow = new MainWindow(tourViewModel, tourLogViewModel);
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services) {
            // Register configuration
            services.AddSingleton<IConfiguration>(_configuration);

            // Register DbContext
            services.AddDbContext<AppDbContext>((serviceProvider, options) => {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Singleton);

            // Register HttpClient
            services.AddHttpClient();

            // Register services
            services.AddSingleton<OpenRouteService>();
            services.AddSingleton<TourService>();
            services.AddSingleton<TourLogService>();
            services.AddSingleton<TourReportService>();

            // Register ViewModels
            services.AddSingleton<TourViewModel>();
            services.AddSingleton<TourLogViewModel>();
            services.AddSingleton<MainViewModel>();
        }
    }
}