using FluentValidation;
using MediatR;
using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenCommandHandler(
    IRiderAccountRepository repository,
    IJwtTokenService tokenService,
    IDateTimeProvider clock) : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = tokenService.HashRefreshToken(request.RefreshToken);
        var stored = await repository.GetRefreshTokenAsync(tokenHash, ct);
        if (stored is null || !stored.IsActive(clock.UtcNow))
            throw new AuthenticationFailedException("Refresh token is invalid or expired.");

        var account = await repository.GetByIdAsync(stored.RiderAccountId, ct)
            ?? throw new AuthenticationFailedException("Refresh token is invalid or expired.");

        await repository.RevokeRefreshTokenAsync(stored, ct);

        return await AuthResultFactory.IssueAsync(account, repository, tokenService, clock, ct);
    }
}
