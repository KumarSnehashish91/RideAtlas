namespace RideAtlas.Application.FuelLog.Dtos;

public record FuelLogEntryDto(
    Guid Id,
    Guid BikeId,
    Guid? RideId,
    DateTime OccurredAt,
    double OdometerKm,
    double Litres,
    decimal PricePerLitre,
    decimal Cost,
    bool IsFullTank,
    string? FuelType,
    string? Station,
    string? StationLocation,
    string? PaymentMethod,
    string? ReceiptPhotoUri,
    string? Notes,
    double? DistanceKm,
    double? MileageKmPerLitre,
    double? CostPerKm,
    DateTime CreatedAt);
