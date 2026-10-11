using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Community.Dtos;

public static class CommunityMappingExtensions
{
    public static CommunityPostDto ToDto(this CommunityPost post, int likeCount, int commentCount, bool likedByViewer) => new(
        post.Id,
        post.RiderAccountId,
        post.TripId,
        post.JournalEntryId,
        post.Caption,
        SplitPhotoUris(post.PhotoUrisCsv),
        post.Location,
        post.DistanceKm,
        post.Visibility,
        likeCount,
        commentCount,
        likedByViewer,
        post.CreatedAt);

    public static CommunityCommentDto ToDto(this CommunityComment comment) => new(
        comment.Id, comment.PostId, comment.AuthorRiderAccountId, comment.Text, comment.CreatedAt);

    public static string? JoinPhotoUris(IReadOnlyList<string>? photoUris) =>
        photoUris is null || photoUris.Count == 0 ? null : string.Join('|', photoUris);

    private static IReadOnlyList<string> SplitPhotoUris(string? csv) =>
        string.IsNullOrWhiteSpace(csv) ? [] : csv.Split('|', StringSplitOptions.RemoveEmptyEntries);
}
