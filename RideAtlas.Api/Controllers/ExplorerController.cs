using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Explorer.Dtos;
using RideAtlas.Application.Explorer.Queries;

namespace RideAtlas.Api.Controllers;

public class ExplorerController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExplorerRideDto>>> Search(
        [FromQuery] string? searchText,
        [FromQuery] string? country,
        [FromQuery] string? state,
        [FromQuery] string? difficulty,
        [FromQuery] string? roadType,
        [FromQuery] double? minScenicRating,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new SearchExplorerRidesQuery(
            searchText, country, state, difficulty, roadType, minScenicRating, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExplorerRideDto>> Get(Guid id, CancellationToken ct)
    {
        var ride = await Sender.Send(new GetExplorerRideQuery(id), ct);
        return Ok(ride);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<ExplorerRideDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var ride = await Sender.Send(new GetExplorerRideBySlugQuery(slug), ct);
        return Ok(ride);
    }
}
