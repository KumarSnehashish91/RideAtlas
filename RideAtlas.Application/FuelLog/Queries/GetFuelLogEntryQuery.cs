using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.FuelLog.Dtos;

namespace RideAtlas.Application.FuelLog.Queries;

public record GetFuelLogEntryQuery(Guid Id, Guid RiderAccountId) : IRequest<FuelLogEntryDto>;

public class GetFuelLogEntryQueryHandler(IFuelLogRepository repository)
    : IRequestHandler<GetFuelLogEntryQuery, FuelLogEntryDto>
{
    public async Task<FuelLogEntryDto> Handle(GetFuelLogEntryQuery request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.FuelLogEntry), request.Id);
        var bikeEntries = await repository.ListAllForBikeAsync(request.RiderAccountId, entry.BikeId, ct);
        return entry.ToDto(FuelMileageCalculator.Compute(bikeEntries, entry));
    }
}
