namespace RideAtlas.Domain.Entities;

public class RiderProfile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? HomeCity { get; set; }
    public string? AvatarUri { get; set; }
    public int? RidingSinceYear { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool ShowLocation { get; set; } = true;
    public bool ShowStats { get; set; } = true;
    public bool ShowBikes { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
