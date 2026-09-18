using System.Text.Json.Serialization;

namespace UrbanFlow_trips_planner.Domain.Entities;

public class StopEntity : IEquatable<StopEntity>
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

    public bool Equals(StopEntity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StopId == other.StopId;
    }

    public override bool Equals(object? obj) => Equals(obj as StopEntity);

    public override int GetHashCode() => StopId.GetHashCode();
}