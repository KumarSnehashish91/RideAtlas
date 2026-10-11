using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.FuelLog.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.FuelLog.Commands;

public record CreateFuelLogEntryCommand(
    Guid RiderAccountId,
    Guid BikeId,
    Guid? RideId,
    DateTime OccurredAt,
    double OdometerKm,
    double Litres,
    decimal PricePerLitre,
    decimal Cost,
    bool IsFullTank,
    string? FuelType,
    string? Station,
    string? StationLocation,
    string? PaymentMethod,
    string? ReceiptPhotoUri,
    string? Notes) : IRequest<FuelLogEntryDto>;

public class CreateFuelLogEntryCommandValidator : AbstractValidator<CreateFuelLogEntryCommand>
{
    public CreateFuelLogEntryCommandValidator()
    {
        RuleFor(x => x.BikeId).NotEmpty();
        RuleFor(x => x.OdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Litres).GreaterThan(0);
        RuleFor(x => x.PricePerLitre).GreaterThan(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}

public class CreateFuelLogEntryCommandHandler(
    IFuelLogRepository repository,
    IBikeRepository bikeRepository) : IRequestHandler<CreateFuelLogEntryCommand, FuelLogEntryDto>
{
    public async Task<FuelLogEntryDto> Handle(CreateFuelLogEntryCommand request, CancellationToken ct)
    {
        _ = await bikeRepository.GetByIdAsync(request.BikeId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.BikeId);

        var entry = new Domain.Entities.FuelLogEntry
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            RideId = request.RideId,
            OccurredAt = request.OccurredAt,
            OdometerKm = request.OdometerKm,
            Litres = request.Litres,
            PricePerLitre = request.PricePerLitre,
            Cost = request.Cost,
            IsFullTank = request.IsFullTank,
            FuelType = request.FuelType,
            Station = request.Station,
            StationLocation = request.StationLocation,
            PaymentMethod = request.PaymentMethod,
            ReceiptPhotoUri = request.ReceiptPhotoUri,
            Notes = request.Notes
        };
        await repository.AddAsync(entry, ct);

        var bikeEntries = await repository.ListAllForBikeAsync(request.RiderAccountId, request.BikeId, ct);
        var derived = FuelMileageCalculator.Compute(bikeEntries, entry);
        return entry.ToDto(derived);
    }
}
