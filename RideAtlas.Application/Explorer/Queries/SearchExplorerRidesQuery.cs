using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Explorer.Dtos;

namespace RideAtlas.Application.Explorer.Queries;

public record SearchExplorerRidesQuery(
    string? SearchText,
    string? Country,
    string? State,
    string? Difficulty,
    string? RoadType,
    double? MinScenicRating,
    int Page,
    int PageSize) : IRequest<PagedResult<ExplorerRideDto>>;

public class SearchExplorerRidesQueryHandler(IExplorerRideRepository repository)
    : IRequestHandler<SearchExplorerRidesQuery, PagedResult<ExplorerRideDto>>
{
    public async Task<PagedResult<ExplorerRideDto>> Handle(SearchExplorerRidesQuery request, CancellationToken ct)
    {
        var filter = new ExplorerRideSearchFilter
        {
            SearchText = request.SearchText,
            Country = request.Country,
            State = request.State,
            Difficulty = request.Difficulty,
            RoadType = request.RoadType,
            MinScenicRating = request.MinScenicRating,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.SearchAsync(filter, ct);
        return new PagedResult<ExplorerRideDto>
        {
            Items = result.Items.Select(r => r.ToDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
