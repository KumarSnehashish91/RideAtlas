using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Settings.Commands;
using RideAtlas.Application.Settings.Dtos;
using RideAtlas.Application.Settings.Queries;

namespace RideAtlas.Api.Controllers;

public record UpdateAppSettingsRequest(
    string ThemeMode,
    string DistanceUnit,
    string FuelUnit,
    string Currency,
    string Language,
    bool MaintenanceRemindersEnabled,
    string? PreferredMapStyle,
    IReadOnlyList<string>? VisibleQuickActionNames);

public class SettingsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<AppSettingsDto>> Get(CancellationToken ct)
    {
        var settings = await Sender.Send(new GetMySettingsQuery(RiderAccountId), ct);
        return Ok(settings);
    }

    [HttpPut]
    public async Task<ActionResult<AppSettingsDto>> Update(UpdateAppSettingsRequest request, CancellationToken ct)
    {
        var settings = await Sender.Send(new UpdateMySettingsCommand(
            RiderAccountId, request.ThemeMode, request.DistanceUnit, request.FuelUnit, request.Currency,
            request.Language, request.MaintenanceRemindersEnabled, request.PreferredMapStyle,
            request.VisibleQuickActionNames), ct);
        return Ok(settings);
    }
}
