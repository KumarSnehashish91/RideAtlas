using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class TripRepository(RideAtlasDbContext db) : ITripRepository
{
    public Task<Trip?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.Trips.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<Trip>> ListAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.Trips
            .Where(x => x.RiderAccountId == riderAccountId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Trip trip, CancellationToken ct = default)
    {
        db.Trips.Add(trip);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Trip trip, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Trip trip, CancellationToken ct = default)
    {
        db.Trips.Remove(trip);
        await db.SaveChangesAsync(ct);
    }
}
