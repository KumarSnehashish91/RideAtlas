using MediatR;
using RideAtlas.Application.Bikes.Dtos;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Bikes.Commands;

public record SetActiveBikeCommand(Guid Id, Guid RiderAccountId) : IRequest<BikeDto>;

public class SetActiveBikeCommandHandler(IBikeRepository repository) : IRequestHandler<SetActiveBikeCommand, BikeDto>
{
    public async Task<BikeDto> Handle(SetActiveBikeCommand request, CancellationToken ct)
    {
        var bike = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Motorcycle), request.Id);

        await repository.ClearActiveFlagAsync(request.RiderAccountId, ct);
        bike.IsActive = true;
        await repository.UpdateAsync(bike, ct);
        return bike.ToDto();
    }
}
