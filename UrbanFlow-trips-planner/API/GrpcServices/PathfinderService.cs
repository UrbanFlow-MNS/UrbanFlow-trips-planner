using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Entities;
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

    public async Task<List<TripEntity>> GetFastestRoutes(float startLong, float startLat, float endLong, float endLat)
    {
        // TODO : Récupération des trips
        List<TripEntity> trips = new();
        int allowedDistanceMargin = 500; // Distance en mètres maximum

        var allUniqueStops = trips
            .SelectMany(t => t.Stops)
            .DistinctBy(s => new { s.Latitude, s.Longitude })
            .ToList();

        var closestStartStops = allUniqueStops
            .Select(s => new
            {
                Stop = s, Distance = DistanceService.GetDistanceInMeters(startLat, startLong, s.Latitude, s.Longitude)
            })
            .Where(x => x.Distance <= allowedDistanceMargin)
            .OrderBy(x => x.Distance)
            .Take(3)
            .Select(x => x.Stop)
            .ToList();

        var closestEndStops = allUniqueStops
            .Select(s => new
                { Stop = s, Distance = DistanceService.GetDistanceInMeters(endLat, endLong, s.Latitude, s.Longitude) })
            .Where(x => x.Distance <= allowedDistanceMargin)
            .OrderBy(x => x.Distance)
            .Take(3)
            .Select(x => x.Stop)
            .ToList();

        var validRoutes = new List<TripMatchResult>();

        foreach (var trip in trips)
        {
            var matchedStartStop = trip.Stops
                .Where(s => closestStartStops.Any(css => css.Latitude == s.Latitude && css.Longitude == s.Longitude))
                .OrderBy(s => s.SequenceOrder)
                .FirstOrDefault();

            var matchedEndStop = trip.Stops
                .Where(s => closestEndStops.Any(ces => ces.Latitude == s.Latitude && ces.Longitude == s.Longitude))
                .OrderByDescending(s => s.SequenceOrder)
                .FirstOrDefault();

            if (matchedStartStop != null && matchedEndStop != null &&
                matchedStartStop.SequenceOrder < matchedEndStop.SequenceOrder)
            {
                validRoutes.Add(new TripMatchResult
                {
                    Trip = trip,
                    StartStop = matchedStartStop,
                    EndStop = matchedEndStop
                });
            }
        }

        // TODO : Calculer le temps de trajet
        return trips;
    }
}