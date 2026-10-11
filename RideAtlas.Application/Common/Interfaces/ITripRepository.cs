using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface ITripRepository
{
    Task<Trip?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<Trip>> ListAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(Trip trip, CancellationToken ct = default);
    Task UpdateAsync(Trip trip, CancellationToken ct = default);
    Task DeleteAsync(Trip trip, CancellationToken ct = default);
}
