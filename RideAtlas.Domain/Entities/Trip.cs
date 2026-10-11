using RideAtlas.Domain.Enums;

namespace RideAtlas.Domain.Entities;

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? BikeId { get; set; }
    public double? DistanceKm { get; set; }
    public decimal? Budget { get; set; }
    public bool IsDraft { get; set; }
    public bool IsCancelled { get; set; }
    public bool HasStarted { get; set; }
    public bool HasRideCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Derives trip status the same way mobile's Trip.status() does: Cancelled/Draft
    /// take priority, a completed ride wins over dates, otherwise dates decide Active vs Planned.</summary>
    public TripStatus Status(DateOnly today)
    {
        if (IsCancelled) return TripStatus.Cancelled;
        if (IsDraft) return TripStatus.Draft;
        if (HasRideCompleted) return TripStatus.Completed;
        if (HasStarted || (today >= StartDate && today <= EndDate)) return TripStatus.Active;
        return TripStatus.Planned;
    }

    public void CompleteRide()
    {
        HasRideCompleted = true;
    }
}
