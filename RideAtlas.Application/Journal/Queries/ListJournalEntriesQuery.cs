using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Journal.Dtos;

namespace RideAtlas.Application.Journal.Queries;

public record ListJournalEntriesQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    Guid? TripId,
    int Page,
    int PageSize) : IRequest<PagedResult<JournalEntryDto>>;

public class ListJournalEntriesQueryHandler(IJournalEntryRepository repository)
    : IRequestHandler<ListJournalEntriesQuery, PagedResult<JournalEntryDto>>
{
    public async Task<PagedResult<JournalEntryDto>> Handle(ListJournalEntriesQuery request, CancellationToken ct)
    {
        var filter = new JournalEntryListFilter
        {
            BikeId = request.BikeId,
            TripId = request.TripId,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.ListAsync(request.RiderAccountId, filter, ct);
        return new PagedResult<JournalEntryDto>
        {
            Items = result.Items.Select(e => e.ToDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
