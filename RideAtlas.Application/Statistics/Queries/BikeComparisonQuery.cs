using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.FuelLog;

namespace RideAtlas.Application.Statistics.Queries;

public record BikeComparisonDto(
    Guid BikeId,
    string Nickname,
    double TotalDistanceKm,
    int RideCount,
    double? AverageMileageKmPerLitre,
    decimal TotalSpend,
    int ServiceCount);

public record BikeComparisonQuery(Guid RiderAccountId) : IRequest<IReadOnlyList<BikeComparisonDto>>;

public class BikeComparisonQueryHandler(
    IBikeRepository bikeRepository,
    IRideRepository rideRepository,
    IFuelLogRepository fuelLogRepository,
    IServiceEntryRepository serviceEntryRepository,
    IExpenseRepository expenseRepository) : IRequestHandler<BikeComparisonQuery, IReadOnlyList<BikeComparisonDto>>
{
    public async Task<IReadOnlyList<BikeComparisonDto>> Handle(BikeComparisonQuery request, CancellationToken ct)
    {
        var bikes = await bikeRepository.ListAsync(request.RiderAccountId, ct);
        var rides = await rideRepository.ListAllAsync(request.RiderAccountId, ct);
        var serviceEntries = await serviceEntryRepository.ListAllForRiderAsync(request.RiderAccountId, ct);
        var expenses = await expenseRepository.ListAllAsync(request.RiderAccountId, ct);

        var result = new List<BikeComparisonDto>(bikes.Count);
        foreach (var bike in bikes)
        {
            var bikeRides = rides.Where(r => r.BikeId == bike.Id).ToList();
            var fuelEntries = await fuelLogRepository.ListAllForBikeAsync(request.RiderAccountId, bike.Id, ct);
            var bikeServices = serviceEntries.Where(s => s.BikeId == bike.Id).ToList();
            var bikeExpenses = expenses.Where(e => e.BikeId == bike.Id).ToList();

            var mileages = fuelEntries
                .Select(f => FuelMileageCalculator.Compute(fuelEntries, f).MileageKmPerLitre)
                .Where(m => m.HasValue)
                .Select(m => m!.Value)
                .ToList();

            var totalSpend = fuelEntries.Sum(f => f.Cost) + bikeServices.Sum(s => s.Cost) + bikeExpenses.Sum(e => e.Amount);

            result.Add(new BikeComparisonDto(
                bike.Id,
                bike.Nickname,
                bikeRides.Sum(r => r.DistanceKm),
                bikeRides.Count,
                mileages.Count > 0 ? mileages.Average() : null,
                totalSpend,
                bikeServices.Count));
        }

        return result;
    }
}
