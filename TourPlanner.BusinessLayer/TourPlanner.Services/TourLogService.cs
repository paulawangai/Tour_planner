using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourLogService
    {
        private readonly AppDbContext _context;

        public TourLogService(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            _context = new AppDbContext(options);
        }

        public List<TourLog> GetAllTourLogs()
        {
            return _context.TourLogs.ToList();
        }

        public void AddTourLog(TourLog tourLog)
        {
            _context.TourLogs.Add(tourLog);
            _context.SaveChanges();
        }

        public void UpdateTourLog(TourLog tourLog)
        {
            _context.TourLogs.Update(tourLog);
            _context.SaveChanges();
        }

        public void DeleteTourLog(TourLog tourLog)
        {
            _context.TourLogs.Remove(tourLog);
            _context.SaveChanges();
        }
    }

}
