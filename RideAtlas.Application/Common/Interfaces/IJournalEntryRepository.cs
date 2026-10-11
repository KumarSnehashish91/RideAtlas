using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class JournalEntryListFilter
{
    public Guid? BikeId { get; init; }
    public Guid? TripId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<PagedResult<JournalEntry>> ListAsync(Guid riderAccountId, JournalEntryListFilter filter, CancellationToken ct = default);
    Task AddAsync(JournalEntry entry, CancellationToken ct = default);
    Task UpdateAsync(JournalEntry entry, CancellationToken ct = default);
    Task DeleteAsync(JournalEntry entry, CancellationToken ct = default);
}
