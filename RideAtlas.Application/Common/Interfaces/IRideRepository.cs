using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class RideListFilter
{
    public Guid? BikeId { get; init; }
    public Guid? TripId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IRideRepository
{
    Task<Ride?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<PagedResult<Ride>> ListAsync(Guid riderAccountId, RideListFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<Ride>> ListAllAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(Ride ride, CancellationToken ct = default);
    Task UpdateAsync(Ride ride, CancellationToken ct = default);
    Task DeleteAsync(Ride ride, CancellationToken ct = default);
}
