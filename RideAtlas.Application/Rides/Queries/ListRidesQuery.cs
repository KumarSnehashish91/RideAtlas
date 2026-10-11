using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Rides.Dtos;

namespace RideAtlas.Application.Rides.Queries;

public record ListRidesQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    Guid? TripId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    int Page,
    int PageSize) : IRequest<PagedResult<RideDto>>;

public class ListRidesQueryHandler(IRideRepository repository)
    : IRequestHandler<ListRidesQuery, PagedResult<RideDto>>
{
    public async Task<PagedResult<RideDto>> Handle(ListRidesQuery request, CancellationToken ct)
    {
        var filter = new RideListFilter
        {
            BikeId = request.BikeId,
            TripId = request.TripId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.ListAsync(request.RiderAccountId, filter, ct);
        return new PagedResult<RideDto>
        {
            Items = result.Items.Select(r => r.ToDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
