using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Dtos;

public static class RidingBuddyMappingExtensions
{
    public static RidingBuddyDto ToDto(this RidingBuddy buddy) => new(
        buddy.Id,
        buddy.Name,
        buddy.HomeCity,
        buddy.AvatarUri,
        buddy.Notes,
        buddy.IsFollowing,
        buddy.RemoteRiderId,
        buddy.AddedAt);
}
