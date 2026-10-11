using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class CommunityPostRepository(RideAtlasDbContext db) : ICommunityPostRepository
{
    public Task<CommunityPost?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.CommunityPosts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<CommunityPost>> ListFeedAsync(Guid viewerRiderAccountId, CommunityFeedFilter filter, CancellationToken ct = default)
    {
        var query = db.CommunityPosts.AsQueryable();

        if (filter.AuthorRiderAccountId is Guid authorId)
            query = query.Where(x => x.RiderAccountId == authorId);

        query = query.Where(x =>
            x.RiderAccountId == viewerRiderAccountId
            || x.Visibility == "Public"
            || (x.Visibility == "Circle" && db.RidingBuddies.Any(b =>
                b.RiderAccountId == viewerRiderAccountId && b.RemoteRiderId == x.RiderAccountId && b.IsFollowing)));

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<CommunityPost>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(CommunityPost post, CancellationToken ct = default)
    {
        db.CommunityPosts.Add(post);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CommunityPost post, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(CommunityPost post, CancellationToken ct = default)
    {
        db.CommunityPosts.Remove(post);
        await db.SaveChangesAsync(ct);
    }
}
