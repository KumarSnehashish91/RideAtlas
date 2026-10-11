using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Reminders.Commands;
using RideAtlas.Application.Reminders.Dtos;
using RideAtlas.Application.Reminders.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateReminderRuleRequest(
    Guid BikeId,
    string Category,
    string? CustomLabel,
    double? IntervalKm,
    int? IntervalDays,
    double BaselineOdometerKm,
    DateOnly BaselineDate,
    string? Notes);

public record UpdateReminderRuleRequest(
    string Category,
    string? CustomLabel,
    double? IntervalKm,
    int? IntervalDays,
    double BaselineOdometerKm,
    DateOnly BaselineDate,
    string? Notes);

public class RemindersController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("rules")]
    public async Task<ActionResult<IReadOnlyList<ReminderRuleDto>>> ListRules([FromQuery] Guid bikeId, CancellationToken ct)
    {
        var rules = await Sender.Send(new ListReminderRulesQuery(RiderAccountId, bikeId), ct);
        return Ok(rules);
    }

    [HttpPost("rules")]
    public async Task<ActionResult<ReminderRuleDto>> CreateRule(CreateReminderRuleRequest request, CancellationToken ct)
    {
        var rule = await Sender.Send(new CreateReminderRuleCommand(
            RiderAccountId, request.BikeId, request.Category, request.CustomLabel, request.IntervalKm,
            request.IntervalDays, request.BaselineOdometerKm, request.BaselineDate, request.Notes), ct);
        return Ok(rule);
    }

    [HttpPut("rules/{id:guid}")]
    public async Task<ActionResult<ReminderRuleDto>> UpdateRule(Guid id, UpdateReminderRuleRequest request, CancellationToken ct)
    {
        var rule = await Sender.Send(new UpdateReminderRuleCommand(
            id, RiderAccountId, request.Category, request.CustomLabel, request.IntervalKm,
            request.IntervalDays, request.BaselineOdometerKm, request.BaselineDate, request.Notes), ct);
        return Ok(rule);
    }

    [HttpDelete("rules/{id:guid}")]
    public async Task<IActionResult> DeleteRule(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteReminderRuleCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("rules/{id:guid}/mark-done")]
    public async Task<ActionResult<ReminderRuleDto>> MarkDone(Guid id, CancellationToken ct)
    {
        var rule = await Sender.Send(new MarkReminderRuleDoneCommand(id, RiderAccountId), ct);
        return Ok(rule);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MaintenanceReminderDto>>> Computed([FromQuery] Guid? bikeId, CancellationToken ct)
    {
        var reminders = await Sender.Send(new ComputedRemindersQuery(RiderAccountId, bikeId), ct);
        return Ok(reminders);
    }
}
