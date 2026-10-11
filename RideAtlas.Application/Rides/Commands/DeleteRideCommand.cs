using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Rides.Commands;

public record DeleteRideCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteRideCommandHandler(IRideRepository repository) : IRequestHandler<DeleteRideCommand>
{
    public async Task Handle(DeleteRideCommand request, CancellationToken ct)
    {
        var ride = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Ride), request.Id);
        await repository.DeleteAsync(ride, ct);
    }
}
