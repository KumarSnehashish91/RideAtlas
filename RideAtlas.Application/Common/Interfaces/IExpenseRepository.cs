using RideAtlas.Application.Common.Models;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Common.Interfaces;

public class ExpenseListFilter
{
    public Guid? BikeId { get; init; }
    public Guid? TripId { get; init; }
    public string? Category { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, Guid riderAccountId, CancellationToken ct = default);
    Task<PagedResult<Expense>> ListAsync(Guid riderAccountId, ExpenseListFilter filter, CancellationToken ct = default);
    Task<IReadOnlyList<Expense>> ListAllAsync(Guid riderAccountId, CancellationToken ct = default);
    Task AddAsync(Expense expense, CancellationToken ct = default);
    Task UpdateAsync(Expense expense, CancellationToken ct = default);
    Task DeleteAsync(Expense expense, CancellationToken ct = default);
}
