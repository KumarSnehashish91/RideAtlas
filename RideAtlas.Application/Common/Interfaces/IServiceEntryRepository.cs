using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class ServiceEntryListFilter
{
    public Guid? BikeId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IServiceEntryRepository
{
    Task<ServiceEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<PagedResult<ServiceEntry>> ListAsync(Guid riderAccountId, ServiceEntryListFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceEntry>> ListAllForBikeAsync(Guid riderAccountId, Guid bikeId, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceEntry>> ListAllForRiderAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(ServiceEntry entry, CancellationToken ct = default);
    Task UpdateAsync(ServiceEntry entry, CancellationToken ct = default);
    Task DeleteAsync(ServiceEntry entry, CancellationToken ct = default);
}
