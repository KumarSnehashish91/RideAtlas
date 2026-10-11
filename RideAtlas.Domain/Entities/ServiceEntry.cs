namespace RideAtlas.Domain.Entities;

public class ServiceEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public Guid BikeId { get; set; }
    public DateOnly ServiceDate { get; set; }
    public double OdometerKm { get; set; }
    public decimal Cost { get; set; }
    public string? Garage { get; set; }
    public string ServiceTypesCsv { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? WorkshopAddress { get; set; }
    public string? Mechanic { get; set; }
    public string? Description { get; set; }
    public decimal? LabourCost { get; set; }
    public decimal? PartsCost { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoicePhotoUri { get; set; }
    public bool WarrantyClaim { get; set; }
    public DateOnly? ExpectedNextServiceDate { get; set; }
    public double? ExpectedNextServiceOdometerKm { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
