using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class FuelLogRepository(RideAtlasDbContext db) : IFuelLogRepository
{
    public Task<FuelLogEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.FuelLogEntries.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<PagedResult<FuelLogEntry>> ListAsync(Guid riderAccountId, FuelLogListFilter filter, CancellationToken ct = default)
    {
        var query = db.FuelLogEntries.Where(x => x.RiderAccountId == riderAccountId);

        if (filter.BikeId is Guid bikeId)
            query = query.Where(x => x.BikeId == bikeId);
        if (filter.FromDate is DateOnly fromDate)
        {
            var from = fromDate.ToDateTime(TimeOnly.MinValue);
            query = query.Where(x => x.OccurredAt >= from);
        }
        if (filter.ToDate is DateOnly toDate)
        {
            var to = toDate.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(x => x.OccurredAt <= to);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<FuelLogEntry>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<FuelLogEntry>> ListAllForBikeAsync(Guid riderAccountId, Guid bikeId, CancellationToken ct = default) =>
        await db.FuelLogEntries
            .Where(x => x.RiderAccountId == riderAccountId && x.BikeId == bikeId)
            .OrderBy(x => x.OdometerKm)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<FuelLogEntry>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.FuelLogEntries.Where(x => x.RiderAccountId == riderAccountId).ToListAsync(ct);

    public async Task AddAsync(FuelLogEntry entry, CancellationToken ct = default)
    {
        db.FuelLogEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FuelLogEntry entry, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(FuelLogEntry entry, CancellationToken ct = default)
    {
        db.FuelLogEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
    }
}
