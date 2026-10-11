namespace RideAtlas.Application.Community.Dtos;

public record CommunityPostDto(
    Guid Id,
    Guid RiderAccountId,
    Guid? TripId,
    Guid? JournalEntryId,
    string? Caption,
    IReadOnlyList<string> PhotoUris,
    string? Location,
    double? DistanceKm,
    string Visibility,
    int LikeCount,
    int CommentCount,
    bool LikedByViewer,
    DateTime CreatedAt);

public record CommunityCommentDto(Guid Id, Guid PostId, Guid AuthorRiderAccountId, string Text, DateTime CreatedAt);
