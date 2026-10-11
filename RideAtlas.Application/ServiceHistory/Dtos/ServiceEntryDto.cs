namespace RideAtlas.Application.ServiceHistory.Dtos;

public record ServiceEntryDto(
    Guid Id,
    Guid BikeId,
    DateOnly ServiceDate,
    double OdometerKm,
    decimal Cost,
    string? Garage,
    IReadOnlyList<string> ServiceTypes,
    string Category,
    string? WorkshopAddress,
    string? Mechanic,
    string? Description,
    decimal? LabourCost,
    decimal? PartsCost,
    string? InvoiceNumber,
    string? InvoicePhotoUri,
    bool WarrantyClaim,
    DateOnly? ExpectedNextServiceDate,
    double? ExpectedNextServiceOdometerKm,
    DateTime CreatedAt);
