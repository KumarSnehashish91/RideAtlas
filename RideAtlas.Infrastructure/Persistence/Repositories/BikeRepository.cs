using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class BikeRepository(RideAtlasDbContext db) : IBikeRepository
{
    public Task<Motorcycle?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.Motorcycles.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<Motorcycle>> ListAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.Motorcycles
            .Where(x => x.RiderAccountId == riderAccountId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Motorcycle bike, CancellationToken ct = default)
    {
        db.Motorcycles.Add(bike);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Motorcycle bike, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Motorcycle bike, CancellationToken ct = default)
    {
        db.Motorcycles.Remove(bike);
        await db.SaveChangesAsync(ct);
    }

    public async Task ClearActiveFlagAsync(Guid riderAccountId, CancellationToken ct = default)
    {
        var activeBikes = await db.Motorcycles
            .Where(x => x.RiderAccountId == riderAccountId && x.IsActive)
            .ToListAsync(ct);
        foreach (var bike in activeBikes)
            bike.IsActive = false;
        await db.SaveChangesAsync(ct);
    }
}
