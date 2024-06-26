using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services {
    public class TourService {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourService));
        private readonly AppDbContext _context;
        private readonly TourReportService _reportService;

        public TourService(AppDbContext context, TourReportService reportService) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            log.Debug("TourService initialized.");
        }

        public IEnumerable<Tour> GetAllTours() {
            log.Debug("Retrieving all tours.");
            return _context.Tours.Include(t => t.TourLogs).ToList();
        }

        public Tour GetTourById(int tourId) {
            log.Debug($"Retrieving tour with ID: {tourId}");
            return _context.Tours.Include(t => t.TourLogs).SingleOrDefault(t => t.TourId == tourId);
        }

        public void AddTour(Tour tour) {
            log.Debug("Adding a new tour.");
            _context.Tours.Add(tour);
            _context.SaveChanges();
            log.Info($"Tour added with ID: {tour.TourId}");
        }

        public void Save() {
            log.Debug("Saving changes.");
            _context.SaveChanges();
        }

        public void DeleteTour(Tour tour) {
            log.Debug($"Deleting tour with ID: {tour.TourId}");
            _context.Tours.Remove(tour);
            _context.SaveChanges();
            log.Info($"Tour deleted with ID: {tour.TourId}");
        }

        public async Task GenerateTourReport(int tourId, string outputPath) {
            log.Debug($"Generating report for tour ID: {tourId}");
            var tour = GetTourById(tourId);
            var tourLogs = _context.TourLogs.Where(log => log.TourId == tourId).ToList();
            _reportService.GenerateTourReport(tour, tourLogs, outputPath);
            log.Info($"Report generated for tour ID: {tourId} at {outputPath}");
        }

        public async Task GenerateSummaryReport(string outputPath) {
            log.Debug("Generating summary report.");
            var tours = GetAllTours().ToList();
            _reportService.GenerateSummaryReport(tours, outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }
    }
}
