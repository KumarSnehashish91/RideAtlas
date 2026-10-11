namespace RideAtlas.Domain.Entities;

public class FuelLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid BikeId { get; set; }
    public Guid? RideId { get; set; }
    public DateTime OccurredAt { get; set; }
    public double OdometerKm { get; set; }
    public double Litres { get; set; }
    public decimal PricePerLitre { get; set; }
    public decimal Cost { get; set; }
    public bool IsFullTank { get; set; }
    public string? FuelType { get; set; }
    public string? Station { get; set; }
    public string? StationLocation { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ReceiptPhotoUri { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
