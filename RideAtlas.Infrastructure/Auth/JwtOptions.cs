namespace RideAtlas.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "RideAtlas";
    public string Audience { get; set; } = "RideAtlas";
    public int AccessTokenLifetimeMinutes { get; set; } = 15;
}
