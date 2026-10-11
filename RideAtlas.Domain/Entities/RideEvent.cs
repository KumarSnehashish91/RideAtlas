namespace RideAtlas.Domain.Entities;

public class RideEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string? MeetingPoint { get; set; }
    public string? Destination { get; set; }
    public Guid? TripId { get; set; }
    public Guid? BikeId { get; set; }
    public string? CoverPhotoUri { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
