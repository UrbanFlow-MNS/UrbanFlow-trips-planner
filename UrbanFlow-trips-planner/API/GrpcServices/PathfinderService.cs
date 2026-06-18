using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.Domain.Services;
using UrbanFlow_trips_planner.Infrastructure.Providers;

namespace UrbanFlow_trips_planner.API.GrpcServices;

public class PathfinderService : IPathfinderService
{
    private readonly IRouteProvider _routeProvider;
    public PathfinderService(IRouteProvider routeProvider)
    {
        _routeProvider = routeProvider;
    }
    
    public async Task<List<TripMatchResult>> GetFastestRoutes(
    float startLong, float startLat, float endLong, float endLat, 
    int userDepartureTimeSeconds,
    IWalkingRoutingService routingService)
    { 
        
        List<RouteEntity> routes;
    try
    {
        routes = await _routeProvider.GetRoutesAsync();
    }
    catch (Exception ex)
    {
        throw new HttpRequestException("GRPC Communication failed", ex, HttpStatusCode.InternalServerError);
    }
        var allUniqueStops = routes
            .SelectMany(r => r.Trips)
            .SelectMany(t => t.Stops)
            .DistinctBy(s => new { s.Latitude, s.Longitude })
            .ToList();

        var closestStartStopsCandidates = allUniqueStops
            .Select(s => new
            {
                Stop = s,
                Distance = DistanceService.GetDistanceInMeters(startLat, startLong, s.Latitude, s.Longitude)
            })
            .OrderBy(x => x.Distance)
            .Take(3)
            .Select(x => x.Stop)
            .ToList();

        var closestEndStopsCandidates = allUniqueStops
            .Select(s => new
            {
                Stop = s, Distance = DistanceService.GetDistanceInMeters(endLat, endLong, s.Latitude, s.Longitude)
            })
            .OrderBy(x => x.Distance)
            .Take(3)
            .Select(x => x.Stop)
            .ToList();

        var startWalkingTimes = new Dictionary<StopEntity, int>();
        foreach (var stop in closestStartStopsCandidates)
        {
            startWalkingTimes[stop] =
                await routingService.GetWalkingTimeSecondsAsync(startLat, startLong, stop.Latitude, stop.Longitude);
        }

        var endWalkingTimes = new Dictionary<StopEntity, int>();
        foreach (var stop in closestEndStopsCandidates)
        {
            endWalkingTimes[stop] =
                await routingService.GetWalkingTimeSecondsAsync(stop.Latitude, stop.Longitude, endLat, endLong);
        }

        var validRoutes = new List<TripMatchResult>();

        foreach (var route in routes)
        {
            foreach (var trip in route.Trips)
            {
                var matchedStartStop = trip.Stops
                    .Where(s => closestStartStopsCandidates.Any(css =>
                        css.Latitude == s.Latitude && css.Longitude == s.Longitude))
                    .OrderBy(s => s.SequenceOrder)
                    .FirstOrDefault();

                var matchedEndStop = trip.Stops
                    .Where(s => closestEndStopsCandidates.Any(ces =>
                        ces.Latitude == s.Latitude && ces.Longitude == s.Longitude))
                    .OrderByDescending(s => s.SequenceOrder)
                    .FirstOrDefault();

                if (matchedStartStop != null && matchedEndStop != null &&
                    matchedStartStop.SequenceOrder < matchedEndStop.SequenceOrder)
                {
                    int walkTimeStartSeconds = startWalkingTimes.First(kvp =>
                        kvp.Key.Latitude == matchedStartStop.Latitude &&
                        kvp.Key.Longitude == matchedStartStop.Longitude).Value;

                    int userArrivalAtStopSeconds = userDepartureTimeSeconds + walkTimeStartSeconds;

                    if (matchedStartStop.ArrivalTime >= userArrivalAtStopSeconds)
                    {
                        int walkTimeEndSeconds = endWalkingTimes.First(kvp =>
                            kvp.Key.Latitude == matchedEndStop.Latitude &&
                            kvp.Key.Longitude == matchedEndStop.Longitude).Value;

                        int transitTimeSeconds = matchedEndStop.ArrivalTime - matchedStartStop.ArrivalTime;

                        int finalArrivalTimeSeconds = matchedEndStop.ArrivalTime + walkTimeEndSeconds;
                        int totalTimeSeconds = finalArrivalTimeSeconds - userDepartureTimeSeconds;

                        validRoutes.Add(new TripMatchResult
                        {
                            RouteId = route.RouteId,
                            Trip = trip,
                            StartStop = matchedStartStop,
                            EndStop = matchedEndStop,
                            StartWalkTimeSeconds = walkTimeStartSeconds,
                            EndWalkTimeSeconds = walkTimeEndSeconds,
                            TransitTimeSeconds = transitTimeSeconds,
                            TotalTimeSeconds = totalTimeSeconds,
                            FinalArrivalTimeSeconds = finalArrivalTimeSeconds
                        });
                    }
                }
            }
        }

        if (validRoutes.Count <= 0)
            throw new HttpRequestException("No trips found", null, HttpStatusCode.NotFound);

        return validRoutes
            .OrderBy(r => r.FinalArrivalTimeSeconds)
            .ToList();
    
}
}