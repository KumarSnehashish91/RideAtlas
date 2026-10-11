using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class RiderAccountRepository(RideAtlasDbContext db) : IRiderAccountRepository
{
    public Task<RiderAccount?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.RiderAccounts.FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<RiderAccount?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.RiderAccounts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(RiderAccount account, CancellationToken ct = default)
    {
        db.RiderAccounts.Add(account);
        await db.SaveChangesAsync(ct);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(ct);
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public async Task RevokeRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
    {
        token.RevokedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
