using Microsoft.EntityFrameworkCore;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(RideAtlasDbContext db) : IExpenseRepository
{
    public Task<Expense?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default) =>
        db.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.RiderAccountId == riderAccountId, ct);

    public async Task<PagedResult<Expense>> ListAsync(Guid riderAccountId, ExpenseListFilter filter, CancellationToken ct = default)
    {
        var query = db.Expenses.Where(x => x.RiderAccountId == riderAccountId);

        if (filter.BikeId is Guid bikeId)
            query = query.Where(x => x.BikeId == bikeId);
        if (filter.TripId is Guid tripId)
            query = query.Where(x => x.TripId == tripId);
        if (!string.IsNullOrWhiteSpace(filter.Category))
            query = query.Where(x => x.Category == filter.Category);
        if (filter.FromDate is DateOnly fromDate)
            query = query.Where(x => x.Date >= fromDate);
        if (filter.ToDate is DateOnly toDate)
            query = query.Where(x => x.Date <= toDate);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.Date)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Expense>
        {
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<Expense>> ListAllAsync(Guid riderAccountId, CancellationToken ct = default) =>
        await db.Expenses.Where(x => x.RiderAccountId == riderAccountId).ToListAsync(ct);

    public async Task AddAsync(Expense expense, CancellationToken ct = default)
    {
        db.Expenses.Add(expense);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Expense expense, CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Expense expense, CancellationToken ct = default)
    {
        db.Expenses.Remove(expense);
        await db.SaveChangesAsync(ct);
    }
}
