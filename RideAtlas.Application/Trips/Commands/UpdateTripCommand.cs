using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Trips.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Trips.Commands;

public record UpdateTripCommand(
    Guid Id,
    Guid RiderAccountId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BikeId,
    double? DistanceKm,
    decimal? Budget,
    bool IsDraft,
    bool IsCancelled) : IRequest<TripDto>;

public class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.DistanceKm).GreaterThanOrEqualTo(0).When(x => x.DistanceKm.HasValue);
        RuleFor(x => x.Budget).GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue);
    }
}

public class UpdateTripCommandHandler(ITripRepository repository, IDateTimeProvider clock)
    : IRequestHandler<UpdateTripCommand, TripDto>
{
    public async Task<TripDto> Handle(UpdateTripCommand request, CancellationToken ct)
    {
        var trip = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Trip), request.Id);

        trip.Name = request.Name.Trim();
        trip.StartDate = request.StartDate;
        trip.EndDate = request.EndDate;
        trip.BikeId = request.BikeId;
        trip.DistanceKm = request.DistanceKm;
        trip.Budget = request.Budget;
        trip.IsDraft = request.IsDraft;
        trip.IsCancelled = request.IsCancelled;

        await repository.UpdateAsync(trip, ct);
        return trip.ToDto(clock.Today);
    }
}
