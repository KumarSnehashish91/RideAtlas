using MediatR;
using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Auth.Queries;

public record MeQuery(Guid RiderAccountId) : IRequest<RiderAccountDto>;

public class MeQueryHandler(IRiderAccountRepository repository) : IRequestHandler<MeQuery, RiderAccountDto>
{
    public async Task<RiderAccountDto> Handle(MeQuery request, CancellationToken ct)
    {
        var account = await repository.GetByIdAsync(request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RiderAccount), request.RiderAccountId);
        return new RiderAccountDto(account.Id, account.FullName, account.Email, account.CreatedAt);
    }
}
