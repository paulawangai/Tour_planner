using System;
using Microsoft.EntityFrameworkCore;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Tour_planner.TourPlanner.DataLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Ensure the DbContext only configures itself if it has not been configured externally.
            if (!optionsBuilder.IsConfigured)
            {
                // Set the base path for the configuration builder to the current directory of the application.
                // This helps in situations where the app might be executed from paths that are not the project root.
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                // Retrieve the connection string from appsettings.json by the key DefaultConnection
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // Use Npgsql with the connection string retrieved from the configuration.
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
