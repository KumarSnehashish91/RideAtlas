namespace RideAtlas.Application.Bikes.Dtos;

public record BikeDto(
    Guid Id,
    string Nickname,
    string Brand,
    string Model,
    int Year,
    int EngineCc,
    string? RegistrationNumber,
    double CurrentOdometerKm,
    bool IsActive,
    bool IsArchived,
    DateTime CreatedAt);
