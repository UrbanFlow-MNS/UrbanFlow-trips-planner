
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner_tests.MockingClasses;

public class FakeRouteProvider : IRouteProvider
{
    private readonly List<RouteEntity> _fakeRoutes;

    public FakeRouteProvider(List<RouteEntity> fakeRoutes)
    {
        _fakeRoutes = fakeRoutes;
    }

    public Task<List<RouteEntity>> GetRoutesAsync()
    {
        return Task.FromResult(_fakeRoutes);
    }

    public Task<List<RouteEntity>> GetRoutesByAgencyAsync(int id)
    {
        return Task.FromResult(_fakeRoutes);
    }

}