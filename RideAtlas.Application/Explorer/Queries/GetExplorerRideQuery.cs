using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Explorer.Dtos;

namespace RideAtlas.Application.Explorer.Queries;

public record GetExplorerRideQuery(Guid Id) : IRequest<ExplorerRideDto>;

public class GetExplorerRideQueryHandler(IExplorerRideRepository repository)
    : IRequestHandler<GetExplorerRideQuery, ExplorerRideDto>
{
    public async Task<ExplorerRideDto> Handle(GetExplorerRideQuery request, CancellationToken ct)
    {
        var ride = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ExplorerRide), request.Id);
        return ride.ToDto();
    }
}
