using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.ServiceHistory.Dtos;

namespace RideAtlas.Application.ServiceHistory.Queries;

public record ListServiceEntriesQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    int Page,
    int PageSize) : IRequest<PagedResult<ServiceEntryDto>>;

public class ListServiceEntriesQueryHandler(IServiceEntryRepository repository)
    : IRequestHandler<ListServiceEntriesQuery, PagedResult<ServiceEntryDto>>
{
    public async Task<PagedResult<ServiceEntryDto>> Handle(ListServiceEntriesQuery request, CancellationToken ct)
    {
        var filter = new ServiceEntryListFilter
        {
            BikeId = request.BikeId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.ListAsync(request.RiderAccountId, filter, ct);
        return new PagedResult<ServiceEntryDto>
        {
            Items = result.Items.Select(e => e.ToDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
