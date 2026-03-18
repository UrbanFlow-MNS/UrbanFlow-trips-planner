using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips;
using Empty = Google.Protobuf.WellKnownTypes.Empty;

namespace UrbanFlow_trips_planner.Infrastructure.Providers;

public class RouteProvider : IRouteProvider
{
    private readonly Tripper.TripperClient _tripperClient;

    public RouteProvider(Tripper.TripperClient tripperClient)
    {
        _tripperClient = tripperClient;
    }

    public async Task<List<RouteEntity>> GetRoutesAsync()
    {
        var reply = await _tripperClient.FindAllAsync(new UrbanFlow_trips.Empty());
        return reply.Routes.Select(MapToEntity).ToList();
    }

    public async Task<List<RouteEntity>> GetRoutesByAgencyAsync(int id)
    {
        var reply = await _tripperClient.FindByIdAsync(new RouteRequest { Id = id });
        return reply.Routes.Select(MapToEntity).ToList();
    }

    private static RouteEntity MapToEntity(CompleteRoute r) => new()
    {
        RouteId        = r.RouteId,
        RouteShortName = r.RouteShortName,
        RouteLongName  = r.RouteLongName,
        RouteTypeName  = r.RouteTypeName,
        Trips = r.Trips.Select(t => new TripEntity
        {
            TripId = t.TripId,
            Stops  = t.Stops.Select(s => new StopEntity
            {
                StopId        = s.StopId,
                StopName      = s.StopName,
                Longitude     = (float)s.Longitude,
                Latitude      = (float)s.Latitude,
                ArrivalTime   = TimeToSeconds(s.ArrivalTime),
                SequenceOrder = s.SequenceOrder
            }).ToList()
        }).ToList()
    };

    private static int TimeToSeconds(string time)
    {
        var parts = time.Split(':');
        return int.Parse(parts[0]) * 3600
               + int.Parse(parts[1]) * 60
               + int.Parse(parts[2]);
    }
}