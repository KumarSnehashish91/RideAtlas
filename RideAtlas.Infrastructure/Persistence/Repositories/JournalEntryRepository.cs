using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class JournalEntryRepository(RideAtlasDbContext db) : IJournalEntryRepository
{
    public Task<JournalEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.JournalEntries.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<PagedResult<JournalEntry>> ListAsync(Guid riderAccountId, JournalEntryListFilter filter, CancellationToken ct = default)
    {
        var query = db.JournalEntries.Where(x => x.RiderAccountId == riderAccountId);

        if (filter.BikeId is Guid bikeId)
            query = query.Where(x => x.BikeId == bikeId);
        if (filter.TripId is Guid tripId)
            query = query.Where(x => x.TripId == tripId);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.Date)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<JournalEntry>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(JournalEntry entry, CancellationToken ct = default)
    {
        db.JournalEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(JournalEntry entry, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(JournalEntry entry, CancellationToken ct = default)
    {
        db.JournalEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
    }
}
