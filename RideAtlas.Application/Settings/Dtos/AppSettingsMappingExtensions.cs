using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Settings.Dtos;

public static class AppSettingsMappingExtensions
{
    public static AppSettingsDto ToDto(this AppSettings settings, string riderName) => new(
        riderName,
        settings.ThemeMode,
        settings.DistanceUnit,
        settings.FuelUnit,
        settings.Currency,
        settings.Language,
        settings.MaintenanceRemindersEnabled,
        settings.PreferredMapStyle,
        SplitQuickActions(settings.VisibleQuickActionNamesCsv));

    public static string? JoinQuickActions(IReadOnlyList<string>? names) =>
        names is null || names.Count == 0 ? null : string.Join(',', names);

    private static IReadOnlyList<string> SplitQuickActions(string? csv) =>
        string.IsNullOrWhiteSpace(csv) ? [] : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
