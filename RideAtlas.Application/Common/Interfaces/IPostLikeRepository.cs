namespace RideAtlas.Application.Common.Interfaces;

public interface IPostLikeRepository
{
    Task<int> CountForPostAsync(Guid postId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default);
    Task RemoveAsync(Guid postId, Guid riderAccountId, CancellationToken ct = default);
}
