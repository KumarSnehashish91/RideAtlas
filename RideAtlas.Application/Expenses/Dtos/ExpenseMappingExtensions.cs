using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Expenses.Dtos;

public static class ExpenseMappingExtensions
{
    public static ExpenseDto ToDto(this Expense expense) => new(
        expense.Id,
        expense.BikeId,
        expense.TripId,
        expense.RideId,
        expense.Category,
        expense.Amount,
        expense.Date,
        expense.Vendor,
        expense.Location,
        expense.PaymentMethod,
        expense.Notes,
        expense.ReceiptPhotoUri,
        expense.OdometerKm,
        expense.CreatedAt);
}
