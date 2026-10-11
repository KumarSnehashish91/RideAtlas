using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Explorer.Dtos;

public static class ExplorerRideMappingExtensions
{
    public static ExplorerRideDto ToDto(this ExplorerRide ride) => new(
        ride.Id,
        ride.Slug,
        ride.Title,
        ride.Country,
        ride.State,
        ride.Region,
        ride.Origin,
        ride.Destination,
        ride.DistanceKm,
        ride.EstimatedDurationHours,
        ride.RecommendedTripDays,
        ride.RoadType,
        ride.Difficulty,
        ride.BestSeason,
        ride.ScenicRating,
        ride.PermitRequired,
        ride.CampingAllowed,
        ride.MinimumExperience,
        ride.PackingTemplate,
        ride.HeroImageUri,
        ride.AiSummary,
        ride.Highlights,
        ride.CreatedAt);
}
