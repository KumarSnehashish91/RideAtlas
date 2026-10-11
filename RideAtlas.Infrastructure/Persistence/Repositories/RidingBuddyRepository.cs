using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class RidingBuddyRepository(RideAtlasDbContext db) : IRidingBuddyRepository
{
    public Task<RidingBuddy?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.RidingBuddies.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<RidingBuddy>> ListAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.RidingBuddies.Where(x => x.RiderAccountId == riderAccountId).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task AddAsync(RidingBuddy buddy, CancellationToken ct = default)
    {
        db.RidingBuddies.Add(buddy);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RidingBuddy buddy, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(RidingBuddy buddy, CancellationToken ct = default)
    {
        db.RidingBuddies.Remove(buddy);
        await db.SaveChangesAsync(ct);
    }
}
