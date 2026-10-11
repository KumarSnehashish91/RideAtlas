using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Reminders.Dtos;

public static class ReminderRuleMappingExtensions
{
    public static ReminderRuleDto ToDto(this ReminderRule rule) => new(
        rule.Id,
        rule.BikeId,
        rule.Category,
        rule.CustomLabel,
        rule.IntervalKm,
        rule.IntervalDays,
        rule.BaselineOdometerKm,
        rule.BaselineDate,
        rule.Notes,
        rule.CreatedAt);
}
