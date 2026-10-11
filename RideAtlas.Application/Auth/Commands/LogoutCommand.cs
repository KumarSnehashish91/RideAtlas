using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest;

public class LogoutCommandHandler(
    IRiderAccountRepository repository,
    IJwtTokenService tokenService) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var stored = await repository.GetRefreshTokenAsync(tokenHash, ct);
        if (stored is not null)
            await repository.RevokeRefreshTokenAsync(stored, ct);
    }
}
