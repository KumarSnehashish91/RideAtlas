namespace RideAtlas.Application.Settings.Dtos;

public record AppSettingsDto(
    string RiderName,
    string ThemeMode,
    string DistanceUnit,
    string FuelUnit,
    string Currency,
    string Language,
    bool MaintenanceRemindersEnabled,
    string? PreferredMapStyle,
    IReadOnlyList<string> VisibleQuickActionNames);
