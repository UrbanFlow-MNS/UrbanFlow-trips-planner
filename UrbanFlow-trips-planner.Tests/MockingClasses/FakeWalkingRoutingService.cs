using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner_tests.MockingClasses;

public class FakeWalkingRoutingService : IWalkingRoutingService
{
    // On simule un temps de marche fixe de 2 minutes (120 secondes) pour simplifier les tests mathématiques
    public Task<int> GetWalkingTimeSecondsAsync(double startLat, double startLon, double endLat, double endLon)
    {
        return Task.FromResult(120); 
    }
}