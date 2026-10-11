using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Community.Commands;
using RideAtlas.Application.Community.Dtos;
using RideAtlas.Application.Community.Queries;

namespace RideAtlas.Api.Controllers;

public record UpdateMyProfileRequest(
    string DisplayName,
    string? Bio,
    string? HomeCity,
    string? AvatarUri,
    int? RidingSinceYear,
    bool IsPublic,
    bool ShowLocation,
    bool ShowStats,
    bool ShowBikes);

public record CreateRidingBuddyRequest(string Name, string? HomeCity, string? AvatarUri, string? Notes, Guid? RemoteRiderId);
public record UpdateRidingBuddyRequest(string Name, string? HomeCity, string? AvatarUri, string? Notes, bool IsFollowing);

public record CreateCommunityPostRequest(
    Guid? TripId, Guid? JournalEntryId, string? Caption, IReadOnlyList<string>? PhotoUris,
    string? Location, double? DistanceKm, string Visibility);
public record UpdateCommunityPostRequest(
    string? Caption, IReadOnlyList<string>? PhotoUris, string? Location, double? DistanceKm, string Visibility);
public record CreateCommentRequest(string Text);

public class CommunityController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("profile")]
    public async Task<ActionResult<RiderProfileDto>> GetMyProfile(CancellationToken ct)
    {
        var profile = await Sender.Send(new GetMyProfileQuery(RiderAccountId), ct);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<RiderProfileDto>> UpdateMyProfile(UpdateMyProfileRequest request, CancellationToken ct)
    {
        var profile = await Sender.Send(new UpdateMyProfileCommand(
            RiderAccountId, request.DisplayName, request.Bio, request.HomeCity, request.AvatarUri,
            request.RidingSinceYear, request.IsPublic, request.ShowLocation, request.ShowStats, request.ShowBikes), ct);
        return Ok(profile);
    }

    [HttpGet("buddies")]
    public async Task<ActionResult<IReadOnlyList<RidingBuddyDto>>> ListBuddies(CancellationToken ct)
    {
        var buddies = await Sender.Send(new ListRidingBuddiesQuery(RiderAccountId), ct);
        return Ok(buddies);
    }

    [HttpPost("buddies")]
    public async Task<ActionResult<RidingBuddyDto>> CreateBuddy(CreateRidingBuddyRequest request, CancellationToken ct)
    {
        var buddy = await Sender.Send(new CreateRidingBuddyCommand(
            RiderAccountId, request.Name, request.HomeCity, request.AvatarUri, request.Notes, request.RemoteRiderId), ct);
        return Ok(buddy);
    }

    [HttpPut("buddies/{id:guid}")]
    public async Task<ActionResult<RidingBuddyDto>> UpdateBuddy(Guid id, UpdateRidingBuddyRequest request, CancellationToken ct)
    {
        var buddy = await Sender.Send(new UpdateRidingBuddyCommand(
            id, RiderAccountId, request.Name, request.HomeCity, request.AvatarUri, request.Notes, request.IsFollowing), ct);
        return Ok(buddy);
    }

    [HttpDelete("buddies/{id:guid}")]
    public async Task<IActionResult> DeleteBuddy(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteRidingBuddyCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpGet("feed")]
    public async Task<ActionResult<PagedResult<CommunityPostDto>>> Feed(
        [FromQuery] Guid? authorRiderAccountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var feed = await Sender.Send(new ListFeedQuery(RiderAccountId, authorRiderAccountId, page, pageSize), ct);
        return Ok(feed);
    }

    [HttpGet("posts/{id:guid}")]
    public async Task<ActionResult<CommunityPostDto>> GetPost(Guid id, CancellationToken ct)
    {
        var post = await Sender.Send(new GetCommunityPostQuery(id, RiderAccountId), ct);
        return Ok(post);
    }

    [HttpPost("posts")]
    public async Task<ActionResult<CommunityPostDto>> CreatePost(CreateCommunityPostRequest request, CancellationToken ct)
    {
        var post = await Sender.Send(new CreateCommunityPostCommand(
            RiderAccountId, request.TripId, request.JournalEntryId, request.Caption, request.PhotoUris,
            request.Location, request.DistanceKm, request.Visibility), ct);
        return Ok(post);
    }

    [HttpPut("posts/{id:guid}")]
    public async Task<ActionResult<CommunityPostDto>> UpdatePost(Guid id, UpdateCommunityPostRequest request, CancellationToken ct)
    {
        var post = await Sender.Send(new UpdateCommunityPostCommand(
            id, RiderAccountId, request.Caption, request.PhotoUris, request.Location, request.DistanceKm, request.Visibility), ct);
        return Ok(post);
    }

    [HttpDelete("posts/{id:guid}")]
    public async Task<IActionResult> DeletePost(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteCommunityPostCommand(id, RiderAccountId), ct);
        return NoContent();
    }

    [HttpPost("posts/{id:guid}/like")]
    public async Task<ActionResult<bool>> ToggleLike(Guid id, CancellationToken ct)
    {
        var liked = await Sender.Send(new ToggleLikeCommand(id, RiderAccountId), ct);
        return Ok(liked);
    }

    [HttpGet("posts/{id:guid}/comments")]
    public async Task<ActionResult<IReadOnlyList<CommunityCommentDto>>> ListComments(Guid id, CancellationToken ct)
    {
        var comments = await Sender.Send(new ListCommentsQuery(id), ct);
        return Ok(comments);
    }

    [HttpPost("posts/{id:guid}/comments")]
    public async Task<ActionResult<CommunityCommentDto>> CreateComment(Guid id, CreateCommentRequest request, CancellationToken ct)
    {
        var comment = await Sender.Send(new CreateCommentCommand(id, RiderAccountId, request.Text), ct);
        return Ok(comment);
    }

    [HttpDelete("comments/{id:guid}")]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken ct)
    {
        await Sender.Send(new DeleteCommentCommand(id, RiderAccountId), ct);
        return NoContent();
    }
}
