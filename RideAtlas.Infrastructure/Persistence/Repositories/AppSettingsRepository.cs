using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class AppSettingsRepository(RideAtlasDbContext db) : IAppSettingsRepository
{
    public Task<AppSettings?> GetByRiderAccountIdAsync(Guid riderAccountId, CancellationToken ct = default) =>
        db.AppSettingsEntries.FirstOrDefaultAsync(x => x.RiderAccountId == riderAccountId, ct);

    public async Task AddAsync(AppSettings settings, CancellationToken ct = default)
    {
        db.AppSettingsEntries.Add(settings);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(AppSettings settings, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
