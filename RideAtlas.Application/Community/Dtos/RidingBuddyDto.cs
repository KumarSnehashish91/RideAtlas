namespace RideAtlas.Application.Community.Dtos;

public record RidingBuddyDto(
    Guid Id,
    string Name,
    string? HomeCity,
    string? AvatarUri,
    string? Notes,
    bool IsFollowing,
    Guid? RemoteRiderId,
    DateTime AddedAt);
