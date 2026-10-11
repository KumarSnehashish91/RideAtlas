using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public interface ICommunityCommentRepository
{
    Task<CommunityComment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CommunityComment>> ListForPostAsync(Guid postId, CancellationToken ct = default);
    Task AddAsync(CommunityComment comment, CancellationToken ct = default);
    Task DeleteAsync(CommunityComment comment, CancellationToken ct = default);
}
