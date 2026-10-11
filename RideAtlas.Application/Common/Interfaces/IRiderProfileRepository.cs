using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IRiderProfileRepository
{
    Task<RiderProfile?> GetByRiderAccountIdAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(RiderProfile profile, CancellationToken ct = default);
    Task UpdateAsync(RiderProfile profile, CancellationToken ct = default);
}
