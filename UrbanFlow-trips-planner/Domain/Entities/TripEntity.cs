using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class TripEntity
{
    [JsonPropertyName("tripId")]
    public int TripId { get; set; }
    
    [JsonPropertyName("stops")]
    public List<StopEntity> Stops { get; set; }
}