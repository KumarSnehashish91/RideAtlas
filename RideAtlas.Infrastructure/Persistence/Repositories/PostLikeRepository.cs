using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class PostLikeRepository(RideAtlasDbContext db) : IPostLikeRepository
{
    public Task<int> CountForPostAsync(Guid postId, CancellationToken ct = default) =>
        db.PostLikes.CountAsync(x => x.PostId == postId, ct);

    public Task<bool> ExistsAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default) =>
        db.PostLikes.AnyAsync(x => x.PostId == postId && x.RiderAccountId == riderAccountId, ct);

    public async Task AddAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default)
    {
        db.PostLikes.Add(new PostLike { PostId = postId, RiderAccountId = riderAccountId });
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default)
    {
        var like = await db.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.RiderAccountId == riderAccountId, ct);
        if (like is null) return;
        db.PostLikes.Remove(like);
        await db.SaveChangesAsync(ct);
    }
}
