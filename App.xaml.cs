using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.ViewModels;
using Tour_planner.TourPlanner.UI.TourPlanner.Views;
using System.Net.Http;

namespace Tour_planner
{
    public partial class App : Application
    {
        public IConfiguration Configuration { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            using (var serviceScope = ServiceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate();
            }

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var tourViewModel = ServiceProvider.GetRequiredService<TourViewModel>();
            var tourLogViewModel = ServiceProvider.GetRequiredService<TourLogViewModel>();

            mainWindow.ToursTab.DataContext = tourViewModel;
            mainWindow.TourLogsTab.DataContext = tourLogViewModel;

            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<MainWindow>();
            services.AddSingleton<TourViewModel>();
            services.AddSingleton<TourLogViewModel>();
            services.AddTransient<TourService>();
            services.AddTransient<TourLogService>();
            services.AddHttpClient<RouteService>();
            services.AddHttpClient<OpenRouteService>()
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri("https://api.openrouteservice.org/");
                    });
            services.AddSingleton<IConfiguration>(Configuration);
        }
    }
}
