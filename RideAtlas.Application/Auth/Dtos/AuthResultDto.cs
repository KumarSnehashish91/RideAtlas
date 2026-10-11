namespace RideAtlas.Application.Auth.Dtos;

public record AuthResultDto(
    RiderAccountDto Account,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);
