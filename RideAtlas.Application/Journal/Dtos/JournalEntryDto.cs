namespace RideAtlas.Application.Journal.Dtos;

public record JournalEntryDto(
    Guid Id,
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Title,
    DateOnly Date,
    string? Notes,
    string? Mood,
    double? DistanceKm,
    IReadOnlyList<string> PhotoUris,
    string? RiderInputRaw,
    string? AiGeneratedStory,
    DateTime CreatedAt);
