using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Domain.Services;

public interface IPathfinderService
{
    public Task<List<TripEntity>> GetTrips();

    public Task<List<TripEntity>> GetTripsBasedOnDistanceFromStartPosAndEndPos(float startLong, float startLat,
        float endLong, float endLat);

    public Task<List<TripMatchResult>> GetFastestRoutes(float startLong, float startLat, float endLong, float endLat,
        IWalkingRoutingService routingService);
}