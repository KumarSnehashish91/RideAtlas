namespace RideAtlas.Application.Packing.Dtos;

public record PackingItemDto(
    Guid Id,
    Guid TripId,
    string Category,
    string Name,
    bool IsPacked,
    int SortOrder,
    DateTime CreatedAt);
