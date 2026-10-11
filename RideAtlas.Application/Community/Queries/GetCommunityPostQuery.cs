using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Queries;

public record GetCommunityPostQuery(Guid Id, Guid ViewerRiderAccountId) : IRequest<CommunityPostDto>;

public class GetCommunityPostQueryHandler(
    ICommunityPostRepository postRepository,
    ICommunityCommentRepository commentRepository,
    IPostLikeRepository postLikeRepository) : IRequestHandler<GetCommunityPostQuery, CommunityPostDto>
{
    public async Task<CommunityPostDto> Handle(GetCommunityPostQuery request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.CommunityPost), request.Id);

        var likeCount = await postLikeRepository.CountForPostAsync(post.Id, ct);
        var commentCount = (await commentRepository.ListForPostAsync(post.Id, ct)).Count;
        var likedByViewer = await postLikeRepository.ExistsAsync(post.Id, request.ViewerRiderAccountId, ct);
        return post.ToDto(likeCount, commentCount, likedByViewer);
    }
}
