namespace UrbanFlow_trips_planner.Domain.Interfaces;

public interface IPrometheusService
{
    Task<string> GetMetricsAsync();
}