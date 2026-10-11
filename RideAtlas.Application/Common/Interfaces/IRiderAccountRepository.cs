using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IRiderAccountRepository
{
    Task<RiderAccount?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<RiderAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(RiderAccount account, CancellationToken ct = default);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
}
