using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;

namespace RideAtlas.Application.Expenses.Commands;

public record DeleteExpenseCommand(Guid Id, Guid RiderAccountId) : IRequest;

public class DeleteExpenseCommandHandler(IExpenseRepository repository) : IRequestHandler<DeleteExpenseCommand>
{
    public async Task Handle(DeleteExpenseCommand request, CancellationToken ct)
    {
        var expense = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Expense), request.Id);
        await repository.DeleteAsync(expense, ct);
    }
}
