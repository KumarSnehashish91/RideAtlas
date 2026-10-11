using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Bikes.Dtos;

public static class BikeMappingExtensions
{
    public static BikeDto ToDto(this Motorcycle bike) => new(
        bike.Id,
        bike.Nickname,
        bike.Brand,
        bike.Model,
        bike.Year,
        bike.EngineCc,
        bike.RegistrationNumber,
        bike.CurrentOdometerKm,
        bike.IsActive,
        bike.IsArchived,
        bike.CreatedAt);
}
