using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Community.Commands;

public record DeleteCommunityPostCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteCommunityPostCommandHandler(ICommunityPostRepository repository) : IRequestHandler<DeleteCommunityPostCommand>
{
    public async Task Handle(DeleteCommunityPostCommand request, CancellationToken ct)
    {
        var post = await repository.GetByIdAsync(request.Id, ct);
        if (post is null || post.RiderAccountId != request.RiderAccountId)
            throw new NotFoundException(nameof(Domain.Entities.CommunityPost), request.Id);
        await repository.DeleteAsync(post, ct);
    }
}
