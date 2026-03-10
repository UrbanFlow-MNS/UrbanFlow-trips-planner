using System.Globalization;
using System.Text.Json.Serialization;
using UrbanFlow_trips_planner.Domain.Interfaces;

namespace UrbanFlow_trips_planner.Domain.Services;

public class OsrmRoutingService : IWalkingRoutingService
{
    private readonly HttpClient _httpClient;

    public OsrmRoutingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "UrbanFlow-Trips-Planner/1.0 (Dev)");
    }

    public async Task<int> GetWalkingTimeSecondsAsync(double startLat, double startLon, double endLat, double endLon)
    {
        string startLonStr = startLon.ToString(CultureInfo.InvariantCulture);
        string startLatStr = startLat.ToString(CultureInfo.InvariantCulture);
        string endLonStr = endLon.ToString(CultureInfo.InvariantCulture);
        string endLatStr = endLat.ToString(CultureInfo.InvariantCulture);
        
        string url = $"https://router.project-osrm.org/route/v1/foot/{startLonStr},{startLatStr};{endLonStr},{endLatStr}?overview=false";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OsrmResponse>(url);

            if (response?.Routes != null && response.Routes.Count > 0)
            {
                return (int)response.Routes[0].Duration;
            }

            return 999999; // Si aucun chemin piéton n'est trouvé
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur OSRM : {ex.Message}");
            return 999999;
        }
    }

    // pour désérialisation
    private class OsrmResponse
    {
        [JsonPropertyName("routes")]
        public List<OsrmRoute> Routes { get; set; }
    }

    private class OsrmRoute
    {
        [JsonPropertyName("duration")]
        public double Duration { get; set; }
    }
}
