using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class ReminderRuleRepository(RideAtlasDbContext db) : IReminderRuleRepository
{
    public Task<ReminderRule?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.ReminderRules.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<ReminderRule>> ListForBikeAsync(Guid bikeId, Guid riderAccountId, CancellationToken ct = default) =>
        await db.ReminderRules
            .Where(x => x.BikeId == bikeId && x.RiderAccountId == riderAccountId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ReminderRule>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.ReminderRules.Where(x => x.RiderAccountId == riderAccountId).ToListAsync(ct);

    public async Task AddAsync(ReminderRule rule, CancellationToken ct = default)
    {
        db.ReminderRules.Add(rule);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(ReminderRule rule, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(ReminderRule rule, CancellationToken ct = default)
    {
        db.ReminderRules.Remove(rule);
        await db.SaveChangesAsync(ct);
    }
}
