using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.FuelLog.Dtos;

namespace RideAtlas.Application.FuelLog.Commands;

public record UpdateFuelLogEntryCommand(
    Guid Id,
    Guid RiderAccountId,
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

public class UpdateFuelLogEntryCommandValidator : AbstractValidator<UpdateFuelLogEntryCommand>
{
    public UpdateFuelLogEntryCommandValidator()
    {
        RuleFor(x => x.OdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Litres).GreaterThan(0);
        RuleFor(x => x.PricePerLitre).GreaterThan(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}

public class UpdateFuelLogEntryCommandHandler(IFuelLogRepository repository)
    : IRequestHandler<UpdateFuelLogEntryCommand, FuelLogEntryDto>
{
    public async Task<FuelLogEntryDto> Handle(UpdateFuelLogEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.FuelLogEntry), request.Id);

        entry.RideId = request.RideId;
        entry.OccurredAt = request.OccurredAt;
        entry.OdometerKm = request.OdometerKm;
        entry.Litres = request.Litres;
        entry.PricePerLitre = request.PricePerLitre;
        entry.Cost = request.Cost;
        entry.IsFullTank = request.IsFullTank;
        entry.FuelType = request.FuelType;
        entry.Station = request.Station;
        entry.StationLocation = request.StationLocation;
        entry.PaymentMethod = request.PaymentMethod;
        entry.ReceiptPhotoUri = request.ReceiptPhotoUri;
        entry.Notes = request.Notes;

        await repository.UpdateAsync(entry, ct);

        var bikeEntries = await repository.ListAllForBikeAsync(request.RiderAccountId, entry.BikeId, ct);
        var derived = FuelMileageCalculator.Compute(bikeEntries, entry);
        return entry.ToDto(derived);
    }
}
