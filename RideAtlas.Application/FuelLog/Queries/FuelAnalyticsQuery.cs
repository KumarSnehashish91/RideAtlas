using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.FuelLog.Queries;

public record FuelTrendPointDto(DateTime OccurredAt, double OdometerKm, double? MileageKmPerLitre, double? CostPerKm);

public record FuelAnalyticsDto(
    double? AverageMileageKmPerLitre,
    double? AverageCostPerKm,
    double TotalLitres,
    decimal TotalCost,
    IReadOnlyList<FuelTrendPointDto> Trend);

public record FuelAnalyticsQuery(Guid RiderAccountId, Guid BikeId) : IRequest<FuelAnalyticsDto>;

public class FuelAnalyticsQueryHandler(IFuelLogRepository repository)
    : IRequestHandler<FuelAnalyticsQuery, FuelAnalyticsDto>
{
    public async Task<FuelAnalyticsDto> Handle(FuelAnalyticsQuery request, CancellationToken ct)
    {
        var entries = await repository.ListAllForBikeAsync(request.RiderAccountId, request.BikeId, ct);

        var trend = entries
            .Select(e => (Entry: e, Derived: FuelMileageCalculator.Compute(entries, e)))
            .Where(x => x.Derived.MileageKmPerLitre.HasValue)
            .Select(x => new FuelTrendPointDto(x.Entry.OccurredAt, x.Entry.OdometerKm, x.Derived.MileageKmPerLitre, x.Derived.CostPerKm))
            .OrderBy(x => x.OccurredAt)
            .ToList();

        var mileageValues = trend.Where(x => x.MileageKmPerLitre.HasValue).Select(x => x.MileageKmPerLitre!.Value).ToList();
        var costValues = trend.Where(x => x.CostPerKm.HasValue).Select(x => x.CostPerKm!.Value).ToList();

        return new FuelAnalyticsDto(
            mileageValues.Count > 0 ? mileageValues.Average() : null,
            costValues.Count > 0 ? costValues.Average() : null,
            entries.Sum(e => e.Litres),
            entries.Sum(e => e.Cost),
            trend);
    }
}
