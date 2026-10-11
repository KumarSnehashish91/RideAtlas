using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Packing.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Packing.Queries;

public record ListPackingItemsQuery(Guid RiderAccountId, Guid TripId) : IRequest<IReadOnlyList<PackingItemDto>>;

public class ListPackingItemsQueryHandler(
    IPackingItemRepository repository,
    ITripRepository tripRepository) : IRequestHandler<ListPackingItemsQuery, IReadOnlyList<PackingItemDto>>
{
    public async Task<IReadOnlyList<PackingItemDto>> Handle(ListPackingItemsQuery request, CancellationToken ct)
    {
        _ = await tripRepository.GetByIdAsync(request.TripId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.TripId);

        var items = await repository.ListForTripAsync(request.TripId, request.RiderAccountId, ct);
        return items
            .OrderBy(i => i.Category)
            .ThenBy(i => i.SortOrder)
            .Select(i => i.ToDto())
            .ToList();
    }
}
