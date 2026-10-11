namespace RideAtlas.Domain.Entities;

public class CommunityPost
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid? TripId { get; set; }
    public Guid? JournalEntryId { get; set; }
    public string? Caption { get; set; }
    public string? PhotoUrisCsv { get; set; }
    public string? Location { get; set; }
    public double? DistanceKm { get; set; }
    public string Visibility { get; set; } = "Public";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
