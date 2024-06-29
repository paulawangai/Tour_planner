using System.Collections.Generic;
using System.Linq;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourService));
        private readonly TourReportService _reportService;
        private readonly AppDbContext _context;

        public TourService(AppDbContext context, TourReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public IEnumerable<Tour> GetAllTours()
        {
            return _context.Tours.Include(t => t.TourLogs);
        }

        public Tour GetTourById(int tourId)
        {
            return _context.Tours.Include(t => t.TourLogs).SingleOrDefault(t => t.TourId == tourId);
        }

        public void AddTour(Tour tour)
        {
            if (string.IsNullOrWhiteSpace(tour.RouteImage))
            {
                tour.RouteImage = "default_route_image.png"; // Ensure default value
            }
            log.Info($"Adding Tour: {tour.TourName}");
            _context.Tours.Add(tour);
            _context.SaveChanges();
        }

        public void UpdateTour(Tour tour)
        {
            _context.Tours.Update(tour);
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

        public void ExportTours(List<Tour> tours, string filePath)
        {
            var json = JsonConvert.SerializeObject(tours, Formatting.Indented);
            File.WriteAllText(filePath, json);
            log.Info($"Tours exported to {filePath}");
        }

        public List<Tour> ImportTours(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var tours = JsonConvert.DeserializeObject<List<Tour>>(json);
            _context.Tours.AddRange(tours);
            _context.SaveChanges();
            log.Info($"Tours imported from {filePath}");
            return tours;
        }

        public void UpdateTourAttributes(int tourId)
        {
            var tour = GetTourById(tourId);
            if (tour != null)
            {
                tour.Popularity = tour.TourLogs.Count;

                if (tour.TourLogs.Count > 0)
                {
                    var averageDifficulty = tour.TourLogs.Average(log => log.Difficulty);
                    var averageTime = tour.TourLogs.Average(log => log.TotalTime.TotalMinutes);
                    var averageDistance = tour.TourLogs.Average(log => log.TotalDistance);
                    tour.ChildFriendliness = (averageDifficulty + averageTime + averageDistance) / 3.0;
                }
                else
                {
                    tour.ChildFriendliness = 0;
                }

                UpdateTour(tour);
            }
        }
    }
}
