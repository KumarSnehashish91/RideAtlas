using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.FuelLog.Dtos;

public static class FuelLogMappingExtensions
{
    public static FuelLogEntryDto ToDto(this FuelLogEntry entry, FuelDerivedStats derived) => new(
        entry.Id,
        entry.BikeId,
        entry.RideId,
        entry.OccurredAt,
        entry.OdometerKm,
        entry.Litres,
        entry.PricePerLitre,
        entry.Cost,
        entry.IsFullTank,
        entry.FuelType,
        entry.Station,
        entry.StationLocation,
        entry.PaymentMethod,
        entry.ReceiptPhotoUri,
        entry.Notes,
        derived.DistanceKm,
        derived.MileageKmPerLitre,
        derived.CostPerKm,
        entry.CreatedAt);
}
