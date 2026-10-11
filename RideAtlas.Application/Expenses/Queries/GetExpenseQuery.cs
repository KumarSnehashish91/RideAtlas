using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Expenses.Dtos;

namespace RideAtlas.Application.Expenses.Queries;

public record GetExpenseQuery(Guid Id, Guid RiderAccountId) : IRequest<ExpenseDto>;

public class GetExpenseQueryHandler(IExpenseRepository repository) : IRequestHandler<GetExpenseQuery, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken ct)
    {
        var expense = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Expense), request.Id);
        return expense.ToDto();
    }
}
