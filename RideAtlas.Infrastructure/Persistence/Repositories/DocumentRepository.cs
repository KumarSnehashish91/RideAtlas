using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class DocumentRepository(RideAtlasDbContext db) : IDocumentRepository
{
    public Task<Document?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.Documents.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<IReadOnlyList<Document>> ListAsync(Guid riderAccountId, Guid? bikeId, CancellationToken ct = default)
    {
        var query = db.Documents.Where(x => x.RiderAccountId == riderAccountId);
        if (bikeId is Guid id)
            query = query.Where(x => x.BikeId == id);
        return await query.OrderBy(x => x.ExpiryDate).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Document>> ListExpiringSoonAsync(Guid riderAccountId, DateOnly today, int withinDays, CancellationToken ct = default)
    {
        var cutoff = today.AddDays(withinDays);
        return await db.Documents
            .Where(x => x.RiderAccountId == riderAccountId
                && x.ReminderEnabled
                && x.ExpiryDate != null
                && x.ExpiryDate <= cutoff)
            .OrderBy(x => x.ExpiryDate)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Document document, CancellationToken ct = default)
    {
        db.Documents.Add(document);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Document document, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Document document, CancellationToken ct = default)
    {
        db.Documents.Remove(document);
        await db.SaveChangesAsync(ct);
    }
}
