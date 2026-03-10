namespace UrbanFlow_trips_planner.Domain.Services;

public interface IWalkingRoutingService
{
    Task<int> GetWalkingTimeSecondsAsync(double startLat, double startLon, double endLat, double endLon);
}