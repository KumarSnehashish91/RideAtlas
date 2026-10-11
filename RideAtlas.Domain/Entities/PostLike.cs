namespace RideAtlas.Domain.Entities;

public class PostLike
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PostId { get; set; }
    public Guid RiderAccountId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
