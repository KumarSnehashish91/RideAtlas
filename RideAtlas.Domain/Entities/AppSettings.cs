namespace RideAtlas.Domain.Entities;

public class AppSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RiderAccountId { get; set; }
    public string ThemeMode { get; set; } = "System";
    public string DistanceUnit { get; set; } = "km";
    public string FuelUnit { get; set; } = "L";
    public string Currency { get; set; } = "INR";
    public string Language { get; set; } = "English";
    public bool MaintenanceRemindersEnabled { get; set; } = true;
    public string? PreferredMapStyle { get; set; }
    public string? VisibleQuickActionNamesCsv { get; set; }
}
