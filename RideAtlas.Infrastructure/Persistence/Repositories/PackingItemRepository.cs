using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class PackingItemRepository(RideAtlasDbContext db) : IPackingItemRepository
{
    public Task<PackingItem?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.PackingItems.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<PackingItem>> ListForTripAsync(Guid tripId, Guid riderAccountId, CancellationToken ct = default) =>
        await db.PackingItems
            .Where(x => x.TripId == tripId && x.RiderAccountId == riderAccountId)
            .ToListAsync(ct);

    public async Task AddAsync(PackingItem item, CancellationToken ct = default)
    {
        db.PackingItems.Add(item);
        await db.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IReadOnlyList<PackingItem> items, CancellationToken ct = default)
    {
        db.PackingItems.AddRange(items);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PackingItem item, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(PackingItem item, CancellationToken ct = default)
    {
        db.PackingItems.Remove(item);
        await db.SaveChangesAsync(ct);
    }
}
