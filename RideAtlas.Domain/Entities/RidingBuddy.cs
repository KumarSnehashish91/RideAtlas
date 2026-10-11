namespace RideAtlas.Domain.Entities;

public class RidingBuddy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? HomeCity { get; set; }
    public string? AvatarUri { get; set; }
    public string? Notes { get; set; }
    public bool IsFollowing { get; set; }
    public Guid? RemoteRiderId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
