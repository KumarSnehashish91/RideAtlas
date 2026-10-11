using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Queries;

public record GetMyProfileQuery(Guid RiderAccountId) : IRequest<RiderProfileDto>;

public class GetMyProfileQueryHandler(IRiderProfileRepository repository, IRiderAccountRepository riderAccountRepository)
    : IRequestHandler<GetMyProfileQuery, RiderProfileDto>
{
    public async Task<RiderProfileDto> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var profile = await repository.GetByRiderAccountIdAsync(request.RiderAccountId, ct);
        if (profile is not null)
            return profile.ToDto();

        var account = await riderAccountRepository.GetByIdAsync(request.RiderAccountId, ct);
        return new RiderProfile
        {
            RiderAccountId = request.RiderAccountId,
            DisplayName = account?.FullName ?? "Rider"
        }.ToDto();
    }
}
