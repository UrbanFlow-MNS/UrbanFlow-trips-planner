using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Application.DTO;

public record TripMatchResult
{
    public TripEntity Trip { get; init; }
    public StopEntity StartStop { get; init; }
    public StopEntity EndStop { get; init; }
}