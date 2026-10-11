using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Packing.Commands;
using RideAtlas.Application.Packing.Dtos;
using RideAtlas.Application.Packing.Queries;

namespace RideAtlas.Api.Controllers;

public record CreatePackingItemRequest(Guid TripId, string Category, string Name, int SortOrder);
public record UpdatePackingItemRequest(string Category, string Name, bool IsPacked, int SortOrder);
public record ApplyPackingTemplateRequest(Guid TripId, string TemplateName);

public class PackingController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("trip/{tripId:guid}")]
    public async Task<ActionResult<IReadOnlyList<PackingItemDto>>> ListForTrip(Guid tripId, CancellationToken ct)
    {
        var items = await Sender.Send(new ListPackingItemsQuery(RiderAccountId, tripId), ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<PackingItemDto>> Create(CreatePackingItemRequest request, CancellationToken ct)
    {
        var item = await Sender.Send(new CreatePackingItemCommand(
            RiderAccountId, request.TripId, request.Category, request.Name, request.SortOrder), ct);
        return Ok(item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PackingItemDto>> Update(Guid id, UpdatePackingItemRequest request, CancellationToken ct)
    {
        var item = await Sender.Send(new UpdatePackingItemCommand(
            id, RiderAccountId, request.Category, request.Name, request.IsPacked, request.SortOrder), ct);
        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeletePackingItemCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("apply-template")]
    public async Task<ActionResult<IReadOnlyList<PackingItemDto>>> ApplyTemplate(ApplyPackingTemplateRequest request, CancellationToken ct)
    {
        var items = await Sender.Send(new ApplyPackingTemplateCommand(RiderAccountId, request.TripId, request.TemplateName), ct);
        return Ok(items);
    }
}
