using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Trips.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Trips.Commands;

public record CreateTripCommand(
    Guid RiderAccountId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BikeId,
    double? DistanceKm,
    decimal? Budget,
    bool IsDraft) : IRequest<TripDto>;

public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.DistanceKm).GreaterThanOrEqualTo(0).When(x => x.DistanceKm.HasValue);
        RuleFor(x => x.Budget).GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue);
    }
}

public class CreateTripCommandHandler(ITripRepository repository, IDateTimeProvider clock)
    : IRequestHandler<CreateTripCommand, TripDto>
{
    public async Task<TripDto> Handle(CreateTripCommand request, CancellationToken ct)
    {
        var trip = new Trip
        {
            RiderAccountId = request.RiderAccountId,
            Name = request.Name.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            BikeId = request.BikeId,
            DistanceKm = request.DistanceKm,
            Budget = request.Budget,
            IsDraft = request.IsDraft
        };
        await repository.AddAsync(trip, ct);
        return trip.ToDto(clock.Today);
    }
}
