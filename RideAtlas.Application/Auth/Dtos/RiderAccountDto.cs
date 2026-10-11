namespace RideAtlas.Application.Auth.Dtos;

public record RiderAccountDto(Guid Id, string FullName, string Email, DateTime CreatedAt);
