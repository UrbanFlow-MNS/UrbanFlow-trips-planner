using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrbanFlow_trips_planner.Domain.Entities;
using UrbanFlow_trips_planner.Application.DTO;
using UrbanFlow_trips_planner.Domain.Interfaces;
using UrbanFlow_trips_planner.API.GrpcServices;

namespace UrbanFlow_trips_planner.Tests
{
    public class FakeWalkingRoutingService : IWalkingRoutingService
    {
        public Task<int> GetWalkingTimeSecondsAsync(double startLat, double startLon, double endLat, double endLon)
        {
            return Task.FromResult(120);
        }
    }

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
                        new StopEntity { StopId = 1, StopName = "Gare", Longitude = "6.1687997", Latitude = "49.1106807", ArrivalTime = "61200", SequenceOrder = 1 },
                        new StopEntity { StopId = 2, StopName = "Saulcy", Longitude = "6.1699000", Latitude = "49.1190000", ArrivalTime = "61800", SequenceOrder = 2 }
                    }
                },
                new TripEntity
                {
                    TripId = "2",
                    Stops = new[]
                    {
                        new StopEntity { StopId = 3, StopName = "Gare", Longitude = "6.1687997", Latitude = "49.1106807", ArrivalTime = "65000", SequenceOrder = 1 },
                        new StopEntity { StopId = 4, StopName = "Saulcy", Longitude = "6.1699000", Latitude = "49.1190000", ArrivalTime = "65300", SequenceOrder = 2 }
                    }
                }
            };

            var routingService = new FakeWalkingRoutingService();
            
            // NOTE IMPORTANTE : 
            // Pour que ce test fonctionne sans passer les fakeTrips en paramètre de GetFastestRoutes, 
            // il faut que tu injectes une fausse source de données dans le constructeur de PathfinderService.
            // Par exemple : var pathfinderService = new PathfinderService(new FakeTripRepository(fakeTrips));
            var pathfinderService = new PathfinderService(); 

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
}