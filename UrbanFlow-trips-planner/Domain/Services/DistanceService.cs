namespace UrbanFlow_trips_planner.Domain.Services;

public class DistanceService : IDistanceService
{
    public static double CalculateDistance(
        double startLat, double startLong, 
        double finalLat, double finalLong)
    {
        const double EarthRadiusKm = 6371.0;

        var dLat = (finalLat - startLat) * Math.PI / 180.0;
        var dLon = (finalLong - startLong) * Math.PI / 180.0;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(startLat * Math.PI / 180.0) * Math.Cos(finalLat * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return (EarthRadiusKm * c)*1000;
    }
}