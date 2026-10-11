namespace RideAtlas.Application.Expenses.Dtos;

public record ExpenseDto(
    Guid Id,
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
    double? OdometerKm,
    DateTime CreatedAt);
