using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Rides.Commands;
using RideAtlas.Application.Rides.Dtos;
using RideAtlas.Application.Rides.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateRideRequest(
    Guid BikeId,
    Guid? TripId,
    DateTime StartedAt,
    DateTime EndedAt,
    double DistanceKm,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    double? WeatherTempC,
    string? Notes);

public record UpdateRideRequest(
    DateTime StartedAt,
    DateTime EndedAt,
    double DistanceKm,
    double? AvgSpeedKmh,
    double? MaxSpeedKmh,
    string? RouteDescription,
    string? WeatherCondition,
    double? WeatherTempC,
    string? Notes);

public record LinkRideToTripRequest(Guid TripId);

public class RidesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<RideDto>>> List(
        [FromQuery] Guid? bikeId,
        [FromQuery] Guid? tripId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(
            new ListRidesQuery(RiderAccountId, bikeId, tripId, fromDate, toDate, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RideDto>> Get(Guid id, CancellationToken ct)
    {
        var ride = await Sender.Send(new GetRideQuery(id, RiderAccountId), ct);
        return Ok(ride);
    }

    [HttpPost]
    public async Task<ActionResult<RideDto>> Create(CreateRideRequest request, CancellationToken ct)
    {
        var ride = await Sender.Send(new CreateRideCommand(
            RiderAccountId, request.BikeId, request.TripId, request.StartedAt, request.EndedAt,
            request.DistanceKm, request.AvgSpeedKmh, request.MaxSpeedKmh,
            request.RouteDescription, request.WeatherCondition, request.WeatherTempC, request.Notes), ct);
        return Ok(ride);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RideDto>> Update(Guid id, UpdateRideRequest request, CancellationToken ct)
    {
        var ride = await Sender.Send(new UpdateRideCommand(
            id, RiderAccountId, request.StartedAt, request.EndedAt, request.DistanceKm,
            request.AvgSpeedKmh, request.MaxSpeedKmh, request.RouteDescription,
            request.WeatherCondition, request.WeatherTempC, request.Notes), ct);
        return Ok(ride);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteRideCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/link-trip")]
    public async Task<ActionResult<RideDto>> LinkTrip(Guid id, LinkRideToTripRequest request, CancellationToken ct)
    {
        var ride = await Sender.Send(new LinkRideToTripCommand(id, request.TripId, RiderAccountId), ct);
        return Ok(ride);
    }
}
