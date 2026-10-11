using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Events.Commands;
using RideAtlas.Application.Events.Dtos;
using RideAtlas.Application.Events.Queries;

namespace RideAtlas.Api.Controllers;

public record CreateRideEventRequest(
    string Title, string? Description, DateTime StartAt, DateTime EndAt,
    string? MeetingPoint, string? Destination, Guid? TripId, Guid? BikeId, string? CoverPhotoUri);

public record UpdateRideEventRequest(
    string Title, string? Description, DateTime StartAt, DateTime EndAt,
    string? MeetingPoint, string? Destination, Guid? TripId, Guid? BikeId, string? CoverPhotoUri, bool IsCancelled);

public record InviteToEventRequest(Guid BuddyId);
public record RespondToInviteRequest(string RsvpStatus);

public class EventsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RideEventDto>>> List(CancellationToken ct)
    {
        var events = await Sender.Send(new ListRideEventsQuery(RiderAccountId), ct);
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RideEventDto>> Get(Guid id, CancellationToken ct)
    {
        var rideEvent = await Sender.Send(new GetRideEventQuery(id, RiderAccountId), ct);
        return Ok(rideEvent);
    }

    [HttpPost]
    public async Task<ActionResult<RideEventDto>> Create(CreateRideEventRequest request, CancellationToken ct)
    {
        var rideEvent = await Sender.Send(new CreateRideEventCommand(
            RiderAccountId, request.Title, request.Description, request.StartAt, request.EndAt,
            request.MeetingPoint, request.Destination, request.TripId, request.BikeId, request.CoverPhotoUri), ct);
        return Ok(rideEvent);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RideEventDto>> Update(Guid id, UpdateRideEventRequest request, CancellationToken ct)
    {
        var rideEvent = await Sender.Send(new UpdateRideEventCommand(
            id, RiderAccountId, request.Title, request.Description, request.StartAt, request.EndAt,
            request.MeetingPoint, request.Destination, request.TripId, request.BikeId, request.CoverPhotoUri,
            request.IsCancelled), ct);
        return Ok(rideEvent);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteRideEventCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/invitees")]
    public async Task<ActionResult<IReadOnlyList<EventInviteeDto>>> ListInvitees(Guid id, CancellationToken ct)
    {
        var invitees = await Sender.Send(new ListEventInviteesQuery(id), ct);
        return Ok(invitees);
    }

    [HttpPost("{id:guid}/invitees")]
    public async Task<ActionResult<EventInviteeDto>> Invite(Guid id, InviteToEventRequest request, CancellationToken ct)
    {
        var invitee = await Sender.Send(new InviteToEventCommand(id, RiderAccountId, request.BuddyId), ct);
        return Ok(invitee);
    }

    [HttpPost("invitees/{inviteeId:guid}/respond")]
    public async Task<ActionResult<EventInviteeDto>> Respond(Guid inviteeId, RespondToInviteRequest request, CancellationToken ct)
    {
        var invitee = await Sender.Send(new RespondToInviteCommand(inviteeId, request.RsvpStatus), ct);
        return Ok(invitee);
    }
}
