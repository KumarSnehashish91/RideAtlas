using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.ServiceHistory.Dtos;

public static class ServiceEntryMappingExtensions
{
    public static ServiceEntryDto ToDto(this ServiceEntry entry) => new(
        entry.Id,
        entry.BikeId,
        entry.ServiceDate,
        entry.OdometerKm,
        entry.Cost,
        entry.Garage,
        SplitServiceTypes(entry.ServiceTypesCsv),
        entry.Category,
        entry.WorkshopAddress,
        entry.Mechanic,
        entry.Description,
        entry.LabourCost,
        entry.PartsCost,
        entry.InvoiceNumber,
        entry.InvoicePhotoUri,
        entry.WarrantyClaim,
        entry.ExpectedNextServiceDate,
        entry.ExpectedNextServiceOdometerKm,
        entry.CreatedAt);

    public static string JoinServiceTypes(IReadOnlyList<string>? serviceTypes) =>
        serviceTypes is null ? string.Empty : string.Join(',', serviceTypes.Select(t => t.Trim()).Where(t => t.Length > 0));

    private static IReadOnlyList<string> SplitServiceTypes(string csv) =>
        string.IsNullOrWhiteSpace(csv) ? [] : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
