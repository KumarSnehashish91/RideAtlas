using FluentValidation;
using MediatR;
using RideAtlas.Application.Common.Interfaces;
using RideAtlas.Application.Expenses.Dtos;
using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Expenses.Commands;

public record CreateExpenseCommand(
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

public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class CreateExpenseCommandHandler(IExpenseRepository repository) : IRequestHandler<CreateExpenseCommand, ExpenseDto>
{
    public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken ct)
    {
        var expense = new Expense
        {
            RiderAccountId = request.RiderAccountId,
            BikeId = request.BikeId,
            TripId = request.TripId,
            RideId = request.RideId,
            Category = request.Category.Trim(),
            Amount = request.Amount,
            Date = request.Date,
            Vendor = request.Vendor,
            Location = request.Location,
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes,
            ReceiptPhotoUri = request.ReceiptPhotoUri,
            OdometerKm = request.OdometerKm
        };
        await repository.AddAsync(expense, ct);
        return expense.ToDto();
    }
}
