using System.Collections.Generic;
using System.Linq;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourService
    {
        private readonly AppDbContext _context;

        public TourService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
    }
}
