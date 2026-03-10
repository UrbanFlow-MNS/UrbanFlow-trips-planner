using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.Domain.Services;

namespace UrbanFlow_trips_planner.API.GrpcServices;

public class PathfinderService : IPathfinderService
{
    public Task<List<TripEntity>> GetTrips()
    {
        throw new NotImplementedException();
    }

    public Task<List<TripEntity>> GetTripsBasedOnDistanceFromStartPosAndEndPos(float startLong, float startLat,
        float endLong, float endLat)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<TripMatchResult>> GetFastestRoutes( float startLong, float startLat, float endLong, float endLat, IWalkingRoutingService routingService)
    {
        List<TripEntity> trips = new();
    
        var allUniqueStops = trips
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
    
        foreach (var trip in trips)
        {
            var matchedStartStop = trip.Stops
                .Where(s => closestStartStopsCandidates.Any(closestStartStop => closestStartStop.Latitude == s.Latitude && closestStartStop.Longitude == s.Longitude))
                .OrderBy(s => s.SequenceOrder)
                .FirstOrDefault();
    
            var matchedEndStop = trip.Stops
                .Where(s => closestEndStopsCandidates.Any(closestEndStop => closestEndStop.Latitude == s.Latitude && closestEndStop.Longitude == s.Longitude))
                .OrderByDescending(s => s.SequenceOrder)
                .FirstOrDefault();
    
            if (matchedStartStop != null && matchedEndStop != null &&
                matchedStartStop.SequenceOrder < matchedEndStop.SequenceOrder)
            {
                int transitTimeSeconds = matchedEndStop.ArrivalTime - matchedStartStop.ArrivalTime;
    
                int walkTimeStartSeconds = startWalkingTimes.First(kvp => kvp.Key.Latitude == matchedStartStop.Latitude && kvp.Key.Longitude == matchedStartStop.Longitude).Value;
                int walkTimeEndSeconds = endWalkingTimes.First(kvp => kvp.Key.Latitude == matchedEndStop.Latitude && kvp.Key.Longitude == matchedEndStop.Longitude).Value;
                
                int totalWalkTimeSeconds = walkTimeStartSeconds + walkTimeEndSeconds;
                int totalTimeSeconds = transitTimeSeconds + totalWalkTimeSeconds;
    
                validRoutes.Add(new TripMatchResult
                {
                    Trip = trip,
                    StartStop = matchedStartStop,
                    EndStop = matchedEndStop,
                    TransitTimeSeconds = transitTimeSeconds,
                    WalkTimeSeconds = totalWalkTimeSeconds,
                    TotalTimeSeconds = totalTimeSeconds
                });
            }
        }
    
        return validRoutes.OrderBy(r => r.TotalTimeSeconds).ToList();
    }
}