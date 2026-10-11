using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Trips.Dtos;

namespace RideAtlas.Application.Trips.Queries;

public record ListTripsQuery(Guid RiderAccountId) : IRequest<IReadOnlyList<TripDto>>;

public class ListTripsQueryHandler(ITripRepository repository, IDateTimeProvider clock)
    : IRequestHandler<ListTripsQuery, IReadOnlyList<TripDto>>
{
    public async Task<IReadOnlyList<TripDto>> Handle(ListTripsQuery request, CancellationToken ct)
    {
        var trips = await repository.ListAsync(request.RiderAccountId, ct);
        return trips.Select(t => t.ToDto(clock.Today)).ToList();
    }
}
