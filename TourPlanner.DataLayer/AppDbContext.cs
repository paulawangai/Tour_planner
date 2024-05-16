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
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../"))
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Connection string: {connectionString}"); 
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("The connection string is null or empty.");
                }

                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
