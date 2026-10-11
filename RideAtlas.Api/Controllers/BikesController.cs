using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Bikes.Commands;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Bikes.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateBikeRequest(
    string Nickname,
    string Brand,
    string Model,
    int Year,
    int EngineCc,
    string? RegistrationNumber,
    double CurrentOdometerKm,
    bool IsActive);

public record UpdateBikeRequest(
    string Nickname,
    string Brand,
    string Model,
    int Year,
    int EngineCc,
    string? RegistrationNumber,
    double CurrentOdometerKm,
    bool IsArchived);

public class BikesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BikeDto>>> List(CancellationToken ct)
    {
        var bikes = await Sender.Send(new ListBikesQuery(RiderAccountId), ct);
        return Ok(bikes);
    }

    [HttpPost]
    public async Task<ActionResult<BikeDto>> Create(CreateBikeRequest request, CancellationToken ct)
    {
        var bike = await Sender.Send(new CreateBikeCommand(
            RiderAccountId, request.Nickname, request.Brand, request.Model, request.Year,
            request.EngineCc, request.RegistrationNumber, request.CurrentOdometerKm, request.IsActive), ct);
        return Ok(bike);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BikeDto>> Update(Guid id, UpdateBikeRequest request, CancellationToken ct)
    {
        var bike = await Sender.Send(new UpdateBikeCommand(
            id, RiderAccountId, request.Nickname, request.Brand, request.Model, request.Year,
            request.EngineCc, request.RegistrationNumber, request.CurrentOdometerKm, request.IsArchived), ct);
        return Ok(bike);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteBikeCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/set-active")]
    public async Task<ActionResult<BikeDto>> SetActive(Guid id, CancellationToken ct)
    {
        var bike = await Sender.Send(new SetActiveBikeCommand(id, RiderAccountId), ct);
        return Ok(bike);
    }
}
