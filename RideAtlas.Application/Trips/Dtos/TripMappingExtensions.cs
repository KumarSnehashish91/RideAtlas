using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Trips.Dtos;

public static class TripMappingExtensions
{
    public static TripDto ToDto(this Trip trip, DateOnly today) => new(
        trip.Id,
        trip.Name,
        trip.StartDate,
        trip.EndDate,
        trip.BikeId,
        trip.DistanceKm,
        trip.Budget,
        trip.IsDraft,
        trip.IsCancelled,
        trip.HasStarted,
        trip.HasRideCompleted,
        trip.Status(today),
        trip.CreatedAt);
}
