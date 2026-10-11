using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Queries;

public record ListRidingBuddiesQuery(Guid RiderAccountId) : IRequest<IReadOnlyList<RidingBuddyDto>>;

public class ListRidingBuddiesQueryHandler(IRidingBuddyRepository repository)
    : IRequestHandler<ListRidingBuddiesQuery, IReadOnlyList<RidingBuddyDto>>
{
    public async Task<IReadOnlyList<RidingBuddyDto>> Handle(ListRidingBuddiesQuery request, CancellationToken ct)
    {
        var buddies = await repository.ListAsync(request.RiderAccountId, ct);
        return buddies.Select(b => b.ToDto()).ToList();
    }
}
