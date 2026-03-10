using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class RouteEntity
{
    [JsonPropertyName("routeId")]
    public int RouteId { get; set; }
    
    [JsonPropertyName("routeShortName")]
    public string RouteShortName { get; set; }
    
    [JsonPropertyName("routeLongName")]
    public string RouteLongName { get; set; }
    
    [JsonPropertyName("routeTypeName")]
    public string RouteTypeName { get; set; }
    
    [JsonPropertyName("trips")]
    public List<TripEntity> Trips { get; set; }
    
    
}