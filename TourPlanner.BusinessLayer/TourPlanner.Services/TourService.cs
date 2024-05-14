using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tour_planner.TourPlanner.DataLayer;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace Tour_planner.TourPlanner.BusinessLayer.TourPlanner.Services
{
    public class TourService
    {
        private readonly AppDbContext _context;

        public TourService(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            _context = new AppDbContext(options);
        }

        public List<Tour> GetAllTours()
        {
            return _context.Tours.ToList();
        }

        public void AddTour(Tour tour)
        {
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
    }

}
