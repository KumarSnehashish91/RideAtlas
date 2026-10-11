namespace RideAtlas.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid? BikeId { get; set; }
    public Guid? TripId { get; set; }
    public Guid? RideId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Vendor { get; set; }
    public string? Location { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptPhotoUri { get; set; }
    public double? OdometerKm { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
