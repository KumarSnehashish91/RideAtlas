namespace RideAtlas.Domain.Entities;

public class JournalEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid? BikeId { get; set; }
    public Guid? TripId { get; set; }
    public Guid? RideId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public string? Mood { get; set; }
    public double? DistanceKm { get; set; }
    public string? PhotoUrisCsv { get; set; }
    public string? RiderInputRaw { get; set; }
    public string? AiGeneratedStory { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
