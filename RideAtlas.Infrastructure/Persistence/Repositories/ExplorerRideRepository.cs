using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class ExplorerRideRepository(RideAtlasDbContext db) : IExplorerRideRepository
{
    public Task<ExplorerRide?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.ExplorerRides.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<ExplorerRide?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        db.ExplorerRides.FirstOrDefaultAsync(x => x.Slug == slug, ct);

    public async Task<PagedResult<ExplorerRide>> SearchAsync(ExplorerRideSearchFilter filter, CancellationToken ct = default)
    {
        var query = db.ExplorerRides.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var text = filter.SearchText.Trim();
            query = query.Where(x => x.Title.Contains(text) || x.Destination.Contains(text) || x.Origin.Contains(text));
        }
        if (!string.IsNullOrWhiteSpace(filter.Country))
            query = query.Where(x => x.Country == filter.Country);
        if (!string.IsNullOrWhiteSpace(filter.State))
            query = query.Where(x => x.State == filter.State);
        if (!string.IsNullOrWhiteSpace(filter.Difficulty))
            query = query.Where(x => x.Difficulty == filter.Difficulty);
        if (!string.IsNullOrWhiteSpace(filter.RoadType))
            query = query.Where(x => x.RoadType == filter.RoadType);
        if (filter.MinScenicRating is double minScenic)
            query = query.Where(x => x.ScenicRating >= minScenic);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.ScenicRating)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<ExplorerRide>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(ExplorerRide ride, CancellationToken ct = default)
    {
        db.ExplorerRides.Add(ride);
        await db.SaveChangesAsync(ct);
    }
}
