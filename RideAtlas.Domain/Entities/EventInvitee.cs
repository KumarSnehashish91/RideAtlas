namespace RideAtlas.Domain.Entities;

public class EventInvitee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Guid BuddyId { get; set; }
    public string RsvpStatus { get; set; } = "Invited";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
