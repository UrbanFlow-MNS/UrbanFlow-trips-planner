using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class TripEntity
{
    [JsonPropertyName("tripId")]
    public string TripId { get; set; }
    
    [JsonPropertyName("stops")]
    public StopEntity[] Stops { get; set; }
}