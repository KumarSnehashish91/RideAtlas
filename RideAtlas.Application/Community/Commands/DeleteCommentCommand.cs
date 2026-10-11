using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Community.Commands;

public record DeleteCommentCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteCommentCommandHandler(ICommunityCommentRepository repository) : IRequestHandler<DeleteCommentCommand>
{
    public async Task Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var comment = await repository.GetByIdAsync(request.Id, ct);
        if (comment is null || comment.AuthorRiderAccountId != request.RiderAccountId)
            throw new NotFoundException(nameof(Domain.Entities.CommunityComment), request.Id);
        await repository.DeleteAsync(comment, ct);
    }
}
