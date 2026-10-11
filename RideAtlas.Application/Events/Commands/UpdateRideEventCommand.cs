using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;

namespace RideAtlas.Application.Events.Commands;

public record UpdateRideEventCommand(
    Guid Id,
    Guid RiderAccountId,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt,
    string? MeetingPoint,
    string? Destination,
    Guid? TripId,
    Guid? BikeId,
    string? CoverPhotoUri,
    bool IsCancelled) : IRequest<RideEventDto>;

public class UpdateRideEventCommandValidator : AbstractValidator<UpdateRideEventCommand>
{
    public UpdateRideEventCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EndAt).GreaterThan(x => x.StartAt);
    }
}

public class UpdateRideEventCommandHandler(IRideEventRepository repository)
    : IRequestHandler<UpdateRideEventCommand, RideEventDto>
{
    public async Task<RideEventDto> Handle(UpdateRideEventCommand request, CancellationToken ct)
    {
        var rideEvent = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RideEvent), request.Id);

        rideEvent.Title = request.Title.Trim();
        rideEvent.Description = request.Description;
        rideEvent.StartAt = request.StartAt;
        rideEvent.EndAt = request.EndAt;
        rideEvent.MeetingPoint = request.MeetingPoint;
        rideEvent.Destination = request.Destination;
        rideEvent.TripId = request.TripId;
        rideEvent.BikeId = request.BikeId;
        rideEvent.CoverPhotoUri = request.CoverPhotoUri;
        rideEvent.IsCancelled = request.IsCancelled;

        await repository.UpdateAsync(rideEvent, ct);
        return rideEvent.ToDto();
    }
}
