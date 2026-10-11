using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class CommunityCommentRepository(RideAtlasDbContext db) : ICommunityCommentRepository
{
    public Task<CommunityComment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.CommunityComments.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<CommunityComment>> ListForPostAsync(Guid postId, CancellationToken ct = default) =>
        await db.CommunityComments.Where(x => x.PostId == postId).OrderBy(x => x.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(CommunityComment comment, CancellationToken ct = default)
    {
        db.CommunityComments.Add(comment);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(CommunityComment comment, CancellationToken ct = default)
    {
        db.CommunityComments.Remove(comment);
        await db.SaveChangesAsync(ct);
    }
}
