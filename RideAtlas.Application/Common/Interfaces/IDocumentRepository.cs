using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<IReadOnlyList<Document>> ListAsync(Guid riderAccountId, Guid? bikeId, CancellationToken ct = default);
    Task<IReadOnlyList<Document>> ListExpiringSoonAsync(Guid riderAccountId, DateOnly today, int withinDays, CancellationToken ct = default);
    Task AddAsync(Document document, CancellationToken ct = default);
    Task UpdateAsync(Document document, CancellationToken ct = default);
    Task DeleteAsync(Document document, CancellationToken ct = default);
}
