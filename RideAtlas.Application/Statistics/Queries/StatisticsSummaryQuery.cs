using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Statistics.Queries;

public record StatisticsSummaryDto(
    int TotalRides,
    int TotalTrips,
    double TotalDistanceKm,
    double AverageRideDistanceKm,
    double LongestRideKm,
    decimal TotalFuelSpend,
    decimal TotalServiceSpend,
    decimal TotalExpenseSpend,
    decimal TotalSpend);

public record StatisticsSummaryQuery(Guid RiderAccountId) : IRequest<StatisticsSummaryDto>;

public class StatisticsSummaryQueryHandler(
    IRideRepository rideRepository,
    ITripRepository tripRepository,
    IFuelLogRepository fuelLogRepository,
    IServiceEntryRepository serviceEntryRepository,
    IExpenseRepository expenseRepository) : IRequestHandler<StatisticsSummaryQuery, StatisticsSummaryDto>
{
    public async Task<StatisticsSummaryDto> Handle(StatisticsSummaryQuery request, CancellationToken ct)
    {
        var rides = await rideRepository.ListAllAsync(request.RiderAccountId, ct);
        var trips = await tripRepository.ListAsync(request.RiderAccountId, ct);
        var fuelEntries = await fuelLogRepository.ListAllForRiderAsync(request.RiderAccountId, ct);
        var serviceEntries = await serviceEntryRepository.ListAllForRiderAsync(request.RiderAccountId, ct);
        var expenses = await expenseRepository.ListAllAsync(request.RiderAccountId, ct);

        var totalDistance = rides.Sum(r => r.DistanceKm);
        var fuelSpend = fuelEntries.Sum(f => f.Cost);
        var serviceSpend = serviceEntries.Sum(s => s.Cost);
        var expenseSpend = expenses.Sum(e => e.Amount);

        return new StatisticsSummaryDto(
            TotalRides: rides.Count,
            TotalTrips: trips.Count,
            TotalDistanceKm: totalDistance,
            AverageRideDistanceKm: rides.Count > 0 ? totalDistance / rides.Count : 0,
            LongestRideKm: rides.Count > 0 ? rides.Max(r => r.DistanceKm) : 0,
            TotalFuelSpend: fuelSpend,
            TotalServiceSpend: serviceSpend,
            TotalExpenseSpend: expenseSpend,
            TotalSpend: fuelSpend + serviceSpend + expenseSpend);
    }
}
