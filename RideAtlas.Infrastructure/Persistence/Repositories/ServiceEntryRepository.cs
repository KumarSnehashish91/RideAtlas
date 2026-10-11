using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class ServiceEntryRepository(RideAtlasDbContext db) : IServiceEntryRepository
{
    public Task<ServiceEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.ServiceEntries.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<PagedResult<ServiceEntry>> ListAsync(Guid riderAccountId, ServiceEntryListFilter filter, CancellationToken ct = default)
    {
        var query = db.ServiceEntries.Where(x => x.RiderAccountId == riderAccountId);

        if (filter.BikeId is Guid bikeId)
            query = query.Where(x => x.BikeId == bikeId);
        if (filter.FromDate is DateOnly fromDate)
            query = query.Where(x => x.ServiceDate >= fromDate);
        if (filter.ToDate is DateOnly toDate)
            query = query.Where(x => x.ServiceDate <= toDate);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.ServiceDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<ServiceEntry>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<ServiceEntry>> ListAllForBikeAsync(Guid riderAccountId, Guid bikeId, CancellationToken ct = default) =>
        await db.ServiceEntries
            .Where(x => x.RiderAccountId == riderAccountId && x.BikeId == bikeId)
            .OrderBy(x => x.OdometerKm)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ServiceEntry>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.ServiceEntries.Where(x => x.RiderAccountId == riderAccountId).ToListAsync(ct);

    public async Task AddAsync(ServiceEntry entry, CancellationToken ct = default)
    {
        db.ServiceEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ServiceEntry entry, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(ServiceEntry entry, CancellationToken ct = default)
    {
        db.ServiceEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
    }
}
