using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.FuelLog;

public record FuelDerivedStats(double? DistanceKm, double? MileageKmPerLitre, double? CostPerKm);

/// <summary>Mileage is only meaningful full-tank to full-tank: it sums every fill in between
/// so partial top-ups don't understate the litres actually burned over that distance.</summary>
public static class FuelMileageCalculator
{
    public static FuelDerivedStats Compute(IReadOnlyList<FuelLogEntry> bikeEntriesOrderedByOdometer, FuelLogEntry target)
    {
        if (!target.IsFullTank)
            return new FuelDerivedStats(null, null, null);

        var ordered = bikeEntriesOrderedByOdometer;
        var previousFullTank = ordered
            .Where(e => e.IsFullTank && e.OdometerKm < target.OdometerKm)
            .OrderByDescending(e => e.OdometerKm)
            .FirstOrDefault();

        if (previousFullTank is null)
            return new FuelDerivedStats(null, null, null);

        var distanceKm = target.OdometerKm - previousFullTank.OdometerKm;
        if (distanceKm <= 0)
            return new FuelDerivedStats(null, null, null);

        var entriesInRange = ordered
            .Where(e => e.OdometerKm > previousFullTank.OdometerKm && e.OdometerKm <= target.OdometerKm)
            .ToList();

        var litresSum = entriesInRange.Sum(e => e.Litres);
        var costSum = entriesInRange.Sum(e => e.Cost);

        double? mileage = litresSum > 0 ? distanceKm / litresSum : null;
        double? costPerKm = distanceKm > 0 ? (double)(costSum / (decimal)distanceKm) : null;

        return new FuelDerivedStats(distanceKm, mileage, costPerKm);
    }
}
