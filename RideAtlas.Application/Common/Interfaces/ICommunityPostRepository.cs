using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class CommunityFeedFilter
{
    public Guid? AuthorRiderAccountId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface ICommunityPostRepository
{
    Task<CommunityPost?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Applies visibility rules relative to the viewer: their own posts (any visibility),
    /// everyone's Public posts, and Circle posts from authors the viewer follows.</summary>
    Task<PagedResult<CommunityPost>> ListFeedAsync(Guid viewerRiderAccountId, CommunityFeedFilter filter, CancellationToken ct = default);
    Task AddAsync(CommunityPost post, CancellationToken ct = default);
    Task UpdateAsync(CommunityPost post, CancellationToken ct = default);
    Task DeleteAsync(CommunityPost post, CancellationToken ct = default);
}
