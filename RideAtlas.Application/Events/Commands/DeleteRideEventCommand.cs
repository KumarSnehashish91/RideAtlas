using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Events.Commands;

public record DeleteRideEventCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteRideEventCommandHandler(IRideEventRepository repository) : IRequestHandler<DeleteRideEventCommand>
{
    public async Task Handle(DeleteRideEventCommand request, CancellationToken ct)
    {
        var rideEvent = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.RideEvent), request.Id);
        await repository.DeleteAsync(rideEvent, ct);
    }
}
