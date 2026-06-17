using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Application.DTO;

public record TripMatchResult
{
    public int RouteId { get; init; } 
    public TripEntity Trip { get; init; }
    public StopEntity StartStop { get; init; }
    public StopEntity EndStop { get; init; }
    public int TransitTimeSeconds { get; init; }
    public int StartWalkTimeSeconds { get; init; }
    public int EndWalkTimeSeconds { get; init; }
    public int TotalWalkTimeSeconds => StartWalkTimeSeconds + EndWalkTimeSeconds;  
    public int TotalTimeSeconds { get; init; }   
    public int FinalArrivalTimeSeconds { get; init; }
    public string FormattedTotalTime => FormatSeconds(TotalTimeSeconds);
    public string FormattedTransitTime => FormatSeconds(TransitTimeSeconds);
    public string FormattedTotalWalkTime => FormatSeconds(TotalWalkTimeSeconds);
    public string FormattedStartWalkTime => FormatSeconds(StartWalkTimeSeconds);
    public string FormattedEndWalkTime => FormatSeconds(EndWalkTimeSeconds);

    private static string FormatSeconds(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);

        if (time.TotalHours >= 1)
        {
            return $"{(int)time.TotalHours} h {time.Minutes:D2}";
        }
        else if (time.TotalMinutes >= 1)
        {
            return $"{time.Minutes} min";
        }
        else
        {
            return "< 1 min";
        }
    }
}
