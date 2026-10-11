using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;

namespace RideAtlas.Application.Events.Queries;

public record ListEventInviteesQuery(Guid EventId) : IRequest<IReadOnlyList<EventInviteeDto>>;

public class ListEventInviteesQueryHandler(IEventInviteeRepository repository)
    : IRequestHandler<ListEventInviteesQuery, IReadOnlyList<EventInviteeDto>>
{
    public async Task<IReadOnlyList<EventInviteeDto>> Handle(ListEventInviteesQuery request, CancellationToken ct)
    {
        var invitees = await repository.ListForEventAsync(request.EventId, ct);
        return invitees.Select(i => i.ToDto()).ToList();
    }
}
