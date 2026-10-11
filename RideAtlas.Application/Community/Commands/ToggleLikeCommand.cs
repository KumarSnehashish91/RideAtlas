using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Community.Commands;

public record ToggleLikeCommand(Guid PostId, Guid RiderAccountId) : IRequest<bool>;

public class ToggleLikeCommandHandler(
    ICommunityPostRepository postRepository,
    IPostLikeRepository postLikeRepository) : IRequestHandler<ToggleLikeCommand, bool>
{
    public async Task<bool> Handle(ToggleLikeCommand request, CancellationToken ct)
    {
        _ = await postRepository.GetByIdAsync(request.PostId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.CommunityPost), request.PostId);

        var alreadyLiked = await postLikeRepository.ExistsAsync(request.PostId, request.RiderAccountId, ct);
        if (alreadyLiked)
        {
            await postLikeRepository.RemoveAsync(request.PostId, request.RiderAccountId, ct);
            return false;
        }

        await postLikeRepository.AddAsync(request.PostId, request.RiderAccountId, ct);
        return true;
    }
}
