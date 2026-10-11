using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Common.Models;
using RideAtlas.Application.Expenses.Dtos;

namespace RideAtlas.Application.Expenses.Queries;

public record ListExpensesQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    Guid? TripId,
    string? Category,
    DateOnly? FromDate,
    DateOnly? ToDate,
    int Page,
    int PageSize) : IRequest<PagedResult<ExpenseDto>>;

public class ListExpensesQueryHandler(IExpenseRepository repository)
    : IRequestHandler<ListExpensesQuery, PagedResult<ExpenseDto>>
{
    public async Task<PagedResult<ExpenseDto>> Handle(ListExpensesQuery request, CancellationToken ct)
    {
        var filter = new ExpenseListFilter
        {
            BikeId = request.BikeId,
            TripId = request.TripId,
            Category = request.Category,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Page = request.Page <= 0 ? 1 : request.Page,
            PageSize = request.PageSize <= 0 ? 20 : request.PageSize
        };
        var result = await repository.ListAsync(request.RiderAccountId, filter, ct);
        return new PagedResult<ExpenseDto>
        {
            Items = result.Items.Select(e => e.ToDto()).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
