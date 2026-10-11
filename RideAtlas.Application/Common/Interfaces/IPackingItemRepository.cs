using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IPackingItemRepository
{
    Task<PackingItem?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<PackingItem>> ListForTripAsync(Guid tripId, Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(PackingItem item, CancellationToken ct = default);
    Task AddRangeAsync(IReadOnlyList<PackingItem> items, CancellationToken ct = default);
    Task UpdateAsync(PackingItem item, CancellationToken ct = default);
    Task DeleteAsync(PackingItem item, CancellationToken ct = default);
}
