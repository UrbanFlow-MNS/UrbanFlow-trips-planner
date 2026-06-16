using UrbanFlow_trips_planner.Application.DTO;

namespace UrbanFlow_trips_planner.Domain.Interfaces;

public interface IPathfinderService
{
    public Task<List<TripMatchResult>?> GetFastestRoutes(
        int   agencyId,
        float startLong, 
        float startLat, 
        float endLong, 
        float endLat, 
        int userDepartureTimeSeconds,
        IWalkingRoutingService routingService);
}