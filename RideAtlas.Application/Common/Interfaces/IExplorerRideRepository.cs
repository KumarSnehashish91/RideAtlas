using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class ExplorerRideSearchFilter
{
    public string? SearchText { get; init; }
    public string? Country { get; init; }
    public string? State { get; init; }
    public string? Difficulty { get; init; }
    public string? RoadType { get; init; }
    public double? MinScenicRating { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IExplorerRideRepository
{
    Task<ExplorerRide?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ExplorerRide?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<PagedResult<ExplorerRide>> SearchAsync(ExplorerRideSearchFilter filter, CancellationToken ct = default);
    Task AddAsync(ExplorerRide ride, CancellationToken ct = default);
}
