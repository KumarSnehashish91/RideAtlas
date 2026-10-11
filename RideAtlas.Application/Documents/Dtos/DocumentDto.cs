namespace RideAtlas.Application.Documents.Dtos;

public record DocumentDto(
    Guid Id,
    Guid? BikeId,
    string Type,
    string Title,
    string? DocumentNumber,
    DateOnly? IssueDate,
    DateOnly? ExpiryDate,
    bool ReminderEnabled,
    string? PhotoUri,
    string? PdfUri,
    string? Notes,
    decimal? Cost,
    DateTime CreatedAt);
