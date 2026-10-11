namespace RideAtlas.Domain.Entities;

public class ExplorerRide
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
    public string? Region { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public double EstimatedDurationHours { get; set; }
    public int RecommendedTripDays { get; set; }
    public string RoadType { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string? BestSeason { get; set; }
    public double ScenicRating { get; set; }
    public bool PermitRequired { get; set; }
    public bool CampingAllowed { get; set; }
    public string? MinimumExperience { get; set; }
    public string? PackingTemplate { get; set; }
    public string? HeroImageUri { get; set; }
    public string? AiSummary { get; set; }
    public string? Highlights { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
