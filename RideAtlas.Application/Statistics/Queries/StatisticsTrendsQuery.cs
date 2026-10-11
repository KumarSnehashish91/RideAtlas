using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Statistics.Queries;

public record MonthlyTrendPointDto(string Month, double DistanceKm, decimal Spend);

public record StatisticsTrendsQuery(Guid RiderAccountId, int Months) : IRequest<IReadOnlyList<MonthlyTrendPointDto>>;

public class StatisticsTrendsQueryHandler(
    IRideRepository rideRepository,
    IFuelLogRepository fuelLogRepository,
    IServiceEntryRepository serviceEntryRepository,
    IExpenseRepository expenseRepository,
    IDateTimeProvider clock) : IRequestHandler<StatisticsTrendsQuery, IReadOnlyList<MonthlyTrendPointDto>>
{
    public async Task<IReadOnlyList<MonthlyTrendPointDto>> Handle(StatisticsTrendsQuery request, CancellationToken ct)
    {
        var months = request.Months <= 0 ? 6 : request.Months;
        var cutoff = clock.Today.AddMonths(-months + 1);
        var cutoffStart = new DateTime(cutoff.Year, cutoff.Month, 1);

        var rides = await rideRepository.ListAllAsync(request.RiderAccountId, ct);
        var fuelEntries = await fuelLogRepository.ListAllForRiderAsync(request.RiderAccountId, ct);
        var serviceEntries = await serviceEntryRepository.ListAllForRiderAsync(request.RiderAccountId, ct);
        var expenses = await expenseRepository.ListAllAsync(request.RiderAccountId, ct);

        var buckets = Enumerable.Range(0, months)
            .Select(i => cutoffStart.AddMonths(i))
            .ToDictionary(d => $"{d:yyyy-MM}", _ => (Distance: 0.0, Spend: 0m));

        foreach (var ride in rides.Where(r => r.StartedAt >= cutoffStart))
        {
            var key = $"{ride.StartedAt:yyyy-MM}";
            if (buckets.TryGetValue(key, out var value))
                buckets[key] = (value.Distance + ride.DistanceKm, value.Spend);
        }
        foreach (var fuel in fuelEntries.Where(f => f.OccurredAt >= cutoffStart))
        {
            var key = $"{fuel.OccurredAt:yyyy-MM}";
            if (buckets.TryGetValue(key, out var value))
                buckets[key] = (value.Distance, value.Spend + fuel.Cost);
        }
        foreach (var service in serviceEntries.Where(s => s.ServiceDate.ToDateTime(TimeOnly.MinValue) >= cutoffStart))
        {
            var key = $"{service.ServiceDate:yyyy-MM}";
            if (buckets.TryGetValue(key, out var value))
                buckets[key] = (value.Distance, value.Spend + service.Cost);
        }
        foreach (var expense in expenses.Where(e => e.Date.ToDateTime(TimeOnly.MinValue) >= cutoffStart))
        {
            var key = $"{expense.Date:yyyy-MM}";
            if (buckets.TryGetValue(key, out var value))
                buckets[key] = (value.Distance, value.Spend + expense.Amount);
        }

        return buckets
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new MonthlyTrendPointDto(kvp.Key, kvp.Value.Distance, kvp.Value.Spend))
            .ToList();
    }
}
