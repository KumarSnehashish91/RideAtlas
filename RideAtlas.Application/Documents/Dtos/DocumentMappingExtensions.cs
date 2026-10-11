using RideAtlas.Domain.Entities;

namespace RideAtlas.Application.Documents.Dtos;

public static class DocumentMappingExtensions
{
    public static DocumentDto ToDto(this Document document) => new(
        document.Id,
        document.BikeId,
        document.Type,
        document.Title,
        document.DocumentNumber,
        document.IssueDate,
        document.ExpiryDate,
        document.ReminderEnabled,
        document.PhotoUri,
        document.PdfUri,
        document.Notes,
        document.Cost,
        document.CreatedAt);
}
