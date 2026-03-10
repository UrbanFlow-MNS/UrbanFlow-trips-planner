using UrbanFlow_trips_planner_tests.MockingClasses;
using UrbanFlow_trips_planner.API.GrpcServices;
using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Tests;

[TestClass]
public class PathfinderServiceTests
{
    [TestMethod]
    public async Task GetFastestRoutes_ShouldReturnTripsOrderedByTotalTime()
    {
        float userStartLon = 6.1680000f;
        float userStartLat = 49.1100000f;
        float destinationLon = 6.1700000f;
        float destinationLat = 49.1200000f;

        var fakeTrips = new List<TripEntity>
        {
            new TripEntity
            {
                TripId = "1",
                Stops = new[]
                {
                    new StopEntity { StopId = 1, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 61200, SequenceOrder = 1 },
                    new StopEntity { StopId = 2, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 61800, SequenceOrder = 2 }
                }
            },
            new TripEntity
            {
                TripId = "2",
                Stops = new[]
                {
                    new StopEntity { StopId = 3, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 65000, SequenceOrder = 1 },
                    new StopEntity { StopId = 4, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 65300, SequenceOrder = 2 }
                }
            }
        };

        var routingService = new FakeWalkingRoutingService();
        var fakeTripProvider = new FakeTripProvider(fakeTrips);
        var pathfinderService = new PathfinderService(fakeTripProvider);

        var result = await pathfinderService.GetFastestRoutes(
            userStartLon, userStartLat, 
            destinationLon, destinationLat, 
            routingService);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        
        var fastestRoute = result.First();
        Assert.AreEqual("2", fastestRoute.Trip.TripId);
        Assert.AreEqual(300, fastestRoute.TransitTimeSeconds);
        Assert.AreEqual(240, fastestRoute.WalkTimeSeconds);
        Assert.AreEqual(540, fastestRoute.TotalTimeSeconds);
    }
}