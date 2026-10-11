using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class RideEventRepository(RideAtlasDbContext db) : IRideEventRepository
{
    public Task<RideEvent?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.RideEvents.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<RideEvent>> ListAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.RideEvents.Where(x => x.RiderAccountId == riderAccountId).OrderBy(x => x.StartAt).ToListAsync(ct);

    public async Task AddAsync(RideEvent rideEvent, CancellationToken ct = default)
    {
        db.RideEvents.Add(rideEvent);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RideEvent rideEvent, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(RideEvent rideEvent, CancellationToken ct = default)
    {
        db.RideEvents.Remove(rideEvent);
        await db.SaveChangesAsync(ct);
    }
}
