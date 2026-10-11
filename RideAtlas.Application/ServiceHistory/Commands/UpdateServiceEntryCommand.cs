using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.ServiceHistory.Dtos;

namespace RideAtlas.Application.ServiceHistory.Commands;

public record UpdateServiceEntryCommand(
    Guid Id,
    Guid RiderAccountId,
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

public class UpdateServiceEntryCommandValidator : AbstractValidator<UpdateServiceEntryCommand>
{
    public UpdateServiceEntryCommandValidator()
    {
        RuleFor(x => x.OdometerKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
    }
}

public class UpdateServiceEntryCommandHandler(IServiceEntryRepository repository)
    : IRequestHandler<UpdateServiceEntryCommand, ServiceEntryDto>
{
    public async Task<ServiceEntryDto> Handle(UpdateServiceEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ServiceEntry), request.Id);

        entry.ServiceDate = request.ServiceDate;
        entry.OdometerKm = request.OdometerKm;
        entry.Cost = request.Cost;
        entry.Garage = request.Garage;
        entry.ServiceTypesCsv = ServiceEntryMappingExtensions.JoinServiceTypes(request.ServiceTypes);
        entry.Category = request.Category.Trim();
        entry.WorkshopAddress = request.WorkshopAddress;
        entry.Mechanic = request.Mechanic;
        entry.Description = request.Description;
        entry.LabourCost = request.LabourCost;
        entry.PartsCost = request.PartsCost;
        entry.InvoiceNumber = request.InvoiceNumber;
        entry.InvoicePhotoUri = request.InvoicePhotoUri;
        entry.WarrantyClaim = request.WarrantyClaim;
        entry.ExpectedNextServiceDate = request.ExpectedNextServiceDate;
        entry.ExpectedNextServiceOdometerKm = request.ExpectedNextServiceOdometerKm;

        await repository.UpdateAsync(entry, ct);
        return entry.ToDto();
    }
}
