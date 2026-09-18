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
        var routes = await GetRoutesOrThrowAsync();
    
        var allUniqueStops = GetUniqueStops(routes);
    
        var closestStartStops = GetClosestStops(allUniqueStops, startLat, startLong);
        var closestEndStops = GetClosestStops(allUniqueStops, endLat, endLong);
    
        var startWalkingTimes = await GetWalkingTimesAsync(
            closestStartStops, startLat, startLong, routingService, fromOrigin: true);
    
        var endWalkingTimes = await GetWalkingTimesAsync(
            closestEndStops, endLat, endLong, routingService, fromOrigin: false);
    
        var validRoutes = BuildValidTripMatches(
            routes, closestStartStops, closestEndStops,
            startWalkingTimes, endWalkingTimes, userDepartureTimeSeconds);
    
        if (validRoutes.Count <= 0)
            throw new HttpRequestException("No trips found", null, HttpStatusCode.NotFound);
    
        return validRoutes
            .OrderBy(r => r.FinalArrivalTimeSeconds)
            .ToList();
    }
    
    private async Task<List<RouteEntity>> GetRoutesOrThrowAsync()
    {
        try
        {
            return await _routeProvider.GetRoutesAsync();
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("GRPC Communication failed", ex, HttpStatusCode.InternalServerError);
        }
    }
    
    private static List<StopEntity> GetUniqueStops(List<RouteEntity> routes)
    {
        return routes
            .SelectMany(r => r.Trips)
            .SelectMany(t => t.Stops)
            .DistinctBy(s => s.StopId)
            .ToList();
    }
    
    private static List<StopEntity> GetClosestStops(
        List<StopEntity> stops, float latitude, float longitude, int count = 3)
    {
        return stops
            .Select(s => new
            {
                Stop = s,
                Distance = DistanceService.GetDistanceInMeters(latitude, longitude, s.Latitude, s.Longitude)
            })
            .OrderBy(x => x.Distance)
            .Take(count)
            .Select(x => x.Stop)
            .ToList();
    }
    
    private static async Task<Dictionary<StopEntity, int>> GetWalkingTimesAsync(
        List<StopEntity> stops, float latitude, float longitude,
        IWalkingRoutingService routingService, bool fromOrigin)
    {
        var walkingTimes = new Dictionary<StopEntity, int>();
    
        foreach (var stop in stops)
        {
            walkingTimes[stop] = fromOrigin
                ? await routingService.GetWalkingTimeSecondsAsync(latitude, longitude, stop.Latitude, stop.Longitude)
                : await routingService.GetWalkingTimeSecondsAsync(stop.Latitude, stop.Longitude, latitude, longitude);
        }
    
        return walkingTimes;
    }
    
    private static List<TripMatchResult> BuildValidTripMatches(
        List<RouteEntity> routes,
        List<StopEntity> closestStartStops,
        List<StopEntity> closestEndStops,
        Dictionary<StopEntity, int> startWalkingTimes,
        Dictionary<StopEntity, int> endWalkingTimes,
        int userDepartureTimeSeconds)
    {
        var validRoutes = new List<TripMatchResult>();
    
        foreach (var route in routes)
        {
            foreach (var trip in route.Trips)
            {
                var match = TryBuildTripMatch(
                    route, trip, closestStartStops, closestEndStops,
                    startWalkingTimes, endWalkingTimes, userDepartureTimeSeconds);
    
                if (match != null)
                    validRoutes.Add(match);
            }
        }
    
        return validRoutes;
    }
    
    private static TripMatchResult? TryBuildTripMatch(
        RouteEntity route,
        TripEntity trip,
        List<StopEntity> closestStartStops,
        List<StopEntity> closestEndStops,
        Dictionary<StopEntity, int> startWalkingTimes,
        Dictionary<StopEntity, int> endWalkingTimes,
        int userDepartureTimeSeconds)
    {
        var matchedStartStop = FindMatchedStop(trip.Stops, closestStartStops, ascending: true);
        var matchedEndStop = FindMatchedStop(trip.Stops, closestEndStops, ascending: false);
    
        if (matchedStartStop == null || matchedEndStop == null)
            return null;
    
        if (matchedStartStop.SequenceOrder >= matchedEndStop.SequenceOrder)
            return null;
    
        int walkTimeStartSeconds = startWalkingTimes[matchedStartStop];
        int userArrivalAtStopSeconds = userDepartureTimeSeconds + walkTimeStartSeconds;
    
        if (matchedStartStop.ArrivalTime < userArrivalAtStopSeconds)
            return null;
    
        int walkTimeEndSeconds = endWalkingTimes[matchedEndStop];
        int transitTimeSeconds = matchedEndStop.ArrivalTime - matchedStartStop.ArrivalTime;
        int finalArrivalTimeSeconds = matchedEndStop.ArrivalTime + walkTimeEndSeconds;
        int totalTimeSeconds = finalArrivalTimeSeconds - userDepartureTimeSeconds;
    
        return new TripMatchResult
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
        };
    }
    
    private static StopEntity? FindMatchedStop(
        IEnumerable<StopEntity> tripStops, List<StopEntity> candidates, bool ascending)
    {
        var candidateIds = candidates.Select(c => c.StopId).ToHashSet();
        var query = tripStops.Where(s => candidateIds.Contains(s.StopId));
    
        return ascending
            ? query.OrderBy(s => s.SequenceOrder).FirstOrDefault()
            : query.OrderByDescending(s => s.SequenceOrder).FirstOrDefault();
    }
    
}