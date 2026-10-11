namespace RideAtlas.Domain.Entities;

public class Ride
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid BikeId { get; set; }
    public Guid? TripId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public double DistanceKm { get; set; }
    public double? AvgSpeedKmh { get; set; }
    public double? MaxSpeedKmh { get; set; }
    public string? RouteDescription { get; set; }
    public string? WeatherCondition { get; set; }
    public double? WeatherTempC { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
