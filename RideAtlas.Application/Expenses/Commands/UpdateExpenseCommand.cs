using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Exceptions;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Expenses.Dtos;

namespace RideAtlas.Application.Expenses.Commands;

public record UpdateExpenseCommand(
    Guid Id,
    Guid RiderAccountId,
    Guid? BikeId,
    Guid? TripId,
    Guid? RideId,
    string Category,
    decimal Amount,
    DateOnly Date,
    string? Vendor,
    string? Location,
    string? PaymentMethod,
    string? Notes,
    string? ReceiptPhotoUri,
    double? OdometerKm) : IRequest<ExpenseDto>;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class UpdateExpenseCommandHandler(IExpenseRepository repository) : IRequestHandler<UpdateExpenseCommand, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(UpdateExpenseCommand request, CancellationToken ct)
    {
        var expense = await repository.GetByIdAsync(request.Id, request.RiderAccountId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Expense), request.Id);

        expense.BikeId = request.BikeId;
        expense.TripId = request.TripId;
        expense.RideId = request.RideId;
        expense.Category = request.Category.Trim();
        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.Vendor = request.Vendor;
        expense.Location = request.Location;
        expense.PaymentMethod = request.PaymentMethod;
        expense.Notes = request.Notes;
        expense.ReceiptPhotoUri = request.ReceiptPhotoUri;
        expense.OdometerKm = request.OdometerKm;

        await repository.UpdateAsync(expense, ct);
        return expense.ToDto();
    }
}
