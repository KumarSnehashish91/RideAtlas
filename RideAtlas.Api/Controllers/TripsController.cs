using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Trips.Commands;
using RideAtlas.Application.Trips.Dtos;
using RideAtlas.Application.Trips.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateTripRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BikeId,
    double? DistanceKm,
    decimal? Budget,
    bool IsDraft);

public record UpdateTripRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BikeId,
    double? DistanceKm,
    decimal? Budget,
    bool IsDraft,
    bool IsCancelled);

public class TripsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TripDto>>> List(CancellationToken ct)
    {
        var trips = await Sender.Send(new ListTripsQuery(RiderAccountId), ct);
        return Ok(trips);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TripDto>> Get(Guid id, CancellationToken ct)
    {
        var trip = await Sender.Send(new GetTripQuery(id, RiderAccountId), ct);
        return Ok(trip);
    }

    [HttpPost]
    public async Task<ActionResult<TripDto>> Create(CreateTripRequest request, CancellationToken ct)
    {
        var trip = await Sender.Send(new CreateTripCommand(
            RiderAccountId, request.Name, request.StartDate, request.EndDate,
            request.BikeId, request.DistanceKm, request.Budget, request.IsDraft), ct);
        return Ok(trip);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TripDto>> Update(Guid id, UpdateTripRequest request, CancellationToken ct)
    {
        var trip = await Sender.Send(new UpdateTripCommand(
            id, RiderAccountId, request.Name, request.StartDate, request.EndDate,
            request.BikeId, request.DistanceKm, request.Budget, request.IsDraft, request.IsCancelled), ct);
        return Ok(trip);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteTripCommand(id, RiderAccountId), ct);
        return NoContent();
    }
}
