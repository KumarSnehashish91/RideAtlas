using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IRideEventRepository
{
    Task<RideEvent?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<RideEvent>> ListAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(RideEvent rideEvent, CancellationToken ct = default);
    Task UpdateAsync(RideEvent rideEvent, CancellationToken ct = default);
    Task DeleteAsync(RideEvent rideEvent, CancellationToken ct = default);
}
