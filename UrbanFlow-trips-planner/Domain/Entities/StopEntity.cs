using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class StopEntity
{
    [JsonPropertyName("stopId")]
    public long StopId { get; set; }
    
    [JsonPropertyName("stopName")]
    public string StopName { get; set; }
    
    [JsonPropertyName("longitude")]
    public string Longitude { get; set; }
    
    [JsonPropertyName("latitude")]
    public string Latitude { get; set; }
    
    [JsonPropertyName("arrivalTime")]
    public string ArrivalTime { get; set; }
    
    [JsonPropertyName("sequenceOrder")]
    public int SequenceOrder { get; set; }
}