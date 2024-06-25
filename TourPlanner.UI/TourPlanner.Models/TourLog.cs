using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tour_planner.TourPlanner.UI.TourPlanner.Models;

public class TourLog : INotifyPropertyChanged
{
    private int tourLogId;
    private DateTime dateTime;
    private string comment;
    private string statusMessage;
    private int difficulty;
    private double totalDistance;
    private TimeSpan totalTime;
    private int rating;
    private int tourId;
    private Tour tour;

    public event PropertyChangedEventHandler PropertyChanged;

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
                dateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                OnPropertyChanged(nameof(DateTime));
            }
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

    public string StatusMessage  
    {
        get { return statusMessage; }
        set
        {
            if (statusMessage != value)
            {
                statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
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
                rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }
    }

    [ForeignKey("Tour")]
    [Required]
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

    public Tour Tour
    {
        get => tour;
        set
        {
            if (tour != value)
            {
                tour = value;
                OnPropertyChanged(nameof(Tour));
            }
        }
    }
}
