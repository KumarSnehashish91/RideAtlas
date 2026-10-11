using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class RideRepository(RideAtlasDbContext db) : IRideRepository
{
    public Task<Ride?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.Rides.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<PagedResult<Ride>> ListAsync(Guid riderAccountId, RideListFilter filter, CancellationToken ct = default)
    {
        var query = db.Rides.Where(x => x.RiderAccountId == riderAccountId);

        if (filter.BikeId is Guid bikeId)
            query = query.Where(x => x.BikeId == bikeId);
        if (filter.TripId is Guid tripId)
            query = query.Where(x => x.TripId == tripId);
        if (filter.FromDate is DateOnly fromDate)
        {
            var from = fromDate.ToDateTime(TimeOnly.MinValue);
            query = query.Where(x => x.StartedAt >= from);
        }
        if (filter.ToDate is DateOnly toDate)
        {
            var to = toDate.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(x => x.StartedAt <= to);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.StartedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Ride>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<Ride>> ListAllAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.Rides.Where(x => x.RiderAccountId == riderAccountId).ToListAsync(ct);

    public async Task AddAsync(Ride ride, CancellationToken ct = default)
    {
        db.Rides.Add(ride);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ride ride, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Ride ride, CancellationToken ct = default)
    {
        db.Rides.Remove(ride);
        await db.SaveChangesAsync(ct);
    }
}
