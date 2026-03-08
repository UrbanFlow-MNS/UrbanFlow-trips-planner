using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Domain.Services;

public class PathfinderService : IPathfinderService
{
    public Task<List<TripEntity>> GetTrips()
    {
        throw new NotImplementedException();
    }

    public Task<List<TripEntity>> GetTripsBasedOnDistanceFromStartPosAndEndPos(float startLong, float startLat, float endLong, float endLat)
    {
        throw new NotImplementedException();
    }

    public Task<List<TripEntity>> GetFastestRoutes(float startLong, float startLat, float endLong, float endLat)
    {
        throw new NotImplementedException();
    }
}