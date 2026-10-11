using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.FuelLog.Dtos;

namespace RideAtlas.Application.FuelLog.Queries;

public record ListFuelLogEntriesQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    DateOnly? FromDate,
    DateOnly? ToDate,
    int Page,
    int PageSize) : IRequest<PagedResult<FuelLogEntryDto>>;

public class ListFuelLogEntriesQueryHandler(IFuelLogRepository repository)
    : IRequestHandler<ListFuelLogEntriesQuery, PagedResult<FuelLogEntryDto>>
{
    public async Task<PagedResult<FuelLogEntryDto>> Handle(ListFuelLogEntriesQuery request, CancellationToken ct)
    {
        var filter = new FuelLogListFilter
        {
            BikeId = request.BikeId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.ListAsync(request.RiderAccountId, filter, ct);

        var byBike = new Dictionary<Guid, IReadOnlyList<Domain.Entities.FuelLogEntry>>();
        var dtos = new List<FuelLogEntryDto>(result.Items.Count);
        foreach (var entry in result.Items)
        {
            if (!byBike.TryGetValue(entry.BikeId, out var bikeEntries))
            {
                bikeEntries = await repository.ListAllForBikeAsync(request.RiderAccountId, entry.BikeId, ct);
                byBike[entry.BikeId] = bikeEntries;
            }
            dtos.Add(entry.ToDto(FuelMileageCalculator.Compute(bikeEntries, entry)));
        }

        return new PagedResult<FuelLogEntryDto>
        {
            Items = dtos,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
