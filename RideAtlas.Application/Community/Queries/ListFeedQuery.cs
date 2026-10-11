using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Queries;

public record ListFeedQuery(Guid ViewerRiderAccountId, Guid? AuthorRiderAccountId, int Page, int PageSize)
    : IRequest<PagedResult<CommunityPostDto>>;

public class ListFeedQueryHandler(
    ICommunityPostRepository postRepository,
    ICommunityCommentRepository commentRepository,
    IPostLikeRepository postLikeRepository) : IRequestHandler<ListFeedQuery, PagedResult<CommunityPostDto>>
{
    public async Task<PagedResult<CommunityPostDto>> Handle(ListFeedQuery request, CancellationToken ct)
    {
        var filter = new CommunityFeedFilter
        {
            AuthorRiderAccountId = request.AuthorRiderAccountId,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await postRepository.ListFeedAsync(request.ViewerRiderAccountId, filter, ct);

        var dtos = new List<CommunityPostDto>(result.Items.Count);
        foreach (var post in result.Items)
        {
            var likeCount = await postLikeRepository.CountForPostAsync(post.Id, ct);
            var commentCount = (await commentRepository.ListForPostAsync(post.Id, ct)).Count;
            var likedByViewer = await postLikeRepository.ExistsAsync(post.Id, request.ViewerRiderAccountId, ct);
            dtos.Add(post.ToDto(likeCount, commentCount, likedByViewer));
        }

        return new PagedResult<CommunityPostDto>
        {
            Items = dtos,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
