
using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Domain.Interfaces;

public interface ITripProvider
{
    Task<List<TripEntity>> GetTripsAsync();
}