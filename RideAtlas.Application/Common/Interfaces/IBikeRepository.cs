using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IBikeRepository
{
    Task<Motorcycle?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<Motorcycle>> ListAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(Motorcycle bike, CancellationToken ct = default);
    Task UpdateAsync(Motorcycle bike, CancellationToken ct = default);
    Task DeleteAsync(Motorcycle bike, CancellationToken ct = default);
    Task ClearActiveFlagAsync(Guid riderAccountId, CancellationToken ct = default);
}
