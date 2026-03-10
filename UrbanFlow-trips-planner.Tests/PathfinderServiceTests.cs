using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrbanFlow_trips_planner_tests.MockingClasses;
using UrbanFlow_trips_planner.API.GrpcServices;
using UrbanFlow_trips_planner.Domain.Entities;

namespace UrbanFlow_trips_planner.Tests;

[TestClass]
public class PathfinderServiceTests
{
    [TestMethod]
    public async Task GetFastestRoutes_ShouldReturnTripsOrderedByFinalArrivalTime()
    {
        float userStartLon = 6.1680000f;
        float userStartLat = 49.1100000f;
        float destinationLon = 6.1700000f;
        float destinationLat = 49.1200000f;
        int userDepartureTimeSeconds = 60000; 

        var fakeRoutes = new List<RouteEntity>
        {
            new RouteEntity
            {
                RouteId = 1,
                RouteShortName = "Ligne Lente",
                Trips = new List<TripEntity>
                {
                    new TripEntity
                    {
                        TripId = "1",
                        Stops = new List<StopEntity>
                        {
                            new StopEntity { StopId = 1, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 60200, SequenceOrder = 1 },
                            new StopEntity { StopId = 2, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 61000, SequenceOrder = 2 }
                        }
                    }
                }
            },
            new RouteEntity
            {
                RouteId = 2,
                RouteShortName = "Ligne Rapide",
                Trips = new List<TripEntity>
                {
                    new TripEntity
                    {
                        TripId = "2",
                        Stops = new List<StopEntity>
                        {
                            new StopEntity { StopId = 3, StopName = "Gare", Longitude = 6.1687997, Latitude = 49.1106807, ArrivalTime = 60300, SequenceOrder = 1 },
                            new StopEntity { StopId = 4, StopName = "Saulcy", Longitude = 6.1699000, Latitude = 49.1190000, ArrivalTime = 60800, SequenceOrder = 2 }
                        }
                    }
                }
            }
        };

        var routingService = new FakeWalkingRoutingService();
        var fakeTripProvider = new FakeRouteProvider(fakeRoutes); 
        var pathfinderService = new PathfinderService(fakeTripProvider);

        var result = await pathfinderService.GetFastestRoutes(
            userStartLon, userStartLat, 
            destinationLon, destinationLat, 
            userDepartureTimeSeconds,
            routingService);

        Console.WriteLine($"Recherche lancée à : {userDepartureTimeSeconds}s depuis minuit\n");

        for (int i = 0; i < result.Count; i++)
        {
            var r = result[i];
            Console.WriteLine($"--- OPTION {i + 1} : {r.Route.RouteShortName} (Trip {r.Trip.TripId}) ---");
            Console.WriteLine($"Depart bus : {r.StartStop.StopName} a {TimeSpan.FromSeconds(r.StartStop.ArrivalTime):hh\\:mm\\:ss}");
            Console.WriteLine($"Arrivee bus : {r.EndStop.StopName} a {TimeSpan.FromSeconds(r.EndStop.ArrivalTime):hh\\:mm\\:ss}");
            Console.WriteLine($"Marche totale : {r.FormattedWalkTime} ({r.WalkTimeSeconds}s)");
            Console.WriteLine($"Dans le bus : {r.FormattedTransitTime} ({r.TransitTimeSeconds}s)");
            Console.WriteLine($"TEMPS GLOBAL : {r.FormattedTotalTime} ({r.TotalTimeSeconds}s)");
            Console.WriteLine($"ARRIVEE FINALE : {TimeSpan.FromSeconds(r.FinalArrivalTimeSeconds):hh\\:mm\\:ss}\n");
        }
        
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        
        var fastestRoute = result.First();
        
        
        Assert.AreEqual("2", fastestRoute.Trip.TripId);
        Assert.AreEqual(500, fastestRoute.TransitTimeSeconds);
        Assert.AreEqual(240, fastestRoute.WalkTimeSeconds);
        Assert.AreEqual(920, fastestRoute.TotalTimeSeconds);
        Assert.AreEqual(60920, fastestRoute.FinalArrivalTimeSeconds);
    }
}