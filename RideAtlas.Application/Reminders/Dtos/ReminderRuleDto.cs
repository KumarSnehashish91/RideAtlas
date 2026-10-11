namespace RideAtlas.Application.Reminders.Dtos;

public record ReminderRuleDto(
    Guid Id,
    Guid BikeId,
    string Category,
    string? CustomLabel,
    double? IntervalKm,
    int? IntervalDays,
    double BaselineOdometerKm,
    DateOnly BaselineDate,
    string? Notes,
    DateTime CreatedAt);
