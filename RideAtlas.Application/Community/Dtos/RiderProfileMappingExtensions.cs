using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Dtos;

public static class RiderProfileMappingExtensions
{
    public static RiderProfileDto ToDto(this RiderProfile profile) => new(
        profile.DisplayName,
        profile.Bio,
        profile.HomeCity,
        profile.AvatarUri,
        profile.RidingSinceYear,
        profile.IsPublic,
        profile.ShowLocation,
        profile.ShowStats,
        profile.ShowBikes);
}
