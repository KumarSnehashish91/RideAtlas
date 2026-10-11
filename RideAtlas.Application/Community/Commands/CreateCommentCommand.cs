using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Commands;

public record CreateCommentCommand(Guid PostId, Guid RiderAccountId, string Text) : IRequest<CommunityCommentDto>;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }
}

public class CreateCommentCommandHandler(
    ICommunityPostRepository postRepository,
    ICommunityCommentRepository commentRepository) : IRequestHandler<CreateCommentCommand, CommunityCommentDto>
{
    public async Task<CommunityCommentDto> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        _ = await postRepository.GetByIdAsync(request.PostId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.CommunityPost), request.PostId);

        var comment = new CommunityComment
        {
            PostId = request.PostId,
            AuthorRiderAccountId = request.RiderAccountId,
            Text = request.Text.Trim()
        };
        await commentRepository.AddAsync(comment, ct);
        return comment.ToDto();
    }
}
