using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Explorer.Dtos;

namespace RideAtlas.Application.Explorer.Queries;

public record GetExplorerRideBySlugQuery(string Slug) : IRequest<ExplorerRideDto>;

public class GetExplorerRideBySlugQueryHandler(IExplorerRideRepository repository)
    : IRequestHandler<GetExplorerRideBySlugQuery, ExplorerRideDto>
{
    public async Task<ExplorerRideDto> Handle(GetExplorerRideBySlugQuery request, CancellationToken ct)
    {
        var ride = await repository.GetBySlugAsync(request.Slug, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.ExplorerRide), request.Slug);
        return ride.ToDto();
    }
}
