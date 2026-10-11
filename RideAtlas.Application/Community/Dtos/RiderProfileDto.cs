namespace RideAtlas.Application.Community.Dtos;

public record RiderProfileDto(
    string DisplayName,
    string? Bio,
    string? HomeCity,
    string? AvatarUri,
    int? RidingSinceYear,
    bool IsPublic,
    bool ShowLocation,
    bool ShowStats,
    bool ShowBikes);
