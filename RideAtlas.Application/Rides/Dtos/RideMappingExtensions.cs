using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Dtos;

public static class RideMappingExtensions
{
    public static RideDto ToDto(this Ride ride) => new(
        ride.Id,
        ride.BikeId,
        ride.TripId,
        ride.StartedAt,
        ride.EndedAt,
        ride.DistanceKm,
        ride.AvgSpeedKmh,
        ride.MaxSpeedKmh,
        ride.RouteDescription,
        ride.WeatherCondition,
        ride.WeatherTempC,
        ride.Notes,
        ride.CreatedAt);
}
