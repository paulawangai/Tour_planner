using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Tour_planner.TourPlanner.UI.TourPlanner.Models
{
    public class TourLog : INotifyPropertyChanged
    {
        private int tourLogId;
        private DateTime dateTime;
        private string comment;
        private int difficulty;
        private double totalDistance;
        private TimeSpan totalTime;
        private int rating;
        private string _statusMessage;
        private int tourId; //Foreign key to the tour
        private Tour tour; //Navigation property for the related tour

        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Key]
        public int TourLogId
        {
            get { return tourLogId; }
            set
            {
                if (tourLogId != value)
                {
                    tourLogId = value;
                    OnPropertyChanged(nameof(TourLogId));
                }
            }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;
                    OnPropertyChanged(nameof(DateTime));
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public string Comment
        {
            get { return comment; }
            set
            {
                if (comment != value)
                {
                    comment = value;
                    OnPropertyChanged(nameof(Comment));
                }
            }
        }

        public int Difficulty
        {
            get { return difficulty; }
            set
            {
                if (difficulty != value)
                {
                    difficulty = value;
                    OnPropertyChanged(nameof(Difficulty));
                }
            }
        }

        public double TotalDistance
        {
            get { return totalDistance; }
            set
            {
                if (totalDistance != value)
                {
                    totalDistance = value;
                    OnPropertyChanged(nameof(TotalDistance));
                }
            }
        }

        public TimeSpan TotalTime
        {
            get { return totalTime; }
            set
            {
                if (totalTime != value)
                {
                    totalTime = value;
                    OnPropertyChanged(nameof(TotalTime));
                }
            }
        }

        public int Rating
        {
            get { return rating; }
            set
            {
                if (rating != value)
                {
                    rating = value; OnPropertyChanged(nameof(Rating));
                }
            }
        }

        [ForeignKey("Tour")]
        public int TourId 
        {
            get => tourId;
            set 
            {
                if (tourId != value)
                {
                    tourId = value; OnPropertyChanged(nameof(TourId));
                }
            }
        }

        public Tour Tour
        {
            get => tour;
            set 
            {
                if (tour!= value)
                {
                    tour = value; OnPropertyChanged(nameof(Tour));
                }

            }
        }
    }

}
