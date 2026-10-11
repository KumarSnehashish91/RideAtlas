using RideAtlas.Application.Auth.Dtos;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Auth;

public static class AuthResultFactory
{
    public static async Task<AuthResultDto> IssueAsync(
        RiderAccount account,
        IRiderAccountRepository repository,
        IJwtTokenService tokenService,
        IDateTimeProvider clock,
        CancellationToken ct)
    {
        var access = tokenService.CreateAccessToken(account);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshExpiresAt = clock.UtcNow.Add(AuthConstants.RefreshTokenLifetime);

        await repository.AddRefreshTokenAsync(new RefreshToken
        {
            RiderAccountId = account.Id,
            TokenHash = tokenService.HashRefreshToken(rawRefreshToken),
            ExpiresAt = refreshExpiresAt
        }, ct);

        return new AuthResultDto(
            new RiderAccountDto(account.Id, account.FullName, account.Email, account.CreatedAt),
            access.Token,
            access.ExpiresAt,
            rawRefreshToken,
            refreshExpiresAt);
    }
}
