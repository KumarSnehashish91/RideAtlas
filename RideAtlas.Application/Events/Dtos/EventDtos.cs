namespace RideAtlas.Application.Events.Dtos;

public record RideEventDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt,
    string? MeetingPoint,
    string? Destination,
    Guid? TripId,
    Guid? BikeId,
    string? CoverPhotoUri,
    bool IsCancelled,
    DateTime CreatedAt);

public record EventInviteeDto(Guid Id, Guid EventId, Guid BuddyId, string RsvpStatus, DateTime CreatedAt);
