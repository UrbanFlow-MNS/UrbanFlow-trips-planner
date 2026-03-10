using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class StopEntity
{
    [JsonPropertyName("stopId")]
    public long StopId { get; set; }
    
    [JsonPropertyName("stopName")]
    public string StopName { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("arrivalTime")]
    public int ArrivalTime { get; set; }
    
    [JsonPropertyName("sequenceOrder")]
    public int SequenceOrder { get; set; }
}