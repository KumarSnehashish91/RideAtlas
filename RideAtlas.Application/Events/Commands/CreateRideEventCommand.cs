using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Events.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Events.Commands;

public record CreateRideEventCommand(
    Guid RiderAccountId,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt,
    string? MeetingPoint,
    string? Destination,
    Guid? TripId,
    Guid? BikeId,
    string? CoverPhotoUri) : IRequest<RideEventDto>;

public class CreateRideEventCommandValidator : AbstractValidator<CreateRideEventCommand>
{
    public CreateRideEventCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EndAt).GreaterThan(x => x.StartAt);
    }
}

public class CreateRideEventCommandHandler(IRideEventRepository repository)
    : IRequestHandler<CreateRideEventCommand, RideEventDto>
{
    public async Task<RideEventDto> Handle(CreateRideEventCommand request, CancellationToken ct)
    {
        var rideEvent = new RideEvent
        {
            RiderAccountId = request.RiderAccountId,
            Title = request.Title.Trim(),
            Description = request.Description,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            MeetingPoint = request.MeetingPoint,
            Destination = request.Destination,
            TripId = request.TripId,
            BikeId = request.BikeId,
            CoverPhotoUri = request.CoverPhotoUri
        };
        await repository.AddAsync(rideEvent, ct);
        return rideEvent.ToDto();
    }
}
