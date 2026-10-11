using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Commands;

public record CreateCommunityPostCommand(
    Guid RiderAccountId,
    Guid? TripId,
    Guid? JournalEntryId,
    string? Caption,
    IReadOnlyList<string>? PhotoUris,
    string? Location,
    double? DistanceKm,
    string Visibility) : IRequest<CommunityPostDto>;

public class CreateCommunityPostCommandValidator : AbstractValidator<CreateCommunityPostCommand>
{
    private static readonly string[] AllowedVisibilities = ["Public", "Circle", "Private"];

    public CreateCommunityPostCommandValidator()
    {
        RuleFor(x => x.Visibility).Must(v => AllowedVisibilities.Contains(v))
            .WithMessage($"Visibility must be one of: {string.Join(", ", AllowedVisibilities)}");
    }
}

public class CreateCommunityPostCommandHandler(ICommunityPostRepository repository)
    : IRequestHandler<CreateCommunityPostCommand, CommunityPostDto>
{
    public async Task<CommunityPostDto> Handle(CreateCommunityPostCommand request, CancellationToken ct)
    {
        var post = new CommunityPost
        {
            RiderAccountId = request.RiderAccountId,
            TripId = request.TripId,
            JournalEntryId = request.JournalEntryId,
            Caption = request.Caption,
            PhotoUrisCsv = CommunityMappingExtensions.JoinPhotoUris(request.PhotoUris),
            Location = request.Location,
            DistanceKm = request.DistanceKm,
            Visibility = request.Visibility
        };
        await repository.AddAsync(post, ct);
        return post.ToDto(likeCount: 0, commentCount: 0, likedByViewer: false);
    }
}
