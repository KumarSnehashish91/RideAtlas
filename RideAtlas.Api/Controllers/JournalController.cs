using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Journal.Commands;
using RideAtlas.Application.Journal.Dtos;
using RideAtlas.Application.Journal.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateJournalEntryRequest(
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Title,
    DateOnly Date,
    string? Notes,
    string? Mood,
    double? DistanceKm,
    IReadOnlyList<string>? PhotoUris,
    string? RiderInputRaw);

public record UpdateJournalEntryRequest(
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Title,
    DateOnly Date,
    string? Notes,
    string? Mood,
    double? DistanceKm,
    IReadOnlyList<string>? PhotoUris,
    string? RiderInputRaw);

public class JournalController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<JournalEntryDto>>> List(
        [FromQuery] Guid? bikeId,
        [FromQuery] Guid? tripId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new ListJournalEntriesQuery(RiderAccountId, bikeId, tripId, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JournalEntryDto>> Get(Guid id, CancellationToken ct)
    {
        var entry = await Sender.Send(new GetJournalEntryQuery(id, RiderAccountId), ct);
        return Ok(entry);
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntryDto>> Create(CreateJournalEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new CreateJournalEntryCommand(
            RiderAccountId, request.BikeId, request.TripId, request.RideId, request.Title, request.Date,
            request.Notes, request.Mood, request.DistanceKm, request.PhotoUris, request.RiderInputRaw), ct);
        return Ok(entry);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<JournalEntryDto>> Update(Guid id, UpdateJournalEntryRequest request, CancellationToken ct)
    {
        var entry = await Sender.Send(new UpdateJournalEntryCommand(
            id, RiderAccountId, request.BikeId, request.TripId, request.RideId, request.Title, request.Date,
            request.Notes, request.Mood, request.DistanceKm, request.PhotoUris, request.RiderInputRaw), ct);
        return Ok(entry);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteJournalEntryCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/generate-narrative")]
    public async Task<ActionResult<JournalEntryDto>> GenerateNarrative(Guid id, CancellationToken ct)
    {
        var entry = await Sender.Send(new GenerateJournalNarrativeCommand(id, RiderAccountId), ct);
        return Ok(entry);
    }
}
