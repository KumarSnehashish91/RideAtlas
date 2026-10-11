using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;

namespace RideAtlas.Application.Events.Queries;

public record ListRideEventsQuery(Guid RiderAccountId) : IRequest<IReadOnlyList<RideEventDto>>;

public class ListRideEventsQueryHandler(IRideEventRepository repository)
    : IRequestHandler<ListRideEventsQuery, IReadOnlyList<RideEventDto>>
{
    public async Task<IReadOnlyList<RideEventDto>> Handle(ListRideEventsQuery request, CancellationToken ct)
    {
        var events = await repository.ListAsync(request.RiderAccountId, ct);
        return events.Select(e => e.ToDto()).ToList();
    }
}
