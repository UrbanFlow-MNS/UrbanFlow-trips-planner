
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner_tests.MockingClasses;

public class FakeTripProvider : ITripProvider
{
    private readonly List<TripEntity> _fakeTrips;

    public FakeTripProvider(List<TripEntity> fakeTrips)
    {
        _fakeTrips = fakeTrips;
    }

    public Task<List<TripEntity>> GetTripsAsync()
    {
        return Task.FromResult(_fakeTrips);
    }
}