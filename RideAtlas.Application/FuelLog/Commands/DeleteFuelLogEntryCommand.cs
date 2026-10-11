using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.FuelLog.Commands;

public record DeleteFuelLogEntryCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteFuelLogEntryCommandHandler(IFuelLogRepository repository) : IRequestHandler<DeleteFuelLogEntryCommand>
{
    public async Task Handle(DeleteFuelLogEntryCommand request, CancellationToken ct)
    {
        var entry = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.FuelLogEntry), request.Id);
        await repository.DeleteAsync(entry, ct);
    }
}
