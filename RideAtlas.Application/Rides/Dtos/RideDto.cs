namespace RideAtlas.Application.Rides.Dtos;

public record RideDto(
    Guid Id,
    Guid BikeId,
    Guid? TripId,
    DateTime StartedAt,
    DateTime EndedAt,
    double DistanceKm,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    double? WeatherTempC,
    string? Notes,
    DateTime CreatedAt);
