using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;

namespace RideAtlas.Application.Community.Commands;

public record UpdateCommunityPostCommand(
    Guid Id,
    Guid RiderAccountId,
    string? Caption,
    IReadOnlyList<string>? PhotoUris,
    string? Location,
    double? DistanceKm,
    string Visibility) : IRequest<CommunityPostDto>;

public class UpdateCommunityPostCommandValidator : AbstractValidator<UpdateCommunityPostCommand>
{
    private static readonly string[] AllowedVisibilities = ["Public", "Circle", "Private"];

    public UpdateCommunityPostCommandValidator()
    {
        RuleFor(x => x.Visibility).Must(v => AllowedVisibilities.Contains(v))
            .WithMessage($"Visibility must be one of: {string.Join(", ", AllowedVisibilities)}");
    }
}

public class UpdateCommunityPostCommandHandler(
    ICommunityPostRepository repository,
    ICommunityCommentRepository commentRepository,
    IPostLikeRepository postLikeRepository) : IRequestHandler<UpdateCommunityPostCommand, CommunityPostDto>
{
    public async Task<CommunityPostDto> Handle(UpdateCommunityPostCommand request, CancellationToken ct)
    {
        var post = await repository.GetByIdAsync(request.Id, ct);
        if (post is null || post.RiderAccountId != request.RiderAccountId)
            throw new NotFoundException(nameof(Domain.Entities.CommunityPost), request.Id);

        post.Caption = request.Caption;
        post.PhotoUrisCsv = CommunityMappingExtensions.JoinPhotoUris(request.PhotoUris);
        post.Location = request.Location;
        post.DistanceKm = request.DistanceKm;
        post.Visibility = request.Visibility;
        await repository.UpdateAsync(post, ct);

        var likeCount = await postLikeRepository.CountForPostAsync(post.Id, ct);
        var commentCount = (await commentRepository.ListForPostAsync(post.Id, ct)).Count;
        var likedByViewer = await postLikeRepository.ExistsAsync(post.Id, request.RiderAccountId, ct);
        return post.ToDto(likeCount, commentCount, likedByViewer);
    }
}
