using MediatR;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Expenses.Queries;

public record ExpenseCategoryTotalDto(string Category, decimal TotalAmount, int Count);

public record ExpenseCategoryBreakdownDto(decimal TotalAmount, IReadOnlyList<ExpenseCategoryTotalDto> ByCategory);

public record ExpenseCategoryBreakdownQuery(
    Guid RiderAccountId,
    Guid? BikeId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IRequest<ExpenseCategoryBreakdownDto>;

public class ExpenseCategoryBreakdownQueryHandler(IExpenseRepository repository)
    : IRequestHandler<ExpenseCategoryBreakdownQuery, ExpenseCategoryBreakdownDto>
{
    public async Task<ExpenseCategoryBreakdownDto> Handle(ExpenseCategoryBreakdownQuery request, CancellationToken ct)
    {
        var expenses = (await repository.ListAllAsync(request.RiderAccountId, ct)).AsEnumerable();

        if (request.BikeId is Guid bikeId)
            expenses = expenses.Where(e => e.BikeId == bikeId);
        if (request.FromDate is DateOnly fromDate)
            expenses = expenses.Where(e => e.Date >= fromDate);
        if (request.ToDate is DateOnly toDate)
            expenses = expenses.Where(e => e.Date <= toDate);

        var list = expenses.ToList();
        var byCategory = list
            .GroupBy(e => e.Category)
            .Select(g => new ExpenseCategoryTotalDto(g.Key, g.Sum(e => e.Amount), g.Count()))
            .OrderByDescending(x => x.TotalAmount)
            .ToList();

        return new ExpenseCategoryBreakdownDto(list.Sum(e => e.Amount), byCategory);
    }
}
