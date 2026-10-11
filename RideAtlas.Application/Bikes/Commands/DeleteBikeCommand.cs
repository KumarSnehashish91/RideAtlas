using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Bikes.Commands;

public record DeleteBikeCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteBikeCommandHandler(IBikeRepository repository) : IRequestHandler<DeleteBikeCommand>
{
    public async Task Handle(DeleteBikeCommand request, CancellationToken ct)
    {
        var bike = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.Id);
        await repository.DeleteAsync(bike, ct);
    }
}
