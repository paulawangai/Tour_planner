using System.Collections.Generic;
using System.Linq;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourLogService
    {
        private readonly AppDbContext _context;

        public TourLogService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<TourLog> GetAllTourLogs()
        {
            return _context.TourLogs.ToList();
        }

        public void AddTourLog(TourLog tourLog)
        {
            _context.TourLogs.Add(tourLog);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void DeleteTourLog(TourLog tourLog)
        {
            _context.TourLogs.Remove(tourLog);
            _context.SaveChanges();
        }
    }
}
