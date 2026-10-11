using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public record AccessTokenResult(string Token, DateTime ExpiresAt);

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(RiderAccount account);
    string GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
}
