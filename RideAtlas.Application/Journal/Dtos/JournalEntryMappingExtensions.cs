using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Journal.Dtos;

public static class JournalEntryMappingExtensions
{
    public static JournalEntryDto ToDto(this JournalEntry entry) => new(
        entry.Id,
        entry.BikeId,
        entry.TripId,
        entry.RideId,
        entry.Title,
        entry.Date,
        entry.Notes,
        entry.Mood,
        entry.DistanceKm,
        SplitPhotoUris(entry.PhotoUrisCsv),
        entry.RiderInputRaw,
        entry.AiGeneratedStory,
        entry.CreatedAt);

    public static string? JoinPhotoUris(IReadOnlyList<string>? photoUris) =>
        photoUris is null || photoUris.Count == 0 ? null : string.Join('|', photoUris);

    private static IReadOnlyList<string> SplitPhotoUris(string? csv) =>
        string.IsNullOrWhiteSpace(csv) ? [] : csv.Split('|', StringSplitOptions.RemoveEmptyEntries);
}
