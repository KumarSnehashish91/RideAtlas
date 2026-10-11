using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class FuelLogListFilter
{
    public Guid? BikeId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IFuelLogRepository
{
    Task<FuelLogEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<PagedResult<FuelLogEntry>> ListAsync(Guid riderAccountId, FuelLogListFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<FuelLogEntry>> ListAllForBikeAsync(Guid riderAccountId, Guid bikeId, CancellationToken ct = default);
    Task<IReadOnlyList<FuelLogEntry>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(FuelLogEntry entry, CancellationToken ct = default);
    Task UpdateAsync(FuelLogEntry entry, CancellationToken ct = default);
    Task DeleteAsync(FuelLogEntry entry, CancellationToken ct = default);
}
