namespace RideAtlas.Domain.Entities;

public class ReminderRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid BikeId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? CustomLabel { get; set; }
    public double? IntervalKm { get; set; }
    public int? IntervalDays { get; set; }
    public double BaselineOdometerKm { get; set; }
    public DateOnly BaselineDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
