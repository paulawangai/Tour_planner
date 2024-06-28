using System.Collections.Generic;
using System.Linq;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using log4net;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services {
    public class TourLogService {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourLogService));
        private readonly AppDbContext _context;

        public TourLogService(AppDbContext context) {
            _context = context;
        }

        public IEnumerable<TourLog> GetAllTourLogs() {
            log.Debug("Retrieving all tour logs");
            return _context.TourLogs.ToList();
        }

        public void AddTourLog(TourLog tourLog) {
            log.Debug($"Adding tour log for tour ID: {tourLog.TourId}");
            _context.TourLogs.Add(tourLog);
            _context.SaveChanges();
            UpdateTourAttributes(tourLog.TourId);
        }

        public void Save() {
            log.Debug("Saving changes to the database");
            _context.SaveChanges();
        }

        public void DeleteTourLog(TourLog tourLog) {
            int tourId = tourLog.TourId;
            log.Debug($"Deleting tour log ID: {tourLog.TourLogId} for tour ID: {tourId}");
            _context.TourLogs.Remove(tourLog);
            _context.SaveChanges();
            UpdateTourAttributes(tourId);
        }

        public IEnumerable<TourLog> SearchTourLogs(string searchText) {
            log.Debug($"Searching tour logs with text: {searchText}");
            if (string.IsNullOrWhiteSpace(searchText)) {
                return GetAllTourLogs();
            }

            searchText = searchText.ToLower();
            return _context.TourLogs
                .Where(tl => tl.Comment.ToLower().Contains(searchText) ||
                             tl.StatusMessage.ToLower().Contains(searchText) ||
                             tl.Tour.TourName.ToLower().Contains(searchText))
                .ToList();
        }

        private void UpdateTourAttributes(int tourId) {
            var tour = _context.Tours.Find(tourId);
            if (tour == null) return;

            var tourLogs = _context.TourLogs.Where(log => log.TourId == tourId).ToList();
            tour.Popularity = tourLogs.Count;

            if (tourLogs.Any()) {
                double avgDifficulty = tourLogs.Average(log => log.Difficulty);
                double avgTotalTime = tourLogs.Average(log => log.TotalTime.TotalHours);
                double avgTotalDistance = tourLogs.Average(log => log.TotalDistance);

                tour.ChildFriendliness = CalculateChildFriendliness(avgDifficulty, avgTotalTime, avgTotalDistance);
            }

            _context.SaveChanges();
            log.Debug($"Updated tour attributes for tour ID: {tourId}");
        }

        private double CalculateChildFriendliness(double avgDifficulty, double avgTotalTime, double avgTotalDistance) {
            // Example formula: Lower difficulty, shorter time, and shorter distance increase child-friendliness
            return (10 - avgDifficulty) + (5 - (avgTotalTime / 2)) + (5 - (avgTotalDistance / 10));
        }
    }
}
