using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services {
    public class TourService {
        private readonly AppDbContext _context;
        private readonly TourReportService _reportService;

        public TourService(AppDbContext context, TourReportService reportService) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }

        public IEnumerable<Tour> GetAllTours() {
            return _context.Tours.Include(t => t.TourLogs).ToList();
        }

        public Tour GetTourById(int tourId) {
            return _context.Tours.Include(t => t.TourLogs).SingleOrDefault(t => t.TourId == tourId);
        }

        public void AddTour(Tour tour) {
            _context.Tours.Add(tour);
            _context.SaveChanges();
        }

        public void Save() {
            _context.SaveChanges();
        }

        public void DeleteTour(Tour tour) {
            _context.Tours.Remove(tour);
            _context.SaveChanges();
        }

        public async Task GenerateTourReport(int tourId, string outputPath) {
            var tour = GetTourById(tourId);
            var tourLogs = _context.TourLogs.Where(log => log.TourId == tourId).ToList();
            _reportService.GenerateTourReport(tour, tourLogs, outputPath);
        }

        public async Task GenerateSummaryReport(string outputPath) {
            var tours = GetAllTours().ToList();
            _reportService.GenerateSummaryReport(tours, outputPath);
        }
    }
}