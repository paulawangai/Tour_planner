using System.Collections.Generic;
using System.Linq;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourService));

        private readonly AppDbContext _context;

        private readonly TourReportService _reportService;


        public TourService(AppDbContext context, TourReportService reportService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }

        public IEnumerable<Tour> GetAllTours()
        {
            return _context.Tours.ToList();
        }

        public Tour GetTourById(int tourId)
        {
            return _context.Tours.SingleOrDefault(t => t.TourId == tourId);
        }

        public void AddTour(Tour tour)
        {
            _context.Tours.Add(tour);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void DeleteTour(Tour tour)
        {
            _context.Tours.Remove(tour);
            _context.SaveChanges();
        }

        public IEnumerable<Tour> SearchTours(string searchText)
        {
            return _context.Tours
                .Where(t => t.TourName.Contains(searchText) ||
                            t.Description.Contains(searchText) ||
                            t.Popularity.ToString().Contains(searchText) ||
                            t.ChildFriendliness.ToString().Contains(searchText))
                .ToList();
        }

        

        public async Task GenerateTourReport(int tourId, string outputPath)
        {
            log.Debug($"Generating report for tour ID: {tourId}");
            var tour = GetTourById(tourId);
            var tourLogs = _context.TourLogs.Where(log => log.TourId == tourId).ToList();
            _reportService.GenerateTourReport(tour, tourLogs, outputPath);
            log.Info($"Report generated for tour ID: {tourId} at {outputPath}");
        }

        public async Task GenerateSummaryReport(string outputPath)
        {
            log.Debug("Generating summary report.");
            var tours = GetAllTours().ToList();
            _reportService.GenerateSummaryReport(tours, outputPath);
            log.Info($"Summary report generated at {outputPath}");
        }
    }
}
