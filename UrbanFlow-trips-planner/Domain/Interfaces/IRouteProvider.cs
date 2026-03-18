
using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Domain.Interfaces;

public interface IRouteProvider
{
    Task<List<RouteEntity>> GetRoutesAsync();
    Task<List<RouteEntity>> GetRoutesByAgencyAsync(int id);

}