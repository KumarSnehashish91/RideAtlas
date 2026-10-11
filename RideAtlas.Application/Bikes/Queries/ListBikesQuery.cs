using MediatR;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Bikes.Queries;

public record ListBikesQuery(Guid RiderAccountId) : IRequest<IReadOnlyList<BikeDto>>;

public class ListBikesQueryHandler(IBikeRepository repository)
    : IRequestHandler<ListBikesQuery, IReadOnlyList<BikeDto>>
{
    public async Task<IReadOnlyList<BikeDto>> Handle(ListBikesQuery request, CancellationToken ct)
    {
        var bikes = await repository.ListAsync(request.RiderAccountId, ct);
        return bikes.Select(b => b.ToDto()).ToList();
    }
}
