using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.ServiceHistory.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.ServiceHistory.Commands;

public record CreateServiceEntryCommand(
    Guid RiderAccountId,
    Guid BikeId,
    DateOnly ServiceDate,
    double OdometerKm,
    decimal Cost,
    string? Garage,
    IReadOnlyList<string>? ServiceTypes,
    string Category,
    string? WorkshopAddress,
    string? Mechanic,
    string? Description,
    decimal? LabourCost,
    decimal? PartsCost,
    string? InvoiceNumber,
    string? InvoicePhotoUri,
    bool WarrantyClaim,
    DateOnly? ExpectedNextServiceDate,
    double? ExpectedNextServiceOdometerKm) : IRequest<ServiceEntryDto>;

public class CreateServiceEntryCommandValidator : AbstractValidator<CreateServiceEntryCommand>
{
    public CreateServiceEntryCommandValidator()
    {
        RuleFor(x => x.BikeId).NotEmpty();
        RuleFor(x => x.OdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
    }
}

public class CreateServiceEntryCommandHandler(
    IServiceEntryRepository repository,
    IBikeRepository bikeRepository) : IRequestHandler<CreateServiceEntryCommand, ServiceEntryDto>
{
    public async Task<ServiceEntryDto> Handle(CreateServiceEntryCommand request, CancellationToken ct)
    {
        _ = await bikeRepository.GetByIdAsync(request.BikeId, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.BikeId);

        var entry = new Domain.Entities.ServiceEntry
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            ServiceDate = request.ServiceDate,
            OdometerKm = request.OdometerKm,
            Cost = request.Cost,
            Garage = request.Garage,
            ServiceTypesCsv = ServiceEntryMappingExtensions.JoinServiceTypes(request.ServiceTypes),
            Category = request.Category.Trim(),
            WorkshopAddress = request.WorkshopAddress,
            Mechanic = request.Mechanic,
            Description = request.Description,
            LabourCost = request.LabourCost,
            PartsCost = request.PartsCost,
            InvoiceNumber = request.InvoiceNumber,
            InvoicePhotoUri = request.InvoicePhotoUri,
            WarrantyClaim = request.WarrantyClaim,
            ExpectedNextServiceDate = request.ExpectedNextServiceDate,
            ExpectedNextServiceOdometerKm = request.ExpectedNextServiceOdometerKm
        };
        await repository.AddAsync(entry, ct);
        return entry.ToDto();
    }
}
