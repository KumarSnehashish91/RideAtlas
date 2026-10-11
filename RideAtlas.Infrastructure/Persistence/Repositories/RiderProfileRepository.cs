using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class RiderProfileRepository(RideAtlasDbContext db) : IRiderProfileRepository
{
    public Task<RiderProfile?> GetByRiderAccountIdAsync(Guid riderAccountId, CancellationToken ct = default) =>
        db.RiderProfiles.FirstOrDefaultAsync(x => x.RiderAccountId == riderAccountId, ct);

    public async Task AddAsync(RiderProfile profile, CancellationToken ct = default)
    {
        db.RiderProfiles.Add(profile);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RiderProfile profile, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
