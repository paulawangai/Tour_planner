using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.DataLayer {
    public class AppDbContext : DbContext 
    {

        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options) 
        {
            _configuration = configuration;
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Tour>()
                .Property(t => t.Popularity)
                .HasDefaultValue(0);

            modelBuilder.Entity<Tour>()
                .Property(t => t.ChildFriendliness)
                .HasDefaultValue(0.0);
        }
    }
}