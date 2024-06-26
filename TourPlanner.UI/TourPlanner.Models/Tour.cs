﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tour_planner.TourPlanner.UI.TourPlanner.Models
{
    public class Tour : INotifyPropertyChanged
    {
        private int tourId;
        private string tourName;
        private string description;
        private string from;
        private string to;
        private string transportType;
        private double tourDistance;
        private TimeSpan estimatedTime;
        private string routeImage;
        private int popularity;
        private double childFriendliness;
        private ICollection<TourLog> tourLogs;

        [Key]
        public int TourId
        {
            get => tourId;
            set
            {
                if (tourId != value)
                {
                    tourId = value;
                    OnPropertyChanged(nameof(TourId));
                }
            }
        }
        public string TourName
        {
            get => tourName;
            set { tourName = value; OnPropertyChanged(nameof(TourName)); }
        }

        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string From
        {
            get => from;
            set { from = value; OnPropertyChanged(nameof(From)); }
        }

        public string To
        {
            get => to;
            set { to = value; OnPropertyChanged(nameof(To)); }
        }

        public string TransportType
        {
            get => transportType;
            set { transportType = value; OnPropertyChanged(nameof(TransportType)); }
        }

        public double TourDistance
        {
            get => tourDistance;
            set { tourDistance = value; OnPropertyChanged(nameof(TourDistance)); }
        }

        public TimeSpan EstimatedTime
        {
            get => estimatedTime;
            set { estimatedTime = value; OnPropertyChanged(nameof(EstimatedTime)); }
        }

        public string RouteImage
        {
            get => routeImage;
            set { routeImage = value; OnPropertyChanged(nameof(RouteImage)); }
        }

        public int Popularity
        {
            get => popularity;
            set { popularity = value; OnPropertyChanged(nameof(Popularity)); }
        }

        public double ChildFriendliness
        {
            get => childFriendliness;
            set { childFriendliness = value; OnPropertyChanged(nameof(ChildFriendliness)); }
        }

        public ICollection<TourLog> TourLogs
        {
            get => tourLogs;
            set
            {
                tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
