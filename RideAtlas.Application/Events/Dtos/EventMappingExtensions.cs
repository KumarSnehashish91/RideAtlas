using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Events.Dtos;

public static class EventMappingExtensions
{
    public static RideEventDto ToDto(this RideEvent rideEvent) => new(
        rideEvent.Id,
        rideEvent.Title,
        rideEvent.Description,
        rideEvent.StartAt,
        rideEvent.EndAt,
        rideEvent.MeetingPoint,
        rideEvent.Destination,
        rideEvent.TripId,
        rideEvent.BikeId,
        rideEvent.CoverPhotoUri,
        rideEvent.IsCancelled,
        rideEvent.CreatedAt);

    public static EventInviteeDto ToDto(this EventInvitee invitee) => new(
        invitee.Id, invitee.EventId, invitee.BuddyId, invitee.RsvpStatus, invitee.CreatedAt);
}
