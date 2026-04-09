using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.Domain.Services;
using UrbanFlow_trips_planner.Infrastructure.Providers;

namespace UrbanFlow_trips_planner.API.GrpcServices;

public class PathfinderService : IPathfinderService
{
    List<RouteEntity> fakeRoutes = new List<RouteEntity>
    {
        new RouteEntity
        {
            RouteId = 1,
            RouteShortName = "Ligne Lente",
            Trips = new List<TripEntity>
            {
                new TripEntity
                {
                    TripId = 1,
                    Stops = new List<StopEntity>
                    {
                        new StopEntity { StopId = 1, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 60200, SequenceOrder = 1 },
                        new StopEntity { StopId = 2, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 61000, SequenceOrder = 2 }
                    }
                }
            }
        },
        new RouteEntity
        {
            RouteId = 2,
            RouteShortName = "Ligne Rapide",
            Trips = new List<TripEntity>
            {
                new TripEntity
                {
                    TripId = 2,
                    Stops = new List<StopEntity>
                    {
                        new StopEntity { StopId = 3, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 60300, SequenceOrder = 1 },
                        new StopEntity { StopId = 4, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 60800, SequenceOrder = 2 }
                    }
                }
            }
        }
    };
    private readonly IRouteProvider _routeProvider;
    public PathfinderService(IRouteProvider routeProvider)
    {
        //_routeProvider = routeProvider;
        _routeProvider = new FakeRouteProvider(fakeRoutes); ;
    }
    
    public async Task<List<TripMatchResult>> GetFastestRoutes(
    int agencyId,
    float startLong, float startLat, float endLong, float endLat, 
    int userDepartureTimeSeconds,
    IWalkingRoutingService routingService)
{
    List<RouteEntity> routes = await _routeProvider.GetRoutesAsync();
    
    var allUniqueStops = routes
        .SelectMany(r => r.Trips)
        .SelectMany(t => t.Stops)
        .DistinctBy(s => new { s.Latitude, s.Longitude })
        .ToList();

    var closestStartStopsCandidates = allUniqueStops
        .Select(s => new { Stop = s, Distance = DistanceService.GetDistanceInMeters(startLat, startLong, s.Latitude, s.Longitude) })
        .OrderBy(x => x.Distance)
        .Take(3)
        .Select(x => x.Stop)
        .ToList();

    var closestEndStopsCandidates = allUniqueStops
        .Select(s => new { Stop = s, Distance = DistanceService.GetDistanceInMeters(endLat, endLong, s.Latitude, s.Longitude) })
        .OrderBy(x => x.Distance)
        .Take(3)
        .Select(x => x.Stop)
        .ToList();

    var startWalkingTimes = new Dictionary<StopEntity, int>();
    foreach (var stop in closestStartStopsCandidates)
    {
        startWalkingTimes[stop] = await routingService.GetWalkingTimeSecondsAsync(startLat, startLong, stop.Latitude, stop.Longitude);
    }
    
    var endWalkingTimes = new Dictionary<StopEntity, int>();
    foreach (var stop in closestEndStopsCandidates)
    {
        endWalkingTimes[stop] = await routingService.GetWalkingTimeSecondsAsync(stop.Latitude, stop.Longitude, endLat, endLong);
    }
    
    var validRoutes = new List<TripMatchResult>();

    Console.WriteLine($"potential route {routes.Count}");
    
    foreach (var route in routes)
    {
        foreach (var trip in route.Trips)
        {
            var matchedStartStop = trip.Stops
                .Where(s => closestStartStopsCandidates.Any(css => css.Latitude == s.Latitude && css.Longitude == s.Longitude))
                .OrderBy(s => s.SequenceOrder)
                .FirstOrDefault();

            var matchedEndStop = trip.Stops
                .Where(s => closestEndStopsCandidates.Any(ces => ces.Latitude == s.Latitude && ces.Longitude == s.Longitude))
                .OrderByDescending(s => s.SequenceOrder)
                .FirstOrDefault();

            if (matchedStartStop != null && matchedEndStop != null &&
                matchedStartStop.SequenceOrder < matchedEndStop.SequenceOrder)
            {
                int walkTimeStartSeconds = startWalkingTimes.First(kvp => kvp.Key.Latitude == matchedStartStop.Latitude && kvp.Key.Longitude == matchedStartStop.Longitude).Value;
                
                int userArrivalAtStopSeconds = userDepartureTimeSeconds + walkTimeStartSeconds;

                if (matchedStartStop.ArrivalTime >= userArrivalAtStopSeconds)
                {
                    int walkTimeEndSeconds = endWalkingTimes.First(kvp => kvp.Key.Latitude == matchedEndStop.Latitude && kvp.Key.Longitude == matchedEndStop.Longitude).Value;
                    
                    int transitTimeSeconds = matchedEndStop.ArrivalTime - matchedStartStop.ArrivalTime;
                    int totalWalkTimeSeconds = walkTimeStartSeconds + walkTimeEndSeconds;
                    
                    int finalArrivalTimeSeconds = matchedEndStop.ArrivalTime + walkTimeEndSeconds;
                    int totalTimeSeconds = finalArrivalTimeSeconds - userDepartureTimeSeconds;

                    validRoutes.Add(new TripMatchResult
                    {
                        Route = route,
                        Trip = trip,
                        StartStop = matchedStartStop,
                        EndStop = matchedEndStop,
                        WalkTimeSeconds = totalWalkTimeSeconds,
                        TransitTimeSeconds = transitTimeSeconds,
                        TotalTimeSeconds = totalTimeSeconds,
                        FinalArrivalTimeSeconds = finalArrivalTimeSeconds
                    });
                }
            }
        }
    }
    validRoutes.ForEach(r => Console.WriteLine($"route {r}"));

    return validRoutes
        .OrderBy(r => r.FinalArrivalTimeSeconds)
        .ToList();
}
}