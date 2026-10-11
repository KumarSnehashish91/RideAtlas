using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Queries;

public record ListCommentsQuery(Guid PostId) : IRequest<IReadOnlyList<CommunityCommentDto>>;

public class ListCommentsQueryHandler(ICommunityCommentRepository repository)
    : IRequestHandler<ListCommentsQuery, IReadOnlyList<CommunityCommentDto>>
{
    public async Task<IReadOnlyList<CommunityCommentDto>> Handle(ListCommentsQuery request, CancellationToken ct)
    {
        var comments = await repository.ListForPostAsync(request.PostId, ct);
        return comments.Select(c => c.ToDto()).ToList();
    }
}
