using RideAtlas.Domain.Enums;

namespace RideAtlas.Application.Trips.Dtos;

public record TripDto(
    Guid Id,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BikeId,
    double? DistanceKm,
    decimal? Budget,
    bool IsDraft,
    bool IsCancelled,
    bool HasStarted,
    bool HasRideCompleted,
    TripStatus Status,
    DateTime CreatedAt);
