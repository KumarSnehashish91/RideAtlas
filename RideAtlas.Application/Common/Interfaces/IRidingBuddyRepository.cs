using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IRidingBuddyRepository
{
    Task<RidingBuddy?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<RidingBuddy>> ListAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(RidingBuddy buddy, CancellationToken ct = default);
    Task UpdateAsync(RidingBuddy buddy, CancellationToken ct = default);
    Task DeleteAsync(RidingBuddy buddy, CancellationToken ct = default);
}
