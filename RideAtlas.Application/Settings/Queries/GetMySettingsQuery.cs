using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Settings.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Settings.Queries;

public record GetMySettingsQuery(Guid RiderAccountId) : IRequest<AppSettingsDto>;

public class GetMySettingsQueryHandler(IAppSettingsRepository repository, IRiderAccountRepository riderAccountRepository)
    : IRequestHandler<GetMySettingsQuery, AppSettingsDto>
{
    public async Task<AppSettingsDto> Handle(GetMySettingsQuery request, CancellationToken ct)
    {
        var account = await riderAccountRepository.GetByIdAsync(request.RiderAccountId, ct);
        var riderName = account?.FullName ?? "Rider";

        var settings = await repository.GetByRiderAccountIdAsync(request.RiderAccountId, ct);
        return (settings ?? new AppSettings { RiderAccountId = request.RiderAccountId }).ToDto(riderName);
    }
}
